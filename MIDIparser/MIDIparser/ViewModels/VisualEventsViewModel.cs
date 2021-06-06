using Microsoft.Win32;
using MIDIparser.EventMessages;
using MIDIparser.Helpers;
using MIDIparser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MIDIparser.ViewModels
{
    class VisualEventsViewModel : INotifyPropertyChanged
    {
        private static VisualEventsViewModel _instance;

        public VisualEventsViewModel Instance
        {
            get => _instance;
            set => _instance = value;
        }

        private IEnumerable<string> eventTypesToSelect = new List<string> {"CreateObject",
        "DeleteObject",
        "ChangeColorObjectLinear",
        "ChangeColorObjectArc",
        "ChangePosObjectLinear",
        "ChangePosObjectArc",
        "ChangeRotObjectLinear",
        "ChangeRotObjectArc",
        "ChangeSprite"};
        private int selectedEventType;
        private ObservableCollection<VisualEventBase> allVisualEvents;
        private int selectedEventID;
        private VisualEventBase selectedEvent;
        private int nextObjectID = 0;

        public VisualEventsViewModel()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            allVisualEvents = new ObservableCollection<VisualEventBase>();
            SelectedEventType = 0;
            EventSystem.Subscribe<OnAskForVisualEventsMessage>(SendEventsCollection);
        }

        public IEnumerable<string> EventTypesToSelect
        {
            get => eventTypesToSelect;
            private set
            {
                eventTypesToSelect = value;
                OnPropertyChange("EventTypesToSelect");
            }
        }

        public int SelectedEventType
        {
            get => selectedEventType;
            set
            {
                selectedEventType = value;
                OnPropertyChange("SelectedEventType");
                OnPropertyChange("EventTypesToSelect");
            }
        }

        public int SelectedEventID
        {
            get => selectedEventID;
            set
            {
                selectedEventID = value;
                OnPropertyChange("SelectedEventID");
            }
        }

        public VisualEventBase SelectedEvent
        {
            get
            {
                if (allVisualEvents.Count <= SelectedEventID)
                {
                    return null;
                }
                return allVisualEvents[SelectedEventID];
            }
            set
            {
                selectedEvent = value;
                SelectedEventID = allVisualEvents.IndexOf(selectedEvent);
                SelectedEventType = (int)SelectedEvent.eventType;
                OnPropertyChange("SelectedEvent");
                OnPropertyChange("AllVisualEvents");
            }
        }

        public ObservableCollection<VisualEventBase> AllVisualEvents
        {
            get => allVisualEvents;
            private set
            {
                allVisualEvents = value;
                OnPropertyChange("AllVisualEvents");
            }
        }

        #region effectParams
        private long startTime;
        //create
        private string spritePath;
        BitmapImage previewImage;
        //setColor
        private Color color;
        //linear/arc  events
        private long eventDuration;
        //positioning
        private float posX;
        private float posY;
        private float posZ;

        private float rotation;


        public long StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                OnPropertyChange("StartTime");
            }
        }

        public string SpritePath
        {
            get => spritePath;
            set
            {
                spritePath = value;
                OnPropertyChange("SpritePath");
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChange("Color");
            }
        }

        public long EventDuration
        {
            get => eventDuration;
            set
            {
                eventDuration = value;
                OnPropertyChange("EventDuration");
            }
        }

        public BitmapImage SpritePreview
        {
            get => previewImage;
            set
            {
                previewImage = value;
                OnPropertyChange("SpritePreview");
            }
        }
        public float PosX
        {
            get => posX;
            set
            {
                posX = value;
                OnPropertyChange("PosX");
            }
        }
        public float PosY
        {
            get => posY;
            set
            {
                posY = value;
                OnPropertyChange("PosY");
            }
        }
        public float PosZ
        {
            get => posZ;
            set
            {
                posZ = value;
                OnPropertyChange("PosZ");
            }
        }
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                OnPropertyChange("Rotation");
            }
        }
        #endregion


        #region commands
        private ICommand _cmdCreateEvent;
        private ICommand _cmdSetSprite;
        private ICommand _cmdDeleteEvent;


        public ICommand CmdCreateEvent
        {
            get
            {
                if (_cmdCreateEvent == null)
                {
                    _cmdCreateEvent = new RelayCommand<ICommand>(x => CreateEvent());
                }
                return _cmdCreateEvent;
            }
        }
        public ICommand CmdSetSprite
        {
            get
            {
                if (_cmdSetSprite == null)
                {
                    _cmdSetSprite = new RelayCommand<ICommand>(x => SelectImageFile());
                }
                return _cmdSetSprite;
            }
        }
        public ICommand CmdDeleteEvent
        {
            get
            {
                if (_cmdDeleteEvent == null)
                {
                    _cmdDeleteEvent = new RelayCommand<ICommand>(x => DeleteSelectedEvent());
                }
                return _cmdDeleteEvent;
            }
        }
        #endregion
        #region commandMethods
        void CreateEvent()
        {
            switch (SelectedEventType)
            {
                case (int)VisualEventTypeEnum.CreateObject:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateCreateDestroyEvent(nextObjectID, StartTime,
                                VisualEventTypeEnum.CreateObject, allVisualEvents.ToList(), SpritePath, PosX, PosY));
                            nextObjectID++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.DeleteObject:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateCreateDestroyEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.DeleteObject, allVisualEvents.ToList()));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangeColorObjectLinear:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangeColorLinearEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangeColorObjectLinear, allVisualEvents.ToList(), new ArgbColor(Color), EventDuration));
                            StartTime += EventDuration;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangeColorObjectArc:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangeColorArcEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangeColorObjectArc, allVisualEvents.ToList(), new ArgbColor(Color), EventDuration));
                            StartTime += EventDuration;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangePosObjectLinear:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangePositionLinearEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangePosObjectLinear, allVisualEvents.ToList(), EventDuration, PosX, PosY));
                            StartTime += EventDuration;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangePosObjectArc:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangePositionDampingEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangePosObjectArc, allVisualEvents.ToList(), EventDuration, PosX, PosY));
                            StartTime += EventDuration;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangeRotObjectLinear:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangeRotationLinearEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangeRotObjectLinear, allVisualEvents.ToList(), EventDuration, Rotation));
                            StartTime += EventDuration;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangeRotObjectArc:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangeRotationArcEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangeRotObjectArc, allVisualEvents.ToList(), EventDuration, Rotation));
                            StartTime += EventDuration;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                case (int)VisualEventTypeEnum.ChangeSprite:
                    {
                        try
                        {
                            allVisualEvents.Add(VisualEffectsFactory.InstantiateChangeSpriteEvent(SelectedEvent.objectId, StartTime,
                                VisualEventTypeEnum.ChangeSprite, allVisualEvents.ToList(), SpritePath));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                    }
                default: break;
            }
            IOrderedEnumerable<VisualEventBase> ordered= allVisualEvents.OrderBy(x => x.objectId);
            AllVisualEvents = new ObservableCollection<VisualEventBase>(ordered.ToList());
        }
        private void SelectImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png)|*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SpritePath = new string(openFileDialog.FileName.ToCharArray());
                SpritePreview = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void DeleteSelectedEvent()
        {
            try
            {
                if (selectedEvent.eventType == VisualEventTypeEnum.CreateObject)
                {
                    if (allVisualEvents.ToList().Exists(x => x.objectId == selectedEvent.objectId && x.eventType != VisualEventTypeEnum.CreateObject))
                    {
                        throw new Exception("Can't delete 'Create' event when there are already other events binded to this object. Delete other events first");
                    }
                    allVisualEvents.Remove(SelectedEvent);
                }
                else
                {
                    allVisualEvents.Remove(SelectedEvent);
                }
                CheckObjectIDIntegrity();
                OnPropertyChange("AllVisualEvents");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void CheckObjectIDIntegrity()
        {
            int checkingID = 0;
            for(int i = 0; i < allVisualEvents.Count; i++)
            {
                if (checkingID != allVisualEvents[i].objectId && checkingID == allVisualEvents[i].objectId + 1)
                {
                    checkingID++;
                }
                else if (checkingID != allVisualEvents[i].objectId && checkingID != allVisualEvents[i].objectId + 1)
                {
                    checkingID++;
                    foreach(VisualEventBase visualEvent in allVisualEvents.Where(x=>x.objectId == allVisualEvents[i].objectId))
                    {
                        visualEvent.objectId = checkingID;
                    }
                }
            }
        }

        #endregion

        #region Events
        void SendEventsCollection(OnAskForVisualEventsMessage msg)
        {
            EventSystem.Publish(new OnSendBackVisualEventsMessage
            {
                allVisualEvents = this.allVisualEvents
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
