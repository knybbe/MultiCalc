using MultiCalc;
using Microsoft.UI.Xaml.Input;

namespace MultiCalc;

public sealed partial class MainPage : Page
{
    private readonly CalculatorEngine _engine = new();

    public MainPage()
    {
        this.InitializeComponent();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        ResultDisplay.Text = _engine.Display;
        // Simple operation indicator
        var op = _engine.CurrentOperation;
        OperationDisplay.Text = string.IsNullOrEmpty(op) ? "" : $" {op} ";
    }

    private void OnNumber(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Content is string txt && txt.Length > 0)
        {
            // Support "00" if we add it, but grid uses single digits
            foreach (char c in txt)
            {
                if (char.IsDigit(c))
                    _engine.InputDigit(c);
            }
            UpdateDisplay();
        }
    }

    private void OnDecimal(object sender, RoutedEventArgs e)
    {
        _engine.InputDecimal();
        UpdateDisplay();
    }

    private void OnOperator(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Content is string op)
        {
            // Normalize symbols to internal
            string normalized = op switch
            {
                "÷" => "/",
                "×" => "*",
                "−" => "-",
                _ => op
            };
            _engine.InputOperator(normalized);
            UpdateDisplay();
        }
    }

    private void OnEquals(object sender, RoutedEventArgs e)
    {
        _engine.Calculate();
        UpdateDisplay();
    }

    private void OnClear(object sender, RoutedEventArgs e)
    {
        _engine.Clear();
        UpdateDisplay();
    }

    private void OnNegate(object sender, RoutedEventArgs e)
    {
        _engine.Negate();
        UpdateDisplay();
    }

    private void OnPercent(object sender, RoutedEventArgs e)
    {
        _engine.Percent();
        UpdateDisplay();
    }

    private void OnBackspace(object sender, RoutedEventArgs e)
    {
        _engine.Backspace();
        UpdateDisplay();
    }

    private void OnOpenParen(object sender, RoutedEventArgs e)
    {
        _engine.InputOpenParenthesis();
        UpdateDisplay();
    }

    private void OnCloseParen(object sender, RoutedEventArgs e)
    {
        _engine.InputCloseParenthesis();
        UpdateDisplay();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);

        // Digits
        if ((e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9) ||
            ((int)e.Key >= 96 && (int)e.Key <= 105))
        {
            int num = (int)e.Key - (int)Windows.System.VirtualKey.Number0;
            if ((int)e.Key >= 96)
                num = (int)e.Key - 96;
            _engine.InputDigit((char)('0' + num));
            UpdateDisplay();
            e.Handled = true;
            return;
        }

        // Operators
        string op = e.Key switch
        {
            Windows.System.VirtualKey.Add or (Windows.System.VirtualKey)107 => "+",
            Windows.System.VirtualKey.Subtract or (Windows.System.VirtualKey)109 => "-",
            Windows.System.VirtualKey.Multiply or (Windows.System.VirtualKey)106 => "*",
            Windows.System.VirtualKey.Divide or (Windows.System.VirtualKey)111 => "/",
            _ => null
        };
        if (op != null)
        {
            _engine.InputOperator(op);
            UpdateDisplay();
            e.Handled = true;
            return;
        }

        // Special keys
        switch (e.Key)
        {
            case Windows.System.VirtualKey.Enter or (Windows.System.VirtualKey)13:
                _engine.Calculate();
                UpdateDisplay();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Back or Windows.System.VirtualKey.Delete:
                _engine.Backspace();
                UpdateDisplay();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Decimal or (Windows.System.VirtualKey)110:
                _engine.InputDecimal();
                UpdateDisplay();
                e.Handled = true;
                break;
            case (Windows.System.VirtualKey)57: // approximate for paren keys, or use other
            case (Windows.System.VirtualKey)219:
                _engine.InputOpenParenthesis();
                UpdateDisplay();
                e.Handled = true;
                break;
            case (Windows.System.VirtualKey)48: // rough
            case (Windows.System.VirtualKey)221:
                _engine.InputCloseParenthesis();
                UpdateDisplay();
                e.Handled = true;
                break;
        }
    }
}
