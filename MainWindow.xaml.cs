using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CalculatorApp.Core;
using System.Globalization;

namespace CalculatorApp;

public partial class MainWindow : Window
{
    private string _currentInput = "0";
    private readonly CalculatorEngine _calculator = new();
    private decimal? _firstNumber;
    private string? _pendingOperation;
    private bool _startNewInput;
    public MainWindow()
    {
        InitializeComponent();
    }
    private void UpdateDisplay()
    {
        DisplayText.Text = _currentInput;
    }
    private void Digit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        string digit = button.Content.ToString()!;

        if (_startNewInput)
        {
            _currentInput = "0";
            _startNewInput = false;

            if (_pendingOperation is null)
                ExpressionText.Text = "";
        }

        if (digit == "." && _currentInput.Contains('.'))
            return;

        _currentInput = _currentInput == "0" && digit != "."
            ? digit
            : _currentInput + digit;

        UpdateDisplay();
    }
    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ResetCalculator();
    }
    private void Backspace_Click(object sender, RoutedEventArgs e)
    {
        if (_startNewInput)
        {
            _currentInput = "0";
            _startNewInput = false;

            if (_pendingOperation is null)
                ExpressionText.Text = "";
        }
        else
        {
            _currentInput = _currentInput.Length <= 1
                ? "0"
                : _currentInput[..^1];
        }

        UpdateDisplay();
    }

    private bool TryReadInput(out decimal number)
    {
        if (decimal.TryParse(
            _currentInput,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out number))
        {
            return true;
        }

        MessageBox.Show("Некоректне або занадто велике число.");
        ResetCalculator();

        return false;
    }

    private void Operation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Content is not string operation)
            return;

        if (!TryReadInput(out decimal current))
            return;

        if (_pendingOperation is not null &&
            !_startNewInput &&
            _firstNumber is decimal first)
        {
            if (!TryCalculate(first, current, _pendingOperation,
                out decimal result))
                return;

            current = result;
            _currentInput = result.ToString(CultureInfo.InvariantCulture);

            UpdateDisplay();
        }

        _firstNumber = current;
        _pendingOperation = operation;
        _startNewInput = true;

        ExpressionText.Text = $"{_currentInput} {operation}";
    }

    private bool TryCalculate(
    decimal first,
    decimal second,
    string operation,
    out decimal result)
    {
        try
        {
            result = _calculator.Calculate(first, second, operation);
            return true;
        }
        catch (DivideByZeroException)
        {
            MessageBox.Show("Ділення на нуль неможливе.");
        }
        catch (OverflowException)
        {
            MessageBox.Show("Результат виходить за допустимий діапазон.");
        }

        result = 0;
        ResetCalculator();

        return false;
    }

    private void Equals_Click(object sender, RoutedEventArgs e)
    {
        if (_firstNumber is null ||
            _pendingOperation is null ||
            _startNewInput)
            return;

        if (!TryReadInput(out decimal second))
            return;

        decimal first = _firstNumber.Value;
        string operation = _pendingOperation;

        if (!TryCalculate(first, second, operation, out decimal result))
            return;

        ExpressionText.Text = $"{first} {operation} {_currentInput} =";

        _currentInput = result.ToString(CultureInfo.InvariantCulture);
        _firstNumber = null;
        _pendingOperation = null;
        _startNewInput = true;

        UpdateDisplay();
    }

    private void ResetCalculator()
    {
        _currentInput = "0";
        _firstNumber = null;
        _pendingOperation = null;
        _startNewInput = false;

        ExpressionText.Text = "";

        UpdateDisplay();
    }
}