using FD.Control;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
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
        public IEnumerable<ViewImage> searchImages()
        {
            if (HttpContext.Request.Query.ContainsKey("q"))
            {
                var iQuery = HttpContext.Request.Query["q"];

                var flsImageFromPixABay = loadFromPixABay(iQuery);
                checkOrFillPreviewImagesCache(flsImageFromPixABay, "PixABay");

                var flsImageFromFlickr = loadFromFlickr(iQuery);
                checkOrFillPreviewImagesCache(flsImageFromFlickr, "Flickr");

                var flsImage = flsImageFromPixABay.Concat(flsImageFromFlickr);

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
            foreach (var fItem in flsImage)
            {
                if (!_cache.Contains(fItem.id))
                {
                    var fPreview = loadPreviewImage(fItem.previewURL);
                    var fPreviewWithWatermark = drawWatermark(fPreview, isWatermark);

                    _cache.Set(new CacheItem(fItem.id, fPreviewWithWatermark), new CacheItemPolicy() { 
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    });
                }
            }
        }

        private byte[] loadPreviewImage(string iPreviewURL)
        {
            var fPreview = new HFDHttpsRequest()
                        .set(iPreviewURL)
                        .getResponseAsBuf();

            return fPreview;
        }

        private IEnumerable<ViewImage> loadFromPixABay(string iQuery)
        {
            var fsParams = "key=1923807-a9f36f13c40dea26ff0b06414&image_type=photo&pretty=true&q=" + HttpUtility.UrlEncode(iQuery);

            var fjsRet = new HFDHttpsRequest()
                    .set("https://pixabay.com/api/?" + fsParams)
                    .getResponseAsJson();

            var fItems = fjsRet["hits"];

            IEnumerable<ViewImage> flsImage = fItems.Select(fTocken => new ViewImage()
            {
                title = fTocken["user"].Value<string>(),
                tags = fTocken["tags"].Value<string>(),
                previewURL = fTocken["previewURL"].Value<string>(),
                pageURL = fTocken["pageURL"].Value<string>(),
            });

            return flsImage;
        }

        private IEnumerable<ViewImage> loadFromFlickr(string iQuery)
        {
            var fsParams = "format=json&nojsoncallback=1&tags=" + HttpUtility.UrlEncode(iQuery);

            var fjsRet = new HFDHttpsRequest()
                    .set("https://api.flickr.com/services/feeds/photos_public.gne?" + fsParams)
                    .getResponseAsJson();

            var fItems = fjsRet["items"];

            IEnumerable<ViewImage> flsImage = fItems.Select(fTocken => new ViewImage()
            {
                title = fTocken["title"].Value<string>(),
                tags = fTocken["tags"].Value<string>(),
                previewURL = fTocken["media"]["m"].Value<string>(),
                pageURL = fTocken["link"].Value<string>(),
            });
            
            return flsImage;
        }

        private static byte[] drawWatermark(byte[] iImage, string isWatermark)
        {
            Image fBMP;
            using (var ms = new MemoryStream(iImage))
            {
                fBMP = new Bitmap(ms);
            }

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
