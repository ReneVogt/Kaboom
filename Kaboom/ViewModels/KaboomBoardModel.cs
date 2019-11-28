using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Com.Revo.Games.Kaboom.ViewModels {
    public class KaboomBoardModel
    {
        public ObservableCollection<ObservableCollection<KaboomCellModel>> Cells { get; } =
            new ObservableCollection<ObservableCollection<KaboomCellModel>>();
        
    }
}
