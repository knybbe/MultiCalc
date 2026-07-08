using MultiCalc;

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
}
