using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MySorts.Models.DemoSorter;

namespace MySorts.ViewModels
{
    public class DemoSorterResultViewModel : BaseViewModel
    {
        public IDemoSorter<int> DemoSorter { get; set; }

        public IEnumerator<DemoSortStep<int>> Enumerator;

        public DemoSorterResultViewModel(int[] arr, IDemoSorter<int> sorter)
        {
            DemoSorter = sorter;
            if (sorter == null)
            {
                Enumerator = null;
                CurResult = arr.Select(y => new ColoredIntViewModel { Data = y, SolidColorBrush = new SolidColorBrush(Colors.Black) }).ToArray();
            }
            else
            {
                Enumerator = sorter.Steps(arr.ToArray()).GetEnumerator();
                CurResult = arr.Select(y => new ColoredIntViewModel{ Data = y, SolidColorBrush = new SolidColorBrush(Colors.Black)}).ToArray();
            }
        }

        #region Observable
        private ColoredIntViewModel[] _curResult;
        public ColoredIntViewModel[] CurResult
        {
            get => _curResult;
            set
            {
                _curResult = value;
                NotifyPropertyChanged(nameof(CurResult));
            }
        }

        private int _stepCt;
        public int StepCt
        {
            get => _stepCt;
            set
            {
                _stepCt = value;
                NotifyPropertyChanged(nameof(StepCt));
            }
        }
        #endregion

        public class ColoredIntViewModel : BaseViewModel
        {
            public SolidColorBrush SolidColorBrush { get; set; }

            public FontWeight FontWeight { get; set; }

            public int Data { get; set; }
        }

        public void SetStep(DemoSortStep<int> step)
        {
            CurResult = step.NewArr.Select(x => new ColoredIntViewModel
            {
                Data = x,
                SolidColorBrush = new SolidColorBrush(Colors.Black),
            }).ToArray();
            CurResult[step.SwappedIndex1].SolidColorBrush = new SolidColorBrush(Colors.Red);
            CurResult[step.SwappedIndex1].FontWeight = FontWeights.Bold; 
            CurResult[step.SwappedIndex2].SolidColorBrush = new SolidColorBrush(Colors.Red);
            CurResult[step.SwappedIndex2].FontWeight = FontWeights.Bold;
        }
    }
}
