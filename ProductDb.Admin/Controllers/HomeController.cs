using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Admin.Helpers.LocalizationHelpers;
using ProductDb.Admin.PageModels.Home;
using ProductDb.Services.AttributesServices;
using ProductDb.Services.AuthenticationServices;
using ProductDb.Services.CategoryServices;
using ProductDb.Services.LanguageServices;
using ProductDb.Services.LogoServices;
using ProductDb.Services.PermissionServices;
using ProductDb.Services.ProductServices;

namespace ProductDb.Admin.Controllers
{
    [Authorize]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    //[Route("{lang}")]
    public class HomeController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IAttributesService attributesService;
        private readonly IProductService productService;
        private readonly IAuthenticationService authenticationService;
        private readonly ILogoService logoService;

        public HomeController(ILanguageService languageService, ICategoryService categoryService, IAttributesService attributesService, IProductService productService, IAuthenticationService authenticationService,
            ILogoService logoService, IUserRolePermissionService userRolePermissionService) : base(languageService, userRolePermissionService)
        {
            this.categoryService = categoryService;
            this.attributesService = attributesService;
            this.productService = productService;
            this.authenticationService = authenticationService;
            this.logoService = logoService;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                CategoryCount = categoryService.CategoryCount(),
                AttributesCount = attributesService.AttributesCount(),
                ProductCount = productService.ProductCount(),
                UserCount = authenticationService.UserCount()

            };
            return View(model);
        }

        
    }
}