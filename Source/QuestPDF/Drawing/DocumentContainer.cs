using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal sealed class DocumentContainer : IDocumentContainer
    {
        internal List<IComponent> Pages { get; set; } = [];
        
        internal Container Compose()
        {
            var container = new Container();
            ComposeContainer(container);
            return container;

            void ComposeContainer(IContainer container)
            {
                if (Pages.Count == 0)
                    Pages.Add(new Page());

                container = container
                    .DebugPointer(DebugPointerType.DocumentStructure, DocumentStructureTypes.Document.ToString())
                    .SemanticDocument();
                
                if (Pages.Count == 1)
                {
                    container.Component(Pages.First());
                    return;
                }

                container
                    .Column(column =>
                    {
                        Pages
                            .SelectMany(x => new List<Action>()
                            {
                                () => column.Item().PageBreak(),
                                () => column.Item().SemanticTag("Part").Component(x)
                            })
                            .Skip(1)
                            .ToList()
                            .ForEach(x => x());
                    });
            }
        }
    }
}