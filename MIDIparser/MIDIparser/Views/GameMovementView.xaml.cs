using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MIDIparser.Views
{
    /// <summary>
    /// Interaction logic for GameMovementView.xaml
    /// </summary>
    public partial class GameMovementView : UserControl
    {
        private readonly Color[] channelToColor = { Color.FromRgb(0x95, 0xff, 0xff), Color.FromRgb(0x95, 0xff, 0x9c), Color.FromRgb(0xa8, 0x95, 0xff), Color.FromRgb(0xff, 0xd3, 0x95)};
        private bool checkScrollToCursor;

        public GameMovementView()
        {
            InitializeComponent();
            EventSystem.Subscribe<OnCreateNoteElementMessage>(DrawNewElement);
            EventSystem.Subscribe<OnRedrawMusicMovesMessage>(CleanupCanvas);
            EventSystem.Subscribe<OnScrollToCursorChangeMessage>(GetScrollToCursor);
        }


        #region EventHandlers
        void DrawNewElement(OnCreateNoteElementMessage msg)
        {
            MakeNoteRectangle(msg.channelID, msg.startTime, msg.duration);
        }
        void CleanupCanvas(OnRedrawMusicMovesMessage msg)
        {
            PlayerMovementCanvas.Children.RemoveRange(2, PlayerMovementCanvas.Children.Count - 2);
        }
        void GetScrollToCursor(OnScrollToCursorChangeMessage msg)
        {
            checkScrollToCursor = msg.newValue;
        }
        #endregion

        public Rectangle MakeNoteRectangle(int noteNumber, long startTime, long duration)
        {
            Rectangle rect = new Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Gray),
                Fill = new SolidColorBrush(channelToColor[noteNumber]),

                Width = (double)duration,
                Height = 32
            };
            rect.SetValue(Canvas.TopProperty, (double)(noteNumber) * 32);
            rect.SetValue(Canvas.LeftProperty, (double)startTime);
            PlayerMovementCanvas.Children.Add(rect);
            return rect;
        }
    }
}
