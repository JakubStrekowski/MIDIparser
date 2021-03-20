using Melanchall.DryWetMidi.Interaction;
using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MIDIparser.ViewModels
{
    class GameMovesSettingsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> channelTitles;
        private string selectedChannel;
        private double beginGenerationTime;
        private double endGenerationTime;
        private PreviewControlEnum movesPaletteControl;
        private bool isEditingActive;

        public ObservableCollection<string> ChannelTitles
        {
            get { return channelTitles; }
            set
            {
                channelTitles = value;
                OnPropertyChange("ChannelTitles");
            }
        }
        public string SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                selectedChannel = value;
                OnPropertyChange("SelectedChannel");

                EventSystem.Publish<OnMovementChannelChangeMessage>(
                    new OnMovementChannelChangeMessage
                    {
                        channelID = channelTitles.IndexOf(selectedChannel)
                    });
            }
        }
        public double BeginGenerationTime
        {
            get { return beginGenerationTime; }
            set
            {
                beginGenerationTime = value;
                OnPropertyChange("BeginGenerationTime");
            }
        }
        public double EndGenerationTime
        {
            get { return endGenerationTime; }
            set
            {
                endGenerationTime = value;
                OnPropertyChange("EndGenerationTime");
            }
        }

        public bool IsEditingActive
        {
            get { return isEditingActive; }
            set
            {
                isEditingActive = value;
                OnPropertyChange("IsEditingActive");
            }
        }
        public bool IsPaletteInactive
        {
            get
            {
                return MovesPaletteControl == PreviewControlEnum.None || MovesPaletteControl == PreviewControlEnum.SetBeginTime || MovesPaletteControl == PreviewControlEnum.SetEndTime;
            }
            set
            {
                MovesPaletteControl = PreviewControlEnum.None;
            }
        }
        public bool IsPaletteCreate
        {
            get
            {
                return MovesPaletteControl == PreviewControlEnum.CreateNewMove ? true : false;
            }
            set
            {
                MovesPaletteControl = PreviewControlEnum.CreateNewMove;
            }
        }
        public bool IsPaletteEdit
        {
            get
            {
                return MovesPaletteControl == PreviewControlEnum.EditExistingMove ? true : false;
            }
            set
            {
                MovesPaletteControl = PreviewControlEnum.EditExistingMove;
            }
        }
        public bool IsPaletteDelete
        {
            get
            {
                return MovesPaletteControl == PreviewControlEnum.DeleteMove ? true : false;
            }
            set
            {
                MovesPaletteControl = PreviewControlEnum.DeleteMove;
            }
        }

        public PreviewControlEnum MovesPaletteControl
        {
            get
            {
                return movesPaletteControl;
            }
            set
            {
                movesPaletteControl = value;
                OnPropertyChange("MovesPaletteControl");
                OnPropertyChange("IsPaletteInactive");
                OnPropertyChange("IsPaletteCreate");
                OnPropertyChange("IsPaletteEdit");
                OnPropertyChange("IsPaletteDelete");

                EventSystem.Publish<OnChangePreviewControlMessage>(
                    new OnChangePreviewControlMessage
                    {
                        currentControl = this.MovesPaletteControl
                    });
            }
        }

        public GameMovesSettingsViewModel()
        {
            EventSystem.Subscribe<OnMidiLoadedMessage>(CopyChannelSelection);
            EventSystem.Subscribe<OnReturnPreviewClickOffsetMessage>(GetPreviewOffsetValue);
            BeginGenerationTime = 0;
            EndGenerationTime = Int32.MaxValue;
            IsEditingActive = false;
            MovesPaletteControl = PreviewControlEnum.None;
        }


        #region commands
        private ICommand _cmdSetBeginTimeClick;
        private ICommand _cmdSetEndTimeClick;
        private ICommand _cmdStartGenerate;
        private ICommand _cmdCleanSelectedMoves;
        private ICommand _cmdCleanAllMoves;

        public ICommand CmdSetBeginTime
        {
            get
            {
                if (_cmdSetBeginTimeClick == null)
                {
                    _cmdSetBeginTimeClick = new RelayCommand<ICommand>(x => SetBeginTime());
                }
                return _cmdSetBeginTimeClick;
            }
        }
        public ICommand CmdSetEndTime
        {
            get
            {
                if (_cmdSetEndTimeClick == null)
                {
                    _cmdSetEndTimeClick = new RelayCommand<ICommand>(x => SetEndTime());
                }
                return _cmdSetEndTimeClick;
            }
        }
        public ICommand CmdStartGenerate
        {
            get
            {
                if (_cmdStartGenerate == null)
                {
                    _cmdStartGenerate = new RelayCommand<ICommand>(x => StartGenerate());
                }
                return _cmdStartGenerate;
            }
        }
        public ICommand CmdCleanSelectedMoves
        {
            get
            {
                if (_cmdCleanSelectedMoves == null)
                {
                    _cmdCleanSelectedMoves = new RelayCommand<ICommand>(x => CleanSelectedMoves());
                }
                return _cmdCleanSelectedMoves;
            }
        }
        public ICommand CmdCleanAllMoves
        {
            get
            {
                if (_cmdCleanAllMoves == null)
                {
                    _cmdCleanAllMoves = new RelayCommand<ICommand>(x => CleanAllMoves());
                }
                return _cmdCleanAllMoves;
            }
        }
        #endregion

        #region commandMethods
        private void SetBeginTime()
        {
            MovesPaletteControl = PreviewControlEnum.SetBeginTime;
        }
        private void SetEndTime()
        {
            MovesPaletteControl = PreviewControlEnum.SetEndTime;
        }
        private void StartGenerate()
        {
            if(BeginGenerationTime > EndGenerationTime)
            {
                MessageBox.Show("Begin time can't be later than end time.");
                return;
            }
            EventSystem.Publish<OnStartGeneratingMovesMessage>(
                new OnStartGeneratingMovesMessage
                {
                    startTime = BeginGenerationTime,
                    endTime = EndGenerationTime
                }) ;
        }
        private void CleanSelectedMoves()
        {
            if (BeginGenerationTime > EndGenerationTime)
            {
                MessageBox.Show("Begin time can't be later than end time.");
                return;
            }
            EventSystem.Publish<OnRedrawMusicMovesMessage>(
                new OnRedrawMusicMovesMessage
                {
                    posBegin = BeginGenerationTime,
                    posEnd = EndGenerationTime
                });
        }
        private void CleanAllMoves()
        {
            string sMessageBoxText = "It will erase all moves. \n Do you want to continue?";
            string sCaption = "Warning";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    EventSystem.Publish<OnRedrawMusicMovesMessage>(
                    new OnRedrawMusicMovesMessage
                    {
                        posBegin = 0,
                        posEnd = 0
                    });
                    break;

                case MessageBoxResult.No:
                    return;
            }
        }
        #endregion


        #region EventHandlers

        public void CopyChannelSelection(OnMidiLoadedMessage msg)
        {
            ChannelTitles = new ObservableCollection<string>(msg.midiChannelsTitles);
            ChannelTitles.RemoveAt(0);
            SelectedChannel = ChannelTitles[0];
            EndGenerationTime = TimeConverter.ConvertFrom(msg.playback.GetDuration(TimeSpanType.Midi), msg.playback.TempoMap);
            IsEditingActive = true;
        }

        public void GetPreviewOffsetValue(OnReturnPreviewClickOffsetMessage msg)
        {
            switch(MovesPaletteControl)
            {
                case PreviewControlEnum.SetBeginTime:
                    BeginGenerationTime = msg.offset;
                    MovesPaletteControl = PreviewControlEnum.None;
                    break;
                case PreviewControlEnum.SetEndTime:
                    EndGenerationTime = msg.offset;
                    MovesPaletteControl = PreviewControlEnum.None;
                    break;
                default:break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
