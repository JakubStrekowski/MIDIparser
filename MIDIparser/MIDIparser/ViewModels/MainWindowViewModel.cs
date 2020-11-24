using Melanchall.DryWetMidi.Core;
using Microsoft.Win32;
using MIDIparser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MIDIparser.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region members
        private DancerSong dancerSong;

        #endregion

        #region constructors
        public MainWindowViewModel()
        {

        }
        #endregion


        #region commands
        private ICommand _cmdOpenFileClick;

        public ICommand CmdOpenFileClick
        {
            get
            {
                if (_cmdOpenFileClick == null)
                {
                    _cmdOpenFileClick = new RelayCommand<ICommand>(x => OpenMidiFile(x));
                }
                return _cmdOpenFileClick;
            }
        }

        private void OpenMidiFile(object item)
        {
            dancerSong = new DancerSong();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MIDI files (*.mid)|*.mid;";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
                dancerSong.midi = MidiFile.Read(openFileDialog.FileName);
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
