using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductDb.Common.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }

        [Column(Order = 200)]
        public DateTime CreatedDate { get; set; }
        [Column(Order = 201)]
        public DateTime? UpdatedDate { get; set; }
        [Column(Order = 202)]
        public int? State { get; set; }
        [Column(Order = 203)]
        public int ProcessedBy { get; set; }

    }
}
