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
using VE.Web.Contracts;
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
            FileInfo fileInfo = GetFileInfo(fileId);
            if (fileInfo == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, $"File '{fileId}' not found.");
            }

            FileStream file = File.OpenRead(fileInfo.FullName);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
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

        [Route("video/all")]
        [HttpGet]
        public IEnumerable<VideosListItem> GetVideoFiles()
        {
            return new DirectoryInfo(GetRootPath())
                .GetFiles()
                .Where(f => Constants.VideoFormatsExtensions.Contains(f.Extension))
                .Select(f => new VideosListItem(f.Name, FilesUtils.BytesToMB(f.Length)));
        }

        [Route("delete/{fileId}")]
        [HttpDelete]
        public HttpResponseMessage Delete(string fileId)
        {
            FileInfo fileInfo = GetFileInfo(fileId);
            if (fileInfo == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, $"File '{fileId}' not found.");
            }

            File.Delete(fileInfo.FullName);

            return this.Request.CreateResponse(HttpStatusCode.OK);
        }

        private static string GetRootPath()
        {
            string root = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) ?? string.Empty,
                FilesUtils.MediaDataFolder);

            return new Uri(root).LocalPath;
        }

        private static FileInfo GetFileInfo(string fileId)
        {
            FileInfo fileInfo = new DirectoryInfo(GetRootPath())
                .GetFiles()
                .FirstOrDefault(f => f.Name.Equals(fileId + f.Extension, StringComparison.InvariantCultureIgnoreCase));

            return fileInfo;
        }
    }
}
