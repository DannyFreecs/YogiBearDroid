using System;

namespace YogiBearX.ViewModel
{
    public class GameField : ViewModelBase
    {
        private Int32 _value;

        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public Int32 Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }
    }
}
