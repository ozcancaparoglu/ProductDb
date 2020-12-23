using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.PageModels.Attributes;
using ProductDb.Admin.PageModels.Language;
using ProductDb.Mapping.BiggBrandDbModels;
using ProductDb.Mapping.BiggBrandDbModelFields;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using System.Linq;
using ProductDb.Services.ProductServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Admin.Helpers.LocalizationHelpers;

namespace ProductDb.Admin.Controllers
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    [Authorize]
    [Route("{lang}/attributes")]
    public class AttributesController : BaseController
    {
        private readonly IAttributesService attributesService;
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;

        public AttributesController(ILanguageService languageService,
            IAttributesService attributesService,
            ICategoryService categoryService,
            IProductService productService,
            IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.attributesService = attributesService;
            this.categoryService = categoryService;
            this.productService = productService;
        }

        [Route("insert")]
        [HttpPost]
        public JsonResult Insert(AttributesViewModel model)
        {
            if (model.Attributes == null || model.Attributes.Id == 0 || model.EntityId == 0)
                return Json(new { result = "Failed" });

            switch (model.AttributeType)
            {
                case "Category":
                    categoryService.AddNewCategoryAttributeMapping(model.Attributes.Id, model.EntityId);
                    return Json(new { result = "Redirect", url = $"/{CurrentLanguage}/category/edit/{model.EntityId}" }); //Url.Action("Edit", "Category", new { id = model.EntityId })

                case "Product":
                    productService.AddNewProductAttributeMapping(model.EntityId, model.Attributes.Id, null, false);
                    return Json(new { result = "Redirect", url = $"/{CurrentLanguage}/products/edit/{model.EntityId}" }); //Url.Action("Edit", "Product", new { id = model.EntityId })

                default:
                    break;
            }

            if (model.AttributeType == GetFieldValues.CategoryTable)
                return Json(new { result = "Failed", url = $"/{CurrentLanguage}/category/edit/{model.EntityId}" });
            else
                return Json(new { result = "Failed", url = $"/{CurrentLanguage}/products/edit/{model.EntityId}" });
        }

        [Route("list")]
        public IActionResult List()
        {
            var model = attributesService.AllAttributes();

            return View(model);
        }

        [Route("create")]
        public IActionResult Create()
        {
            var model = new AttributesModel();

            return View(model);
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(AttributesModel model)
        {
            attributesService.AddNewAttributes(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var model = attributesService.AttributesById(id);

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost]
        public IActionResult Edit(AttributesModel model)
        {

            attributesService.EditAttributes(model);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        [Route("setstate/{id}")]
        public IActionResult State(int id)
        {
            attributesService.SetState(id);

            return RedirectToAction("list", new { lang = CurrentLanguage });
        }

        #region Attribute Values

        [Route("values-list/{id}")]
        public IActionResult ValuesList(int id)
        {
            var model = attributesService.AllAttributesValue(id);

            ViewBag.AttributeId = id;
            ViewBag.AttributeName = attributesService.AttributesById(id).Name;

            return View(model);
        }

        [Route("values-create/{id}")]
        public IActionResult ValuesCreate(int id)
        {
            var model = new AttributesValueViewModel
            {
                AttributeName = attributesService.AttributesById(id).Name,
                AttributesValue = new AttributesValueModel { AttributesId = id, ProcessedBy = _userId },
            };

            ViewBag.TableName = GetFieldValues.AttributesValueTable;
            ViewBag.FieldNames = GetFieldValues.AttributesValueFields;

            return View(model);
        }

        [Route("values-create/{id}")]
        [HttpPost]
        public IActionResult ValuesCreate(AttributesValueViewModel model, LanguageViewModel languageViewModel)
        {
            /*
             * Sebebi şu olabilir model.Id
             * ve model.AttributeId olunca sadece yapabiliyor.
             * model.UnitId eklenince yapmadı.
             * Maplenmiş bir Dto objesi kullanırsan yapabiliyor
             * ViewModel'de yapmıyor olabilir sonra kontrol et.
             */

            //model.AttributesValue.Id = 0; //AttributeId'yi model'in id'sine yazıyor otomatik, aynı muhabbet language muhabbetinde de var. Core'un işleri kamiller id görünce her yere basıyor. 

            var entity = attributesService.AddNewAttributesValue(model.AttributesValue);

            languageService.AddNewLanguageValues(languageViewModel.LanguageValues, entity.Id);

            return RedirectToAction("ValuesList", new { lang = CurrentLanguage, id = model.AttributesValue.AttributesId });
        }

        [Route("values-edit/{id}")]
        public IActionResult ValuesEdit(int id)
        {
            var attributeId = attributesService.AttributeIdWithAttributesValueId(id);
            var attribute = attributesService.AttributesById(attributeId);

            var model = new AttributesValueViewModel
            {
                AttributesValue = attributesService.AttributesValueById(id),
                AttributeName = attribute.Name
            };

            ViewBag.TableName = GetFieldValues.AttributesValueTable;
            ViewBag.FieldNames = GetFieldValues.AttributesValueFields;

            return View(model);
        }

        [Route("values-edit/{id}")]
        [HttpPost]
        public IActionResult Edit(AttributesValueViewModel model, LanguageViewModel languageViewModel)
        {
            var entity = attributesService.EditAttributesValue(model.AttributesValue);

            languageService.EditLanguageValues(languageViewModel.LanguageValues, entity.Id, GetFieldValues.AttributesValueTable);

            return RedirectToAction("ValuesEdit", new { lang = CurrentLanguage, id = entity.Id });
        }

        [Route("set-state-values/{id}")]
        public IActionResult StateValues(int id)
        {
            var attributeId = attributesService.AttributeIdWithAttributesValueId(id);

            attributesService.SetStateAttributesValue(id);

            return RedirectToAction("ValuesList", new { lang = CurrentLanguage, id = attributeId });
        }

        #endregion


    }
}


