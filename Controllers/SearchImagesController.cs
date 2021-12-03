using Flick.Controllers.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;

namespace Flick.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchImagesController : ControllerBase
    {
        private readonly ILogger<SearchImagesController> _logger;

        private readonly FileCache _cache;

        public SearchImagesController(ILogger<SearchImagesController> logger, FileCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IEnumerable<ViewImage>> searchImages()
        {
            if (HttpContext.Request.Query.ContainsKey("q"))
            {
                var iQuery = HttpContext.Request.Query["q"];

                var taskPixABay = Task.Run(async () => {
                    IEnumerable<ViewImage> flsImageFromPixABay = await loadFromPixABay(iQuery);
                    checkOrFillPreviewImagesCache(flsImageFromPixABay, "PixABay");
                    return flsImageFromPixABay;
                });

                var taskFlickr = Task.Run(async () => {
                    IEnumerable<ViewImage> flsImageFromFlickr = await loadFromFlickr(iQuery);
                    checkOrFillPreviewImagesCache(flsImageFromFlickr, "Flickr");
                    return flsImageFromFlickr;
                });

                Task.WhenAll(new [] { taskPixABay, taskFlickr }).Wait();
                
                var flsImage = taskPixABay.Result.Concat(taskFlickr.Result);

                return setPreviewURLToCach(flsImage);
            }
            else
            {
                Response.StatusCode = 403;

                return null;
            }
        }

        private IEnumerable<ViewImage> setPreviewURLToCach(IEnumerable<ViewImage> flsImage)
        {
            return flsImage.Select(fImage => fImage.setPreviewURLToCached("/previewImage?id=" + fImage.id));
        }

        private void checkOrFillPreviewImagesCache(IEnumerable<ViewImage> flsImage, string isWatermark)
        {
            var flsImagesNeedLoad = flsImage.Where(img => !_cache.Contains(img.id));

            var loadTasks = flsImagesNeedLoad.Select(async fItem =>
            {
                var fPreviewStream = await loadPreviewImage(fItem.previewURL);
                var fPreviewWithWatermark = drawWatermark(fPreviewStream, isWatermark);

                _cache.Set(new CacheItem(fItem.id, fPreviewWithWatermark), new CacheItemPolicy()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            });
            Task.WhenAll(loadTasks).Wait();
        }

        private async Task<Stream> loadPreviewImage(string iPreviewURL)
        {
            using (HttpClient client = new HttpClient())
            {
                var fPreviewResponse = await client.GetAsync(iPreviewURL);
                return await fPreviewResponse.Content.ReadAsStreamAsync();
            }
        }

        private async Task<IEnumerable<ViewImage>> loadFromPixABay(string iQuery)
        {
            var fsParams = "key=1923807-a9f36f13c40dea26ff0b06414&image_type=photo&pretty=true&q=" + HttpUtility.UrlEncode(iQuery);
            using (HttpClient client = new HttpClient())
            {
                var fjsResponse = await client.GetAsync("https://pixabay.com/api/?" + fsParams);
                var jsonString = await fjsResponse.Content.ReadAsStringAsync();
                var fjsRet = JsonConvert.DeserializeObject<PixABay>(jsonString);
                var fItems = fjsRet.Hits;

                IEnumerable<ViewImage> flsImage = fItems.Select(fImage => new ViewImage()
                {
                    title = fImage.User,
                    tags = fImage.Tags,
                    previewURL = fImage.PreviewURL,
                    pageURL = fImage.PageURL,
                });

                return flsImage;
            }
        }

        private async Task<IEnumerable<ViewImage>> loadFromFlickr(string iQuery)
        {
            var fsParams = "format=json&nojsoncallback=1&tags=" + HttpUtility.UrlEncode(iQuery);

            using (HttpClient client = new HttpClient())
            {
                var fjsResponse = await client.GetAsync("https://api.flickr.com/services/feeds/photos_public.gne?" + fsParams);
                var jsonString = await fjsResponse.Content.ReadAsStringAsync();
                var fjsRet = JsonConvert.DeserializeObject<Flickr>(jsonString);
                var fItems = fjsRet.Items;

                IEnumerable<ViewImage> flsImage = fItems.Select(fImage => new ViewImage()
                {
                    title = fImage.Title,
                    tags = fImage.Tags,
                    previewURL = fImage.Media["m"],
                    pageURL = fImage.Link,
                });

                return flsImage;
            }
        }

        private static byte[] drawWatermark(Stream iImage, string isWatermark)
        {
            Image fBMP = new Bitmap(iImage);

            Font font = new Font("Arial", 40, FontStyle.Italic, GraphicsUnit.Pixel);
            Point atpoint = new Point(fBMP.Width / 2, fBMP.Height / 2);
            Color fColor = Color.FromArgb(80, 200, 200, 200);
            SolidBrush brush = new SolidBrush(fColor);
            
            Graphics graphics = Graphics.FromImage(fBMP);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            graphics.DrawString(isWatermark, font, brush, atpoint, sf);
            graphics.Dispose();

            MemoryStream fMemoryStream = new MemoryStream();
            fBMP.Save(fMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] fretBuf = fMemoryStream.ToArray();
            return fretBuf;
        }
    }

}
