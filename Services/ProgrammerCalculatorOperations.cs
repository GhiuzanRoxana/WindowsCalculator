using System;
using System.Windows;

namespace Calculator.Services
{
    internal class ProgrammerCalculatorOperations
    {
        private readonly NumberFormatter _numberFormatter;

        public ProgrammerCalculatorOperations(NumberFormatter numberFormatter)
        {
            _numberFormatter = numberFormatter ?? throw new ArgumentNullException(nameof(numberFormatter));
        }

        public (long result, string expression) ShiftLeft(double value)
        {
            if (value != Math.Floor(value) || value < 0)
            {
                throw new ArgumentException("Operația de deplasare la stânga necesită un număr întreg pozitiv");
            }

            long intValue = (long)value;
            long result = intValue << 1;
            string expression = $"{intValue} << 1 = ";

            return (result, expression);
        }

        public (long result, string expression) ShiftRight(double value)
        {
            if (value != Math.Floor(value) || value < 0)
            {
                throw new ArgumentException("Operația de deplasare la dreapta necesită un număr întreg pozitiv");
            }

            long intValue = (long)value;
            long result = intValue >> 1;
            string expression = $"{intValue} >> 1 = ";

            return (result, expression);
        }

        public string ConvertToBase(long value, int targetBase)
        {
            switch (targetBase)
            {
                case 2:
                    return Convert.ToString(value, 2);
                case 8:
                    return Convert.ToString(value, 8);
                case 10:
                    return value.ToString();
                case 16:
                    return Convert.ToString(value, 16).ToUpper();
                default:
                    throw new ArgumentException($"Baza {targetBase} nu este suportată");
            }
        }

        public long ParseFromBase(string value, int sourceBase)
        {
            try
            {
                return Convert.ToInt64(value.Replace(",", ""), sourceBase);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Nu s-a putut converti '{value}' din baza {sourceBase}", ex);
            }
        }
    }
}