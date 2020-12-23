using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDb.Common.GlobalEntity;
using ProductDb.Common.Helpers.JwtHelper;

namespace ProductDb.Api.Export.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetToken([FromBody]LoginModel loginModel)
        {
            if (loginModel.Username != "burak" && loginModel.Password != "123456")
                return Unauthorized();

            var token = new JwtTokenBuilder()
                              .AddSecurityKey(JwtSecurityKey.Create("hurriyet-RC5qGgO1z0-7orEMKl2D6"))
                              .AddSubject("ProductDb")
                              .AddIssuer("aristo-it")
                              .AddAudience("aristo-it")
                              .AddExpiry(120000)
                              .Build();
            return Ok(token.Value);

        }

        [Authorize]
        [Route("check-token-validation")]
        [HttpGet]
        public IActionResult CheckTokenValidation()
        {
            return Ok(1);
        }
    }



}