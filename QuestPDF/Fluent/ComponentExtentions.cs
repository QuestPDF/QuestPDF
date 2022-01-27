using System;
using System.Linq.Expressions;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    internal class ComponentDescriptor<TComponent> where TComponent : IComponent
    {
        public TComponent Component { get; }
        
        public ComponentDescriptor(TComponent component)
        {
            Component = component;
        }

        public IContainer Slot(Expression<Func<TComponent, ISlot>> selector)
        {
            AssureThatTheSlotIsNotConfiguredYet(selector);
                
            var slot = new Slot();
            Component.SetPropertyValue(selector, slot);
            return slot;
        }
        
        public void Slot<TArgument>(Expression<Func<TComponent, ISlot<TArgument>>> selector, Action<TArgument, IContainer> handler)
        {
            AssureThatTheSlotIsNotConfiguredYet(selector);
            
            var slot = new Slot<TArgument>
            {
                GetContent = argument =>
                {
                    var container = new Container();
                    handler(argument, container);
                    return container;
                }
            };
            
            Component.SetPropertyValue(selector, slot);
        }

        private void AssureThatTheSlotIsNotConfiguredYet<TSlot>(Expression<Func<TComponent, TSlot>> selector) where TSlot : class
        {
            var existingValue = Component.GetPropertyValue(selector);

            if (existingValue != null)
                throw new DocumentComposeException($"The slot {selector.GetPropertyName()} of the component {(typeof(TComponent).Name)} was already used.");
        }
    }
    
    public static class ComponentExtensions
    {
        public static void Component<T>(this IContainer element, T component) where T : IComponent
        {
            element.Component(component, null);
        }
        
        public static void Component<T>(this IContainer element) where T : IComponent, new()
        {
            element.Component(new T(), null);
        }

        internal static void Component<T>(this IContainer element, T component, Action<ComponentDescriptor<T>>? handler = null) where T : IComponent
        {
            var descriptor = new ComponentDescriptor<T>(component);
            handler?.Invoke(descriptor);

            if (System.Diagnostics.Debugger.IsAttached)
                element = element.DebugPointer(component.GetType().Name, highlight: false);

            component.Compose(element.Container());
        }
        
        internal static void Component<T>(this IContainer element, Action<ComponentDescriptor<T>>? handler = null) where T : IComponent, new()
        {
            element.Component(new T(), handler);
        }

        internal static void Slot(this IContainer element, ISlot slot)
        {
            var child = (slot as Slot)?.Child;
            element.Element(child);
        }
        
        internal static void Slot<TArgument>(this IContainer element, ISlot<TArgument> slot, TArgument argument)
        {
            var child = (slot as Slot<TArgument>)?.GetContent(argument) ?? Empty.Instance;
            element.Element(child);
        }
    }
}