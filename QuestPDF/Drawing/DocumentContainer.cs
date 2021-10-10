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
        internal TextStyle DefaultTextStyle { get; set; } = TextStyle.Default;
        internal List<IComponent> Pages { get; set; } = new List<IComponent>();
        
        internal Container Compose()
        {
            var container = new Container();
            
            container
                .Stack(stack =>
                {
                    Pages
                        .SelectMany(x => new List<Action>()
                        {
                            () => stack.Item().PageBreak(),
                            () => stack.Item().Component(x)
                        })
                        .Skip(1)
                        .ToList()
                        .ForEach(x => x());
                });

            return container;
        }
    }
}