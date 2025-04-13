using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public partial class MemoryDialog : Window, INotifyPropertyChanged
    {
        public ObservableCollection<double> MemoryItems { get; private set; }
        public double? SelectedValue { get; private set; }
        public event EventHandler<double> ItemDeleted;
        public event EventHandler<double> ItemCleared;
        public event EventHandler<(double Item, double Value)> ItemAddition;
        public event EventHandler<(double Item, double Value)> ItemSubtraction;

        private double _currentValue;

        public bool HasMemoryItems => MemoryItems != null && MemoryItems.Count > 0;
        public bool HasNoMemoryItems => !HasMemoryItems;

        public MemoryDialog(List<double> memoryValues, double currentValue)
        {
            InitializeComponent();

            _currentValue = currentValue;
            MemoryItems = new ObservableCollection<double>(memoryValues);

            this.DataContext = this;

            MemoryListView.ItemsSource = MemoryItems;

            OnPropertyChanged(nameof(HasMemoryItems));
            OnPropertyChanged(nameof(HasNoMemoryItems));
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MemoryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MemoryListView.SelectedItem != null)
            {
                SelectedValue = (double)MemoryListView.SelectedItem;
                DialogResult = true;
                Close();
            }
        }

        private void ClearItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is double value)
            {
                ItemCleared?.Invoke(this, value);
                int index = MemoryItems.IndexOf(value);
                if (index >= 0)
                {
                    MemoryItems[index] = 0;
                }
            }
        }

        private void AddToItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is double value)
            {
                ItemAddition?.Invoke(this, (value, _currentValue));
                int index = MemoryItems.IndexOf(value);
                if (index >= 0)
                {
                    MemoryItems[index] += _currentValue;
                }
            }
        }

        private void SubtractFromItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is double value)
            {
                ItemSubtraction?.Invoke(this, (value, _currentValue));
                int index = MemoryItems.IndexOf(value);
                if (index >= 0)
                {
                    MemoryItems[index] -= _currentValue;
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is double value)
            {
                ItemDeleted?.Invoke(this, value);
                MemoryItems.Remove(value);

                if (MemoryItems.Count == 0)
                {
                    OnPropertyChanged(nameof(HasMemoryItems));
                    OnPropertyChanged(nameof(HasNoMemoryItems));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void MemoryListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MemoryListView.SelectedItem != null)
            {
                double selectedValue = (double)MemoryListView.SelectedItem;

            }
            else
            {
            }
        }
    }
}