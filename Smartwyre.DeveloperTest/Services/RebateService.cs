using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Services.RebateCalculators;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IEnumerable<IRebateCalculator> _rebateCalculators;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IEnumerable<IRebateCalculator> rebateCalculators)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
        _rebateCalculators = rebateCalculators;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        if (rebate == null)
        {
            return CalculateRebateResult.Failure($"Rebate not found with identifier: {request.RebateIdentifier}");
        }

        var product = _productDataStore.GetProduct(request.ProductIdentifier);
        if (product == null)
        {
            return CalculateRebateResult.Failure($"Product not found with identifier: {request.ProductIdentifier}");
        }

        var calculator = _rebateCalculators.FirstOrDefault(c => c.CanCalculate(rebate.Incentive));
        if (calculator == null)
        {
            return CalculateRebateResult.Failure($"No calculator found for incentive type: {rebate.Incentive}");
        }

        var parameters = new RebateCalculationParameters
        {
            Rebate = rebate,
            Product = product,
            Volume = request.Volume
        };

        var calculationResult = calculator.Calculate(parameters);
        if (!calculationResult.IsSuccess)
        {
            return CalculateRebateResult.Failure(calculationResult.ErrorMessage ?? "Calculation failed");
        }

        var calculation = new RebateCalculation
        {
            RebateIdentifier = rebate.Identifier,
            Amount = calculationResult.Amount,
            IncentiveType = rebate.Incentive,
            Identifier = Guid.NewGuid().ToString(),
        };
        _rebateDataStore.StoreCalculationResult(calculation);

        return CalculateRebateResult.Successful();
    }
}
