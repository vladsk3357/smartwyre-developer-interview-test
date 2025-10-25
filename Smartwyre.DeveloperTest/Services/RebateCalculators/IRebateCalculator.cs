using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public interface IRebateCalculator
{
    bool CanCalculate(IncentiveType incentiveType);
    RebateCalculationResult Calculate(RebateCalculationParameters parameters);
}