using Microsoft.AspNetCore.Mvc;
using ProductDb.Common.Cache;
using ProductDb.Services.PermissionServices;
using System;

namespace ProductDb.Admin.Areas.PMS.Controllers
{
    [Area("PMS")]
    [Route("PMS/cache")]
    public class CacheController : BaseController
    {
        private readonly ICacheManager cacheManager;

        public CacheController(ICacheManager cacheManager, IUserRolePermissionService userRolePermissionService)
            : base(userRolePermissionService)
        {
            this.cacheManager = cacheManager;
        }

        [HttpPost]
        [Route("ClearCache")]
        public IActionResult ClearCache(string key)
        {
            try
            {
                cacheManager.Remove(key);
                return Json(new { Status = true, Message = "Cache Cleared" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }
    }
}