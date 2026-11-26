namespace CalculatorApp.Core;

public class CalculatorEngine
{
    public decimal Calculate(decimal first, decimal second, string operation)
    {
        return operation switch
        {
            "+" => first + second,
            "−" => first - second,
            "×" => first * second,

            "÷" when second == 0 =>
                throw new DivideByZeroException("Ділення на нуль неможливе."),

            "÷" => first / second,

            _ => throw new ArgumentException("Невідома операція.")
        };
    }
}