using Microsoft.AspNetCore.Mvc;
using ProductDb.Services.AuthenticationServices;

namespace ProductDb.Admin.Components
{
    public class ChangePasswordViewComponent : ViewComponent
    {
        private readonly IAuthenticationService authenticationService;

        public ChangePasswordViewComponent(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public IViewComponentResult Invoke(int userId)
        {
            var model = authenticationService.GetById(userId);

            return View("_ChangePasswordView", model);
        }
    }
}
