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
        private readonly int[] scaleOptions = {100, 50, 20, 10, 5, 2, 1};
        private int presentedScale;
        private bool scrollToCursor;
        private bool snapToGrid;
        private long gridSize;
        private long gridPadding;
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

                EventSystem.Publish<OnGeneralSettingsChangeMessage>(
                    new OnGeneralSettingsChangeMessage
                    {
                        scrollToCursor = this.scrollToCursor,
                        snapToGrid = this.snapToGrid,
                        gridSize = this.gridSize,
                        gridPadding = this.gridPadding
                    });
                OnPropertyChange("ScrollToCursor");
            }
        }
        public bool SnapToGrid
        {
            get => snapToGrid;
            set
            {
                snapToGrid = value;

                EventSystem.Publish<OnGeneralSettingsChangeMessage>(
                    new OnGeneralSettingsChangeMessage
                    {
                        scrollToCursor = this.scrollToCursor,
                        snapToGrid = this.snapToGrid,
                        gridSize = this.gridSize,
                        gridPadding = this.gridPadding
                    });
                OnPropertyChange("SnapToGrid");
            }
        }

        public long GridSize
        {
            get => gridSize;
            set
            {
                gridSize = value;

                EventSystem.Publish<OnGeneralSettingsChangeMessage>(
                    new OnGeneralSettingsChangeMessage
                    {
                        scrollToCursor = this.scrollToCursor,
                        snapToGrid = this.snapToGrid,
                        gridSize = this.gridSize,
                        gridPadding = this.gridPadding
                    });
                OnPropertyChange("GridSize");
            }
        }
        public long GridPadding
        {
            get => gridPadding;
            set
            {
                if(value < gridSize)
                {
                    gridPadding = value;

                    EventSystem.Publish<OnGeneralSettingsChangeMessage>(
                        new OnGeneralSettingsChangeMessage
                        {
                            scrollToCursor = this.scrollToCursor,
                            snapToGrid = this.snapToGrid,
                            gridSize = this.gridSize,
                            gridPadding = this.gridPadding
                        }) ;
                    OnPropertyChange("GridPadding");
                }
            }
        }

        public GeneralSettingsViewModel()
        {
            PresentedScale = 3;
            EventSystem.Subscribe<OnMidiLoadedMessage>(GetDefaultGridSize);

        }

        #region EventHandlers
        public void GetDefaultGridSize(OnMidiLoadedMessage msg)
        {
            GridSize = msg.ticksPerQuarterNote;
            //RecalculateCanvasElements();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
