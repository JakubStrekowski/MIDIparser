using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.ViewModels
{
    class GeneralSettingsViewModel : INotifyPropertyChanged
    {
        private readonly int[] scaleOptions = {1, 2, 5, 10, 20, 50, 100};
        private int presentedScale;
        private bool scrollToCursor;
        public int PresentedScale
        {
            get => presentedScale;
            set
            {
                presentedScale = value;

                EventSystem.Publish<OnSongPreviewScaleChangeMessage>(
                    new OnSongPreviewScaleChangeMessage
                    {
                        newScale = scaleOptions[presentedScale]
                    });
                OnPropertyChange("PresentedScale");
            }
        }
        public bool ScrollToCursor
        {
            get => scrollToCursor;
            set
            {
                scrollToCursor = value;

                EventSystem.Publish<OnScrollToCursorChangeMessage>(
                    new OnScrollToCursorChangeMessage
                    {
                        newValue = value
                    });
                OnPropertyChange("ScrollToCursor");
            }
        }

        public GeneralSettingsViewModel()
        {
            PresentedScale = 3;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
