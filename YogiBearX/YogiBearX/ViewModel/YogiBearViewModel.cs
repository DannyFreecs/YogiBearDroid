using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using YogiBearX.Model;

namespace YogiBearX.ViewModel
{
    public class YogiBearViewModel : ViewModelBase
    {
        private YogiBearModel model;
        private bool paused = true;

        //properties
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand StepCommand { get; set; }
        public ObservableCollection<GameField> Fields { get; set; }

        public Int32 ColumnCount { get { return model.Map.Count; } }
        //játékidő
        public String GameTime { get { return TimeSpan.FromSeconds(model.GameTime).ToString(@"mm\:ss"); } }
        //Kosarak száma
        public String Baskets { get { return (model.Baskets).ToString(); } }

        //Events
        public event EventHandler NewGame;
        public event EventHandler<GameOverEventArgs> VM_GameOver;
        //Ctor
        public YogiBearViewModel(YogiBearModel m)
        {
            model = m;
            model.GameOver += new EventHandler<GameOverEventArgs>(OnVM_GameOver);

            NewGameCommand = new DelegateCommand(param => { OnNewGame(); CopyToFields(); });
            PauseCommand = new DelegateCommand(param => { OnPause(); });
            StepCommand = new DelegateCommand(param => MoveBear(Convert.ToInt32(param)));

            Fields = new ObservableCollection<GameField>();
            model.Refresh += RefreshTable;
        }

        public void CopyToFields()
        {
            OnPropertyChanged("ColumnCount");
            Fields.Clear();
            for (Int32 i = 0; i < model.Map.Count; i++)
            {
                for (Int32 j = 0; j < model.Map.Count; j++)
                {
                    Fields.Add(new GameField
                    {
                        Value = model.Map[i][j],
                        X = i,
                        Y = j
                    });
                }
            }
            OnPropertyChanged("GameTime");
            OnPropertyChanged("Baskets");
        }

        public void RefreshGameFields()
        {
            //CopyToFields();
            foreach (GameField f in Fields)
                f.Value = model.Map[f.X][f.Y];
        }

        private void RefreshTable(object sender, EventArgs e)
        {
            RefreshGameFields();

            OnPropertyChanged("Fields");
            OnPropertyChanged("GameTime");
            OnPropertyChanged("Baskets");
        }

        // Új játék indításának eseménykiváltása.
        private void OnNewGame()
        {
            if (paused)
            {
                paused = !paused;
            }

            if (NewGame != null)
                NewGame(this, EventArgs.Empty);
        }

        private void MoveBear(Int32 direction)
        {
            if (!paused)
            { 
                switch (direction)
                {
                case 1:
                    if (model.IsFloor(model.PlayerPos.X - 1, model.PlayerPos.Y)) model.Up();
                    break;
                case 3:
                    if (model.IsFloor(model.PlayerPos.X + 1, model.PlayerPos.Y)) model.Down();
                    break;
                case 4:
                    if (model.IsFloor(model.PlayerPos.X, model.PlayerPos.Y + 1)) model.Right();
                    break;
                case 2:
                    if (model.IsFloor(model.PlayerPos.X, model.PlayerPos.Y - 1)) model.Left();
                    break;
                default:
                    break;
                }
            }

            RefreshGameFields();
            OnPropertyChanged("Fields");
        }

        private void OnPause()
        {
            paused = !paused;

            if (paused)
            {
                model.time.Stop();
                model.patrolling.Stop();
            }
            else
            {
                model.time.Start();
                model.patrolling.Start();
            }
        }

        //Játék vége eseménykezelője
        private void OnVM_GameOver(object sender, GameOverEventArgs e)
        {
            OnPause();

            if (VM_GameOver != null)
            {
                VM_GameOver(this, new GameOverEventArgs(e.result));
            }
        }
    }
}
