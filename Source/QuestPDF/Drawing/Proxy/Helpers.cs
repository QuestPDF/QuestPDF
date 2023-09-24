using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Proxy;

internal static class Helpers
{
    public static void ApplyInfiniteLayoutDebugging(this Container container)
    {
        container.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy ? y : new OverflowDebuggingProxy(y));
        });
    }
    
    public static void ApplyOverlayDebugging(this TreeNode<OverflowDebuggingProxy> hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        void Traverse(TreeNode<OverflowDebuggingProxy> parent)
        {
            if (parent.Value.SpacePlanType == null)
                return;
            
            if (parent.Value.SpacePlanType == SpacePlanType.FullRender)
                return;
            
            var hasWraps = parent.Children.Any(x => x.Value.SpacePlanType == SpacePlanType.Wrap);
            var hasPartialRenders = parent.Children.Any(x => x.Value.SpacePlanType == SpacePlanType.PartialRender);

            // TODO: using hasPartialRenders in the condition below, helps in certain cases and breaks others, investigate reasons
            if (hasWraps || hasPartialRenders)
            {
                foreach (var child in parent.Children)
                    Traverse(child);
            }
            else
            {
                parent.Value.CreateProxy(x => new ContentOverflowDebugArea { Child = x });
            }
        }
    }

    public static void RemoveProxiesOfType<T>(this Container content) where T : ElementProxy
    {
        content.VisitChildren(x =>
        {
            x.CreateProxy(y => y is T proxy ? proxy.Child : y);
        });
    }
    
    public static void RemoveExistingProxies(this Container content)
    {
        content.VisitChildren(x =>
        {
            x.CreateProxy(y => y is ElementProxy proxy ? proxy.Child : y);
        });
    }
    
    public static void ApplyCanvasCache(this Element hierarchyRoot)
    {
        Traverse(hierarchyRoot);
        
        /// returns true when certain elements meets all criteria to be cached
        bool Traverse(Element parent)
        {
            if (!CanBeCached(parent))
                return false;
            
            if (parent is IContainer container)
            {
                return Traverse(container.Child as Element);
            }

            var children = parent.GetChildren().ToList();
            var childrenAreApplicable = children.Select(Traverse).ToArray();

            if (childrenAreApplicable.All(x => x))
                return true;

            var proxyIndex = 0;
            
            parent.CreateProxy(element =>
            {
                var isApplicable = childrenAreApplicable[proxyIndex];
                proxyIndex++;

                return isApplicable ? new CanvasCacheProxy(element) : element;
            });

            return false;
        }

        bool CanBeCached(IElement element)
        {
            if (element is TextBlock textBlock)
            {
                foreach (var textBlockItem in textBlock.Items)
                {
                    if (textBlockItem is TextBlockPageNumber)
                        return false;
                    
                    if (textBlockItem is TextBlockElement textBlockElement && !Traverse(textBlockElement.Element))
                        return false;
                }

                return true;
            }
            else if (element is DynamicHost dynamicHost)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}