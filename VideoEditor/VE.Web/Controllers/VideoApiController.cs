using System.Threading.Tasks;
using System.Web.Http;
using VE.Web.Services;

namespace VE.Web.Controllers
{
    [RoutePrefix("api/video")]
    public class VideoApiController : ApiController
    {
        [Route("test")]
        [HttpGet]
        public async Task<string> Test()
        {
            new FfmpegService().GetFrame("E:\\Code\\MMS\\video-editor\\VideoEditor\\VE.Web\\AppData\\sample.mp4", 1);
            new FfmpegService().Join("E:\\Code\\MMS\\video-editor\\VideoEditor\\VE.Web\\AppData\\sample.mp4",
                "E:\\Code\\MMS\\V80310-215224.mp4");

            return await Task.FromResult("Test");
        }
    }
}
