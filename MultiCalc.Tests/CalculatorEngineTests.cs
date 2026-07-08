using MultiCalc;
using Xunit;

namespace MultiCalc.Tests;

public class CalculatorEngineTests
{
    private readonly CalculatorEngine _calc = new();

    [Fact]
    public void StartsWithZero()
    {
        Assert.Equal("0", _calc.Display);
    }

    [Theory]
    [InlineData('1', "1")]
    [InlineData('2', "2")]
    [InlineData('0', "0")]
    public void InputDigit_Works(char digit, string expected)
    {
        _calc.Clear();
        _calc.InputDigit(digit);
        Assert.Equal(expected, _calc.Display);
    }

    [Fact]
    public void BasicAddition()
    {
        _calc.InputDigit('2');
        _calc.InputOperator("+");
        _calc.InputDigit('3');
        _calc.Calculate();
        Assert.Equal("5", _calc.Display);
    }

    [Fact]
    public void BasicSubtraction()
    {
        _calc.InputDigit('1'); _calc.InputDigit('0');
        _calc.InputOperator("-");
        _calc.InputDigit('3');
        _calc.Calculate();
        Assert.Equal("7", _calc.Display);
    }

    [Fact]
    public void BasicMultiplication()
    {
        _calc.InputDigit('4');
        _calc.InputOperator("*");
        _calc.InputDigit('5');
        _calc.Calculate();
        Assert.Equal("20", _calc.Display);
    }

    [Fact]
    public void BasicDivision()
    {
        _calc.InputDigit('2'); _calc.InputDigit('0');
        _calc.InputOperator("/");
        _calc.InputDigit('4');
        _calc.Calculate();
        Assert.Equal("5", _calc.Display);
    }

    [Fact]
    public void DivisionByZero_SetsError()
    {
        _calc.InputDigit('1');
        _calc.InputOperator("/");
        _calc.InputDigit('0');
        _calc.Calculate();
        Assert.Equal("Error", _calc.Display);
    }

    [Fact]
    public void ChainedOperations()
    {
        // 2 + 3 = 5, then * 4 = 20
        _calc.InputDigit('2');
        _calc.InputOperator("+");
        _calc.InputDigit('3');
        _calc.Calculate();
        Assert.Equal("5", _calc.Display);

        _calc.InputOperator("*");
        _calc.InputDigit('4');
        _calc.Calculate();
        Assert.Equal("20", _calc.Display);
    }

    [Fact]
    public void DecimalInput()
    {
        _calc.InputDigit('1');
        _calc.InputDecimal();
        _calc.InputDigit('5');
        _calc.InputOperator("+");
        _calc.InputDigit('2');
        _calc.Calculate();
        Assert.Equal("3.5", _calc.Display);
    }

    [Fact]
    public void Negate()
    {
        _calc.InputDigit('5');
        _calc.Negate();
        Assert.Equal("-5", _calc.Display);
        _calc.Negate();
        Assert.Equal("5", _calc.Display);
    }

    [Fact]
    public void Percent()
    {
        _calc.InputDigit('2'); _calc.InputDigit('0'); _calc.InputDigit('0');
        _calc.Percent();
        Assert.Equal("2", _calc.Display); // 200 / 100 = 2
    }

    [Fact]
    public void Backspace()
    {
        _calc.InputDigit('1');
        _calc.InputDigit('2');
        _calc.InputDigit('3');
        _calc.Backspace();
        Assert.Equal("12", _calc.Display);
        _calc.Backspace();
        _calc.Backspace();
        Assert.Equal("0", _calc.Display);
    }

    [Fact]
    public void ClearResets()
    {
        _calc.InputDigit('9');
        _calc.InputOperator("+");
        _calc.InputDigit('1');
        _calc.Clear();
        Assert.Equal("0", _calc.Display);
        Assert.Equal(string.Empty, _calc.CurrentOperation);
    }

    [Theory]
    [InlineData("1+2=", "3")]
    [InlineData("10-3-2=", "5")]
    public void MultipleEqualsAndChaining(string sequence, string expectedFinal)
    {
        // Very simple interpreter for the sequence for test
        _calc.Clear();
        foreach (char c in sequence)
        {
            if (char.IsDigit(c))
                _calc.InputDigit(c);
            else if (c == '.')
                _calc.InputDecimal();
            else if (c is '+' or '-' or '*' or '/')
                _calc.InputOperator(c.ToString());
            else if (c == '=')
                _calc.Calculate();
        }
        Assert.Equal(expectedFinal, _calc.Display);
    }

    [Fact]
    public void LeadingZeroHandling()
    {
        _calc.InputDigit('0');
        _calc.InputDigit('0');
        _calc.InputDigit('7');
        Assert.Equal("7", _calc.Display);
    }
}
