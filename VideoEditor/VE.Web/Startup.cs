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
                new JsonMediaTypeFormatter()
            });

            // Web API
            appBuilder.UseWebApi(httpConfiguration);

            // Static Files Server
            var physicalFileSystem = new PhysicalFileSystem(@"../../wwwroot");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem,
                StaticFileOptions =
                {
                    FileSystem = physicalFileSystem,
                    ServeUnknownFileTypes = true
                },
                DefaultFilesOptions =
                {
                    DefaultFileNames = new[]
                    {
                        "index.html"
                    }
                }
            };

            appBuilder.UseFileServer(options);
        }
    }
}