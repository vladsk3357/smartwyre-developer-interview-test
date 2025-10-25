namespace Smartwyre.DeveloperTest.Types;

public class RebateCalculationParameters
{
    public required Rebate Rebate { get; init; }
    public required Product Product { get; init; }
    public decimal? Volume { get; init; }
}