using Xunit;
using Smartwyre.DeveloperTest.Types;
using System;
using Smartwyre.DeveloperTest.Services.RebateCalculators;

namespace Smartwyre.DeveloperTest.Tests.Services.RebateCalculators;

public class FixedRateRebateCalculatorTests
{
    private readonly FixedRateRebateCalculator _calculator;

    public FixedRateRebateCalculatorTests()
    {
        _calculator = new FixedRateRebateCalculator();
    }

    [Fact]
    public void CanCalculate_WithFixedRateRebate_ReturnsTrue()
    {
        var result = _calculator.CanCalculate(IncentiveType.FixedRateRebate);
        Assert.True(result);
    }

    [Theory]
    [InlineData(IncentiveType.AmountPerUom)]
    [InlineData(IncentiveType.FixedCashAmount)]
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
        Assert.Equal("Volume is required for Fixed Rate Rebate calculation", result.ErrorMessage);
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
            Rebate = new Rebate { Percentage = 0.1m },
            Product = new Product 
            { 
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
                Price = 10
            },
            Volume = 10
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product does not support Fixed Rate Rebate incentive", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithZeroRebatePercentage_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Percentage = 0 },
            Product = new Product 
            { 
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = 10
            },
            Volume = 10
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Rebate percentage must be greater than zero", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithZeroProductPrice_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Percentage = 0.1m },
            Product = new Product 
            { 
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = 0
            },
            Volume = 10
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product price must be greater than zero", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithValidParameters_ReturnsSuccess()
    {
        var price = 10m;
        var percentage = 0.1m;
        var volume = 5m;
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Percentage = percentage },
            Product = new Product 
            { 
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                Price = price
            },
            Volume = volume
        };

        var result = _calculator.Calculate(parameters);

        Assert.True(result.IsSuccess);
        Assert.Equal(price * percentage * volume, result.Amount);
    }
}
