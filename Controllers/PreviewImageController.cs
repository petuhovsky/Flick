using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Caching;
using System.Threading;

namespace Flick.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreviewImageController : ControllerBase
    {
        private readonly ILogger<PreviewImageController> _logger;

        private readonly FileCache _cache;

        public PreviewImageController(ILogger<PreviewImageController> logger, FileCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public ActionResult Get()
        {
            if (HttpContext.Request.Query.ContainsKey("id"))
            {
                string iImageId = HttpContext.Request.Query["id"];

                if (_cache.Contains(iImageId))
                {
                    string fileName = (string)_cache.Get(iImageId);
                    if (fileName != null)
                    {
                        FileStream fileStream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        return File(fileStream, "image/webp");
                    }
                }
            }

            Response.StatusCode = 400;

            return null;
        }
    }
}
