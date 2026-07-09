using System;
using System.Data;
using System.Globalization;

namespace MultiCalc;

/// <summary>
/// Pure C# calculator engine. No UI dependencies. Handles basic operations.
/// State machine for display, pending operations, and error conditions.
/// </summary>
public class CalculatorEngine
{
    private string _display = "0";
    private double _accumulator = 0;
    private string _pendingOperator = string.Empty;
    private bool _isNewEntry = true;
    private bool _hasError = false;
    private bool _hasParens = false;

    public string Display => _hasError ? "Error" : _display;

    public string CurrentOperation => string.IsNullOrEmpty(_pendingOperator) ? string.Empty : _pendingOperator;

    public void InputDigit(char digit)
    {
        if (_hasError) Clear();

        if (_isNewEntry)
        {
            _display = digit.ToString();
            _isNewEntry = false;
        }
        else
        {
            if (_display == "0")
                _display = digit.ToString();
            else
                _display += digit;
        }
    }

    public void InputDecimal()
    {
        if (_hasError) Clear();

        if (_isNewEntry)
        {
            _display = "0.";
            _isNewEntry = false;
        }
        else if (!_display.Contains("."))
        {
            _display += ".";
        }
    }

    public void InputOperator(string op)
    {
        if (_hasError) return;

        if (!string.IsNullOrEmpty(_pendingOperator) && !_isNewEntry)
        {
            Calculate();
            if (_hasError) return;
        }

        if (double.TryParse(_display, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            _accumulator = value;
        }

        _pendingOperator = op;
        _isNewEntry = true;
    }

    public void Calculate()
    {
        if (_hasError) return;

        if (_hasParens || _display.Contains('(') || _display.Contains(')'))
        {
            // Use full expression evaluation for parenthesis support (supports precedence and grouping)
            try
            {
                var dt = new DataTable();
                var expressionResult = Convert.ToDouble(dt.Compute(_display, null));
                _display = FormatResult(expressionResult);
                _accumulator = expressionResult;
                _pendingOperator = string.Empty;
                _isNewEntry = true;
                _hasParens = false;
            }
            catch
            {
                SetError();
            }
            return;
        }

        if (string.IsNullOrEmpty(_pendingOperator))
        {
            _isNewEntry = true;
            return;
        }

        if (!double.TryParse(_display, NumberStyles.Any, CultureInfo.InvariantCulture, out double current))
        {
            SetError();
            return;
        }

        double result;
        try
        {
            result = _pendingOperator switch
            {
                "+" => _accumulator + current,
                "-" => _accumulator - current,
                "×" or "*" => _accumulator * current,
                "÷" or "/" => current == 0 ? throw new DivideByZeroException() : _accumulator / current,
                _ => current
            };
        }
        catch (DivideByZeroException)
        {
            SetError();
            return;
        }
        catch
        {
            SetError();
            return;
        }

        _display = FormatResult(result);
        _accumulator = result;
        _pendingOperator = string.Empty;
        _isNewEntry = true;
    }

    public void Clear()
    {
        _display = "0";
        _accumulator = 0;
        _pendingOperator = string.Empty;
        _isNewEntry = true;
        _hasError = false;
        _hasParens = false;
    }

    public void ClearEntry()
    {
        _display = "0";
        _isNewEntry = true;
    }

    public void Negate()
    {
        if (_hasError || _display == "0") return;

        if (_display.StartsWith("-"))
            _display = _display[1..];
        else
            _display = "-" + _display;
    }

    public void Percent()
    {
        if (_hasError) return;

        if (double.TryParse(_display, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            // Simple percent: divide current by 100. For 200 + 10% semantics, caller/UI can use differently.
            double result = value / 100.0;
            _display = FormatResult(result);
            _isNewEntry = true;
        }
    }

    public void Backspace()
    {
        if (_hasError || _isNewEntry)
        {
            ClearEntry();
            return;
        }

        if (_display.Length <= 1 || (_display.Length == 2 && _display.StartsWith("-")))
        {
            _display = "0";
            _isNewEntry = true;
        }
        else
        {
            _display = _display[..^1];
        }
    }

    public void SetValue(double value)
    {
        _display = FormatResult(value);
        _isNewEntry = true;
    }

    public void InputOpenParenthesis()
    {
        if (_hasError) Clear();

        if (!_isNewEntry && string.IsNullOrEmpty(_pendingOperator))
        {
            // implicit multiply, e.g. 2(
            _display += "*(";
        }
        else
        {
            _display += "(";
        }
        _isNewEntry = true;
        _hasParens = true;
    }

    public void InputCloseParenthesis()
    {
        if (_hasError) return;

        _display += ")";
        _isNewEntry = false;
        _hasParens = true;
    }

    private void SetError()
    {
        _hasError = true;
        _display = "Error";
        _pendingOperator = string.Empty;
        _isNewEntry = true;
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
