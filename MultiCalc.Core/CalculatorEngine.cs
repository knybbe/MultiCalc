using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace MultiCalc;

/// <summary>
/// Pure C# calculator engine. No UI dependencies. Handles basic operations.
/// State machine for display, pending operations, and error conditions.
/// </summary>
public class CalculatorEngine
{
    private string _expression = "0";
    private bool _hasError = false;
    private bool _justEvaluated = false;

    public string Display => _hasError ? "Error" : _expression;

    public string CurrentOperation => string.Empty; // no longer used in expression mode

    public void InputDigit(char digit)
    {
        if (_hasError) Clear();

        if (_justEvaluated)
        {
            _expression = digit.ToString();
            _justEvaluated = false;
            return;
        }

        if (_expression == "0")
            _expression = digit.ToString();
        else
            _expression += digit;
    }

    public void InputDecimal()
    {
        if (_hasError) Clear();

        if (_justEvaluated)
        {
            _expression = "0.";
            _justEvaluated = false;
            return;
        }

        if (_expression == "0" || string.IsNullOrEmpty(_expression))
        {
            _expression = "0.";
        }
        else if (!_expression.EndsWith("."))
        {
            _expression += ".";
        }
    }

    public void InputOperator(string op)
    {
        if (_hasError) return;

        if (_justEvaluated)
        {
            _justEvaluated = false;
            // continue expression from the result, e.g. "5+"
        }

        _expression += op;
    }

    public void Calculate()
    {
        if (_hasError) return;

        if (string.IsNullOrWhiteSpace(_expression))
        {
            _expression = "0";
            return;
        }

        try
        {
            string toCompute = NormalizeForCompute(_expression);
            var dt = new DataTable();
            var result = Convert.ToDouble(dt.Compute(toCompute, null));
            _expression = FormatResult(result);
            _justEvaluated = true;
        }
        catch
        {
            SetError();
        }
    }

    private string NormalizeForCompute(string expr)
    {
        if (string.IsNullOrEmpty(expr)) return "0";
        return expr
            .Replace("×", "*")
            .Replace("÷", "/")
            .Replace("−", "-")
            .Replace(" ", "")
            .Trim();
    }

    public void Clear()
    {
        _expression = "0";
        _hasError = false;
        _justEvaluated = false;
    }

    public void ClearEntry()
    {
        _expression = "0";
        _justEvaluated = false;
    }

    public void Negate()
    {
        if (_hasError || _expression == "0" || string.IsNullOrEmpty(_expression)) return;

        if (_justEvaluated)
            _justEvaluated = false;

        if (_expression.StartsWith("-"))
            _expression = _expression[1..];
        else
            _expression = "-" + _expression;
    }

    public void Percent()
    {
        if (_hasError) return;

        if (_justEvaluated)
            _justEvaluated = false;

        // Try to treat current expression (or last simple number) as value
        string toEval = NormalizeForCompute(_expression);
        if (double.TryParse(toEval, NumberStyles.Any, CultureInfo.InvariantCulture, out double value) ||
            (toEval.Length > 0 && double.TryParse(toEval.Split(new[] { '+', '-', '*', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "0", out value)))
        {
            double result = value / 100.0;
            _expression = FormatResult(result);
            _justEvaluated = true;
        }
    }

    public void Backspace()
    {
        if (_hasError || _expression == "Error")
        {
            ClearEntry();
            return;
        }

        if (_justEvaluated)
        {
            _justEvaluated = false;
            // fallthrough to edit the result value
        }

        if (_expression.Length <= 1 || (_expression.Length == 2 && _expression.StartsWith("-")))
        {
            _expression = "0";
            _justEvaluated = false;
        }
        else
        {
            _expression = _expression[..^1];
        }
    }

    public void SetValue(double value)
    {
        _expression = FormatResult(value);
        _justEvaluated = true;
    }

    public void SetExpression(string value)
    {
        _expression = value ?? "0";
        _hasError = false;
        _justEvaluated = false;
    }

    public void InputOpenParenthesis()
    {
        if (_hasError) Clear();

        if (_justEvaluated)
        {
            _justEvaluated = false;
            _expression = "(";  // after a result, start a fresh sub-expression
            return;
        }

        if (_expression == "0" || string.IsNullOrEmpty(_expression))
        {
            _expression = "(";
        }
        else if (char.IsDigit(_expression[^1]) || _expression[^1] == ')')
        {
            // implicit multiply before (
            _expression += "*(";
        }
        else
        {
            _expression += "(";
        }
    }

    public void InputCloseParenthesis()
    {
        if (_hasError) return;

        if (_justEvaluated)
            _justEvaluated = false;

        _expression += ")";
    }

    private void SetError()
    {
        _hasError = true;
        _expression = "Error";
        _justEvaluated = true;
    }

    private static string FormatResult(double value)
    {
        if (double.IsInfinity(value) || double.IsNaN(value))
            return "Error";

        // Avoid scientific notation for reasonable range; limit decimals
        if (Math.Abs(value) > 1e12 || (Math.Abs(value) < 1e-6 && value != 0))
        {
            return value.ToString("G10", CultureInfo.InvariantCulture);
        }

        string s = value.ToString("G15", CultureInfo.InvariantCulture);
        // Trim trailing zeros after decimal
        if (s.Contains('.'))
        {
            s = s.TrimEnd('0').TrimEnd('.');
        }
        return s;
    }
}
