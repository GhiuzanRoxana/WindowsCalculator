using System;
using System.Collections.Generic;
namespace Calculator.Services
{
    public class MemoryOperations
    {
        private double _memoryValue = 0;
        private readonly List<double> _memoryStack = new List<double>();

        public void Clear()
        {
            _memoryValue = 0;
            _memoryStack.Clear();
        }

        public double Recall()
        {
            return _memoryValue;
        }

        public void Add(double value)
        {
           
                _memoryValue = value;
                 _memoryStack.Add(_memoryValue);
        }

        public void Subtract(double value)
        {
            _memoryValue = -value;
            _memoryStack.Add(-value);
        }

        public void Store(double value)
        {
            _memoryValue = value;
            _memoryStack.Add(value);
        }

        public List<double> GetMemoryStack()
        {
            return new List<double>(_memoryStack);
        }

        public void ClearItem(double item)
        {
            int index = _memoryStack.IndexOf(item);
            if (index >= 0)
            {
                _memoryStack[index] = 0;
                if (index == _memoryStack.Count - 1)
                {
                    _memoryValue = 0;
                }
            }
        }

        public void AddToItem(double item, double value)
        {
            int index = _memoryStack.IndexOf(item);
            if (index >= 0)
            {
                _memoryStack[index] += value;
                if (index == _memoryStack.Count - 1)
                {
                    _memoryValue = _memoryStack[index];
                }
            }
        }

        public void SubtractFromItem(double item, double value)
        {
            int index = _memoryStack.IndexOf(item);
            if (index >= 0)
            {
                _memoryStack[index] -= value;
                if (index == _memoryStack.Count - 1)
                {
                    _memoryValue = _memoryStack[index];
                }
            }
        }

        public void DeleteItem(double item)
        {
            int index = _memoryStack.IndexOf(item);
            if (index >= 0)
            {
                _memoryStack.RemoveAt(index);
                if (_memoryStack.Count > 0)
                {
                    _memoryValue = _memoryStack[_memoryStack.Count - 1];
                }
                else
                {
                    _memoryValue = 0;
                }
            }
        }
    }
}