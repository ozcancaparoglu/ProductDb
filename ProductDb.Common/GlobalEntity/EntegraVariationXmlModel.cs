using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductDb.Common.GlobalEntity
{
    public class EntegraVariationXmlModel
    {

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Supplier { get; set; }
        public string FullDescription { get; set; }
      
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public string ProductCategory { get; set; }
        public string MainProductCategory { get; set; }
        public decimal Desi { get; set; }
        public List<string> ProductPictures { get; set; }
        public List<VariationQuantitty> VariationQuantitties { get; set; }
    }

    public class VariationQuantitty
    {
        public string BarCode { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public double StockQuantity { get; set; }
    }

    [Serializable]
    [XmlRoot("XML")]
    public class EntegraVariationXMLSerializer
    {
        [XmlArray("Products"), XmlArrayItem(typeof(EntegraXmlModel), ElementName = "Product")]
        public List<EntegraVariationXmlModel> XmlProducts { get; set; }
    }
}
