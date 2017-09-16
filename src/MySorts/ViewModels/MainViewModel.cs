using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using MySorts.Models;

namespace MySorts.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MultipleSorter<int> _multipleSorter;

        public MainViewModel()
        {
            _multipleSorter = new MultipleSorter<int>(new []
            {
                new SorterDescription<int>("Пузырек", new BubleSorter<int>()), 
                //new SorterDescription<int>("QuickSort", new QuickSorter<int>()), 
                new SorterDescription<int>("Stooge", new StoogeSorter<int>())
            });
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Пузырек",
                    Values = new ChartValues<long>(new []{ 0L })
                },
                new LineSeries
                {
                    Title = "QuickSort",
                    Values = new ChartValues<long>(new []{ 0L })
                },
                new LineSeries
                {
                    Title = "Stooge",
                    Values = new ChartValues<long>(new []{ 0L })
                }
            };
            Labels = new ObservableCollection<string>(new []{ "0" });
        }

        #region Commands
        private ICommand _manualAddCommand;
        public ICommand ManualAddCommand
        {
            get
            {
                return _manualAddCommand ?? (_manualAddCommand =
                    new Command((param) => true, ManualAddExecute));
            }
        }

        private ICommand _fileAddCommand;
        public ICommand FileAddCommand
        {
            get
            {
                return _fileAddCommand ?? (_fileAddCommand =
                           new Command((param) => true, FileAddExecute));
            }
        }

        private void ManualAddExecute()
        {
            try
            {
                if (string.IsNullOrEmpty(ManualAddText))
                    return;
                DoSort(ArrayReader<int>.Read(ManualAddText));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        private void FileAddExecute()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Txt|*.txt"
            };
            if (dialog.ShowDialog()== true)
            {
                IsSorting = true;
                Task.Run(() =>
                {
                    int[] arr;
                    using (var s = dialog.OpenFile())
                    using (var fs = new StreamReader(s))
                    {
                        arr = ArrayReader<int>.Read(fs);
                    }
                    DoSort(arr);
                })
                .ContinueWith(task => IsSorting = false);
            }
        }

        private void DoSort(int[] array)
        {
            IsSorting = true;
            _multipleSorter
                .Sort(array)
                .ContinueWith(task =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var sortResult in task.Result)
                        {
                            AddSortResult(sortResult);
                        }
                        IsSorting = false;
                    });
                });
        }

        private void AddSortResult(SortResult<int> sortResult)
        {
            var ser = SeriesCollection.Single(x => x.Title == sortResult.SorterDescription.SortAlgName);
            for (int i = 0; i < ser.Values.Count; i++)
            {
                if ((long) ser.Values[i] < sortResult.TotalTimeMs)
                {
                    ser.Values.Insert(i, sortResult.TotalTimeMs);
                    Labels.Insert(i, sortResult.ArrayLength.ToString());
                }
            }
        }
        #endregion

        #region Observable

        private bool _isSorting;
        public bool IsSorting
        {
            get=>_isSorting;
            set
            {
                _isSorting = value;
                NotifyPropertyChanged(nameof(IsSorting));
            }
        }

        private string _manulAddText;
        public string ManualAddText
        {
            get => _manulAddText;
            set
            {
                _manulAddText = value;
                NotifyPropertyChanged(nameof(ManualAddText));
            }
        }

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                NotifyPropertyChanged(nameof(SeriesCollection));
            }
        }

        private ObservableCollection<string> _labels;
        public ObservableCollection<string> Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                NotifyPropertyChanged(nameof(Labels));
            }
        }
        #endregion
    }
}
