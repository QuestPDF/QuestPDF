using QuestPDF.Infrastructure;

namespace QuestPDF.Elements.Table
{
    /// <summary>
    /// Root of the structure composed by the Table API.
    /// <para>
    /// Serves as a discovery marker: when a document is semantic-aware, the generator traverses
    /// the content of every container marked with the SemanticTable method to find this element
    /// and enable the automated accessibility tagging (Table, THead, TBody, TFoot, TR, TH, TD).
    /// </para>
    /// </summary>
    internal sealed class TableRoot : ContainerElement
    {
        public Table HeaderTable { get; set; }
        public Table ContentTable { get; set; }
        public Table FooterTable { get; set; }

        public Container HeaderSlot { get; set; }
        public Container ContentSlot { get; set; }
        public Container FooterSlot { get; set; }

        private bool IsSemanticTaggingEnabled { get; set; }

        public void EnableSemanticTagging()
        {
            if (IsSemanticTaggingEnabled)
                return;

            IsSemanticTaggingEnabled = true;

            HeaderTable.EnableAutomatedSemanticTagging = true;
            ContentTable.EnableAutomatedSemanticTagging = true;
            FooterTable.EnableAutomatedSemanticTagging = true;

            HeaderSlot.CreateProxy(x => new SemanticTag { TagType = "THead", Child = x });
            ContentSlot.CreateProxy(x => new SemanticTag { TagType = "TBody", Child = x });
            FooterSlot.CreateProxy(x => new SemanticTag { TagType = "TFoot", Child = x });
        }
    }
}
