using System.Windows;
using System.Windows.Input;
using Calculator.ViewModel;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private CalculatorView _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new CalculatorView();
            DataContext = _viewModel;

            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                _viewModel.EqualsCommand.Execute(null);
            }
            else
            {
                _viewModel.HandleKeyInput(e.Key);
            }
        }
    }
}