using System;

namespace Com.Revo.Games.Kaboom.ViewModels {
    public sealed class KaboomCellClickEventArgs : EventArgs
    {
        public KaboomCellModel ClickedCell { get; }
        public KaboomCellClickType ClickType { get; }
        public KaboomCellClickEventArgs(KaboomCellModel clickedCell, KaboomCellClickType clickType)
        {
            ClickedCell = clickedCell;
            ClickType = clickType;
        }

        public override string ToString() => $"Cell: ({ClickedCell?.X}/{ClickedCell?.Y}/{ClickedCell?.State}) Type: {ClickType}";
    }
}
