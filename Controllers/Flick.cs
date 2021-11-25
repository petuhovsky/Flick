using FD.Control;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flick.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Flick : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Flick> _logger;

        public Flick(ILogger<Flick> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IEnumerable<ViewImage> Get(string iTags)
        {
            var fBuf = new byte[1024];
            var fTask = HttpContext.Request.Body.ReadAsync(fBuf,0,1024);
            var fsBuf = Encoding.UTF8.GetString(fBuf,0, fTask.Result);
            var fjsnRes = JsonConvert.DeserializeObject<JObject>(fsBuf);
            var fvTags = fjsnRes.First<JToken>();
            var fvTags1 = fvTags.First<JToken>();
            var fsTags = fvTags1.Value<string>();
            //selec image from cash
            List<ViewImage> flsImage = new List<ViewImage>();

            var fsParams = "tags=" + fsTags + "&format=json&nojsoncallback=1";
            var fjsRet = new HFDHttpsRequest()
                    .set("https://api.flickr.com/services/feeds/photos_public.gne?"
                    + fsParams
                    )
                    .getResponseAsJson();
            var fValues = fjsRet.ContainsKey("items");
            var fItems = fjsRet["items"];
            foreach (var fTocken in fItems)
            {
                var flink = fTocken["link"];
                var fTitle = fTocken["title"];
                var fTags = fTocken["tags"];
                var fpreviewURL = fTocken["previewURL"];
                //return { id: item.id, title: item.title, tags: item.tags, previewURL: item.previewURL, pageURL: item.link};

                flsImage.Add(new ViewImage()
                {
                    previewURL = fpreviewURL.Value<string>(),
                    pageURL = flink.Value<string>(),
                    title = fTitle.Value<string>(),
                    tags = fTags.Value<string>()
                });

            }

            return flsImage;


        }
    }
}
