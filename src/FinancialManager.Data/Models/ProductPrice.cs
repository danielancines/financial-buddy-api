using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FinancialManager.Data.Models;

public class ProductPrice
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("store")]
    public string? Store { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; } = DateTime.Now;

    [BsonElement("price")]
    public double Price { get; set; }
}
