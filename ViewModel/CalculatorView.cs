using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Calculator.Commands;
using Calculator.Services;
using System.IO;
using System.Globalization;

namespace Calculator.ViewModel
{
    public class CalculatorView : INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private readonly NumberFormatter _numberFormatter;
        private readonly StandardCalculatorOperations _standardOps;
        private readonly ProgrammerCalculatorOperations _programmerOps;
        private readonly MemoryOperations _memoryOps;
        private readonly BaseValues _baseValues;


        private bool _isStandardMode = true;
        private bool _digitGrouping = false;
        private int _currentBase = 10;
        private string _display = "0";
        private string _currentExpression = "";
        private double _currentValue = 0;
        private double _previousValue = 0;
        private string _currentOperator = "";
        private bool _isNewCalculation = true;
        private bool _isUpdatingSettings = false;


        public BaseValues BaseValues => _baseValues;
        public bool IsHexDigitEnabled => !_isStandardMode && _currentBase >= 16;
        public bool IsDecDigitEnabled => !_isStandardMode && _currentBase >= 10;
        public bool IsOctDigitEnabled => !_isStandardMode && _currentBase >= 8;
        public bool IsBinDigitEnabled => !_isStandardMode && _currentBase >= 2;

        public bool IsButtonAEnabled => !_isStandardMode && _currentBase == 16;
        public bool IsButtonBEnabled => !_isStandardMode && _currentBase == 16;
        public bool IsButtonCEnabled => !_isStandardMode && _currentBase == 16;
        public bool IsButtonDEnabled => !_isStandardMode && _currentBase == 16;
        public bool IsButtonEEnabled => !_isStandardMode && _currentBase == 16;
        public bool IsButtonFEnabled => !_isStandardMode && _currentBase == 16;

        public bool IsButton0Enabled => true; 
        public bool IsButton1Enabled => true; 
        public bool IsButton2Enabled => !_isStandardMode ? _currentBase > 2 : true;
        public bool IsButton3Enabled => !_isStandardMode ? _currentBase > 2 : true;
        public bool IsButton4Enabled => !_isStandardMode ? _currentBase > 4 : true;
        public bool IsButton5Enabled => !_isStandardMode ? _currentBase > 4 : true;
        public bool IsButton6Enabled => !_isStandardMode ? _currentBase > 6 : true;
        public bool IsButton7Enabled => !_isStandardMode ? _currentBase > 6 : true;
        public bool IsButton8Enabled => !_isStandardMode ? _currentBase > 8 : true;
        public bool IsButton9Enabled => !_isStandardMode ? _currentBase > 8 : true;
        public bool IsDecimalPointEnabled => _isStandardMode;
        public bool IsBaseSelectionEnabled => !_isStandardMode;

        public bool HasMemoryValue => _memoryOps != null && _memoryOps.GetMemoryStack().Count > 0;
        public CalculatorView()
        {
            _settingsService = new SettingsService();
            _baseValues = new BaseValues();

            string groupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            MessageBox.Show($"Separatori configurați:\nGrup: '{groupSeparator}'\nZecimal: '{decimalSeparator}'", "Info");

            var settings = _settingsService.LoadSettings();
            _isStandardMode = settings.isStandardMode;
            _digitGrouping = settings.digitGrouping;
            _currentBase = settings.currentBase;

            _numberFormatter = new NumberFormatter(_digitGrouping);
            _standardOps = new StandardCalculatorOperations(_numberFormatter);
            _programmerOps = new ProgrammerCalculatorOperations(_numberFormatter);
            _memoryOps = new MemoryOperations();

            SetCurrentBase(_currentBase);
            UpdateBaseValues();
        }


