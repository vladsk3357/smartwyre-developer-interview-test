using Moq;
using Xunit;
using Smartwyre.DeveloperTest.Types;
using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services.RebateCalculators;
using Smartwyre.DeveloperTest.Services;

namespace Smartwyre.DeveloperTest.Tests.Services;

public class RebateServiceTests
{
    private readonly Mock<IRebateDataStore> _mockRebateDataStore;
    private readonly Mock<IProductDataStore> _mockProductDataStore;
    private readonly Mock<IRebateCalculator> _mockCalculator;

    public RebateServiceTests()
    {
        _mockRebateDataStore = new Mock<IRebateDataStore>();
        _mockProductDataStore = new Mock<IProductDataStore>();
        _mockCalculator = new Mock<IRebateCalculator>();
    }

    [Fact]
    public void Calculate_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.Calculate(null));
    }

    [Fact]
    public void Calculate_WhenRebateNotFound_ReturnsFailureResult()
    {
        // Arrange
        var service = CreateService();
        var request = new CalculateRebateRequest { RebateIdentifier = "R1", ProductIdentifier = "P1" };
        _mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((Rebate)null);

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Rebate not found", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WhenProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var service = CreateService();
        var request = new CalculateRebateRequest { RebateIdentifier = "R1", ProductIdentifier = "P1" };
        _mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate());
        _mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((Product)null);

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Product not found", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WhenNoMatchingCalculator_ReturnsFailureResult()
    {
        // Arrange
        var service = CreateService();
        var request = new CalculateRebateRequest { RebateIdentifier = "R1", ProductIdentifier = "P1" };
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product());
        _mockCalculator.Setup(x => x.CanCalculate(It.IsAny<IncentiveType>())).Returns(false);

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No calculator found", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WhenCalculationFails_ReturnsFailureResult()
    {
        // Arrange
        var service = CreateService();
        var request = new CalculateRebateRequest { RebateIdentifier = "R1", ProductIdentifier = "P1" };
        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product());
        _mockCalculator.Setup(x => x.CanCalculate(It.IsAny<IncentiveType>())).Returns(true);
        _mockCalculator.Setup(x => x.Calculate(It.IsAny<RebateCalculationParameters>()))
            .Returns(RebateCalculationResult.Failure("Calculation failed"));

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Calculation failed", result.ErrorMessage);
    }

    [Fact]
    public void Calculate_WhenSuccessful_ReturnsSuccessResultAndStoresCalculation()
    {
        // Arrange
        var service = CreateService();
        var request = new CalculateRebateRequest { RebateIdentifier = "R1", ProductIdentifier = "P1" };
        var rebate = new Rebate { Identifier = "R1", Incentive = IncentiveType.FixedCashAmount };
        _mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);
        _mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product());
        _mockCalculator.Setup(x => x.CanCalculate(It.IsAny<IncentiveType>())).Returns(true);
        _mockCalculator.Setup(x => x.Calculate(It.IsAny<RebateCalculationParameters>()))
            .Returns(RebateCalculationResult.Success(100m));

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.True(result.Success);
        _mockRebateDataStore.Verify(x => x.StoreCalculationResult(It.IsAny<RebateCalculation>()), Times.Once);
    }

    private RebateService CreateService()
    {
        return new RebateService(
            _mockRebateDataStore.Object,
            _mockProductDataStore.Object,
            [_mockCalculator.Object]);
    }
}
