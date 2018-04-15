using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using VE.Web;

[assembly: OwinStartup(typeof(Startup))]
namespace VE.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var httpConfiguration = new HttpConfiguration();

            // Routes
            httpConfiguration.MapHttpAttributeRoutes();

            // Formatters
            httpConfiguration.Formatters.AddRange(new MediaTypeFormatter[]
            {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            });

            // Web API
            appBuilder.UseWebApi(httpConfiguration);
        }
    }
}