using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ChannelAdvisorUpdateVM
    {
        public int productId { get; set; }
        public ChannelAdvisorUpdateModel productUpdateModel { get; set; }
        public List<Image> Images { get; set; }
        public List<AttributeUpdate> Value { get; set; }
    }

    public class AttributeUpdate
    {
        public int ProfileID { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
