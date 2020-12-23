using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductDb.Mapping.BiggBrandDbModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductDb.Admin.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly Services.AuthenticationServices.IAuthenticationService authenticationService;
        private readonly IConfiguration configuration;

        public AuthController(Services.AuthenticationServices.IAuthenticationService authenticationService, IConfiguration configuration)
        {
            this.authenticationService = authenticationService;
            this.configuration = configuration;
        }

        [Route("login")]
        public IActionResult Login(string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.Message = "";
            return View();
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(string useremail, string password,string ReturnUrl)
        {
            //authenticationService.CreateUser("ocaparoglu", "test@test.com", "1q2w3e4r", 5);
            var authUser = authenticationService.Authenticate(useremail, password);
            if (authUser != null)
            {
                if (!string.IsNullOrWhiteSpace(authUser.Username))
                {
                    await LoginAsync(authUser);
                    // Return URL
                    if (!string.IsNullOrWhiteSpace(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return Redirect("~/Home/Index");
                }
            }
            ViewBag.Message = "Kullanıcı Bilgilerini Kontrol Ediniz.";
            return View();
        }

        private async Task LoginAsync(UserModel authUser)
        {
            var properties = new AuthenticationProperties
            {
                //AllowRefresh = false,
                //IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authUser.Username),
                new Claim(ClaimTypes.Role, authUser.UserRole.Name),
                new Claim(ClaimTypes.Sid, authUser.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, properties);
        }

        [Route("logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!configuration.GetValue<bool>("Account:ShowLogoutPrompt"))
            {
                return await Logout();
            }

            return View();
        }

        [HttpPost]
        [Route("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Login", "Auth");
        }

        [Route("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}