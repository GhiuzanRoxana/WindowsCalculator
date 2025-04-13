using System;
using System.Globalization;

namespace Calculator.Services
{
    public class NumberFormatter
    {
        private bool _digitGroupingEnabled;

        public NumberFormatter(bool digitGroupingEnabled = false)
        {
            _digitGroupingEnabled = digitGroupingEnabled;
        }

        public bool DigitGroupingEnabled
        {
            get => _digitGroupingEnabled;
            set => _digitGroupingEnabled = value;
        }

        public string FormatNumber(double number)
        {
            if (double.IsInfinity(number) || double.IsNaN(number))
                return number.ToString(CultureInfo.InvariantCulture);

            string result;

            if (!_digitGroupingEnabled)
            {
                result = number.ToString("G", CultureInfo.InvariantCulture);
            }
            else
            {
                string numStr = number.ToString("G", CultureInfo.InvariantCulture);
                string[] parts = numStr.Split('.');

                string intPart = parts[0];
                string formattedInt = "";

                for (int i = 0; i < intPart.Length; i++)
                {
                    if (i > 0 && (intPart.Length - i) % 3 == 0)
                        formattedInt += ","; 

                    formattedInt += intPart[i];
                }

                if (parts.Length > 1)
                    result = formattedInt + "." + parts[1];
                else
                    result = formattedInt;
            }

            return result;
        }

        public double ParseNumber(string text)
        {
            string cleanText = text;
            if (_digitGroupingEnabled)
            {
                cleanText = text.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, "");
            }

            if (double.TryParse(cleanText, NumberStyles.Any, CultureInfo.CurrentCulture, out double result))
            {
                return result;
            }

            if (double.TryParse(cleanText, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return 0;
        }
    }
}