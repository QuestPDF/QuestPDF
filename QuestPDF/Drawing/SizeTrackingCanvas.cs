using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal class SizeTrackingCanvas : FreeCanvas
    {
        private readonly List<Size> _pageSizes = new();
        public IReadOnlyList<Size> PageSizes => _pageSizes;

        public override void BeginPage(Size size)
        {
            _pageSizes.Add(size);
        }
    }
}
