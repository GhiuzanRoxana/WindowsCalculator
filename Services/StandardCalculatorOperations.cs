using System;
using System.Windows;

namespace Calculator.Services
{
    internal class StandardCalculatorOperations
    {
        private readonly NumberFormatter _numberFormatter;

        public StandardCalculatorOperations(NumberFormatter numberFormatter)
        {
            _numberFormatter = numberFormatter ?? throw new ArgumentNullException(nameof(numberFormatter));
        }

        public (double result, string expression) CalculateSquareRoot(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Nu se poate calcula radicalul unui număr negativ");
            }

            double result = Math.Sqrt(value);
            string expression = $"√({value}) = ";

            return (result, expression);
        }

        public (double result, string expression) CalculateSquare(double value)
        {
            double result = value * value;
            string expression = $"sqr({value}) = ";

            return (result, expression);
        }

        public (double result, string expression) CalculateInverse(double value)
        {
            if (value == 0)
            {
                throw new DivideByZeroException("Nu se poate calcula inversul lui zero");
            }

            double result = 1 / value;
            string expression = $"1/({value}) = ";

            return (result, expression);
        }

        public (double result, string expression) CalculatePercentage(double currentValue, double previousValue, string currentOperator)
        {
            if (string.IsNullOrEmpty(currentOperator) || previousValue == 0)
            {
                double result = currentValue / 100;
                return (result, $"{currentValue}% = ");
            }
            else
            {
                double percentValue = previousValue * (currentValue / 100);
                return (percentValue, $"{previousValue} {currentOperator} {currentValue}% = ");
            }
        }

        public string FormatNumber(double number)
        {
            return _numberFormatter.FormatNumber(number);
        }

        public double ParseNumber(string text)
        {
            return _numberFormatter.ParseNumber(text);
        }
    }
}