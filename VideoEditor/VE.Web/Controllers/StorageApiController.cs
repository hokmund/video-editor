using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace VE.Web.Controllers
{
    [RoutePrefix("api/storage")]
    public class StorageApiController : ApiController
    {
        [Route("upload")]
        [HttpPost]
        public async Task Upload()
        {
            string root = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) ?? string.Empty,
                "AppData");

            var provider = new MultipartFormDataStreamProvider(new Uri(root).LocalPath);

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
    }
}
