using System.Linq;
using System.Web.Http;
using VE.Web.Contracts;
using VE.Web.Models;
using VE.Web.Services;

namespace VE.Web.Controllers
{
    [RoutePrefix("api/video")]
    public class VideoApiController : ApiController
    {
        private readonly IFfmpegService service = new FfmpegService();

        [Route("getFrame")]
        [HttpGet]
        public string GetFrame(string name, int time)
        {
            var inputVideo = $"{FilesUtils.InputsDataFolder}\\{name}";
            return service.GetFrame(inputVideo, time).Replace('\\', '/');
        }

        [Route("join")]
        [HttpPost]
        public string Join([FromBody] JoinRequest model)
        {
            if (model.Files.Length == 0)
            {
                return null;
            }

            var joinedVideo = service.Join(
                model.Files.Select(
                    file => $"{FilesUtils.InputsDataFolder}\\{file}"
                )
                .ToArray()
            );

            return joinedVideo;
        }

        [Route("convert")]
        [HttpPost]
        public string Convert([FromBody] ConvertRequest model)
        {
            return service.Convert($"{FilesUtils.OutputsDataFolder}\\{model.File}", VideoConversionOptions.FromConvertRequest(model));
        }
    }
}
