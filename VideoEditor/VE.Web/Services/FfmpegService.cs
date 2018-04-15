using System;
using System.Diagnostics;
using System.IO;
using VE.Web.Contracts;

namespace VE.Web.Services
{
    public class FfmpegService : IFfmpegService
    {
        private const string AppDataFolder = "AppData";
        private const string FfmpegPath = "Libs\\ffmpeg.exe";

        public string GetFrame(string inputVideo, int timeInSeconds)
        {
            var outputFrame = GetTempFile("{0}_{1}_sec.jpeg", inputVideo, timeInSeconds);
            var parameters = $"-ss {timeInSeconds} -i {inputVideo} -frames:v 1 -y {outputFrame}";

            var process = ConfigureFfmpegProcess(parameters);

            process.Start();
            process.WaitForExit();

            return outputFrame;
        }

        public string Join(string firstVideo, string secondVideo)
        {
            var outputVideo = GetTempFile("{0}_{1}.mp4", firstVideo, secondVideo);
            return outputVideo;
        }

        public string Convert(string inputVideo, string format)
        {
            throw new NotImplementedException();
        }

        private static string GetTempFile(string fileNameTemplate, params object[] inputs)
        {
            return Path.Combine(AppDataFolder, string.Format(fileNameTemplate, inputs));
        }

        private static Process ConfigureFfmpegProcess(string parameters)
        {
            return new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = FfmpegPath,
                    Arguments = parameters
                }
            };
        }
    }
}
