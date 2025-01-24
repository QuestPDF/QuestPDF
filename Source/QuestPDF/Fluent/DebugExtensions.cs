﻿using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class DebugExtensions
    {
        /// <summary>
        /// Draws a labeled box around its inner content.
        /// Useful for visual debugging and pinpointing output from specific code blocks.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/debug-area.html">Learn more</a>
        /// </summary>
        /// <param name="text">Optional label displayed within the box.</param>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer DebugArea(this IContainer parent, string? text = null, Color? color = null)
        {
            var container = new Container();

            parent.Component(new DebugArea
            {
                Child = container,
                Text = text ?? string.Empty,
                Color = color ?? Colors.Red.Medium
            });

            return container;
        }

        /// <summary>
        /// <para>
        ///     Inserts a virtual debug element visible in the document hierarchy tree in the QuestPDF Companion App,
        ///     as well as in the enhanced debugging message provided by the <see cref="DocumentLayoutException" />.
        /// </para>
        /// 
        /// <para>It helps with understanding and navigation of the document hierarchy.</para>
        /// <a href="https://www.questpdf.com/api-reference/debug-pointer.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// This debug element does not appear in the final PDF output.
        /// </remarks>
        /// <param name="elementTraceText">Text visible somewhere in the "element trace" content identifying given document fragment.</param>
        public static IContainer DebugPointer(this IContainer parent, string label)
        {
            return parent.DebugPointer(DebugPointerType.UserDefined, label);
        }
        
        internal static IContainer DebugPointer(this IContainer parent, DebugPointerType type, string label)
        {
            return parent.Element(new DebugPointer
            {
                Type = type,
                Label = label
            });
        }
        
        internal static IContainer LayoutOverflowVisualization(this IContainer parent)
        {
            return parent.Element(new LayoutOverflowVisualization());
        }
    }
}