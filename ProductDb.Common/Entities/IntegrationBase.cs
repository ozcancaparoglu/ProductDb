using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProductDb.Common.Entities
{
    [Serializable]
    public abstract class IntegrationBase
    {
        #region Product Fields

        [XmlIgnore]
        [JsonIgnore]
        public virtual int SupplierId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int RootCategoryId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string Sku { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string Gtip { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string Name { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string Model { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string ShortDescription { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string FullDescription { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string CategoryName { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int? ParentProductDockId { get; set; }

        #endregion

        #region Prices, VatRate, Desi & Stock

        [XmlIgnore]
        [JsonIgnore]
        public virtual decimal? Cost { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual decimal? PsfPrice { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual decimal? Desi { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string VatRate { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string Currency { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string Brand { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int Stock { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int? Weight { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int? Length { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int? Width { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual int? Height { get; set; }

        #endregion

        #region Images & Attributes & Variants

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<string> DownloadImageUrls { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<AttributesAndValues> AttributesAndValues { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Variants> Variants { get; set; }

        #endregion

        #region Seo

        [XmlIgnore]
        [JsonIgnore]
        public virtual string MetaKeywords { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string MetaTitle { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string MetaDescription { get; set; }

        #endregion

    }

    [Serializable]
    public class Variants
    {
        [XmlIgnore]
        [JsonIgnore]
        public string Sku { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string Barcode { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public int? Stock { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public decimal? Cost { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public decimal? PsfPrice { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual string ShortDescription { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<AttributesAndValues> AttributesAndValues { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<string> DownloadImageUrls { get; set; }
    }


    [Serializable]
    public class AttributesAndValues
    {
        [XmlIgnore]
        [JsonIgnore]
        public string AttributeName { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string AttributeValue { get; set; }

    }
}
