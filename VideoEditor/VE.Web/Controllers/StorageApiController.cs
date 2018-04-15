using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using VE.Web.Models;
using VE.Web.Services;

namespace VE.Web.Controllers
{
    [RoutePrefix("api/storage")]
    public class StorageApiController : ApiController
    {
        [Route("upload")]
        [HttpPost]
        public async Task Upload()
        {
            var provider = new MultipartFormDataStreamProvider(GetRootPath());

            await this.Request.Content.ReadAsMultipartAsync(provider);

            foreach (MultipartFileData file in provider.FileData)
            {
                string fileName = file.Headers.ContentDisposition.FileName.Trim('"');

                string filePath = Path.Combine(
                    Path.GetDirectoryName(file.LocalFileName) ?? string.Empty,
                    $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}");

                File.Move(file.LocalFileName, filePath);
            }
        }


        [Route("download/{fileId}")]
        [HttpGet]
        public HttpResponseMessage Download(string fileId)
        {
            FileInfo fileInfo = new DirectoryInfo(GetRootPath())
                .GetFiles()
                .FirstOrDefault(f => f.Name.Equals(fileId + f.Extension, StringComparison.InvariantCultureIgnoreCase));

            HttpResponseMessage response = this.Request.CreateResponse();

            if (fileInfo == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase = $"File '{fileId}' not found.";

                return response;
            }

            FileStream file = File.OpenRead(fileInfo.FullName);

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StreamContent(file);
            response.Content.Headers.ContentLength = file.Length;

            string mimeType = MimeMapping.GetMimeMapping(fileInfo.FullName);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file.Name
            };

            return response;
        }

        [Route("getVideosList")]
        [HttpPost]
        public IEnumerable<VideosListItem> GetVideosList()
        {
            FileInfo[] files = new DirectoryInfo(GetRootPath())
                .GetFiles();

            return files.Select(file => new VideosListItem(file.Name, FilesUtils.BytesToMB(file.Length)));
        }

        private static string GetRootPath()
        {
            string root = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) ?? string.Empty,
                FilesUtils.MediaDataFolder);

            return new Uri(root).LocalPath;
        }
    }
}
