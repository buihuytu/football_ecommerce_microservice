using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Entities
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)] // MongoDB ObjectId phải là chuỗi 24 ký tự hex (hexadecimal string), ví dụ: 64fdf21ef293ed4a75e132ab 
        public string Id { get; set; } 
    }
}
