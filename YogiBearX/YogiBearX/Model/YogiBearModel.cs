using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using YogiBearX.Persistence;
using YogiBearX.ViewModel;

namespace YogiBearX.Model
{
    //egészekből álló 2D pont osztály
    public class IntPoint
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public IntPoint(Int32 x, Int32 y) { X = x; Y = y; }
    }

    //Maci Laci játék típusa
    public class YogiBearModel
    {
        #region Fields

        private List<List<Int32>> map; //pálya
        private IntPoint playerpos; //maci laci pozíciója
        private List<Ranger> rangers; //vadőrök pozíciói
        private Int32 baskets; // piknik kosarak száma
        public ITimer patrolling { get; set; } //vadőrök járőrözésének ideje
        public ITimer time { get; set; } //játékidő stopper
        private Int32 gametime; //játékidő

        #endregion

        #region Properties

        //Getters
        public List<List<Int32>> Map { get { return map; } set { this.map = value; } }
        public IntPoint PlayerPos { get { return playerpos; } }
        public List<Ranger> Rangers { get { return rangers; } }
        public Int32 Baskets { get { return baskets; } }
        public Int32 GameTime { get { return gametime; } }
        //        public System.Windows.Forms.Timer Patrolling { get { return patrolling; } }

        #endregion

        #region Constructor

        //A modell példányosítása
        public YogiBearModel()
        {
            map = new List<List<Int32>>();
            playerpos = new IntPoint(0, 0);
            rangers = new List<Ranger>();
            baskets = 0;

            patrolling = DependencyService.Get<ITimer>();
            patrolling.Interval = 700;

            time = DependencyService.Get<ITimer>();
            time.Interval = 1000;

            gametime = 0;
        }

        #endregion

        #region Public Methods

        //Új játék esetén a modell inicializálása
        public void InitGame()
        {
            time.Elapsed -= Time_Tick;
            patrolling.Elapsed -= Patrolling_Tick;
            map = new List<List<Int32>>();
            rangers = new List<Ranger>();
            baskets = 0;
            playerpos = new IntPoint(0, 0);
            gametime = 0;
        }

        //Új játék kezdése
        public async Task<bool> NewGame()
        {
            if (map.Count != 0) InitGame();

            YogiBearData data = new YogiBearData();
            map = await data.LoadFromFileAsync();

            if (map == null)
            {
                map = data.LoadFirstLevel();
            }

            //Vadőrök lista feltöltése, piknikkosarak megszámolása
            for (Int32 i = 0; i < map.Count; i++)
                for (Int32 j = 0; j < map.Count; j++)
                {
                    if (map[i][j] == 4)
                        rangers.Add(new Ranger(i, j, Convert.ToBoolean(rangers.Count % 2)));

                    if (map[i][j] == 2)
                        baskets++;
                }

            //esemény-eseménykezelő párosítások, időzítők indítása
            time.Elapsed += new EventHandler(Time_Tick);
            patrolling.Elapsed += new EventHandler(Patrolling_Tick);
            patrolling.Start();
            time.Start();

            return true;
        }

        //Maci Laci lépésének érvényességének vizsgálata
        //Ha "bent" van a pályán és nem akadály/vadőr
        public bool IsFloor(int row, int col)
        {
            return (row >= 0 && row < map.Count && col >= 0 && col < map.Count && (map[row][col] == 0 || map[row][col] == 2 || map[row][col] == 3));
        }

        //Felfelé lépés 1 mezővel, vizsgáljuk kosarat vettünk-e fel, és ha nincs akkor győzelem esemény
        public void Up()
        {
            if (map[playerpos.X - 1][playerpos.Y] == 2) baskets--;

            map[playerpos.X][playerpos.Y] = 0;
            map[playerpos.X - 1][playerpos.Y] = 3;
            playerpos.X--;
            if (baskets == 0) OnGameOver(new GameOverEventArgs(true));
        }

        //Balra lépés 1 mezővel, vizsgáljuk kosarat vettünk-e fel, és ha nincs akkor győztünk
        public void Left()
        {
            if (map[playerpos.X][playerpos.Y - 1] == 2) baskets--;

            map[playerpos.X][playerpos.Y] = 0;
            map[playerpos.X][playerpos.Y - 1] = 3;
            playerpos.Y--;

            if (baskets == 0) OnGameOver(new GameOverEventArgs(true));
        }

        //Lefelé lépés 1 mezővel, vizsgáljuk kosarat vettünk-e fel, és ha nincs akkor győztünk
        public void Down()
        {
            if (map[playerpos.X + 1][playerpos.Y] == 2) baskets--;

            map[playerpos.X][playerpos.Y] = 0;
            map[playerpos.X + 1][playerpos.Y] = 3;
            playerpos.X++;
            if (baskets == 0) OnGameOver(new GameOverEventArgs(true));
        }

        //Jobbra lépés 1 mezővel, vizsgáljuk kosarat vettünk-e fel, és ha nincs akkor győztünk
        public void Right()
        {
            if (map[playerpos.X][playerpos.Y + 1] == 2) baskets--;

            map[playerpos.X][playerpos.Y] = 0;
            map[playerpos.X][playerpos.Y + 1] = 3;
            playerpos.Y++;
            if (baskets == 0) OnGameOver(new GameOverEventArgs(true));
        }

        #endregion

        #region Private Methods

        //Vadőrök szempontjából (row, col) érvényes mező-e
        //Ha bent van a pályán, és nem akadály/kosár/Maci Laci
        private bool IsPatrolField(Int32 row, Int32 col)
        {
            return (row >= 0 && row < map.Count && col >= 0 && col < map.Count && map[row][col] == 0);
        }

        //Egy vadőr következő lépése
        private void RangerMove(Ranger ranger)
        {
            //Ha függőlegesen járőrözik
            if (ranger.Direction)
            {
                if (IsPatrolField(ranger.Position.X, ranger.Position.Y + ranger.Velocity))
                {
                    map[ranger.Position.X][ranger.Position.Y] = 0;
                    map[ranger.Position.X][ranger.Position.Y + ranger.Velocity] = 4;
                    ranger.SetYPos(ranger.Position.Y + ranger.Velocity);
                }
                else //ha érvénytelen mező jönne, akkor megfordul
                {
                    ranger.Velocity *= -1;
                }
            }
            else //Ha vízszintesen járőrözik
            {
                if (IsPatrolField(ranger.Position.X + ranger.Velocity, ranger.Position.Y))
                {
                    map[ranger.Position.X][ranger.Position.Y] = 0;
                    map[ranger.Position.X + ranger.Velocity][ranger.Position.Y] = 4;
                    ranger.SetXPos(ranger.Position.X + ranger.Velocity);
                }
                else //ha érvénytelen mező jönne, akkor megfordul
                {
                    ranger.Velocity *= -1;
                }
            }
        }

        //Adott vadőr elkapja-e Maci Lacit
        private bool RangerCatch(Ranger ranger)
        {
            for (Int32 i = -1; i < 2; i++)
                for (Int32 j = -1; j < 2; j++)
                {
                    Int32 row = ranger.Position.X + i;
                    Int32 col = ranger.Position.Y + j;

                    if (playerpos.X == row && playerpos.Y == col)
                        return true;
                }

            return false;
        }

        //Bármelyik vadőr elkapta-e Maci Lacit?
        //Ha igen, akkor Vereség esemény kiváltása
        private bool IsCaught()
        {
            for (Int32 i = 0; i < rangers.Count; i++)
            { 
                if (RangerCatch(rangers[i]))
                {
                    //OnGameOver(new GameOverEventArgs(false));
                    return true;
                }
            }

            return false;
        }



        #endregion

        #region Events

        //Játék vége eseménye
        public event EventHandler<GameOverEventArgs> GameOver;
        //Játéktábla frissítése eseménye
        public event EventHandler Refresh;

        #endregion

        #region Event Methods

        //Játék vége esemény kiváltása
        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            EventHandler<GameOverEventArgs> handler = GameOver;
            if (handler != null)
                handler(this, e);
        }

        //Játéktábla frissítése esemény kiváltása
        public virtual void OnRefresh()
        {
            EventHandler handler = Refresh;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }



        #endregion

        #region Event handlers

        //patrolling Timer Tick eseményének a kezelője
        //Adott időközönként léptetjük az összes vadőrt
        private void Patrolling_Tick(object sender, EventArgs e)
        {
            for (Int32 i = 0; i < rangers.Count; i++)
            {
                RangerMove(rangers[i]);  //mindenkit léptetünk
            }

            OnRefresh();
            if(IsCaught()) OnGameOver(new GameOverEventArgs(false)); //megvizsgáljuk nem kapta-e valaki el Maci Lacit
        }

        //Játékidő haladásának eseménykezelője
        private void Time_Tick(object sender, EventArgs e)
        {
            gametime++;
        }

        #endregion

    }

    #region EventArgs

    //Játék vége eseményargumentum típusa
    public class GameOverEventArgs : EventArgs
    {
        public Boolean result { get; set; }
        public GameOverEventArgs(Boolean res) { result = res; }
    }

    #endregion
}
