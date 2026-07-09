using MultiCalc;
using Windows.UI.Xaml.Input;

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
            (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9))
        {
            int num = (int)e.Key - (int)Windows.System.VirtualKey.Number0;
            if (e.Key >= Windows.System.VirtualKey.NumberPad0)
                num = (int)e.Key - (int)Windows.System.VirtualKey.NumberPad0;
            _engine.InputDigit((char)('0' + num));
            UpdateDisplay();
            e.Handled = true;
            return;
        }

        // Operators
        string op = e.Key switch
        {
            Windows.System.VirtualKey.Add or Windows.System.VirtualKey.NumberPadAdd => "+",
            Windows.System.VirtualKey.Subtract or Windows.System.VirtualKey.NumberPadSubtract => "-",
            Windows.System.VirtualKey.Multiply or Windows.System.VirtualKey.NumberPadMultiply => "*",
            Windows.System.VirtualKey.Divide or Windows.System.VirtualKey.NumberPadDivide => "/",
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
            case Windows.System.VirtualKey.Enter or Windows.System.VirtualKey.NumberPadEnter:
                _engine.Calculate();
                UpdateDisplay();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Back or Windows.System.VirtualKey.Delete:
                _engine.Backspace();
                UpdateDisplay();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Decimal or Windows.System.VirtualKey.NumberPadDecimal:
                _engine.InputDecimal();
                UpdateDisplay();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.LeftParenthesis:
                _engine.InputOpenParenthesis();
                UpdateDisplay();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.RightParenthesis:
                _engine.InputCloseParenthesis();
                UpdateDisplay();
                e.Handled = true;
                break;
        }
    }
}
