using ProductDb.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Mapping.BiggBrandDbModels
{
    public class VatRateModel: EntityBaseModel
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
