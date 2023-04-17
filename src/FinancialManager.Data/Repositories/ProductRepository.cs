using FinancialManager.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace FinancialManager.Data.Repositories;

public class ProductRepository
{
    private readonly IMongoCollection<Product> _productsCollection;
    //private FinancialManagerDbContext _context;
    public ProductRepository(FinancialBuddyDatabaseSettings financialBuddyDatabaseSettings)
    {
        var mongoClient = new MongoClient(financialBuddyDatabaseSettings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(financialBuddyDatabaseSettings.DatabaseName);
        _productsCollection = mongoDatabase.GetCollection<Product>(financialBuddyDatabaseSettings.ProductsCollectionName);
    }
    public IEnumerable<Product> Get()
    {
        return from product in this._productsCollection.AsQueryable()
               select product;
    }

    public IEnumerable<ProductPrice> GetPrices(Guid id)
    {
        var selectedProduct = this._productsCollection.Find(p => p.Id.Equals(id)).FirstOrDefault();
        if (selectedProduct == null)
            return Enumerable.Empty<ProductPrice>();

        return selectedProduct.Prices;
    }

    public bool Add(Product product)
    {
        this._productsCollection.InsertOne(product);
        return true;
    }

    public bool AddPrice(Guid id, ProductPrice productPrice)
    {
        var product = this._productsCollection.Find(p => p.Id == id).FirstOrDefault();
        if (product == null)
            return false;

        productPrice.Id = ObjectId.GenerateNewId().ToString();

        var filter = Builders<Product>.Filter.Eq("Id", id);
        var update = Builders<Product>.Update.AddToSet("prices", productPrice);

        var updateResult = this._productsCollection.UpdateOne(filter, update);

        if (updateResult != null)
            return updateResult.ModifiedCount > 0 ? true : false;

        return false;
    }

    public bool Delete(Guid id)
    {
        var product = this._productsCollection.Find(p => p.Id == id).FirstOrDefault();
        if (product == null)
            return false;

        var result = this._productsCollection.DeleteOne(p => p.Id == id);
        return result.DeletedCount > 0;
    }
}