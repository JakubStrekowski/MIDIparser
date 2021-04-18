using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MIDIparser.ViewModels
{
    public enum EColorOptions
    {
        UpArrow = 0,
        RightArrow,
        LeftArrow,
        DownArrow,
        Background,
        UiBackground,
        UiText
    }
    class GeneralSettingsViewModel : INotifyPropertyChanged
    {
        private readonly int[] scaleOptions = {100, 50, 20, 10, 5, 2, 1};
        Color[] colorSettings = { Color.FromArgb(255, 185, 255, 255), Color.FromArgb(255, 63, 255, 13), Color.FromArgb(255, 162, 0, 255), Color.FromArgb(255, 255, 173, 0), Color.FromArgb(255, 106, 106, 106), Color.FromArgb(255, 79, 79, 79), Color.FromArgb(255, 255, 255, 255)};
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
                UpdateEditorSettings();
                OnPropertyChange("ScrollToCursor");
            }
        }
        public bool SnapToGrid
        {
            get => snapToGrid;
            set
            {
                snapToGrid = value;
                UpdateEditorSettings();
                OnPropertyChange("SnapToGrid");
            }
        }

        public long GridSize
        {
            get => gridSize;
            set
            {
                gridSize = value;
                UpdateEditorSettings();
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
                    UpdateEditorSettings();
                    OnPropertyChange("GridPadding");
                }
            }
        }

        public Color UpArrowColor
        {
            get => colorSettings[(int)EColorOptions.UpArrow];
            set
            {
                colorSettings[(int)EColorOptions.UpArrow] = value;
                OnPropertyChange("UpArrowColor");
                UpdateColorSettings();
            }
        }
        public Color RightArrowColor
        {
            get => colorSettings[(int)EColorOptions.RightArrow];
            set
            {
                colorSettings[(int)EColorOptions.RightArrow] = value;
                OnPropertyChange("RightArrowColor");
                UpdateColorSettings();
            }
        }
        public Color LeftArrowColor
        {
            get => colorSettings[(int)EColorOptions.LeftArrow];
            set
            {
                colorSettings[(int)EColorOptions.LeftArrow] = value;
                OnPropertyChange("LeftArrowColor");
                UpdateColorSettings();
            }
        }
        public Color DownArrowColor
        {
            get => colorSettings[(int)EColorOptions.DownArrow];
            set
            {
                colorSettings[(int)EColorOptions.DownArrow] = value;
                OnPropertyChange("DownArrowColor");
                UpdateColorSettings();
            }
        }
        public Color BackgroundColor
        {

            get => colorSettings[(int)EColorOptions.Background];
            set
            {
                colorSettings[(int)EColorOptions.Background] = value;
                OnPropertyChange("BackgroundColor");
                UpdateColorSettings();
            }
        }
        public Color UiColor
        {

            get => colorSettings[(int)EColorOptions.UiBackground];
            set
            {
                colorSettings[(int)EColorOptions.UiBackground] = value;
                OnPropertyChange("UiColor");
                UpdateColorSettings();
            }
        }
        public Color UiTextColor
        {

            get => colorSettings[(int)EColorOptions.UiText];
            set
            {
                colorSettings[(int)EColorOptions.UiText] = value;
                OnPropertyChange("UiTextColor");
                UpdateColorSettings();
            }
        }

        public GeneralSettingsViewModel()
        {
            PresentedScale = 3;
            EventSystem.Subscribe<OnMidiLoadedMessage>(GetDefaultParams);
            colorSettings = new Color[(int)EColorOptions.UiText + 1];
            UpArrowColor = Color.FromArgb(255, 185, 255, 255);
            RightArrowColor = Color.FromArgb(255, 63, 255, 13);
            LeftArrowColor = Color.FromArgb(255, 162, 0, 255);
            DownArrowColor = Color.FromArgb(255, 255, 173, 0);
            BackgroundColor = Color.FromArgb(255, 106, 106, 106);
            UiColor = Color.FromArgb(255, 79, 79, 79);
            UiTextColor = Color.FromArgb(255, 255, 255, 255);
        }

        #region EventHandlers
        public void GetDefaultParams(OnMidiLoadedMessage msg)
        {
            GridSize = msg.ticksPerquarterNote;
            UpArrowColor = Color.FromArgb(255, 185, 255, 255);
            RightArrowColor = Color.FromArgb(255, 63, 255, 13);
            LeftArrowColor = Color.FromArgb(255, 162, 0, 255);
            DownArrowColor = Color.FromArgb(255, 255, 173, 0);
            BackgroundColor = Color.FromArgb(255, 106, 106, 106);
            UiColor = Color.FromArgb(255, 79, 79, 79);
            UiTextColor = Color.FromArgb(255, 255, 255, 255);

            //RecalculateCanvasElements();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateEditorSettings()
        {
            EventSystem.Publish<OnGeneralSettingsChangeMessage>(
                new OnGeneralSettingsChangeMessage
                {
                    scrollToCursor = this.scrollToCursor,
                    snapToGrid = this.snapToGrid,
                    gridSize = this.gridSize,
                    gridPadding = this.gridPadding
                });
        }
        private void UpdateColorSettings()
        {
             EventSystem.Publish<OnGeneralSettingsColorChangeMessage>(
                 new OnGeneralSettingsColorChangeMessage
                 {
                     colorSettings = this.colorSettings
                 });
        }
    }
}
