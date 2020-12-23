using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductDb.Common.GlobalEntity
{
    [XmlRoot(ElementName = "Product")]
    public class EntegraXmlVariantModel
    {
        [XmlElement(ElementName = "SKU")]
        public string Sku { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "Barcode")]
        public string Barcode { get; set; }
        [XmlElement(ElementName = "CurrencyType")]
        public string CurrencyType { get; set; }
        [XmlElement(ElementName = "Quantity")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "Brand")]
        public string Brand { get; set; }
        public List<string> ProductPictures { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "ShortDescription")]
        public string ShortDescription { get; set; }
        [XmlElement(ElementName = "Price")]
        public decimal Price { get; set; }
        [XmlElement(ElementName = "Desi")]
        public decimal Desi { get; set; }
        [XmlElement(ElementName = "Variants")]
        public Variants Variants { get; set; }

    }

    [XmlRoot(ElementName = "Attribute")]
    public class Spec
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "variant")]
    public class Variant
    {
        [XmlElement(ElementName = "Attribute")]
        public List<Spec> Attribute { get; set; }
        [XmlElement(ElementName = "VariantSku")]
        public string Sku { get; set; }
        [XmlElement(ElementName = "VariantBarcode")]
        public string Barcode { get; set; }
        [XmlElement(ElementName = "VariantQuantity")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "VariantPrice")]
        public decimal Price { get; set; }

    }

    [XmlRoot(ElementName = "variants")]
    public class Variants
    {
        [XmlElement(ElementName = "variant")]
        public List<Variant> Variant { get; set; }
    }

    [Serializable]
    [XmlRoot("XML")]
    public class EntegraXMLVariantSerializer
    {
        [XmlArray("Products"), XmlArrayItem(typeof(EntegraXmlVariantModel), ElementName = "Product")]
        public List<EntegraXmlVariantModel> XmlProducts { get; set; }
    }

}
