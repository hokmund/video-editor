using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VE.Web.Contracts;
using VE.Web.Models;

namespace VE.Web.Services
{
    public class FfmpegService : IFfmpegService
    {
        private const string FfmpegPath = "Libs\\ffmpeg.exe";
        private const string FfprobePath = "Libs\\ffprobe.exe";

        public string GetFrame(string inputVideo, int timeInSeconds)
        {
            var inputVideoName = Path.GetFileNameWithoutExtension(inputVideo);
            var outputFrame = TempFilesUtils.GetMediaFile("{0}_{1}_sec.jpeg", inputVideoName, timeInSeconds);
            var parameters = $"-ss {timeInSeconds} -i {inputVideo} -frames:v 1 -y {outputFrame}";

            using (var process = ConfigureProcess(FfmpegPath, parameters))
            {
                process.Start();
                process.WaitForExit();
            }

            return outputFrame;
        }

        public string Join(params string[] inputs)
        {
            var tempVideos = AdjustResolutions(inputs);

            var outputVideo = TempFilesUtils.GetMediaFile("{0}.mp4", Guid.NewGuid());

            var files = "";
            var filters = "-filter_complex \"";

            for (var i = 0; i < tempVideos.Count; i++)
            {
                files += $"-i {tempVideos[i]} ";
                filters += $"[{i}:v:0][{i}:a:0]";
            }

            var parameters = files + filters + $"concat=n={tempVideos.Count}:v=1:a=1[outv][outa]\" " +
                             $"-map \"[outv]\" -map \"[outa]\" {outputVideo}";

            using (var process = ConfigureProcess(FfmpegPath, parameters))
            {
                process.Start();
                process.WaitForExit();
            }

            TempFilesUtils.CleanTempFiles(tempVideos);

            return outputVideo;
        }

        public string Convert(string inputVideo, VideoConversionOptions options)
        {
            //var parameters = $"-i {inputVideo} -y {Path.ChangeExtension(inputVideo, format)}";

            //using (var process = ConfigureProcess(FfmpegPath, parameters))
            //{
            //    process.Start();

            //    process.WaitForExit();
            //}

            return string.Empty;
        }

        private static IList<string> AdjustResolutions(string[] inputs)
        {
            var maxHeight = 0;
            var maxWidth = 0;

            var resolutions = new Dictionary<string, int[]>();

            foreach (var input in inputs)
            {
                var parameters = "-v error -select_streams v:0 -show_entries " +
                    $"stream=width,height -of csv=s=x:p=0 {input}";

                using (var process = ConfigureProcess(FfprobePath, parameters))
                {
                    process.Start();
                    process.WaitForExit();

                    var line = process.StandardOutput.ReadLine();

                    if (line != null)
                    {
                        var args = line.Split('x').Select(int.Parse).ToArray();
                        resolutions[input] = args;

                        maxWidth = Math.Max(maxWidth, args[0]);
                        maxHeight = Math.Max(maxHeight, args[1]);
                    }
                }
            }

            var tempVideos = new List<string>();

            foreach (var input in inputs)
            {
                var tempVideo = $"{Path.GetFileNameWithoutExtension(input)}_{Guid.NewGuid()}.{Path.GetExtension(input)}";
                tempVideo = TempFilesUtils.GetTempFile(tempVideo);

                // Upscale video if it is too small.
                if (resolutions[input][0] != maxWidth || resolutions[input][1] != maxHeight)
                {
                    var parameters = $"-i {input} -vf " +
                                     $"\"scale=w={maxWidth}:h={maxHeight}:force_original_aspect_ratio=1," +
                                     $"pad={maxWidth}:{maxHeight}:(ow-iw)/2:(oh-ih)/2\" " +
                                     $"{tempVideo}";

                    using (var process = ConfigureProcess(FfmpegPath, parameters))
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                }
                else
                {
                    File.Copy(input, tempVideo);
                }

                tempVideos.Add(tempVideo);
            }

            return tempVideos;
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
