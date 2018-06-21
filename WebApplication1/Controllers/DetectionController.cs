using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DetectionController : Controller
    {
        private const string FaceApiKey = "9428881160954b0ba5be3986c5e0931f";
        private const string FaceApiUriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

        // GET: Detection
        public ActionResult Index()
        {
            string s;

            FacesModel faces = new FacesModel
            {
                ImagePath = GetFullUrl("Content/Images/DefaultFaceImage.jpg")
            };

            System.Diagnostics.Debug.WriteLine(faces.ImagePath);
            return View(faces);
        }

        // GET: Detection/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Detection/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Detection/Create
        [HttpPost]
        public ActionResult Create(FacesModel faces)
        {
            try
            {
                // TODO: Add insert logic here
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string mappedFileName;

                    if (file.ContentLength > 0)
                    {
                        Bitmap bmp;
                        Task<string> result;
                        string fileName;

                        fileName = Path.GetFileName(file.FileName);
                        faces.ImagePath = GetFullUrl(string.Format("Content/Images/{0}", fileName));
                        mappedFileName = Path.Combine(Server.MapPath("~/Content/Images"), fileName);

                        // Resize image to our size (300x300)
                        bmp = new Bitmap(file.InputStream);
                        bmp = ResizeImage(bmp, 300, 300);
                        bmp.Save(mappedFileName);
                        //file.SaveAs(mappedFileName);

                        // Call Face Api and wait for result
                        result = MakeAnalysisRequestAsync(mappedFileName);
                        result.Wait();
                        faces.Load(result.Result);
                    }
                }
                return View("Index", faces);
            }
            catch
            {
                return View();
            }
        }

        // GET: Detection/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Detection/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Detection/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Detection/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private Task<string> MakeAnalysisRequestAsync(string imageFilePath)
        {
            HttpClient client;
            HttpResponseMessage response;
            string requestParameters;
            string uri;
            Task<string> result;
            byte[] byteData;


            result = Task.Run(async () =>
            {
                string contentstring;

                // Create HTTP client and assemble face API URL
                contentstring = string.Empty;
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", FaceApiKey);
                requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                    "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                    "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

                uri = string.Format("{0}?{1}", FaceApiUriBase, requestParameters);

                // Decompose image file to a byte-array and place in a media type header
                byteData = GetImageAsByteArray(imageFilePath);
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Execute the REST API call.
                    response = await client.PostAsync(uri, content);

                    // Get the JSON response.
                    contentstring = await response.Content.ReadAsStringAsync();
                    return contentstring;
                }
            });
            return result;
        }

        private byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private string GetFullUrl(string relativePath)
        {
            string fullPath;
            string baseUrl;

            baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            fullPath = string.Format("{0}{1}", baseUrl, relativePath);
            return fullPath;
        }
    }
}
