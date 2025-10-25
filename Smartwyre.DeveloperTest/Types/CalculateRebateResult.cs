#nullable enable

namespace Smartwyre.DeveloperTest.Types;

public class CalculateRebateResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static CalculateRebateResult Failure(string errorMessage) => 
        new() { Success = false, ErrorMessage = errorMessage };

    public static CalculateRebateResult Successful() => 
        new() { Success = true };
}
