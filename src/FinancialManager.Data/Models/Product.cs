using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinancialManager.Data.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("barcode")]
    public string? Barcode { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("store")]
    public string? Store { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

}