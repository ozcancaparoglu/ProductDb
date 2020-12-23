using System;
using System.Collections.Generic;
using System.Text;

namespace ProductDb.Common.GlobalEntity
{
    public class ChannelAdvisorPriceUpdateModel
    {
        public int ProductId { get; set; }
        public ChannelAdvisorPrice UpdatePrice { get; set; }
    }
    public class ChannelAdvisorPrice
    {
        public decimal BuyItNowPrice { get; set; }
    }
}
