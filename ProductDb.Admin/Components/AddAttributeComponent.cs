using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Attributes;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Services.AttributesServices;
using System.Collections.Generic;

namespace ProductDb.Admin.Components
{
    public class AddAttributeViewComponent : ViewComponent
    {
        private readonly IAttributesService attributeService;

        public AddAttributeViewComponent(IAttributesService attributeService)
        {
            this.attributeService = attributeService;
        }

        public IViewComponentResult Invoke(List<AttributesModel> parentAttributesModels, List<AttributesModel> attributesModels, int entityId, string type)
        {
            var model = new AttributesViewModel
            {
                AttributeType = type,
                AddAttributes = attributeService.AllAttributesExcept(parentAttributesModels, attributesModels),
                Attributes = new AttributesModel(),
                EntityId = entityId
            };

            return View("_AddAttributeView", model);
        }
    }
}
