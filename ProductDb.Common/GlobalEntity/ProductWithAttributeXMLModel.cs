using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductDb.Common.GlobalEntity
{
    public class ProductWithAttributeXMLModel
    {

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Brand { get; set; }
        public string Supplier { get; set; }
        public string FullDescription { get; set; }
        public string BarCode { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public double StockQuantity { get; set; }
        public string ProductCategory { get; set; }
        public string MainProductCategory { get; set; }
        public decimal Desi { get; set; }
        public List<string> ProductPictures { get; set; }
        public List<Attribute> Attributes { get; set; }
    }

    [Serializable]
    [XmlRoot("XML")]
    public class ProductWithAttributeXMLModelSerializer
    {
        [XmlArray("Products"), XmlArrayItem(typeof(ProductWithAttributeXMLModel), ElementName = "Product")]
        public List<ProductWithAttributeXMLModel> XmlProducts { get; set; }
    }

}
