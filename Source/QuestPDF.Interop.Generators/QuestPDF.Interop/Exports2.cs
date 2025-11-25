// AUTO-GENERATED on 11/25/2025 16:47:29

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace QuestPDF.Interop;

public static unsafe partial class Exports
{
    
    
    
    
    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__column_descriptor__spacing__e47553e3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void ColumnDescriptor_Spacing_e47553e3(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<ColumnDescriptor>(target);
    
    containerObject.Spacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__column_descriptor__item__2cf2ad89", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ColumnDescriptor_Item_2cf2ad89(IntPtr target)
{
    var containerObject = UnboxHandle<ColumnDescriptor>(target);
    
    var result = containerObject.Item();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__before__1bfecdf8", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DecorationDescriptor_Before_1bfecdf8(IntPtr target)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    var result = containerObject.Before();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__before__bf5ce29e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void DecorationDescriptor_Before_bf5ce29e(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    containerObject.Before(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__content__9ec35667", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DecorationDescriptor_Content_9ec35667(IntPtr target)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    var result = containerObject.Content();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__content__391a971a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void DecorationDescriptor_Content_391a971a(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    containerObject.Content(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__after__4cf66f67", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DecorationDescriptor_After_4cf66f67(IntPtr target)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    var result = containerObject.After();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__after__4c35dd57", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void DecorationDescriptor_After_4c35dd57(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    containerObject.After(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__header__3dc5ae50", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DecorationDescriptor_Header_3dc5ae50(IntPtr target)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    var result = containerObject.Header();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__header__4e45db15", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void DecorationDescriptor_Header_4e45db15(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    containerObject.Header(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__footer__dd101ef5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DecorationDescriptor_Footer_dd101ef5(IntPtr target)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    var result = containerObject.Footer();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__decoration_descriptor__footer__0bfdeeb8", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void DecorationDescriptor_Footer_0bfdeeb8(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<DecorationDescriptor>(target);
    
    containerObject.Footer(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__spacing__e466eaa7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_Spacing_e466eaa7(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.Spacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__vertical_spacing__44456280", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_VerticalSpacing_44456280(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.VerticalSpacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__horizontal_spacing__a035fbb4", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_HorizontalSpacing_a035fbb4(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.HorizontalSpacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__baseline_top__96b48f7f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_BaselineTop_96b48f7f(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.BaselineTop();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__baseline_middle__2ee97366", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_BaselineMiddle_2ee97366(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.BaselineMiddle();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__baseline_bottom__1878876e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_BaselineBottom_1878876e(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.BaselineBottom();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__align_left__0c3a1762", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_AlignLeft_0c3a1762(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.AlignLeft();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__align_center__d09c92f2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_AlignCenter_d09c92f2(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.AlignCenter();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__align_right__99b3ac01", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_AlignRight_99b3ac01(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.AlignRight();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__align_justify__3f036912", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_AlignJustify_3f036912(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.AlignJustify();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__align_space_around__cfaed88d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void InlinedDescriptor_AlignSpaceAround_cfaed88d(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    containerObject.AlignSpaceAround();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__inlined_descriptor__item__3a4e6d7b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr InlinedDescriptor_Item_3a4e6d7b(IntPtr target)
{
    var containerObject = UnboxHandle<InlinedDescriptor>(target);
    
    var result = containerObject.Item();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__layers_descriptor__layer__f8c1dd4f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr LayersDescriptor_Layer_f8c1dd4f(IntPtr target)
{
    var containerObject = UnboxHandle<LayersDescriptor>(target);
    
    var result = containerObject.Layer();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__layers_descriptor__primary_layer__c2eb4a19", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr LayersDescriptor_PrimaryLayer_c2eb4a19(IntPtr target)
{
    var containerObject = UnboxHandle<LayersDescriptor>(target);
    
    var result = containerObject.PrimaryLayer();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__row_descriptor__spacing__09cc7a62", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void RowDescriptor_Spacing_09cc7a62(IntPtr target, float spacing, int unit)
{
    var containerObject = UnboxHandle<RowDescriptor>(target);
    
    containerObject.Spacing(spacing, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__row_descriptor__relative_column__33b15d53", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr RowDescriptor_RelativeColumn_33b15d53(IntPtr target, float size)
{
    var containerObject = UnboxHandle<RowDescriptor>(target);
    
    var result = containerObject.RelativeColumn(size);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__row_descriptor__constant_column__a41e161e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr RowDescriptor_ConstantColumn_a41e161e(IntPtr target, float size)
{
    var containerObject = UnboxHandle<RowDescriptor>(target);
    
    var result = containerObject.ConstantColumn(size);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__row_descriptor__relative_item__f4570b47", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr RowDescriptor_RelativeItem_f4570b47(IntPtr target, float size)
{
    var containerObject = UnboxHandle<RowDescriptor>(target);
    
    var result = containerObject.RelativeItem(size);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__row_descriptor__constant_item__4f927836", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr RowDescriptor_ConstantItem_4f927836(IntPtr target, float size, int unit)
{
    var containerObject = UnboxHandle<RowDescriptor>(target);
    
    var result = containerObject.ConstantItem(size, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__row_descriptor__auto_item__fc084be8", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr RowDescriptor_AutoItem_fc084be8(IntPtr target)
{
    var containerObject = UnboxHandle<RowDescriptor>(target);
    
    var result = containerObject.AutoItem();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__spacing__2a69d201", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_Spacing_2a69d201(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.Spacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__vertical_spacing__593ca4c3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_VerticalSpacing_593ca4c3(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.VerticalSpacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__horizontal_spacing__a9d6ceae", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_HorizontalSpacing_a9d6ceae(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.HorizontalSpacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__columns__160f5f35", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_Columns_160f5f35(IntPtr target, int value)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.Columns(value);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__align_left__fc5e4cb9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_AlignLeft_fc5e4cb9(IntPtr target)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.AlignLeft();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__align_center__3d81b2fe", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_AlignCenter_3d81b2fe(IntPtr target)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.AlignCenter();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__align_right__e9aa71bc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void GridDescriptor_AlignRight_e9aa71bc(IntPtr target)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    containerObject.AlignRight();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__grid_descriptor__item__3e7cf6ba", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr GridDescriptor_Item_3e7cf6ba(IntPtr target, int columns)
{
    var containerObject = UnboxHandle<GridDescriptor>(target);
    
    var result = containerObject.Item(columns);
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__multi_column_descriptor__spacing__b96a0ed7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void MultiColumnDescriptor_Spacing_b96a0ed7(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<MultiColumnDescriptor>(target);
    
    containerObject.Spacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__multi_column_descriptor__columns__f9027e4e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void MultiColumnDescriptor_Columns_f9027e4e(IntPtr target, int value)
{
    var containerObject = UnboxHandle<MultiColumnDescriptor>(target);
    
    containerObject.Columns(value);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__multi_column_descriptor__balance_height__a0509325", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void MultiColumnDescriptor_BalanceHeight_a0509325(IntPtr target, bool enable)
{
    var containerObject = UnboxHandle<MultiColumnDescriptor>(target);
    
    containerObject.BalanceHeight(enable);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__multi_column_descriptor__content__68196264", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr MultiColumnDescriptor_Content_68196264(IntPtr target)
{
    var containerObject = UnboxHandle<MultiColumnDescriptor>(target);
    
    var result = containerObject.Content();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__multi_column_descriptor__spacer__9d6eea5d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr MultiColumnDescriptor_Spacer_9d6eea5d(IntPtr target)
{
    var containerObject = UnboxHandle<MultiColumnDescriptor>(target);
    
    var result = containerObject.Spacer();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__table_descriptor__columns_definition__1b198f41", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TableDescriptor_ColumnsDefinition_1b198f41(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<TableDescriptor>(target);
    
    containerObject.ColumnsDefinition(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__table_descriptor__extend_last_cells_to_table_bottom__22a2235b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TableDescriptor_ExtendLastCellsToTableBottom_22a2235b(IntPtr target)
{
    var containerObject = UnboxHandle<TableDescriptor>(target);
    
    containerObject.ExtendLastCellsToTableBottom();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__table_descriptor__header__227448b3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TableDescriptor_Header_227448b3(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<TableDescriptor>(target);
    
    containerObject.Header(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__table_descriptor__footer__a74a23a5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TableDescriptor_Footer_a74a23a5(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<TableDescriptor>(target);
    
    containerObject.Footer(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__table_descriptor__cell__1f40892e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TableDescriptor_Cell_1f40892e(IntPtr target)
{
    var containerObject = UnboxHandle<TableDescriptor>(target);
    
    var result = containerObject.Cell();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__table_cell_descriptor__cell__1061edf9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TableCellDescriptor_Cell_1061edf9(IntPtr target)
{
    var containerObject = UnboxHandle<TableCellDescriptor>(target);
    
    var result = containerObject.Cell();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__table_columns_definition_descriptor__constant_column__e71e4979", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TableColumnsDefinitionDescriptor_ConstantColumn_e71e4979(IntPtr target, float width, int unit)
{
    var containerObject = UnboxHandle<TableColumnsDefinitionDescriptor>(target);
    
    containerObject.ConstantColumn(width, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__table_columns_definition_descriptor__relative_column__940a67b1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TableColumnsDefinitionDescriptor_RelativeColumn_940a67b1(IntPtr target, float width)
{
    var containerObject = UnboxHandle<TableColumnsDefinitionDescriptor>(target);
    
    containerObject.RelativeColumn(width);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__align_left__4a573634", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_AlignLeft_4a573634(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.AlignLeft();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__align_center__def2b616", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_AlignCenter_def2b616(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.AlignCenter();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__align_right__de6eaa17", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_AlignRight_de6eaa17(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.AlignRight();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__justify__1501b0fa", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_Justify_1501b0fa(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.Justify();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__align_start__947ba696", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_AlignStart_947ba696(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.AlignStart();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__align_end__5aefafc5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_AlignEnd_5aefafc5(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.AlignEnd();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__clamp_lines__f1b02b03", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_ClampLines_f1b02b03(IntPtr target, int maxLines, IntPtr ellipsis)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.ClampLines(maxLines, Marshal.PtrToStringUni(ellipsis));
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__paragraph_spacing__c3629bd6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_ParagraphSpacing_c3629bd6(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.ParagraphSpacing(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__paragraph_first_line_indentation__414498e7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_ParagraphFirstLineIndentation_414498e7(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.ParagraphFirstLineIndentation(value, (QuestPDF.Infrastructure.Unit)unit);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__span__41a383c0", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_Span_41a383c0(IntPtr target, IntPtr text)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.Span(Marshal.PtrToStringUni(text));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__line__17db2520", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_Line_17db2520(IntPtr target, IntPtr text)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.Line(Marshal.PtrToStringUni(text));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__empty_line__70ae8fc0", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_EmptyLine_70ae8fc0(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.EmptyLine();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__current_page_number__2097e179", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_CurrentPageNumber_2097e179(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.CurrentPageNumber();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__total_pages__604d3e19", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_TotalPages_604d3e19(IntPtr target)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.TotalPages();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__begin_page_number_of_section__340accfc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_BeginPageNumberOfSection_340accfc(IntPtr target, IntPtr sectionName)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.BeginPageNumberOfSection(Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__end_page_number_of_section__deee569a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_EndPageNumberOfSection_deee569a(IntPtr target, IntPtr sectionName)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.EndPageNumberOfSection(Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__page_number_within_section__51768233", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_PageNumberWithinSection_51768233(IntPtr target, IntPtr sectionName)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.PageNumberWithinSection(Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__total_pages_within_section__250c06e5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_TotalPagesWithinSection_250c06e5(IntPtr target, IntPtr sectionName)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.TotalPagesWithinSection(Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__section_link__c9b32c1a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_SectionLink_c9b32c1a(IntPtr target, IntPtr text, IntPtr sectionName)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.SectionLink(Marshal.PtrToStringUni(text), Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__hyperlink__f38a28c7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_Hyperlink_f38a28c7(IntPtr target, IntPtr text, IntPtr url)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.Hyperlink(Marshal.PtrToStringUni(text), Marshal.PtrToStringUni(url));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__element__862752ab", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextDescriptor_Element_862752ab(IntPtr target, int alignment)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    var result = containerObject.Element((QuestPDF.Infrastructure.TextInjectedElementAlignment)alignment);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_descriptor__element__ff63896d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void TextDescriptor_Element_ff63896d(IntPtr target, delegate* unmanaged[Cdecl]<IntPtr, void> handler, int alignment)
{
    var containerObject = UnboxHandle<TextDescriptor>(target);
    
    containerObject.Element(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); }, (QuestPDF.Infrastructure.TextInjectedElementAlignment)alignment);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__font_color__a0d06e42", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_FontColor_a0d06e42(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.FontColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__background_color__5461b453", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_BackgroundColor_5461b453(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.BackgroundColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__font_size__c989487d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_FontSize_c989487d(IntPtr descriptor, float value)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.FontSize(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__line_height__a1b4697a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_LineHeight_a1b4697a(IntPtr descriptor, float? factor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.LineHeight(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__letter_spacing__92f86a26", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_LetterSpacing_92f86a26(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.LetterSpacing(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__word_spacing__1f794add", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_WordSpacing_1f794add(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.WordSpacing(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__italic__4f023aba", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Italic_4f023aba(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Italic(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__wrap_anywhere__39249a82", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_WrapAnywhere_39249a82(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.WrapAnywhere(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__strikethrough__41841206", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Strikethrough_41841206(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Strikethrough(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__underline__2e1ae473", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Underline_2e1ae473(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Underline(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__overline__add25860", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Overline_add25860(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Overline(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_color__5d18d151", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationColor_5d18d151(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_thickness__c7c23c84", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationThickness_c7c23c84(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationThickness(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_solid__f64746d1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationSolid_f64746d1(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationSolid();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_double__41cf8a18", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationDouble_41cf8a18(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDouble();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_wavy__1761acf2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationWavy_1761acf2(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationWavy();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_dotted__e940537a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationDotted_e940537a(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDotted();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__decoration_dashed__a85f7344", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DecorationDashed_a85f7344(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDashed();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__thin__e9036638", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Thin_e9036638(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Thin();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__extra_light__33bbe020", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_ExtraLight_33bbe020(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraLight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__light__37ef1bc2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Light_37ef1bc2(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Light();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__normal_weight__18d360b3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_NormalWeight_18d360b3(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.NormalWeight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__medium__5ef8b80e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Medium_5ef8b80e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Medium();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__semi_bold__0b92f7b7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_SemiBold_0b92f7b7(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.SemiBold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__bold__0dfa9061", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Bold_0dfa9061(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Bold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__extra_bold__c4fbc0a6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_ExtraBold_c4fbc0a6(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraBold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__black__0cc8d698", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Black_0cc8d698(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Black();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__extra_black__c7698d85", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_ExtraBlack_c7698d85(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraBlack();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__normal_position__5e5176cb", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_NormalPosition_5e5176cb(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.NormalPosition();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__subscript__db9bd4eb", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Subscript_db9bd4eb(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Subscript();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__superscript__a9b46b1e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_Superscript_a9b46b1e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Superscript();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__direction_auto__fbed9e71", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DirectionAuto_fbed9e71(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionAuto();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__direction_from_left_to_right__09e2e3bc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DirectionFromLeftToRight_09e2e3bc(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionFromLeftToRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__direction_from_right_to_left__6cc5cb9e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DirectionFromRightToLeft_6cc5cb9e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionFromRightToLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__enable_font_feature__136a164d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_EnableFontFeature_136a164d(IntPtr descriptor, IntPtr featureName)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.EnableFontFeature(Marshal.PtrToStringUni(featureName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_span_descriptor__disable_font_feature__5bd81de9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextSpanDescriptor_DisableFontFeature_5bd81de9(IntPtr descriptor, IntPtr featureName)
{
    var containerObject = UnboxHandle<TextSpanDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DisableFontFeature(Marshal.PtrToStringUni(featureName));
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__font_color__a0d06e42", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_FontColor_a0d06e42(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.FontColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__background_color__5461b453", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_BackgroundColor_5461b453(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.BackgroundColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__font_size__c989487d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_FontSize_c989487d(IntPtr descriptor, float value)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.FontSize(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__line_height__a1b4697a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_LineHeight_a1b4697a(IntPtr descriptor, float? factor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.LineHeight(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__letter_spacing__92f86a26", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_LetterSpacing_92f86a26(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.LetterSpacing(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__word_spacing__1f794add", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_WordSpacing_1f794add(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.WordSpacing(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__italic__4f023aba", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Italic_4f023aba(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Italic(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__wrap_anywhere__39249a82", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_WrapAnywhere_39249a82(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.WrapAnywhere(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__strikethrough__41841206", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Strikethrough_41841206(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Strikethrough(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__underline__2e1ae473", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Underline_2e1ae473(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Underline(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__overline__add25860", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Overline_add25860(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Overline(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_color__5d18d151", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationColor_5d18d151(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_thickness__c7c23c84", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationThickness_c7c23c84(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationThickness(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_solid__f64746d1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationSolid_f64746d1(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationSolid();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_double__41cf8a18", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationDouble_41cf8a18(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDouble();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_wavy__1761acf2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationWavy_1761acf2(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationWavy();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_dotted__e940537a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationDotted_e940537a(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDotted();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__decoration_dashed__a85f7344", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DecorationDashed_a85f7344(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDashed();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__thin__e9036638", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Thin_e9036638(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Thin();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__extra_light__33bbe020", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_ExtraLight_33bbe020(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraLight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__light__37ef1bc2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Light_37ef1bc2(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Light();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__normal_weight__18d360b3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_NormalWeight_18d360b3(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.NormalWeight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__medium__5ef8b80e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Medium_5ef8b80e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Medium();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__semi_bold__0b92f7b7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_SemiBold_0b92f7b7(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.SemiBold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__bold__0dfa9061", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Bold_0dfa9061(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Bold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__extra_bold__c4fbc0a6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_ExtraBold_c4fbc0a6(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraBold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__black__0cc8d698", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Black_0cc8d698(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Black();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__extra_black__c7698d85", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_ExtraBlack_c7698d85(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraBlack();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__normal_position__5e5176cb", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_NormalPosition_5e5176cb(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.NormalPosition();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__subscript__db9bd4eb", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Subscript_db9bd4eb(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Subscript();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__superscript__a9b46b1e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_Superscript_a9b46b1e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Superscript();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__direction_auto__fbed9e71", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DirectionAuto_fbed9e71(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionAuto();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__direction_from_left_to_right__09e2e3bc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DirectionFromLeftToRight_09e2e3bc(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionFromLeftToRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__direction_from_right_to_left__6cc5cb9e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DirectionFromRightToLeft_6cc5cb9e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionFromRightToLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__enable_font_feature__136a164d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_EnableFontFeature_136a164d(IntPtr descriptor, IntPtr featureName)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.EnableFontFeature(Marshal.PtrToStringUni(featureName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_page_number_descriptor__disable_font_feature__5bd81de9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextPageNumberDescriptor_DisableFontFeature_5bd81de9(IntPtr descriptor, IntPtr featureName)
{
    var containerObject = UnboxHandle<TextPageNumberDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DisableFontFeature(Marshal.PtrToStringUni(featureName));
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__align_left__da139fee", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_AlignLeft_da139fee(IntPtr target)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.AlignLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__align_center__3661d942", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_AlignCenter_3661d942(IntPtr target)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.AlignCenter();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__align_right__28e79232", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_AlignRight_28e79232(IntPtr target)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.AlignRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__justify__f4a5d951", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Justify_f4a5d951(IntPtr target)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.Justify();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__align_start__c97cfc2b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_AlignStart_c97cfc2b(IntPtr target)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.AlignStart();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__align_end__e0ace0c1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_AlignEnd_e0ace0c1(IntPtr target)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.AlignEnd();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__clamp_lines__2a5db05c", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_ClampLines_2a5db05c(IntPtr target, int maxLines, IntPtr ellipsis)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.ClampLines(maxLines, Marshal.PtrToStringUni(ellipsis));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__paragraph_spacing__6dcddcea", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_ParagraphSpacing_6dcddcea(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.ParagraphSpacing(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__paragraph_first_line_indentation__5d11cadd", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_ParagraphFirstLineIndentation_5d11cadd(IntPtr target, float value, int unit)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(target);
    
    var result = containerObject.ParagraphFirstLineIndentation(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__font_color__a0d06e42", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_FontColor_a0d06e42(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.FontColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__background_color__5461b453", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_BackgroundColor_5461b453(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.BackgroundColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__font_size__c989487d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_FontSize_c989487d(IntPtr descriptor, float value)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.FontSize(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__line_height__a1b4697a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_LineHeight_a1b4697a(IntPtr descriptor, float? factor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.LineHeight(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__letter_spacing__92f86a26", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_LetterSpacing_92f86a26(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.LetterSpacing(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__word_spacing__1f794add", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_WordSpacing_1f794add(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.WordSpacing(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__italic__4f023aba", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Italic_4f023aba(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Italic(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__wrap_anywhere__39249a82", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_WrapAnywhere_39249a82(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.WrapAnywhere(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__strikethrough__41841206", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Strikethrough_41841206(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Strikethrough(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__underline__2e1ae473", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Underline_2e1ae473(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Underline(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__overline__add25860", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Overline_add25860(IntPtr descriptor, bool value)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Overline(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_color__5d18d151", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationColor_5d18d151(IntPtr descriptor, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_thickness__c7c23c84", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationThickness_c7c23c84(IntPtr descriptor, float factor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationThickness(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_solid__f64746d1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationSolid_f64746d1(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationSolid();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_double__41cf8a18", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationDouble_41cf8a18(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDouble();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_wavy__1761acf2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationWavy_1761acf2(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationWavy();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_dotted__e940537a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationDotted_e940537a(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDotted();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__decoration_dashed__a85f7344", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DecorationDashed_a85f7344(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DecorationDashed();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__thin__e9036638", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Thin_e9036638(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Thin();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__extra_light__33bbe020", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_ExtraLight_33bbe020(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraLight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__light__37ef1bc2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Light_37ef1bc2(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Light();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__normal_weight__18d360b3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_NormalWeight_18d360b3(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.NormalWeight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__medium__5ef8b80e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Medium_5ef8b80e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Medium();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__semi_bold__0b92f7b7", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_SemiBold_0b92f7b7(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.SemiBold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__bold__0dfa9061", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Bold_0dfa9061(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Bold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__extra_bold__c4fbc0a6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_ExtraBold_c4fbc0a6(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraBold();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__black__0cc8d698", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Black_0cc8d698(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Black();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__extra_black__c7698d85", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_ExtraBlack_c7698d85(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.ExtraBlack();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__normal_position__5e5176cb", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_NormalPosition_5e5176cb(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.NormalPosition();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__subscript__db9bd4eb", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Subscript_db9bd4eb(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Subscript();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__superscript__a9b46b1e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_Superscript_a9b46b1e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.Superscript();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__direction_auto__fbed9e71", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DirectionAuto_fbed9e71(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionAuto();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__direction_from_left_to_right__09e2e3bc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DirectionFromLeftToRight_09e2e3bc(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionFromLeftToRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__direction_from_right_to_left__6cc5cb9e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DirectionFromRightToLeft_6cc5cb9e(IntPtr descriptor)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DirectionFromRightToLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__enable_font_feature__136a164d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_EnableFontFeature_136a164d(IntPtr descriptor, IntPtr featureName)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.EnableFontFeature(Marshal.PtrToStringUni(featureName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__text_block_descriptor__disable_font_feature__5bd81de9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr TextBlockDescriptor_DisableFontFeature_5bd81de9(IntPtr descriptor, IntPtr featureName)
{
    var containerObject = UnboxHandle<TextBlockDescriptor>(descriptor);
    FreeHandle(descriptor);
    var result = containerObject.DisableFontFeature(Marshal.PtrToStringUni(featureName));
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__image_descriptor__use_original_image__d3be84c2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ImageDescriptor_UseOriginalImage_d3be84c2(IntPtr target, bool value)
{
    var containerObject = UnboxHandle<ImageDescriptor>(target);
    
    var result = containerObject.UseOriginalImage(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__image_descriptor__with_raster_dpi__78f617ee", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ImageDescriptor_WithRasterDpi_78f617ee(IntPtr target, int dpi)
{
    var containerObject = UnboxHandle<ImageDescriptor>(target);
    
    var result = containerObject.WithRasterDpi(dpi);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__image_descriptor__fit_width__7b9aa4d6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ImageDescriptor_FitWidth_7b9aa4d6(IntPtr target)
{
    var containerObject = UnboxHandle<ImageDescriptor>(target);
    
    var result = containerObject.FitWidth();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__image_descriptor__fit_height__c888daad", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ImageDescriptor_FitHeight_c888daad(IntPtr target)
{
    var containerObject = UnboxHandle<ImageDescriptor>(target);
    
    var result = containerObject.FitHeight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__image_descriptor__fit_area__4dbf28f5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ImageDescriptor_FitArea_4dbf28f5(IntPtr target)
{
    var containerObject = UnboxHandle<ImageDescriptor>(target);
    
    var result = containerObject.FitArea();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__image_descriptor__fit_unproportionally__3d7bad76", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr ImageDescriptor_FitUnproportionally_3d7bad76(IntPtr target)
{
    var containerObject = UnboxHandle<ImageDescriptor>(target);
    
    var result = containerObject.FitUnproportionally();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__dynamic_image_descriptor__use_original_image__1dabc6b8", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DynamicImageDescriptor_UseOriginalImage_1dabc6b8(IntPtr target, bool value)
{
    var containerObject = UnboxHandle<DynamicImageDescriptor>(target);
    
    var result = containerObject.UseOriginalImage(value);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__dynamic_image_descriptor__with_raster_dpi__a72018d5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr DynamicImageDescriptor_WithRasterDpi_a72018d5(IntPtr target, int dpi)
{
    var containerObject = UnboxHandle<DynamicImageDescriptor>(target);
    
    var result = containerObject.WithRasterDpi(dpi);
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__svg_image_descriptor__fit_width__ae37e277", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr SvgImageDescriptor_FitWidth_ae37e277(IntPtr target)
{
    var containerObject = UnboxHandle<SvgImageDescriptor>(target);
    
    var result = containerObject.FitWidth();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__svg_image_descriptor__fit_height__7e11f169", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr SvgImageDescriptor_FitHeight_7e11f169(IntPtr target)
{
    var containerObject = UnboxHandle<SvgImageDescriptor>(target);
    
    var result = containerObject.FitHeight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__svg_image_descriptor__fit_area__6abce393", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr SvgImageDescriptor_FitArea_6abce393(IntPtr target)
{
    var containerObject = UnboxHandle<SvgImageDescriptor>(target);
    
    var result = containerObject.FitArea();
    return BoxHandle(result);
}

    
    
[UnmanagedCallersOnly(EntryPoint = "questpdf__container__align_left__68bfdc67", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AlignLeft_68bfdc67(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AlignLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__align_center__4fb1e0d1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AlignCenter_4fb1e0d1(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AlignCenter();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__align_right__a1c1a1bf", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AlignRight_a1c1a1bf(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AlignRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__align_top__f275ca95", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AlignTop_f275ca95(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AlignTop();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__align_middle__95fef9e8", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AlignMiddle_95fef9e8(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AlignMiddle();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__align_bottom__d33d0520", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AlignBottom_d33d0520(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AlignBottom();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__stack__9026c7f9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Stack_9026c7f9(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Stack(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__column__24d6ceed", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Column_24d6ceed(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Column(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__width__a346e20f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Width_a346e20f(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Width(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__min_width__c00f1915", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_MinWidth_c00f1915(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.MinWidth(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__max_width__7e85a057", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_MaxWidth_7e85a057(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.MaxWidth(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__height__36ac3a02", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Height_36ac3a02(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Height(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__min_height__58cc06b0", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_MinHeight_58cc06b0(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.MinHeight(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__max_height__0b76e199", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_MaxHeight_0b76e199(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.MaxHeight(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__content_from_left_to_right__191523c1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ContentFromLeftToRight_191523c1(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ContentFromLeftToRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__content_from_right_to_left__a31dbd9d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ContentFromRightToLeft_a31dbd9d(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ContentFromRightToLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__debug_area__a69b9c65", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_DebugArea_a69b9c65(IntPtr parent, IntPtr text, global::QuestPDF.Infrastructure.Color? color)
{
    var containerObject = UnboxHandle<IContainer>(parent);
    FreeHandle(parent);
    var result = containerObject.DebugArea(Marshal.PtrToStringUni(text), color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__debug_pointer__9d669879", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_DebugPointer_9d669879(IntPtr parent, IntPtr label)
{
    var containerObject = UnboxHandle<IContainer>(parent);
    FreeHandle(parent);
    var result = containerObject.DebugPointer(Marshal.PtrToStringUni(label));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__decoration__0b39c58e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Decoration_0b39c58e(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Decoration(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__aspect_ratio__fd5bc0dc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_AspectRatio_fd5bc0dc(IntPtr element, float ratio, int option)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.AspectRatio(ratio, (QuestPDF.Infrastructure.AspectRatioOption)option);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__placeholder__a560192f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Placeholder_a560192f(IntPtr element, IntPtr text)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Placeholder(Marshal.PtrToStringUni(text));
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__show_once__c6224013", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ShowOnce_c6224013(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ShowOnce();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__skip_once__b3d4c7bf", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_SkipOnce_b3d4c7bf(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.SkipOnce();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__show_entire__16629c88", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ShowEntire_16629c88(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ShowEntire();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__ensure_space__0cbedd6a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_EnsureSpace_0cbedd6a(IntPtr element, float minHeight)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.EnsureSpace(minHeight);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__prevent_page_break__2e3cab6a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PreventPageBreak_2e3cab6a(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PreventPageBreak();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__page_break__4287fb55", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_PageBreak_4287fb55(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.PageBreak();
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__container__be126adc", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Container_be126adc(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Container();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__external_link__2d85fe5d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ExternalLink_2d85fe5d(IntPtr element, IntPtr url)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ExternalLink(Marshal.PtrToStringUni(url));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__hyperlink__40aee55c", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Hyperlink_40aee55c(IntPtr element, IntPtr url)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Hyperlink(Marshal.PtrToStringUni(url));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__location__3f137901", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Location_3f137901(IntPtr element, IntPtr locationName)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Location(Marshal.PtrToStringUni(locationName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__section__b2687119", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Section_b2687119(IntPtr element, IntPtr sectionName)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Section(Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__internal_link__1f644a62", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_InternalLink_1f644a62(IntPtr element, IntPtr locationName)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.InternalLink(Marshal.PtrToStringUni(locationName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__section_link__d27b4828", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_SectionLink_d27b4828(IntPtr element, IntPtr sectionName)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.SectionLink(Marshal.PtrToStringUni(sectionName));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__show_if__da52e306", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ShowIf_da52e306(IntPtr element, bool condition)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ShowIf(condition);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__unconstrained__a43107f6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Unconstrained_a43107f6(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Unconstrained();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__stop_paging__81b05f34", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_StopPaging_81b05f34(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.StopPaging();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__scale_to_fit__bb0b4e57", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ScaleToFit_bb0b4e57(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ScaleToFit();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__repeat__e198bc84", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Repeat_e198bc84(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Repeat();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__lazy__971e7b54", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Lazy_971e7b54(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> contentBuilder)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Lazy(x => { var boxed = BoxHandle(x); contentBuilder(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__lazy_with_cache__a33b5f9b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_LazyWithCache_a33b5f9b(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> contentBuilder)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.LazyWithCache(x => { var boxed = BoxHandle(x); contentBuilder(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__z_index__9cd9a32e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ZIndex_9cd9a32e(IntPtr element, int indexValue)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ZIndex(indexValue);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__capture_content_position__845fb313", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_CaptureContentPosition_845fb313(IntPtr element, IntPtr id)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.CaptureContentPosition(Marshal.PtrToStringUni(id));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__extend__291e835a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Extend_291e835a(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Extend();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__extend_vertical__e63e1d72", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ExtendVertical_e63e1d72(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ExtendVertical();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__extend_horizontal__c6d6d128", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ExtendHorizontal_c6d6d128(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ExtendHorizontal();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__grid__8839fc83", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Grid_8839fc83(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Grid(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__image__9155d14a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Image_9155d14a(IntPtr parent, IntPtr filePath)
{
    var containerObject = UnboxHandle<IContainer>(parent);
    FreeHandle(parent);
    var result = containerObject.Image(Marshal.PtrToStringUni(filePath));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__inlined__33b27c8d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Inlined_33b27c8d(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Inlined(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__layers__03ce5bdd", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Layers_03ce5bdd(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Layers(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__line_vertical__ab97b857", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_LineVertical_ab97b857(IntPtr element, float thickness, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.LineVertical(thickness, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__line_horizontal__a6f7f11f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_LineHorizontal_a6f7f11f(IntPtr element, float thickness, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.LineHorizontal(thickness, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__multi_column__193479d6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_MultiColumn_193479d6(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.MultiColumn(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding__5daecb7e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Padding_5daecb7e(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Padding(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding_horizontal__7a6b255d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PaddingHorizontal_7a6b255d(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PaddingHorizontal(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding_vertical__91122aaa", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PaddingVertical_91122aaa(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PaddingVertical(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding_top__de3b7b3b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PaddingTop_de3b7b3b(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PaddingTop(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding_bottom__74ad0a7b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PaddingBottom_74ad0a7b(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PaddingBottom(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding_left__103ee738", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PaddingLeft_103ee738(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PaddingLeft(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__padding_right__89d1cf61", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_PaddingRight_89d1cf61(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.PaddingRight(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__rotate_left__c5193e66", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_RotateLeft_c5193e66(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.RotateLeft();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__rotate_right__004c9c52", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_RotateRight_004c9c52(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.RotateRight();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__rotate__c33f62ac", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Rotate_c33f62ac(IntPtr element, float angle)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Rotate(angle);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__row__39fce557", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Row_39fce557(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Row(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__scale__05521931", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Scale_05521931(IntPtr element, float factor)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Scale(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__scale_horizontal__14d1a9be", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ScaleHorizontal_14d1a9be(IntPtr element, float factor)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ScaleHorizontal(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__scale_vertical__5bc8a8a5", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ScaleVertical_5bc8a8a5(IntPtr element, float factor)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ScaleVertical(factor);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__flip_horizontal__744e4fe9", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_FlipHorizontal_744e4fe9(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.FlipHorizontal();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__flip_vertical__a91487f3", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_FlipVertical_a91487f3(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.FlipVertical();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__flip_over__ce1ff345", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_FlipOver_ce1ff345(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.FlipOver();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__shrink__4221b85b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Shrink_4221b85b(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Shrink();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__shrink_vertical__e5042c3c", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ShrinkVertical_e5042c3c(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ShrinkVertical();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__shrink_horizontal__588cfd0f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_ShrinkHorizontal_588cfd0f(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.ShrinkHorizontal();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__box__480d9cac", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Box_480d9cac(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Box();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__minimal_box__0e9d8cd2", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_MinimalBox_0e9d8cd2(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.MinimalBox();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border__a6712928", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Border_a6712928(IntPtr element, float all, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Border(all, color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__background__68f98b81", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Background_68f98b81(IntPtr element, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Background(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border__17f3b5e4", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Border_17f3b5e4(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Border(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_vertical__7922384b", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderVertical_7922384b(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderVertical(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_horizontal__34913f34", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderHorizontal_34913f34(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderHorizontal(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_left__803ed1e6", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderLeft_803ed1e6(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderLeft(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_right__de8ca6bf", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderRight_de8ca6bf(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderRight(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_top__c469b91f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderTop_c469b91f(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderTop(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_bottom__59b8a019", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderBottom_59b8a019(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderBottom(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__corner_radius__bf7cb39f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_CornerRadius_bf7cb39f(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.CornerRadius(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__corner_radius_top_left__41d08c72", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_CornerRadiusTopLeft_41d08c72(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.CornerRadiusTopLeft(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__corner_radius_top_right__1497678a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_CornerRadiusTopRight_1497678a(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.CornerRadiusTopRight(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__corner_radius_bottom_left__3a8d234a", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_CornerRadiusBottomLeft_3a8d234a(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.CornerRadiusBottomLeft(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__corner_radius_bottom_right__b07c1d8d", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_CornerRadiusBottomRight_b07c1d8d(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.CornerRadiusBottomRight(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_color__2a24bda0", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderColor_2a24bda0(IntPtr element, global::QuestPDF.Infrastructure.Color color)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderColor(color);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_alignment_outside__ce5e63fa", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderAlignmentOutside_ce5e63fa(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderAlignmentOutside();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_alignment_middle__66a27445", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderAlignmentMiddle_66a27445(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderAlignmentMiddle();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_alignment_inside__8cef56b1", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_BorderAlignmentInside_8cef56b1(IntPtr element)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.BorderAlignmentInside();
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__svg__f547d46e", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Svg_f547d46e(IntPtr container, IntPtr svg)
{
    var containerObject = UnboxHandle<IContainer>(container);
    FreeHandle(container);
    var result = containerObject.Svg(Marshal.PtrToStringUni(svg));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__table__d49da987", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Table_d49da987(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> handler)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Table(x => { var boxed = BoxHandle(x); handler(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__text__357e362f", CallConvs = new[] { typeof(CallConvCdecl) })]
public static void Container_Text_357e362f(IntPtr element, delegate* unmanaged[Cdecl]<IntPtr, void> content)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    containerObject.Text(x => { var boxed = BoxHandle(x); content(boxed); FreeHandle(boxed); });
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__text__854cc4c4", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Text_854cc4c4(IntPtr element, IntPtr text)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.Text(text);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__text__3f6b5b07", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_Text_3f6b5b07(IntPtr container, IntPtr text)
{
    var containerObject = UnboxHandle<IContainer>(container);
    FreeHandle(container);
    var result = containerObject.Text(Marshal.PtrToStringUni(text));
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__translate_x__351baebe", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_TranslateX_351baebe(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.TranslateX(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

[UnmanagedCallersOnly(EntryPoint = "questpdf__container__translate_y__d99602db", CallConvs = new[] { typeof(CallConvCdecl) })]
public static IntPtr Container_TranslateY_d99602db(IntPtr element, float value, int unit)
{
    var containerObject = UnboxHandle<IContainer>(element);
    FreeHandle(element);
    var result = containerObject.TranslateY(value, (QuestPDF.Infrastructure.Unit)unit);
    return BoxHandle(result);
}

    
}