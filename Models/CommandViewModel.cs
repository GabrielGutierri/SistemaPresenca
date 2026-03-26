using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TrabalhoInterdisciplinar.Models
{
    public class CommandViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        public DateTime recvTime { get; set; }
        public string attrName { get; set; }
        public string attrType { get; set; }
        public string attrValue { get; set; }
    }
}
