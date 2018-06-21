using System;

namespace YogiBearX.Model
{
    //Vadőrök típusa
    public class Ranger
    {
        private IntPoint position; //Játékbeli pozíció
        private bool direction; //Vertikális/Horizontális irányban járőrözik-e
        private Int32 velocity; //sebességvektor iránya

        //Properties
        public IntPoint Position { get { return position; } }
        public bool Direction { get { return direction; } }
        public Int32 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //Konstruktor
        public Ranger(Int32 row, Int32 col, bool dir)
        {
            position = new IntPoint(row, col);
            direction = dir;
            velocity = 1;
        }

        //Public Setters
        public void SetXPos(Int32 val) { position.X = val; }
        public void SetYPos(Int32 val) { position.Y = val; }
    }
}
