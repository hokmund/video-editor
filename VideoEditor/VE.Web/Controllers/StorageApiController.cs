using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
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
            string root = HostingEnvironment.MapPath("~/AppData");
            var provider = new MultipartFormDataStreamProvider(root);

            await this.Request.Content.ReadAsMultipartAsync(provider);

            foreach (MultipartFileData file in provider.FileData)
            {
                Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                Trace.WriteLine("Server file path: " + file.LocalFileName);
            }
        }
    }
}
