using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class FixedRateRebateCalculator : IRebateCalculator
{
    public bool CanCalculate(IncentiveType incentiveType) => 
        incentiveType == IncentiveType.FixedRateRebate;

    public RebateCalculationResult Calculate(RebateCalculationParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(parameters.Rebate);
        ArgumentNullException.ThrowIfNull(parameters.Product);

        if (!parameters.Volume.HasValue)
        {
            return RebateCalculationResult.Failure("Volume is required for Fixed Rate Rebate calculation");
        }

        // Business logic validations
        if (parameters.Volume.Value <= 0)
        {
            return RebateCalculationResult.Failure("Volume must be greater than zero");
        }

        if (!parameters.Product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
        {
            return RebateCalculationResult.Failure("Product does not support Fixed Rate Rebate incentive");
        }

        if (parameters.Rebate.Percentage == 0)
        {
            return RebateCalculationResult.Failure("Rebate percentage must be greater than zero");
        }

        if (parameters.Product.Price == 0)
        {
            return RebateCalculationResult.Failure("Product price must be greater than zero");
        }

        var amount = parameters.Product.Price * parameters.Rebate.Percentage * parameters.Volume.Value;
        return RebateCalculationResult.Success(amount);
    }
}