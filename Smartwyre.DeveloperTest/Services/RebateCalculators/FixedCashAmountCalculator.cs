using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class FixedCashAmountCalculator : IRebateCalculator
{
    public bool CanCalculate(IncentiveType incentiveType) =>
        incentiveType == IncentiveType.FixedCashAmount;

    public RebateCalculationResult Calculate(RebateCalculationParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(parameters.Rebate);
        ArgumentNullException.ThrowIfNull(parameters.Product);

        // Business logic validations
        if (!parameters.Product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
        {
            return RebateCalculationResult.Failure("Product does not support Fixed Cash Amount incentive");
        }

        if (parameters.Rebate.Amount == 0)
        {
            return RebateCalculationResult.Failure("Rebate amount must be greater than zero");
        }

        return RebateCalculationResult.Success(parameters.Rebate.Amount);
    }
}