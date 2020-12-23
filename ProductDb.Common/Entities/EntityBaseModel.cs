using System;

namespace ProductDb.Common.Entities
{
    public abstract class EntityBaseModel
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? State { get; set; }
        public int ProcessedBy { get; set; }
    }
}
