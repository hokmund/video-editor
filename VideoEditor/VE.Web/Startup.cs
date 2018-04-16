using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
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
				new JsonMediaTypeFormatter()
			});

			// Web API
			appBuilder.UseWebApi(httpConfiguration);

			// Static Files Server
			var physicalFileSystem = new PhysicalFileSystem(@"./wwwroot");

#if DEBUG
			physicalFileSystem = new PhysicalFileSystem(@"../../wwwroot");
#endif

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
			appBuilder.UseStaticFiles(new StaticFileOptions()
			{
				RequestPath = new PathString("/AppData"),
				FileSystem = new PhysicalFileSystem(@"AppData")
			});
		}
	}
}