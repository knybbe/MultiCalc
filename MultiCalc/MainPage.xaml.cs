using MultiCalc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace MultiCalc;

public sealed partial class MainPage : Page
{
    private readonly CalculatorEngine _engine = new();

    public MainPage()
    {
        this.InitializeComponent();
        ExpressionInput.Text = _engine.Display;
        ExpressionInput.KeyDown += ExpressionInput_KeyDown;
    }

    private void OnExpressionChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            _engine.SetExpression(tb.Text);
        }
    }

    private void ExpressionInput_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter || (int)e.Key == 13)
        {
            _engine.Calculate();
            ExpressionInput.Text = _engine.Display;
            ExpressionInput.SelectAll();
            e.Handled = true;
        }
        else if (e.Key == Windows.System.VirtualKey.Escape)
        {
            _engine.Clear();
            ExpressionInput.Text = _engine.Display;
            e.Handled = true;
        }
    }

    private void OnNumber(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Content is string txt && txt.Length > 0)
        {
            foreach (char c in txt)
            {
                if (char.IsDigit(c))
                    _engine.InputDigit(c);
            }
            ExpressionInput.Text = _engine.Display;
        }
    }

    private void OnDecimal(object sender, RoutedEventArgs e)
    {
        _engine.InputDecimal();
        ExpressionInput.Text = _engine.Display;
    }

    private void OnOperator(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Content is string op)
        {
            // pass the button symbol as-is; engine + normalize at eval handles ÷ × − / * etc
            _engine.InputOperator(op);
            ExpressionInput.Text = _engine.Display;
        }
    }

    private void OnEquals(object sender, RoutedEventArgs e)
    {
        _engine.Calculate();
        ExpressionInput.Text = _engine.Display;
        ExpressionInput.SelectAll();
    }

    private void OnClear(object sender, RoutedEventArgs e)
    {
        _engine.Clear();
        ExpressionInput.Text = _engine.Display;
    }

    private void OnNegate(object sender, RoutedEventArgs e)
    {
        _engine.Negate();
        ExpressionInput.Text = _engine.Display;
    }

    private void OnPercent(object sender, RoutedEventArgs e)
    {
        _engine.Percent();
        ExpressionInput.Text = _engine.Display;
    }

    private void OnBackspace(object sender, RoutedEventArgs e)
    {
        _engine.Backspace();
        ExpressionInput.Text = _engine.Display;
    }

    private void OnOpenParen(object sender, RoutedEventArgs e)
    {
        _engine.InputOpenParenthesis();
        ExpressionInput.Text = _engine.Display;
    }

    private void OnCloseParen(object sender, RoutedEventArgs e)
    {
        _engine.InputCloseParenthesis();
        ExpressionInput.Text = _engine.Display;
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);

        // Let the ExpressionInput TextBox handle normal typing (digits, operators, parens, decimal, backspace).
        // Handle a few global/special keys here for convenience.

        switch (e.Key)
        {
            case Windows.System.VirtualKey.Enter or (Windows.System.VirtualKey)13:
                _engine.Calculate();
                ExpressionInput.Text = _engine.Display;
                ExpressionInput.SelectAll();
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Escape:
                _engine.Clear();
                ExpressionInput.Text = _engine.Display;
                e.Handled = true;
                break;
            case Windows.System.VirtualKey.Back or Windows.System.VirtualKey.Delete:
                // If the TextBox isn't focused, still allow backspace via engine
                if (ExpressionInput.FocusState == FocusState.Unfocused)
                {
                    _engine.Backspace();
                    ExpressionInput.Text = _engine.Display;
                    e.Handled = true;
                }
                break;
        }
    }
}
