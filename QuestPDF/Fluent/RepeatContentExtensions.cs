using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RepeatContentExtensions
    {
        private static IContainer RepeatContent(this IContainer element, Action<RepeatContentSetter> handler)
        {
            var repeatContentSetter = element as RepeatContentSetter ?? new RepeatContentSetter();
            handler(repeatContentSetter);
            
            return element.Element(repeatContentSetter);
        }

        public static IContainer RepeatContentWhenPaging(this IContainer element)
        {
            return element.RepeatContent(x => x.RepeatContent = true);
        }
        
        public static IContainer DoNotRepeatContentWhenPaging(this IContainer element)
        {
            return element.RepeatContent(x => x.RepeatContent = false);
        }
    }
}