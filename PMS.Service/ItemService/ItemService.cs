using System.Collections.Generic;
using System.Linq;
using PMS.Data.Entities.Item;
using PMS.Data.Repository;

namespace PMS.Service.ItemService
{
    public class ItemService : IItemService
    {
        private IRepository<ItemVatRate> _itemVatRateRepository;
        public ItemService(IRepository<ItemVatRate> itemVatRateRepository)
        {
            _itemVatRateRepository = itemVatRateRepository;
        }
        public ItemVatRate ItemVateRateByRate(int Rate)
        {
            return _itemVatRateRepository.Find(a => a.VateRate == Rate);
        }

        public List<ItemVatRate> ItemVateRateByRate()
        {
            return _itemVatRateRepository.GetAll().ToList();
        }
    }
}
