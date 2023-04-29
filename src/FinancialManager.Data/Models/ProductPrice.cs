using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FinancialManager.Data.Models;

public class ProductPrice
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("store")]
    public string? Store { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; } = DateTime.Now;

    [BsonElement("price")]
    public double Price { get; set; }
}
