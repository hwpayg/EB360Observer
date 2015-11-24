using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EB360Observer.Entity
{
    [Serializable]
    public class EBItem : TimeBaseEntity<long>
    {
        public string Skuid { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PriceDesc { get; set; }
        public string Gift { get; set; }
        public string PromotionDesc { get; set; }
        public string ProductUrl { get; set; }
        public string ElectricBusiness { get; set; }
    }

    public class TimeBaseEntity<T> : BaseEntity<T>
    {
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? CreateTime { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdateTime { get; set; }
    }
    [Serializable]
    public class BaseEntity<TType>
    {
        public TType Id { get; set; }
    }

}
