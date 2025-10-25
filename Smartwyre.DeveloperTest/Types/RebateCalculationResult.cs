namespace Smartwyre.DeveloperTest.Types;

public class RebateCalculationResult
{
    public bool IsSuccess { get; }
    public decimal Amount { get; }
    public string ErrorMessage { get; }

    private RebateCalculationResult(bool isSuccess, decimal amount = 0, string errorMessage = null)
    {
        IsSuccess = isSuccess;
        Amount = amount;
        ErrorMessage = errorMessage;
    }

    public static RebateCalculationResult Success(decimal amount) => 
        new(true, amount);

    public static RebateCalculationResult Failure(string errorMessage) => 
        new(false, errorMessage: errorMessage);
}