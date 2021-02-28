﻿using MIDIparser.EventMessages;
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
        private bool isCreatingMove;
        private double newMovePosition;
        private int newMoveType;
        private bool checkScrollToCursor;
        private PreviewControlEnum controlEnum;
        private Line beginLine;
        private Line endLine;

        public GameMovementView()
        {
            InitializeComponent();
            EventSystem.Subscribe<OnCreateNoteElementMessage>(DrawNewElement);
            EventSystem.Subscribe<OnRedrawMusicMovesMessage>(CleanupCanvas);
            EventSystem.Subscribe<OnScrollToCursorChangeMessage>(GetScrollToCursor);
            EventSystem.Subscribe<OnChangePreviewControlMessage>(GetPreviewControlChange);
        }


        #region EventHandlers
        void DrawNewElement(OnCreateNoteElementMessage msg)
        {
            MakeNoteRectangle(msg.channelID, msg.startTime, msg.duration);
        }
        void CleanupCanvas(OnRedrawMusicMovesMessage msg)
        {
            if(msg.posBegin == 0 && msg.posEnd == 0)
            {
                for (int i = 0; i < PlayerMovementCanvas.Children.Count; i++)
                {
                    if (PlayerMovementCanvas.Children[i] is Rectangle)
                    {
                        if ((PlayerMovementCanvas.Children[i] as Rectangle).Name.Contains("Note"))
                        {
                            PlayerMovementCanvas.Children.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < PlayerMovementCanvas.Children.Count; i++)
                {
                    if (PlayerMovementCanvas.Children[i] is Rectangle)
                    {
                        if ((PlayerMovementCanvas.Children[i] as Rectangle).Name.Contains("Note"))
                        {
                            if ((double)(PlayerMovementCanvas.Children[i] as FrameworkElement).GetValue(Canvas.LeftProperty) > msg.posBegin 
                                && (double)(PlayerMovementCanvas.Children[i] as FrameworkElement).GetValue(Canvas.LeftProperty) < msg.posEnd )
                            {
                                PlayerMovementCanvas.Children.RemoveAt(i);
                                i--;
                                continue;
                            }
                        }
                    }
                }
            }
        }
        void GetScrollToCursor(OnScrollToCursorChangeMessage msg)
        {
            checkScrollToCursor = msg.newValue;
        }
        void GetPreviewControlChange(OnChangePreviewControlMessage msg)
        {
            controlEnum = msg.currentControl;
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
            rect.Name = "Note" + PlayerMovementCanvas.Children.Count;
            rect.SetValue(Canvas.TopProperty, (double)(noteNumber) * 32);
            rect.SetValue(Canvas.LeftProperty, (double)startTime);
            PlayerMovementCanvas.Children.Add(rect);
            return rect;
        }

        private void CanvasLeftClick(object sender, MouseButtonEventArgs e)
        {
            if(controlEnum != PreviewControlEnum.None)
            {
                if(controlEnum == PreviewControlEnum.SetBeginTime)
                {
                    SetBeginTimeImplementation(e);
                }
                if (controlEnum == PreviewControlEnum.SetEndTime)
                {
                    SetEndTimeImplementiation(e);
                }
                if(controlEnum == PreviewControlEnum.CreateNewMove)
                {
                    CreateNewMoveImplementation(e);
                }
                if(controlEnum == PreviewControlEnum.DeleteMove)
                {
                    if(e.Source is Rectangle)
                    {
                        if ((e.Source as Rectangle).Name.Contains("Note"))
                        {
                            PlayerMovementCanvas.Children.Remove(e.Source as FrameworkElement);
                        }
                    }
                }
            }
        }

        private void CreateNewMoveImplementation(MouseButtonEventArgs e)
        {
            if (isCreatingMove)
            {
                if (e.GetPosition(PlayerMovementCanvas).X > newMovePosition)
                {
                    MakeNoteRectangle(newMoveType, (long)newMovePosition, (long)e.GetPosition(PlayerMovementCanvas).X - (long)newMovePosition);
                }
                else
                {
                    MessageBox.Show("End position has to be later than begin position");
                }
                newMovePosition = -1;
                isCreatingMove = false;
            }
            else if (!isCreatingMove)
            {
                if (e.GetPosition(PlayerMovementCanvas).Y < 130)
                {
                    if (e.GetPosition(PlayerMovementCanvas).Y < 32) newMoveType = 0;
                    if (e.GetPosition(PlayerMovementCanvas).Y >= 32 && e.GetPosition(PlayerMovementCanvas).Y < 65) newMoveType = 1;
                    if (e.GetPosition(PlayerMovementCanvas).Y >= 65 && e.GetPosition(PlayerMovementCanvas).Y < 97) newMoveType = 2;
                    if (e.GetPosition(PlayerMovementCanvas).Y >= 97 && e.GetPosition(PlayerMovementCanvas).Y < 130) newMoveType = 3;
                    newMovePosition = e.GetPosition(PlayerMovementCanvas).X;
                    isCreatingMove = true;
                }
                else
                {
                    newMovePosition = -1;
                }
            }
        }

        private void SetEndTimeImplementiation(MouseButtonEventArgs e)
        {
            if (endLine != null)
            {
                PlayerMovementCanvas.Children.Remove(endLine);
            }
            endLine = new Line
            {
                Stroke = new SolidColorBrush(Colors.Brown),
                Y1 = 0,
                Y2 = PlayerMovementCanvas.ActualHeight - 32,
                X1 = e.GetPosition(PlayerMovementCanvas).X,
                X2 = e.GetPosition(PlayerMovementCanvas).X,
                StrokeThickness = 1,
                StrokeDashArray = { 5, 5 }
            };
            endLine.Name = "EndLine";
            PlayerMovementCanvas.Children.Add(endLine);

            EventSystem.Publish<OnReturnPreviewClickOffsetMessage>(
                new OnReturnPreviewClickOffsetMessage
                {
                    offset = e.GetPosition(PlayerMovementCanvas).X
                });
            controlEnum = PreviewControlEnum.None;
        }

        private void SetBeginTimeImplementation(MouseButtonEventArgs e)
        {
            if (beginLine != null)
            {
                PlayerMovementCanvas.Children.Remove(beginLine);
            }
            beginLine = new Line
            {
                Stroke = new SolidColorBrush(Colors.RosyBrown),
                Y1 = 0,
                Y2 = PlayerMovementCanvas.ActualHeight - 32,
                X1 = e.GetPosition(PlayerMovementCanvas).X,
                X2 = e.GetPosition(PlayerMovementCanvas).X,
                StrokeThickness = 1,
                StrokeDashArray = { 5, 5 }
            };
            beginLine.Name = "BeginLine";
            PlayerMovementCanvas.Children.Add(beginLine);

            EventSystem.Publish<OnReturnPreviewClickOffsetMessage>(
                new OnReturnPreviewClickOffsetMessage
                {
                    offset = e.GetPosition(PlayerMovementCanvas).X
                });
            controlEnum = PreviewControlEnum.None;
        }
    }
}
