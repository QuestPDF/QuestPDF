using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal class DocumentContainer : IDocumentContainer
    {
        internal List<IComponent> Pages { get; set; } = new List<IComponent>();
        
        internal Container Compose()
        {
            var container = new Container();
            
            container
                .DebugPointer("Document")
                .Column(column =>
                {
                    Pages
                        .SelectMany(x => new List<Action>()
                        {
                            () => column.Item().PageBreak(),
                            () => column.Item().Component(x)
                        })
                        .Skip(1)
                        .ToList()
                        .ForEach(x => x());
                });

            return container;
        }
    }
}