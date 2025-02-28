using System;
using System.Linq.Expressions;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ComponentExtensions
    {
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="component"]/*' />
        /// <param name="component">Instance of the class implementing the <see cref="IComponent"></see> interface.</param>
        public static void Component<T>(this IContainer element, T component) where T : IComponent
        {
            element.Component(component);
        }
        
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="component"]/*' />
        public static void Component<T>(this IContainer element) where T : IComponent, new()
        {
            element.Component(new T());
        }
    }
}