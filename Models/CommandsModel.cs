using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TrabalhoInterdisciplinar.Models
{
    public class CommandsModel
    {
        public ObjectId _id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime recvTime { get; set; }
        public string attrName { get; set; }
        public string attrType { get; set; }
        public string attrValue { get; set; }
    }
}
