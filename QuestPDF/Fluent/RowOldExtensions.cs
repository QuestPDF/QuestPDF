// using System;
// using QuestPDF.Elements;
// using QuestPDF.Infrastructure;
//
// namespace QuestPDF.Fluent
// {
//     public class RowDescriptor
//     {
//         internal RowOld Row { get; } = new RowOld();
//
//         public void Spacing(float value)
//         {
//             Row.Spacing = value;
//         }
//         
//         public IContainer ConstantItem(float width)
//         {
//             return Column(constantWidth: width);
//         }
//         
//         public IContainer RelativeItem(float width = 1)
//         {
//             return Column(relativeWidth: width);
//         }
//         
//         public IContainer Column(float constantWidth = 0, float relativeWidth = 0)
//         {
//             var element = new RowOldElement(constantWidth, relativeWidth);
//             
//             Row.Items.Add(element);
//             return element;
//         }
//     }
//     
//     public static class RowExtensions
//     {
//         public static void Row(this IContainer element, Action<RowDescriptor> handler)
//         {
//             var descriptor = new RowDescriptor();
//             handler(descriptor);
//             element.Element(descriptor.Row);
//         }
//     }
// }