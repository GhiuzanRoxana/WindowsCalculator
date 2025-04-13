using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.Services
{
    public class BaseValues : INotifyPropertyChanged
    {
        private string _hexValue = "0";
        private string _decValue = "0";
        private string _octValue = "0";
        private string _binValue = "0";

        public string HexValue
        {
            get => _hexValue;
            set
            {
                if (_hexValue != value)
                {
                    _hexValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DecValue
        {
            get => _decValue;
            set
            {
                if (_decValue != value)
                {
                    _decValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OctValue
        {
            get => _octValue;
            set
            {
                if (_octValue != value)
                {
                    _octValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BinValue
        {
            get => _binValue;
            set
            {
                if (_binValue != value)
                {
                    _binValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}