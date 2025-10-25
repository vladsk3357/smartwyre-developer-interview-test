using System;
using System.Collections.Concurrent;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore : IRebateDataStore
{
    private static readonly ConcurrentDictionary<string, Rebate> _rebates = new();
    private static readonly ConcurrentDictionary<string, RebateCalculation> _calculations = new();

    public RebateDataStore()
    {
        _rebates.TryAdd("REB-1", new Rebate
        {
            Identifier = "REB-1",
            Amount = 50m,
            Incentive = IncentiveType.FixedCashAmount,
            Percentage = 0m
        });

        _rebates.TryAdd("REB-2", new Rebate
        {
            Identifier = "REB-2",
            Amount = 0m,
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0.1m
        });

        _rebates.TryAdd("REB-3", new Rebate
        {
            Identifier = "REB-3",
            Amount = 10m,
            Incentive = IncentiveType.AmountPerUom,
            Percentage = 0m
        });
    }

    public Rebate GetRebate(string rebateIdentifier)
    {
        ArgumentNullException.ThrowIfNull(rebateIdentifier);

        return _rebates.TryGetValue(rebateIdentifier, out var rebate) ? rebate : null;
    }

    public void StoreCalculationResult(RebateCalculation calculation)
    {
        ArgumentNullException.ThrowIfNull(calculation);
        ArgumentNullException.ThrowIfNull(calculation.RebateIdentifier);

        _calculations.AddOrUpdate(
            calculation.RebateIdentifier,
            calculation,
            (_, _) => calculation);

    }
}
