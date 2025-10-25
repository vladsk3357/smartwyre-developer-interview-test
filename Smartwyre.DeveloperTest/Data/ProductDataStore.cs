using System.Collections.Concurrent;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore : IProductDataStore
{
    private static readonly ConcurrentDictionary<string, Product> _products = new();

    public ProductDataStore()
    {
        _products.TryAdd("PROD-1", new Product
        {
            Identifier = "PROD-1",
            Price = 100m,
            Uom = "KG",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate
        });

        _products.TryAdd("PROD-2", new Product
        {
            Identifier = "PROD-2",
            Price = 200m,
            Uom = "KG",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        });
    }

    public Product GetProduct(string productIdentifier)
    {
        return _products.TryGetValue(productIdentifier, out var product) ? product : null;
    }
}
