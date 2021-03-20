using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MIDIparser.ViewModels
{
    class ExportSettingsViewModel : INotifyPropertyChanged
    {
        private long maxMoveTapThreshold;

        public long MaxMoveTapThreshold { get => maxMoveTapThreshold; 
            set
            {
                maxMoveTapThreshold = value;
                OnPropertyChange("MaxMoveTapThreshold");
            }
        }

        public ExportSettingsViewModel()
        {
            MaxMoveTapThreshold = 720;
        }

        #region commands
        private ICommand _cmdExportClick;

        public ICommand CmdExportClick
        {
            get
            {
                if (_cmdExportClick == null)
                {
                    _cmdExportClick = new RelayCommand<ICommand>(x => StartExporting());
                }
                return _cmdExportClick;
            }
        }

        #endregion

        #region commandMethods
        private void StartExporting()
        {

            EventSystem.Publish(
                new OnStartExportMessage
                {
                    MaxLengthToTap = MaxMoveTapThreshold
                });
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
