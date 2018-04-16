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

        private static readonly Dictionary<VideoFormat, string> VideoCodecs = new Dictionary<VideoFormat, string>
        {
            [VideoFormat.Flv] = "flv",
            [VideoFormat.Mp4] = "libx264",
            [VideoFormat.Webm] = "libvpx",
            [VideoFormat.Ogv] = "libtheora",
        };

        private static readonly Dictionary<VideoFormat, string> AudioCodecs = new Dictionary<VideoFormat, string>
        {
            [VideoFormat.Flv] = "libmp3lame",
            [VideoFormat.Mp4] = "libmp3lame",
            [VideoFormat.Webm] = "libvorbis",
            [VideoFormat.Ogv] = "libvorbis",
        };

        public string GetFrame(string inputVideo, int timeInSeconds)
        {
            var inputVideoName = Path.GetFileNameWithoutExtension(inputVideo);
            var outputFrame = FilesUtils.GetMediaFile("{0}_{1}_sec.jpeg", inputVideoName, timeInSeconds);
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

            var outputVideo = FilesUtils.GetMediaFile("{0}.mp4", Guid.NewGuid());

            var files = "";
            var filters = "-filter_complex \"";

            for (var i = 0; i < tempVideos.Count; i++)
            {
                files += $"-i {tempVideos[i]} ";
                filters += $"[{i}:v:0][{i}:a:0]";
            }

            var parameters = files + filters + $"concat=n={tempVideos.Count}:v=1:a=1[outv][outa]\" " +
                             $"-map \"[outv]\" -map \"[outa]\" -y {outputVideo}";

            using (var process = ConfigureProcess(FfmpegPath, parameters))
            {
                process.Start();
                process.WaitForExit();
            }

            FilesUtils.CleanTempFiles(tempVideos);

            return outputVideo;
        }

        public string Convert(string inputVideo, VideoConversionOptions options)
        {
            var outputFile = FilesUtils.GetMediaFile(
                "{0}_{1}.{2}",
                Path.GetFileNameWithoutExtension(inputVideo),
                Guid.NewGuid(),
                options.Format.ToString().ToLower());

            var parameters = $"-i {inputVideo} ";

            if (VideoCodecs.ContainsKey(options.Format))
            {
                parameters += $"-vcodec {VideoCodecs[options.Format]} ";
            }

            if (options.Width != 0 && options.Height != 0)
            {
                parameters += $"-s {options.Width}x{options.Height} ";
            }

            if (options.HorizontalAspect != 0 && options.VerticalAspect != 0)
            {
                parameters += $"-aspect {options.Width}:{options.Height} ";
            }

            if (options.Bitrate > 0)
            {
                parameters += $"-b:v {options.Bitrate}k ";
            }

            if (AudioCodecs.ContainsKey(options.Format))
            {
                parameters += $"-acodec {AudioCodecs[options.Format]}  -b:a 64k ";
            }

            parameters += $"-y {outputFile}";

            using (var process = ConfigureProcess(FfmpegPath, parameters))
            {
                process.Start();

                process.WaitForExit();
            }

            return outputFile;
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
                var tempVideo = $"{Path.GetFileNameWithoutExtension(input)}_{Guid.NewGuid()}{Path.GetExtension(input)}";
                tempVideo = FilesUtils.GetTempFile(tempVideo);

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
