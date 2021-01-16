using System;
using System.Linq.Expressions;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    class ComponentDescriptor<T> where T : IComponent
    {
        public T Component { get; }
        
        public ComponentDescriptor(T component)
        {
            Component = component;
        }

        public IContainer Slot(Expression<Func<T, ISlot>> selector)
        {
            try
            {
                var existingValue = Component.GetPropertyValue(selector);

                if (existingValue != null)
                    throw new DocumentComposeException($"The slot {selector.GetPropertyName()} of the component {(typeof( T).Name)} was already used.");

                var slot = new Slot();
                Component.SetPropertyValue(selector, slot);
                return slot;
            }
            catch (DocumentComposeException)
            {
                throw;
            }
            catch
            {
                throw new DocumentComposeException("Every slot in a component should be a public property with getter and setter.");
            }
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

        static void Component<T>(this IContainer element, T component, Action<ComponentDescriptor<T>>? handler = null) where T : IComponent
        {
            var descriptor = new ComponentDescriptor<T>(component);
            handler?.Invoke(descriptor);
            
            component.Compose(element.Container());
        }
        
        static void Component<T>(this IContainer element, Action<ComponentDescriptor<T>>? handler = null) where T : IComponent, new()
        {
            element.Component(new T(), handler);
        }

        static void Slot(this IContainer element, ISlot slot)
        {
            var child = (slot as Slot)?.Child;
            element.Element(child);
        }
    }
}