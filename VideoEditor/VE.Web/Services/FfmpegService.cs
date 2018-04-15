using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VE.Web.Contracts;

namespace VE.Web.Services
{
    public class FfmpegService : IFfmpegService
    {
        private const string AppDataFolder = "AppData";
        private const string FfmpegPath = "Libs\\ffmpeg.exe";
        private const string FfprobePath = "Libs\\ffprobe.exe";
        
        public string GetFrame(string inputVideo, int timeInSeconds)
        {
            var outputFrame = GetTempFile("{0}_{1}_sec.jpeg", Path.GetFileNameWithoutExtension(inputVideo), timeInSeconds);
            var parameters = $"-ss {timeInSeconds} -i {inputVideo} -frames:v 1 -y {outputFrame}";

            var process = ConfigureProcess(FfmpegPath, parameters);

            process.Start();
            process.WaitForExit();

            return outputFrame;
        }

        public string Join(params string[] inputs)
        {
            var tempVideos = AdjustResolutions(inputs);

            var outputVideo = GetTempFile("{0}.mp4", Guid.NewGuid());

            var files = "";
            var filters = "-filter_complex \"";

            for (var i = 0; i < tempVideos.Count; i++)
            {
                files += $"-i {tempVideos[i]} ";
                filters += $"[{i}:v:0][{i}:a:0]";
            }

            var parameters = files + filters + $"concat=n={tempVideos.Count}:v=1:a=1[outv][outa]\" " +
                             $"-map \"[outv]\" -map \"[outa]\" {outputVideo}";

            var process = ConfigureProcess(FfmpegPath, parameters);

            process.Start();
            process.WaitForExit();

            CleanTempFiles(tempVideos);

            return outputVideo;
        }

        private static void CleanTempFiles(IEnumerable<string> tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        private static IList<string> AdjustResolutions(string[] inputs)
        {
            var maxHeight = 0;
            var maxWidth = 0;

            foreach (var input in inputs)
            {
                var parameters = "-v error -select_streams v:0 -show_entries " + 
                    $"stream=width,height -of csv=s=x:p=0 {input}";

                var process = ConfigureProcess(FfprobePath, parameters);
                process.Start();
                process.WaitForExit();

                var line = process.StandardOutput.ReadLine();

                if (line != null)
                {
                    var args = line.Split('x');

                    maxWidth = Math.Max(maxWidth, int.Parse(args[0]));
                    maxHeight = Math.Max(maxHeight, int.Parse(args[1]));
                }
            }

            var tempVideos = new List<string>();

            foreach (var input in inputs)
            {
                var tempVideo = $"{Path.GetFileNameWithoutExtension(input)}_{Guid.NewGuid()}.{Path.GetExtension(input)}";
                var parameters = $"-i {input} -vf " +
                                 $"\"scale=w={maxWidth}:h={maxHeight}:force_original_aspect_ratio=1," +
                                 $"pad={maxWidth}:{maxHeight}:(ow-iw)/2:(oh-ih)/2\" " +
                                 $"{Path.Combine(AppDataFolder, tempVideo)}";

                var process = ConfigureProcess(FfmpegPath, parameters);

                process.Start();
                process.WaitForExit();

                tempVideos.Add(tempVideo);
            }

            return tempVideos;
        }

        public string Convert(string inputVideo, string format)
        {
            throw new NotImplementedException();
        }

        private static string GetTempFile(string fileNameTemplate, params object[] inputs)
        {
            return Path.Combine(AppDataFolder, string.Format(fileNameTemplate, inputs));
        }

        private static Process ConfigureProcess(string utility, string parameters)
        {
            return new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = utility,
                    Arguments = parameters
                }
            };
        }
    }
}