        private void SaveSettings()
        {
            _settingsService.SaveSettings(_isStandardMode, _digitGrouping, _currentBase);
        }
        public bool IsStandardMode
        {
            get => _isStandardMode;
            set
            {
                if (_isStandardMode != value)
                {
                    _isStandardMode = value;
                    if (_isStandardMode)
                    {
                        _currentBase = 10;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HamburgerLabel));
                    SaveSettings();
                    NotifyButtonStateChanged();
                }
            }
        }
        private void UpdateMemoryButtonState()
        {
            OnPropertyChanged(nameof(HasMemoryValue));
        }
        public bool DigitGrouping
        {
            get => _digitGrouping;
            set
            {
                if (_isUpdatingSettings) return;

                _isUpdatingSettings = true;
                try
                {
                    if (_digitGrouping != value)
                    {
                        _digitGrouping = value;
                        OnPropertyChanged();

                        if (_numberFormatter != null)
                        {
                            _numberFormatter.DigitGroupingEnabled = value;
                            RefreshDisplay();
                        }

                        SaveSettings();
                    }
                }
                finally
                {
                    _isUpdatingSettings = false;
                }
            }
        }
        public string HamburgerLabel => IsStandardMode ? "Standard" : "Programmer";
        public string Display
        {
            get => _display;
            set
            {
                if (_display != value)
                {
                    _display = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CurrentExpression
        {
            get => _currentExpression;
            set
            {
                if (_currentExpression != value)
                {
                    _currentExpression = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateDisplay()
        {
            if (_display == "0" || _isNewCalculation)
            {
                Display = _numberFormatter.FormatNumber(_currentValue);
            }
        }
        private void RefreshDisplay()
        {
            try
            {
                UpdateBaseValues();
                DisplayInCurrentBase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la actualizarea afișajului: {ex.Message}", "Eroare");
            }
        }
        private double GetValueFromDisplay()
        {
            if (_isStandardMode || _currentBase == 10)
            {
                return _numberFormatter.ParseNumber(_display);
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(_display) || _display == "0")
                        return 0;

                    return _programmerOps.ParseFromBase(_display, _currentBase);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la conversia din baza {_currentBase}: {ex.Message}", "Eroare");
                    return 0;
                }
            }
        }


        #region PrincipalMenuAndModeChanger
        public ICommand ToggleModeCommand => new RelayCommand(_ =>
        {
            IsStandardMode = !IsStandardMode;
            SaveSettings();
            ClearCommand.Execute(null);

            NotifyButtonStateChanged();
        });
        public ICommand ToggleDigitGroupingCommand => new RelayCommand(_ =>
        {
            _digitGrouping = !_digitGrouping;

            _numberFormatter.DigitGroupingEnabled = _digitGrouping;

            RefreshDisplay();

            OnPropertyChanged(nameof(DigitGrouping));

            SaveSettings();

            MessageBox.Show($"Digit Grouping setat la: {_digitGrouping}", "Debug");
        });
        public ICommand ShowAboutCommand => new RelayCommand(_ =>
        {
            MessageBox.Show("Calculator Application\nDeveloped by:Ghiuzan Roxana Ana Maria\nGroup:10LF232", "INFO");
        });

        #endregion

        #region SimpleOperations
        public ICommand NumberCommand => new RelayCommand(param =>
        {
            if (param is string digit)
            {
                if (!_isStandardMode)
                {
                    int digitValue = int.Parse(digit);
                    if (digitValue >= _currentBase)
                        return;
                }

                if (_isNewCalculation)
                {
                    Display = digit;
                    _isNewCalculation = false;
                }
                else
                {
                    if (Display == "0")
                        Display = digit;
                    else
                        Display += digit;
                }

                if (_isStandardMode)
                {
                    _currentValue = _numberFormatter.ParseNumber(Display);

                    if (_digitGrouping)
                    {
                        RefreshDisplay();
                    }
                }
                else
                {
                    _currentValue = _programmerOps.ParseFromBase(Display, _currentBase);

                    UpdateBaseValues();
                }
            }
        });
        public ICommand DecimalCommand => new RelayCommand(_ =>
        {
            if (_isNewCalculation)
            {
                Display = "0.";
                _isNewCalculation = false;
            }
            else if (!Display.Contains("."))
            {
                Display += ".";
            }
        });
        public ICommand OperatorCommand => new RelayCommand(param =>
        {
            if (param is string op)
            {
                if (!string.IsNullOrEmpty(_currentOperator))
                {
                    EqualsCommand.Execute(null);
                }

                _previousValue = GetValueFromDisplay();
                _currentOperator = op;
                CurrentExpression = $"{_previousValue} {op} ";
                _isNewCalculation = true;
            }
        });
        public ICommand EqualsCommand => new RelayCommand(_ =>
        {
            if (!string.IsNullOrEmpty(_currentOperator))
            {
                double currentValue = GetValueFromDisplay();
                CurrentExpression += $"{currentValue} =";

                CalculateResult();

                _currentOperator = "";
                _isNewCalculation = true;
            }
        });

        #endregion

        #region ClearCommands
        public ICommand ClearCommand => new RelayCommand(_ =>
        {
            Display = "0";
            CurrentExpression = "";
            _currentOperator = "";
            _currentValue = 0;
            _previousValue = 0;
            _isNewCalculation = true;
        });
        public ICommand ClearEntryCommand => new RelayCommand(_ =>
        {
            Display = "0";
            _isNewCalculation = true;
        });
        public ICommand BackspaceCommand => new RelayCommand(_ =>
        {
            if (!_isNewCalculation && Display.Length > 0)
            {
                if (Display.Length == 1 || (Display.Length == 2 && Display[0] == '-'))
                {
                    Display = "0";
                    _isNewCalculation = true;
                }
                else
                {
                    Display = Display.Substring(0, Display.Length - 1);
                }

                _currentValue = GetValueFromDisplay();
            }
        });
        #endregion

        #region MathFunctions
        public ICommand ToggleSignCommand => new RelayCommand(_ =>
        {
            if (Display != "0")
            {
                if (Display.StartsWith("-"))
                    Display = Display.Substring(1);
                else
                    Display = "-" + Display;

                _currentValue = GetValueFromDisplay();
            }
        });
        public ICommand SquareRootCommand => new RelayCommand(_ =>
        {
            try
            {
                double currentValue = GetValueFromDisplay();
                var (result, expression) = _standardOps.CalculateSquareRoot(currentValue);

                CurrentExpression = expression;
                _currentValue = result;

                UpdateBaseValues();

                DisplayInCurrentBase();

                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la calculul radicalului: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        public ICommand SquareCommand => new RelayCommand(_ =>
        {
            try
            {
                double currentValue = GetValueFromDisplay();
                var (result, expression) = _standardOps.CalculateSquare(currentValue);

                CurrentExpression = expression;
                _currentValue = result;

                UpdateBaseValues();

                DisplayInCurrentBase();

                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la ridicarea la pătrat: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        public ICommand InverseCommand => new RelayCommand(_ =>
        {
            try
            {
                double currentValue = GetValueFromDisplay();
                var (result, expression) = _standardOps.CalculateInverse(currentValue);

                CurrentExpression = expression;
                _currentValue = result;
                Display = _numberFormatter.FormatNumber(result);
                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la calculul inversului: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        public ICommand PercentageCommand => new RelayCommand(_ =>
        {
            try
            {
                double currentValue = GetValueFromDisplay();
                var (result, expression) = _standardOps.CalculatePercentage(currentValue, _previousValue, _currentOperator);

                CurrentExpression = expression;
                _currentValue = result;
                Display = _numberFormatter.FormatNumber(result);
                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la calculul procentului: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        #endregion


        #region ShiftCommands
        public ICommand ShiftLeftCommand => new RelayCommand(_ =>
        {
            try
            {
                double currentValue = GetValueFromDisplay();
                var (result, expression) = _programmerOps.ShiftLeft(currentValue);

                CurrentExpression = expression;
                _currentValue = result;


                UpdateBaseValues();
                DisplayInCurrentBase();

                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la deplasarea la stânga: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        public ICommand ShiftRightCommand => new RelayCommand(_ =>
        {
            try
            {
                double currentValue = GetValueFromDisplay();
                var (result, expression) = _programmerOps.ShiftRight(currentValue);
                CurrentExpression = expression;
                _currentValue = result;
                Display = _numberFormatter.FormatNumber(result);

                UpdateBaseValues();

                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la deplasarea la dreapta: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        #endregion

        #region MemoryCommands
        public ICommand MemoryClearCommand => new RelayCommand(_ =>
        {
            _memoryOps.Clear();
            UpdateMemoryButtonState();
        });
        public ICommand MemoryRecallCommand => new RelayCommand(_ =>
        {
            double value = _memoryOps.Recall();
            Display = _numberFormatter.FormatNumber(value);
            _currentValue = value;
            _isNewCalculation = true;
        });
        public ICommand MemoryAddCommand => new RelayCommand(_ =>
        {
            double currentValue = GetValueFromDisplay();
            _memoryOps.Add(currentValue);
            UpdateMemoryButtonState();
        });
        public ICommand MemorySubtractCommand => new RelayCommand(_ =>
        {
            double currentValue = GetValueFromDisplay();
            _memoryOps.Subtract(currentValue);
            UpdateMemoryButtonState();
        });
        public ICommand MemoryShowCommand => new RelayCommand(_ =>
        {
            var memoryValues = _memoryOps.GetMemoryStack();
            double currentValue = GetValueFromDisplay();

            var dialog = new MemoryDialog(memoryValues, currentValue);
            dialog.Owner = Application.Current.MainWindow;

            if (memoryValues.Count > 0)
            {
                dialog.ItemDeleted += (sender, item) => _memoryOps.DeleteItem(item);
                dialog.ItemCleared += (sender, item) => _memoryOps.ClearItem(item);
                dialog.ItemAddition += (sender, data) => _memoryOps.AddToItem(data.Item, data.Value);
                dialog.ItemSubtraction += (sender, data) => _memoryOps.SubtractFromItem(data.Item, data.Value);

                if (dialog.ShowDialog() == true && dialog.SelectedValue.HasValue)
                {
                    _currentValue = dialog.SelectedValue.Value;
                    UpdateBaseValues();
                    DisplayInCurrentBase();
                    _isNewCalculation = true;
                }
                else
                {
                    dialog.ShowDialog();
                }
            }
            else
            {
                dialog.ShowDialog();
            }
        });
        public ICommand MemoryStoreCommand => new RelayCommand(_ =>
        {
            double currentValue = GetValueFromDisplay();
            _memoryOps.Store(currentValue);
            UpdateMemoryButtonState();
        });

        #endregion

        #region AuxMethods
        private void CalculateResult()
        {
            try
            {
                double result = 0;
                double secondValue = GetValueFromDisplay();

                switch (_currentOperator)
                {
                    case "+":
                        result = _previousValue + secondValue;
                        break;
                    case "-":
                        result = _previousValue - secondValue;
                        break;
                    case "×":
                        result = _previousValue * secondValue;
                        break;
                    case "÷":
                        if (secondValue == 0)
                            throw new DivideByZeroException("Nu se poate împărți la zero.");
                        result = _previousValue / secondValue;
                        break;
                }

                _currentValue = result;

                UpdateBaseValues();

                DisplayInCurrentBase();

                _isNewCalculation = true;
            }
            catch (DivideByZeroException)
            {
                Display = "Nu se poate împărți la zero";
                _isNewCalculation = true;
            }
            catch (Exception ex)
            {
                Display = "Eroare";
                _isNewCalculation = true;
                MessageBox.Show($"Eroare la calcul: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DisplayInCurrentBase()
        {
            if (_isStandardMode)
            {
                Display = _numberFormatter.FormatNumber(_currentValue);
            }
            else
            {
                switch (_currentBase)
                {
                    case 16:
                        Display = _baseValues.HexValue;
                        break;
                    case 10:
                        Display = _baseValues.DecValue;
                        break;
                    case 8:
                        Display = _baseValues.OctValue;
                        break;
                    case 2:
                        Display = _baseValues.BinValue;
                        break;
                    default:
                        Display = _baseValues.DecValue;
                        break;
                }
            }
        }
        public void HandleKeyInput(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
            {
                int digit = key - Key.D0;
                NumberCommand.Execute(digit.ToString());
                return;
            }

            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                int digit = key - Key.NumPad0;
                NumberCommand.Execute(digit.ToString());
                return;
            }

            switch (key)
            {
                case Key.Add:
                    OperatorCommand.Execute("+");
                    break;
                case Key.Subtract:
                    OperatorCommand.Execute("-");
                    break;
                case Key.Multiply:
                    OperatorCommand.Execute("×");
                    break;
                case Key.Divide:
                    OperatorCommand.Execute("÷");
                    break;

                case Key.OemPlus:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        OperatorCommand.Execute("+");
                    break;
                case Key.OemMinus:
                    OperatorCommand.Execute("-");
                    break;
                case Key.D8:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        OperatorCommand.Execute("×");
                    break;
                case Key.OemQuestion:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        OperatorCommand.Execute("÷");
                    break;

                case Key.Decimal:
                case Key.OemPeriod:
                    DecimalCommand.Execute(null);
                    break;

                case Key.Enter:
                    EqualsCommand.Execute(null);
                    break;

                case Key.Back:
                    BackspaceCommand.Execute(null);
                    break;
                case Key.Delete:
                    ClearEntryCommand.Execute(null);
                    break;
                case Key.Escape:
                    ClearCommand.Execute(null);
                    break;
            }
        }
        private void UpdateBaseValues()
        {
            try
            {
                long intValue = (long)_currentValue;

                _baseValues.HexValue = _programmerOps.ConvertToBase(intValue, 16);
                _baseValues.DecValue = _programmerOps.ConvertToBase(intValue, 10);
                _baseValues.OctValue = _programmerOps.ConvertToBase(intValue, 8);
                _baseValues.BinValue = _programmerOps.ConvertToBase(intValue, 2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la conversia în baze: {ex.Message}", "Eroare");
            }
        }
        private void SetCurrentBase(int baseValue)
        {
            _currentBase = baseValue;
            SaveSettings();

            UpdateBaseValues();

            NotifyButtonStateChanged();
        }
        private void NotifyButtonStateChanged()
        {
            OnPropertyChanged(nameof(IsButtonAEnabled));
            OnPropertyChanged(nameof(IsButtonBEnabled));
            OnPropertyChanged(nameof(IsButtonCEnabled));
            OnPropertyChanged(nameof(IsButtonDEnabled));
            OnPropertyChanged(nameof(IsButtonEEnabled));
            OnPropertyChanged(nameof(IsButtonFEnabled));

            OnPropertyChanged(nameof(IsButton0Enabled));
            OnPropertyChanged(nameof(IsButton1Enabled));
            OnPropertyChanged(nameof(IsButton2Enabled));
            OnPropertyChanged(nameof(IsButton3Enabled));
            OnPropertyChanged(nameof(IsButton4Enabled));
            OnPropertyChanged(nameof(IsButton5Enabled));
            OnPropertyChanged(nameof(IsButton6Enabled));
            OnPropertyChanged(nameof(IsButton7Enabled));
            OnPropertyChanged(nameof(IsButton8Enabled));
            OnPropertyChanged(nameof(IsButton9Enabled));

            OnPropertyChanged(nameof(IsDecimalPointEnabled));
        }

        #endregion

        #region ConvertToBaseCommands
        public ICommand ConvertToHexCommand => new RelayCommand(_ =>
        {
            if (_isStandardMode) return;

            SetCurrentBase(16);
            Display = _baseValues.HexValue;
        });
        public ICommand ConvertToDecCommand => new RelayCommand(_ =>
        {
            if (_isStandardMode) return; 

            SetCurrentBase(10);
            Display = _baseValues.DecValue;
        });
        public ICommand ConvertToOctCommand => new RelayCommand(_ =>
        {
            if (_isStandardMode) return;  

            SetCurrentBase(8);
            Display = _baseValues.OctValue;
        });
        public ICommand ConvertToBinCommand => new RelayCommand(_ =>
        {
            if (_isStandardMode) return; 

            SetCurrentBase(2);
            Display = _baseValues.BinValue;
        });
        public ICommand HexNumberCommand => new RelayCommand(param =>
        {
            if (param is string hexDigit)
            {
                if (_isNewCalculation)
                {
                    Display = hexDigit;
                    _isNewCalculation = false;
                }
                else
                {
                    if (Display == "0")
                        Display = hexDigit;
                    else
                        Display += hexDigit;
                }

                _currentValue = _programmerOps.ParseFromBase(Display, 16);

                UpdateBaseValues();

                if (_digitGrouping)
                {
                    RefreshDisplay();
                }
            }
        });
        #endregion


        #region CutCopyPasteCommands
        public ICommand CutCommand => new RelayCommand(_ =>
        {
            SaveToClipboardFile(Display);
            Display = "0";
            _currentValue = 0;
            _isNewCalculation = true;
        });
        public ICommand CopyCommand => new RelayCommand(_ =>
        {
            SaveToClipboardFile(Display);
        });
        public ICommand PasteCommand => new RelayCommand(_ =>
        {
            string clipboardValue = LoadFromClipboardFile();
            if (!string.IsNullOrEmpty(clipboardValue))
            {
                try
                {
                    double parsedValue;

                    if (_isStandardMode || _currentBase == 10)
                    {
                        parsedValue = _numberFormatter.ParseNumber(clipboardValue);
                    }
                    else
                    {
                        parsedValue = _programmerOps.ParseFromBase(clipboardValue, _currentBase);
                    }

                    _currentValue = parsedValue;

                    if (_isStandardMode)
                    {
                        Display = _numberFormatter.FormatNumber(parsedValue);
                    }
                    else
                    {
                        UpdateBaseValues();
                        DisplayInCurrentBase();
                    }

                    _isNewCalculation = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Eroare la lipirea valorii: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        });
        #endregion


        #region SaveCutCopyPasteCommands
        private void SaveToClipboardFile(string value)
        {
            try
            {
                string filePath = "C:\\Users\\Roxana\\Desktop\\MAP\\Calculator\\Calculator\\Support\\clipboard.txt";

                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    writer.WriteLine(value);
                }

                MessageBox.Show($"Valoarea '{value}' a fost salvată în:\n{filePath}", "Debug Salvare");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea în clipboard: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string LoadFromClipboardFile()
        {
            try
            {
                string filePath = "C:\\Users\\Roxana\\Desktop\\MAP\\Calculator\\Calculator\\Support\\clipboard.txt";

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string result = reader.ReadLine() ?? "";
                        MessageBox.Show($"Valoarea '{result}' a fost încărcată din:\n{filePath}", "Debug Încărcare");
                        return result;
                    }
                }
                else
                {
                    MessageBox.Show($"Fișierul nu există la calea:\n{filePath}", "Debug Încărcare");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea din clipboard: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return "";
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}