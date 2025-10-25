using System;
using Microsoft.Extensions.DependencyInjection;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.RebateCalculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        var serviceProvider = services.BuildServiceProvider();
        var rebateService = serviceProvider.GetRequiredService<IRebateService>();

        Console.WriteLine("Welcome to Rebate Calculator!");
        Console.WriteLine("-----------------------------");

        Console.Write("Enter Rebate Identifier (e.g., REB-1): ");
        var rebateIdentifier = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(rebateIdentifier))
        {
            Console.WriteLine("Rebate Identifier cannot be empty.");
            return;
        }

        Console.Write("Enter Product Identifier (e.g., PROD-1): ");
        var productIdentifier = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(productIdentifier))
        {
            Console.WriteLine("Product Identifier cannot be empty.");
            return;
        }

        Console.Write("Enter Volume (must be greater than 0): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal volume))
        {
            Console.WriteLine("Invalid volume input. Please enter a valid number.");
            return;
        }

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };

        try
        {
            var result = rebateService.Calculate(request);
            Console.WriteLine();
            
            if (result.Success)
            {
                Console.WriteLine("Calculation succeeded!");
            }
            else
            {
                Console.WriteLine("Calculation failed:");
                Console.WriteLine($"  Reason: {result.ErrorMessage}");
            }
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Data stores as Singleton because they use static in-memory collections
        services.AddSingleton<IRebateDataStore, RebateDataStore>();
        services.AddSingleton<IProductDataStore, ProductDataStore>();

        // Calculators as Singleton because they are stateless
        services.AddSingleton<IRebateCalculator, FixedCashAmountCalculator>();
        services.AddSingleton<IRebateCalculator, FixedRateRebateCalculator>();
        services.AddSingleton<IRebateCalculator, AmountPerUomCalculator>();

        // Main service as Transient because it orchestrates the workflow
        services.AddTransient<IRebateService, RebateService>();
    }
}
