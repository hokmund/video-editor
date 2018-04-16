using System.Linq;
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

        [Route("getFrame")]
        [HttpGet]
        public string GetFrame(string name, int time)
        {
            var inputVideo = $"{FilesUtils.InputsDataFolder}\\{name}";
            return new FfmpegService().GetFrame(inputVideo, time).Replace('\\', '/');
        }

        [Route("join")]
        [HttpPost]
        public string Join([FromBody]JoinRequest model)
        {
            if (model.Files.Length == 0)
            {
                return null;
            }

            var service = new FfmpegService();

            var joinedVideo = service.Join(
                model.Files.Select(
                    file => $"{FilesUtils.InputsDataFolder}\\{file}"
                )
                .ToArray()
            );

            var resultVideo = service.Convert(joinedVideo, VideoConversionOptions.FromJoinRequest(model));

            return resultVideo;
        }
    }
}
