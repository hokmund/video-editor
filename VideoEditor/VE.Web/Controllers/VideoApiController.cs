using System.Threading.Tasks;

namespace VE.Web.Controllers
{
    [RoutePrefix("api/video")]
    public class VideoApiController : ApiController
    {
        [Route("test")]
        [HttpGet]
        public async Task<string> Test()
        {
            return await Task.FromResult("Test");
        }
    }
}
