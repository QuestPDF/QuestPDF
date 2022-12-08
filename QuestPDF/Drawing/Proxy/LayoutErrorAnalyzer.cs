using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Proxy
{
    internal static class LayoutErrorAnalyzer
    {
        public static void ApplyLayoutDebugging(this Container content)
        {
            content.VisitChildren(x =>
            {
                x.CreateProxy(y => y is ElementProxy ? y : new DebuggingProxy(y));
            });
        }
        
        public static void RemoveExistingProxies(this Container content)
        {
            content.VisitChildren(x =>
            {
                x.CreateProxy(y => y is ElementProxy proxy ? proxy.Child : y);
            });
        }

        public static LayoutErrorTrace CollectLayoutErrorTrace(this Container content)
        {
            var proxies = content.ExtractProxyOfType<DebuggingProxy>();
            return Map(proxies);

            LayoutErrorTrace Map(TreeNode<DebuggingProxy> proxy)
            {
                var element = proxy.Value.Child;
                
                return new LayoutErrorTrace
                {
                    ElementType = element.GetType().Name,
                    IsSingleChildContainer = element is ContainerElement,
                    ElementProperties = element.GetElementConfiguration().ToList(),
                    
                    Measurements = proxy
                        .Value
                        .Measurements
                        .Select(x => new LayoutErrorMeasurement
                        {
                            AvailableSpace = new QuestPDF.Previewer.Size()
                            {
                                Width = x.AvailableSpace.Width,
                                Height = x.AvailableSpace.Height
                            },
                            SpacePlan = new QuestPDF.Previewer.SpacePlan()
                            {
                                Width = x.SpacePlan.Width,
                                Height = x.SpacePlan.Height,
                                Type = x.SpacePlan.Type
                            }
                        })
                        .ToList(),
                    
                    Children = proxy.Children.Select(Map).ToList()
                };
            }
        }
    }
}