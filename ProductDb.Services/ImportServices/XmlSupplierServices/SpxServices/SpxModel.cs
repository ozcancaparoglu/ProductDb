using ProductDb.Common.Entities;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProductDb.Services.ImportServices.XmlSupplierServices.SpxServices
{
    [XmlRoot("item")]
    public class SpxModel : IntegrationBase
    {

        [XmlElement(ElementName = "id", Namespace = "http://base.google.com/ns/1.0")]
        public override string Model { get; set; }

        [XmlElement(ElementName = "title")]
        public override string Name { get; set; }

        [XmlElement(ElementName = "description")]
        public override string FullDescription { get; set; }

        [XmlElement(ElementName = "product_category", Namespace = "http://base.google.com/ns/1.0")]
        public override string CategoryName { get; set; }

        [XmlElement(ElementName = "tax", Namespace = "http://base.google.com/ns/1.0")]
        public override string VatRate { get; set; }

        [XmlElement(ElementName = "availability", Namespace = "http://base.google.com/ns/1.0")]
        public string Availability { get; set; }

        [XmlElement(ElementName = "brand", Namespace = "http://base.google.com/ns/1.0")]
        public override string Brand { get; set; }

        [XmlElement(ElementName = "sale_price", Namespace = "http://base.google.com/ns/1.0")]
        public string BuyingPriceString { get; set; }

        [XmlElement(ElementName = "price", Namespace = "http://base.google.com/ns/1.0")]
        public string SellingPriceString { get; set; }

        [XmlElement(ElementName = "image_link", Namespace = "http://base.google.com/ns/1.0")]
        public string Image { get; set; }

        [XmlElement(ElementName = "color", Namespace = "http://base.google.com/ns/1.0")]
        public override string ShortDescription { get; set; }

        [XmlElement(ElementName = "size", Namespace = "http://base.google.com/ns/1.0")]
        public List<string> Size { get; set; }

        [XmlElement(ElementName = "stock", Namespace = "http://base.google.com/ns/1.0")]
        public List<string> Stok { get; set; }

        [XmlElement(ElementName = "gtin", Namespace = "http://base.google.com/ns/1.0")]
        public List<string> Gtin { get; set; }

    }
}
