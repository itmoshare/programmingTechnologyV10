using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MySorts.Models;

namespace MySorts.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
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
                var res = ArrayReader<int>.Read(ManualAddText);
                // TODO
                Thread.Sleep(5000);
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
                    var res = new BubleSorter<int>().Sort(arr);
                    //TODO
                    Thread.Sleep(5000);
                })
                .ContinueWith(task => IsSorting = false);
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

        #endregion
    }
}
