using System.Threading.Tasks;
using System.Web.Http;
using VE.Web.Models;
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
            new FfmpegService().Convert(
                "E:\\Code\\MMS\\video-editor\\VideoEditor\\VE.Web\\AppData\\Media\\sample2.mp4",
                new VideoConversionOptions
                {
                    Format = VideoFormat.Avi,
                    Height = 1280,
                    Width = 1920,
                    VerticalAspect = 2,
                    HorizontalAspect = 6,
                    Bitrate = 364
                });

            return await Task.FromResult("Test");
        }
    }
}
