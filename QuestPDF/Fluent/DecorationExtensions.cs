using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class DecorationDescriptor
    {
        internal Decoration Decoration { get; } = new Decoration();
        
        public IContainer Before()
        {
            var container = new Container();
            Decoration.Before = container;
            return container;
        }
        
        public void Before(Action<IContainer> handler)
        {
            handler?.Invoke(Before());
        }
        
        public IContainer Content()
        {
            var container = new Container();
            Decoration.Content = container;
            return container;
        }
        
        public void Content(Action<IContainer> handler)
        {
            handler?.Invoke(Content());
        }
        
        public IContainer After()
        {
            var container = new Container();
            Decoration.After = container;
            return container;
        }
        
        public void After(Action<IContainer> handler)
        {
            handler?.Invoke(After());
        }

        #region Obsolete

        // public IContainer Header()
        // {
        //     var container = new Container();
        //     Decoration.Before = container;
        //     return container;
        // }
        //
        // public void Header(Action<IContainer> handler)
        // {
        //     handler?.Invoke(Header());
        // }
        //
        // public IContainer Footer()
        // {
        //     var container = new Container();
        //     Decoration.After = container;
        //     return container;
        // }
        //
        // public void Footer(Action<IContainer> handler)
        // {
        //     handler?.Invoke(Footer());
        // }

        #endregion
    }
    
    public static class DecorationExtensions
    {
        public static void Decoration(this IContainer element, Action<DecorationDescriptor> handler)
        {
            var descriptor = new DecorationDescriptor();
            handler(descriptor);
            
            element.Element(descriptor.Decoration);
        }
    }
}