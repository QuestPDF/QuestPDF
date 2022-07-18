using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuestPDF.Drawing
{
    static class DocumentGenerator
    {
        internal static void GeneratePdf(Stream stream, IDocument document)
        {
            CheckIfStreamIsCompatible(stream);
            
            var metadata = document.GetMetadata();
            var canvas = new PdfCanvas(stream, metadata);
            RenderDocument(canvas, document);
        }
        
        internal static void GenerateXps(Stream stream, IDocument document)
        {
            CheckIfStreamIsCompatible(stream);
            
            var metadata = document.GetMetadata();
            var canvas = new XpsCanvas(stream, metadata);
            RenderDocument(canvas, document);
        }

        private static void CheckIfStreamIsCompatible(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("The library requires a Stream object with the 'write' capability available (the CanWrite flag). Please consider using the MemoryStream class.");
            
            if (!stream.CanSeek)
                throw new ArgumentException("The library requires a Stream object with the 'seek' capability available (the CanSeek flag). Please consider using the MemoryStream class.");
        }
        
        internal static ICollection<byte[]> GenerateImages(IDocument document)
        {
            var metadata = document.GetMetadata();
            var canvas = new ImageCanvas(metadata);
            RenderDocument(canvas, document);

            return canvas.Images;
        }

        internal static ICollection<PreviewerPicture> GeneratePreviewerPictures(IDocument document)
        {
            var canvas = new SkiaPictureCanvas();
            RenderDocument(canvas, document);
            return canvas.Pictures;
        }
        
        internal static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var container = new DocumentContainer();
            document.Compose(container);
            var content = container.Compose();
            ApplyDefaultTextStyle(content, TextStyle.LibraryDefault);
            
            var metadata = document.GetMetadata();
            var pageContext = new PageContext();

            var debuggingState = metadata.ApplyDebugging ? ApplyDebugging(content) : null;
            
            if (metadata.ApplyCaching)
                ApplyCaching(content);

            RenderPass(pageContext, new FreeCanvas(), content, metadata, debuggingState);
            
            if (metadata.ApplyInspection)
                ApplyInspection(content);
            
            RenderPass(pageContext, canvas, content, metadata, debuggingState);

            if (metadata.ApplyInspection)
            {
                var x = TraverseStatisticsToJson(content);
                var y = JsonSerializer.Serialize(x);   
            }
        }

        internal static string TraverseStructure(Element container)
        {
            var builder = new StringBuilder();
            var nestingLevel = 0;

            Traverse(container);
            return builder.ToString();

            void Traverse(Element item)
            {
                var indent = new string(' ', nestingLevel * 4);
                var title = item.GetType().Name;
                builder.AppendLine(indent + title);

                nestingLevel++;
                
                foreach (var child in item.GetChildren())
                    Traverse(child);
                
                nestingLevel--;
            }
        }

        internal static string TraverseStatistics(Element container, int pageNumber)
        {
            var builder = new StringBuilder();
            var nestingLevel = 0;

            Traverse(container);
            return builder.ToString();

            void Traverse(Element item)
            {
                if (item is DebuggingProxy or CacheProxy or Container)
                {
                    Traverse(item.GetChildren().First());
                    return;
                }
                
                var inspectionItem = item as InspectionProxy;

                if (inspectionItem == null)
                {
                    var children = item.GetChildren().ToList();
                        
                    if (children.Count > 1)
                        nestingLevel++;
                    
                    children.ForEach(Traverse);
                    
                    if (children.Count > 1)
                        nestingLevel--;
                    
                    return;
                }

                if (!inspectionItem.Statistics.ContainsKey(pageNumber))
                    return;
                
                var statistics = inspectionItem.Statistics[pageNumber];
                
                var indent = new string(' ', nestingLevel * 4);
                var title = statistics.Element.GetType().Name;
                
                builder.AppendLine(indent + title);
                builder.AppendLine(indent + new string('-', title.Length));
                
                builder.AppendLine(indent + "Size: " + statistics.Size);
                builder.AppendLine(indent + "Position: " + statistics.Position);
                
                foreach (var configuration in DebuggingState.GetElementConfiguration(statistics.Element))
                    builder.AppendLine(indent + configuration);
                
                builder.AppendLine();
                
                Traverse(inspectionItem.Child);
            }
        }
        
        internal static InspectionElement TraverseStatisticsToJson(Element container)
        {
            return Traverse(container);
            
            InspectionElement? Traverse(Element item)
            {
                InspectionElement? result = null;
                Element currentItem = item;
                
                while (true)
                {
                    if (currentItem is InspectionProxy proxy)
                    {
                        if (proxy.Child.GetType() == typeof(Container))
                        {
                            currentItem = proxy.Child;
                            continue;
                        }
                        
                        var statistics = GetInspectionElement(proxy);

                        if (statistics == null)
                            return null;
                        
                        if (result == null)
                        {
                            result = statistics;
                        }
                        else
                        {
                            result.Children.Add(statistics);
                        }

                        currentItem = proxy.Child;
                    }
                    else
                    {
                        var children = currentItem.GetChildren().ToList();

                        if (children.Count == 0)
                        {
                            return result;
                        }
                        else if (children.Count == 1)
                        {
                            currentItem = children.First();
                            continue;
                        }
                        else
                        {
                            children
                                .Select(Traverse)
                                .Where(x => x != null)
                                .ToList()
                                .ForEach(result.Children.Add);

                            return result;
                        }
                    }
                }
            }

            static InspectionElement? GetInspectionElement(InspectionProxy inspectionProxy)
            {
                var locations = inspectionProxy
                    .Statistics
                    .Keys
                    .Select(x =>
                    {
                        var statistics = inspectionProxy.Statistics[x];
                        
                        return new InspectionElementLocation
                        {
                            PageNumber = x,
                            Top = statistics.Position.Y,
                            Left = statistics.Position.X,
                            Width = statistics.Size.Width,
                            Height = statistics.Size.Height,
                        };
                    })
                    .ToList();
                
                return new InspectionElement
                {
                    Element = inspectionProxy.Child.GetType().Name,
                    Location = locations,
                    Properties = GetElementConfiguration(inspectionProxy.Child),
                    Children = new List<InspectionElement>()
                };
            }
            
            static Dictionary<string, string> GetElementConfiguration(IElement element)
            {
                return element
                    .GetType()
                    .GetProperties()
                    .Select(x => new
                    {
                        Property = x.Name.PrettifyName(),
                        Value = x.GetValue(element)
                    })
                    .Where(x => !(x.Value is IElement))
                    .Where(x => x.Value is string || !(x.Value is IEnumerable))
                    .Where(x => !(x.Value is TextStyle))
                    .ToDictionary(x => x.Property, x => FormatValue(x.Value));

                string FormatValue(object value)
                {
                    const int maxLength = 100;
                    
                    var text = value?.ToString() ?? "-";

                    if (text.Length < maxLength)
                        return text;

                    return text.AsSpan(0, maxLength).ToString() + "...";
                }
            }
        }

        internal static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, Container content, DocumentMetadata documentMetadata, DebuggingState? debuggingState)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.VisitChildren(x => x?.Initialize(pageContext, canvas));
            content.VisitChildren(x => (x as IStateResettable)?.ResetState());
            
            canvas.BeginDocument();

            var currentPage = 1;
            
            while(true)
            {
                pageContext.SetPageNumber(currentPage);
                debuggingState?.Reset();
                
                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
                    canvas.EndDocument();
                    ThrowLayoutException();
                }

                try
                {
                    canvas.BeginPage(spacePlan);
                    content.Draw(spacePlan);
                }
                catch (Exception exception)
                {
                    canvas.EndDocument();
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                canvas.EndPage();

                if (currentPage >= documentMetadata.DocumentLayoutExceptionThreshold)
                {
                    canvas.EndDocument();
                    ThrowLayoutException();
                }
                
                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;

                currentPage++;
            }
            
            canvas.EndDocument();

            void ThrowLayoutException()
            {
                var message = $"Composed layout generates infinite document. This may happen in two cases. " +
                              $"1) Your document and its layout configuration is correct but the content takes more than {documentMetadata.DocumentLayoutExceptionThreshold} pages. " +
                              $"In this case, please increase the value {nameof(DocumentMetadata)}.{nameof(DocumentMetadata.DocumentLayoutExceptionThreshold)} property configured in the {nameof(IDocument.GetMetadata)} method. " +
                              $"2) The layout configuration of your document is invalid. Some of the elements require more space than is provided." +
                              $"Please analyze your documents structure to detect this element and fix its size constraints.";

                var elementTrace = debuggingState?.BuildTrace() ?? "Debug trace is available only in the DEBUG mode.";

                throw new DocumentLayoutException(message, elementTrace);
            }
        }

        private static void ApplyCaching(Container content)
        {
            content.VisitChildren(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
        }

        private static DebuggingState ApplyDebugging(Container content)
        {
            var debuggingState = new DebuggingState();

            content.VisitChildren(x =>
            {
                x.CreateProxy(y => y is ElementProxy ? y : new DebuggingProxy(debuggingState, y));
            });

            return debuggingState;
        }
        
        private static void ApplyInspection(Container content)
        {
            content.VisitChildren(x =>
            {
                x.CreateProxy(y => y is ElementProxy ? y : new InspectionProxy(y));
            });
        }

        internal static void ApplyDefaultTextStyle(this Element? content, TextStyle documentDefaultTextStyle)
        {
            if (content == null)
                return;
            
            if (content is TextBlock textBlock)
            {
                foreach (var textBlockItem in textBlock.Items)
                {
                    if (textBlockItem is TextBlockSpan textSpan)
                    {
                        textSpan.Style.ApplyGlobalStyle(documentDefaultTextStyle);
                    }
                    else if (textBlockItem is TextBlockElement textElement)
                    {
                        ApplyDefaultTextStyle(textElement.Element, documentDefaultTextStyle);
                    }
                }
                
                return;
            }

            if (content is DynamicHost dynamicHost)
                dynamicHost.TextStyle.ApplyGlobalStyle(documentDefaultTextStyle);
            
            var targetTextStyle = documentDefaultTextStyle;
            
            if (content is DefaultTextStyle defaultTextStyleElement)
            {
                defaultTextStyleElement.TextStyle.ApplyParentStyle(documentDefaultTextStyle);
                targetTextStyle = defaultTextStyleElement.TextStyle;
            }
            
            foreach (var child in content.GetChildren())
                ApplyDefaultTextStyle(child, targetTextStyle);
        }
    }
}