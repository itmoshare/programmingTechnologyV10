using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using MySorts.Models;
using MySorts.Models.DemoSorter;
using MySorts.Models.Sorters;
using MySorts.Views;

namespace MySorts.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MultipleSorter<int> _multipleSorter;
        private readonly SortResultsStorage<int> _sortResultsStorage;
        private int[] _lastResult;

        public MainViewModel()
        {
            _multipleSorter = new MultipleSorter<int>(new []
            {
                new SorterDescription<int>("Пузырек", new BubleSorter<int>()), 
                new SorterDescription<int>("QuickSort", new QuickSorter<int>()), 
                new SorterDescription<int>("Shell", new ShellSorter<int>())
            });
            SeriesCollection = new SeriesCollection(
                new CartesianMapper<SortResult<int>>()
                .X(x => x.ArrayLength)
                .Y(y => y.TotalTimeMs));
            _sortResultsStorage = new SortResultsStorage<int>(SeriesCollection);
            ClearExecute();
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

        private ICommand _demoSortCommand;
        public ICommand DemoSortCommand
        {
            get
            {
                return _demoSortCommand ?? (_demoSortCommand =
                           new Command((param) => true, DemoSortExecute));
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

        private ICommand _fileSaveCommand;
        public ICommand FileSaveCommand
        {
            get
            {
                return _fileSaveCommand ?? (_fileSaveCommand =
                           new Command((param) => true, FileSaveExecute));
            }
        }

        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand =
                           new Command((param) => true, ClearExecute));
            }
        }

        private void ManualAddExecute()
        {
            try
            {
                if (string.IsNullOrEmpty(ManualAddText))
                    return;
                DoSort(ArrayIO<int>.Read(ManualAddText));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        private void DemoSortExecute()
        {
            var demoSortWindow = new DemoSortWindow();
            demoSortWindow.ViewModel = new DemoSortViewModel(
                ArrayIO<int>.Read(ManualAddText), new IDemoSorter<int>[]
                {
                    new BubleDemoSorter<int>(), 
                    new ShellDemoSorter<int>(),
                    new QuickDemoSorter<int>(), 
                });
            demoSortWindow.ShowDialog();
        }

        private void FileAddExecute()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Txt|*.txt"
            };
            if (dialog.ShowDialog()== true)
            {
                Task.Factory.StartNew(async () =>
                {
                    int[] arr;
                    using (var s = dialog.OpenFile())
                    using (var fs = new StreamReader(s))
                    {
                        arr = ArrayIO<int>.Read(fs);
                    }
                    await DoSort(arr);
                });
            }
        }

        private void FileSaveExecute()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "TXT|*.txt"
            };
            if (dialog.ShowDialog() == true)
            {
                Task.Factory.StartNew(async () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsBusy = true;
                    });
                    using (var stream = dialog.OpenFile())
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        await ArrayIO<int>.Write(streamWriter, _lastResult);
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsBusy = false;
                    });
                });
            }
        }

        private void ClearExecute()
        {
            _lastResult = new int[0];
            _sortResultsStorage.Clear();
            _sortResultsStorage.AddResults(new[]
            {
                new SortResult<int>
                {
                    ArrayLength = 0,
                    TotalTimeMs = 0,
                    SorterDescription = new SorterDescription<int>("Пузырек", null)
                },
                new SortResult<int>
                {
                    ArrayLength = 0,
                    TotalTimeMs = 0,
                    SorterDescription = new SorterDescription<int>("QuickSort", null)
                },
                new SortResult<int>
                {
                    ArrayLength = 0,
                    TotalTimeMs = 0,
                    SorterDescription = new SorterDescription<int>("Shell", null)
                },
            });
        }

        private async Task DoSort(int[] array)
        {
            IsBusy = true;
            var res = await _multipleSorter.Sort(array);
            Application.Current.Dispatcher.Invoke(() =>
            {
                _lastResult = res.First().SortedArray;
                _sortResultsStorage.AddResults(res);
                IsBusy = false;
            });
        }
        #endregion

        #region Observable

        private bool _isBusy;
        public bool IsBusy
        {
            get=>_isBusy;
            set
            {
                _isBusy = value;
                NotifyPropertyChanged(nameof(IsBusy));
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
        #endregion

        class SortResultsStorage<TValue>
        {
            private Dictionary<string, List<SortResult<TValue>>> _results;

            private readonly SeriesCollection _series;

            public SortResultsStorage(SeriesCollection series)
            {
                _results = new Dictionary<string, List<SortResult<TValue>>>();
                _series = series;
            }

            public void Clear()
            {
                _results = new Dictionary<string, List<SortResult<TValue>>>();
                _series.Clear();
            }

            public void AddResults(IEnumerable<SortResult<TValue>> results)
            {
                foreach (var res in results)
                {
                    if (!_results.ContainsKey(res.SorterDescription.SortAlgName))
                    {
                        _results.Add(res.SorterDescription.SortAlgName, new List<SortResult<TValue>>());
                        _series.Add(new LineSeries
                        {
                            Title = res.SorterDescription.SortAlgName,
                            LineSmoothness = 0,
                            Values = new ChartValues<SortResult<TValue>>()
                        });
                    }

                    var curAlgResults = _results[res.SorterDescription.SortAlgName];
                    bool inserted = false;
                    for (int i = 0; i < curAlgResults.Count; i++)
                    {
                        // Get average result
                        if (curAlgResults[i].ArrayLength == res.ArrayLength)
                        {
                            var avres = new SortResult<TValue>
                            {
                                SorterDescription = res.SorterDescription,
                                ArrayLength = res.ArrayLength,
                                TotalTimeMs = (res.TotalTimeMs + curAlgResults[i].TotalTimeMs) / 2
                            };
                            InsertResult(i, avres, true);
                            inserted = true;
                            break;
                        }
                        if (curAlgResults[i].ArrayLength > res.ArrayLength)
                        {
                            InsertResult(i, res);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted)
                        InsertResult(curAlgResults.Count, res);
                }
            }

            private void InsertResult(int i, SortResult<TValue> res, bool rewrite = false)
            {
                if (rewrite)
                {
                    _results[res.SorterDescription.SortAlgName][i] = res;
                    _series.Single(x => x.Title == res.SorterDescription.SortAlgName).Values.RemoveAt(i);
                    _series.Single(x => x.Title == res.SorterDescription.SortAlgName).Values.Insert(i, res);
                }
                else
                {
                    _results[res.SorterDescription.SortAlgName].Insert(i, res);
                    _series.Single(x => x.Title == res.SorterDescription.SortAlgName).Values.Insert(i, res);
                }
            }
        }
    }
}
