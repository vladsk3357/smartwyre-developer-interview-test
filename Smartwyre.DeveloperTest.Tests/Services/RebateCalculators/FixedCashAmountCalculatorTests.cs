using Xunit;
using Smartwyre.DeveloperTest.Types;
using System;
using Smartwyre.DeveloperTest.Services.RebateCalculators;

namespace Smartwyre.DeveloperTest.Tests.Services.RebateCalculators;

public class FixedCashAmountCalculatorTests
{
    private readonly FixedCashAmountCalculator _calculator;

    public FixedCashAmountCalculatorTests()
    {
        _calculator = new FixedCashAmountCalculator();
    }

    [Fact]
    public void CanCalculate_WithFixedCashAmount_ReturnsTrue()
    {
        var result = _calculator.CanCalculate(IncentiveType.FixedCashAmount);
        Assert.True(result);
    }

    [Theory]
    [InlineData(IncentiveType.AmountPerUom)]
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
            Product = null,
        };

        Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(parameters));
    }

    [Fact]
    public void Calculate_WithUnsupportedProduct_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Amount = 100 },
            Product = new Product { SupportedIncentives = SupportedIncentiveType.AmountPerUom }
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product does not support Fixed Cash Amount incentive", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithZeroRebateAmount_ReturnsFailure()
    {
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Amount = 0 },
            Product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount }
        };

        var result = _calculator.Calculate(parameters);

        Assert.False(result.IsSuccess);
        Assert.Equal("Rebate amount must be greater than zero", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WithValidParameters_ReturnsSuccess()
    {
        var rebateAmount = 100m;
        var parameters = new RebateCalculationParameters
        {
            Rebate = new Rebate { Amount = rebateAmount },
            Product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount }
        };

        var result = _calculator.Calculate(parameters);

        Assert.True(result.IsSuccess);
        Assert.Equal(rebateAmount, result.Amount);
    }
}
