using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Common.Dto;
using PMS.LogoService.LogoModels;
using PMS.LogoService.LogoService;
using PMS.Mapping;

namespace PMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ILogoService logoService;

        public ItemController(ILogoService logoService)
        {
            this.logoService = logoService;
        }

        [HttpPost]
        [Route("InsertItem/{companyId}")]
        public async Task<IActionResult> InsertItem(ItemCard itemCard, int companyId)
        {
            try
            {
                // Model Prepare
                var itemCardModel = new LogoItemModel();
                itemCardModel.Item = new Item();

                itemCardModel.Item.CODE = itemCard.Sku;
                itemCardModel.Item.UNITSET_CODE = "ADET";
                itemCardModel.Item.MARKCODE = itemCard.Brand;
                itemCardModel.Item.NAME = itemCard.Name;
                itemCardModel.Item.VAT = itemCard.VatRate;
                // Unit Prepare
                itemCardModel.Item.UNITS = new ItemUnitsModel();

                ItemUnitModel unit = new ItemUnitModel();
                unit.BARCODE = itemCard.Barcode;
                unit.UNIT_CODE = "ADET";

                itemCardModel.Item.UNITS.ItemUnitModel = new List<ItemUnitModel>();
                itemCardModel.Item.UNITS.ItemUnitModel.Add(unit);

                // Mark Prepare
                var itemMark = new LogoMarksModel();
                itemMark.MarkModel = new MarkModel();
                itemMark.MarkModel.CODE = itemCard.Brand;
                itemMark.MarkModel.DESCR = itemCard.Brand;
                itemMark.MarkModel.SPECODE = itemCard.Brand;
                // Insert Mark And Item
                var markResult = await logoService.InsertItemMark(itemMark, companyId);
                var result = await logoService.InsertItem(itemCardModel, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}