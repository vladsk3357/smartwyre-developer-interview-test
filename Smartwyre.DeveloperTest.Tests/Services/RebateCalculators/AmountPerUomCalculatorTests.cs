using System;
using Xunit;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Services.RebateCalculators;

namespace Smartwyre.DeveloperTest.Tests.Services.RebateCalculators;

public class AmountPerUomCalculatorTests
{
    private readonly AmountPerUomCalculator _calculator;

    public AmountPerUomCalculatorTests()
    {
        _calculator = new AmountPerUomCalculator();
    }

    [Fact]
    public void CanCalculate_WithAmountPerUom_ReturnsTrue()
    {
        var result = _calculator.CanCalculate(IncentiveType.AmountPerUom);
        Assert.True(result);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount)]
    [InlineData(IncentiveType.FixedRateRebate)]
    public void CanCalculate_WithOtherIncentiveTypes_ReturnsFalse(IncentiveType incentiveType)
    {
        var result = _calculator.CanCalculate(incentiveType);
        Assert.False(result);
    }

    [Fact]
    public void Calculate_WithNullParameters_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(null));
    }

    [Fact]
    public void Calculate_WithNullRebate_ThrowsArgumentNullException()
    {
        var parameters = new RebateCalculationParameters
        {
            Product = new Product(),
            Volume = 10,
            Rebate = null,
        };

        Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(parameters));
    }

    [Fact]
    public void Calculate_WithNullProduct_ThrowsArgumentNullException()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate(),
            Volume = 10,
            Product = null,
        };

        Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(parameters));
    }

    [Fact]
    public void Calculate_WithMissingVolume_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate(),
            Product = new Product()
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Volume is required for Amount Per UOM calculation", result.ErrorMessage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Calculate_WithInvalidVolume_ReturnsFailure(decimal volume)
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate(),
            Product = new Product(),
            Volume = volume
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Volume must be greater than zero", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithUnsupportedProduct_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Amount = 10 },
            Product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount },
            Volume = 10
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product does not support Amount Per UOM incentive", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithZeroRebateAmount_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Amount = 0 },
            Product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom },
            Volume = 10
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Rebate amount must be greater than zero", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithValidParameters_ReturnsSuccess()
    {
        var rebateAmount = 5m;
        var volume = 10m;
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Amount = rebateAmount },
            Product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom },
            Volume = volume
        };

        var result = _calculator.Calculate(parameters);

        Assert.True(result.IsSuccess);
        Assert.Equal(rebateAmount * volume, result.Amount);
    }
}
