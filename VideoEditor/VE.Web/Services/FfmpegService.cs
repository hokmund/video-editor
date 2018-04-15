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
            var outputFrame = $"{AppDataFolder}\\{Path.GetFileNameWithoutExtension(inputVideo)}_{Guid.NewGuid()}.jpeg";
            var command = $"-ss {timeInSeconds} -i {inputVideo} -frames:v 1 -y {outputFrame}";

            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = FfmpegPath,
                    Arguments = command
                }
            };

            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return outputFrame;
        }

        public string Join(string firstVideo, string secondVideo)
        {
            throw new System.NotImplementedException();
        }

        public string Convert(string inputVideo, string format)
        {
            throw new System.NotImplementedException();
        }
    }
}
