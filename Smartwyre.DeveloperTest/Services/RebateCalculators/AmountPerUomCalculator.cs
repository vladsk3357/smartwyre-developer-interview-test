using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.RebateCalculators;

public class AmountPerUomCalculator : IRebateCalculator
{
    public bool CanCalculate(IncentiveType incentiveType) =>
        incentiveType == IncentiveType.AmountPerUom;

    public RebateCalculationResult Calculate(RebateCalculationParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(parameters.Rebate);
        ArgumentNullException.ThrowIfNull(parameters.Product);

        if (!parameters.Volume.HasValue)
        {
            return RebateCalculationResult.Failure("Volume is required for Amount Per UOM calculation");
        }

        // Business logic validations
        if (parameters.Volume.Value <= 0)
        {
            return RebateCalculationResult.Failure("Volume must be greater than zero");
        }

        if (!parameters.Product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
        {
            return RebateCalculationResult.Failure("Product does not support Amount Per UOM incentive");
        }

        if (parameters.Rebate.Amount == 0)
        {
            return RebateCalculationResult.Failure("Rebate amount must be greater than zero");
        }

        var amount = parameters.Rebate.Amount * parameters.Volume.Value;
        return RebateCalculationResult.Success(amount);
    }
}