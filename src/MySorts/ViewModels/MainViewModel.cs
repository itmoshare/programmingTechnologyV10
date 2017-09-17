using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Microsoft.Win32;
using MySorts.Models;

namespace MySorts.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MultipleSorter<int> _multipleSorter;
        private readonly SortResultsStorage<int> _sortResultsStorage;

        public MainViewModel()
        {
            _multipleSorter = new MultipleSorter<int>(new []
            {
                new SorterDescription<int>("Пузырек", new BubleSorter<int>()), 
                //new SorterDescription<int>("QuickSort", new QuickSorter<int>()), 
                new SorterDescription<int>("Stooge", new StoogeSorter<int>())
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

        private ICommand _fileAddCommand;
        public ICommand FileAddCommand
        {
            get
            {
                return _fileAddCommand ?? (_fileAddCommand =
                           new Command((param) => true, FileAddExecute));
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
                Task.Run(async () =>
                {
                    int[] arr;
                    using (var s = dialog.OpenFile())
                    using (var fs = new StreamReader(s))
                    {
                        arr = ArrayReader<int>.Read(fs);
                    }
                    await DoSort(arr);
                });
            }
        }

        private void ClearExecute()
        {
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
                    SorterDescription = new SorterDescription<int>("Stooge", null)
                },
            });
        }

        private async Task DoSort(int[] array)
        {
            IsSorting = true;
            var res = await _multipleSorter.Sort(array);
            Application.Current.Dispatcher.Invoke(() =>
            {
                _sortResultsStorage.AddResults(res);
                IsSorting = false;
            });
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
