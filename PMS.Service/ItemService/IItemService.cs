using PMS.Data.Entities;
using PMS.Data.Entities.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Service.ItemService
{
    public interface IItemService
    {
        List<ItemVatRate> ItemVateRateByRate();
        ItemVatRate ItemVateRateByRate(int Rate);
        
    }
}
