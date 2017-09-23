using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MySorts.Models.DemoSorter;

namespace MySorts.ViewModels
{
    public class DemoSortViewModel : BaseViewModel
    {
        public DemoSortViewModel(int[] arr, IEnumerable<IDemoSorter<int>> sorters)
        {
            SourceArray = new ObservableCollection<int>(arr);
            DemoSorterSteps = new ObservableCollection<DemoSorterResultViewModel>(
                sorters.Select(x => new DemoSorterResultViewModel(arr, x)));
            DemoSorterSteps.Insert(0, new DemoSorterResultViewModel(arr, null));
        }

        public ObservableCollection<int> SourceArray { get; }

        private ObservableCollection<DemoSorterResultViewModel> _demoSorterSteps;
        public ObservableCollection<DemoSorterResultViewModel> DemoSorterSteps
        {
            get => _demoSorterSteps;
            set
            {
                _demoSorterSteps = value;
                NotifyPropertyChanged(nameof(DemoSorterSteps));
            }
        }

        #region Commands
        private ICommand _nextStepCommand;
        public ICommand NextStepCommand
        {
            get
            {
                return _nextStepCommand ?? (_nextStepCommand =
                           new Command((param) => true, NextStepExecute));
            }
        }

        private void NextStepExecute()
        {
            foreach (var demoSorterResultViewModel in DemoSorterSteps)
            {
                if (demoSorterResultViewModel.Enumerator != null)
                {
                    if (!demoSorterResultViewModel.Enumerator.MoveNext())
                        demoSorterResultViewModel.Enumerator = null;
                    else
                    {
                        if (demoSorterResultViewModel.Enumerator.Current != null)
                            demoSorterResultViewModel.SetStep(demoSorterResultViewModel.Enumerator.Current);
                        demoSorterResultViewModel.StepCt++;
                    }
                }
            }
        }
        #endregion
    }
}
