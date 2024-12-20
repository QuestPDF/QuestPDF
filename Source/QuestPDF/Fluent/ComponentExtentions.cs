using System;
using System.Linq.Expressions;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
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
                    throw new DocumentComposeException($"The slot {selector.GetPropertyName()} of the component {(typeof(T).Name)} was already used.");

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
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="component"]/*' />
        /// <param name="component">Instance of the class implementing the <see cref="IComponent"></see> interface.</param>
        public static void Component<T>(this IContainer element, T component) where T : IComponent
        {
            element.Component(component, null);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="component"]/*' />
        public static void Component<T>(this IContainer element) where T : IComponent, new()
        {
            element.Component(new T(), null);
        }

        static void Component<T>(this IContainer element, T component, Action<ComponentDescriptor<T>>? handler = null) where T : IComponent
        {
            var descriptor = new ComponentDescriptor<T>(component);
            handler?.Invoke(descriptor);

            var componentContainer = element
                .Container()
                .DebugPointer(DebugPointerType.Component, component.GetType().Name);
            
            component.Compose(componentContainer);
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