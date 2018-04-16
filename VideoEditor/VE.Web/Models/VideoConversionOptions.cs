using System;

namespace VE.Web.Models
{
    public class VideoConversionOptions
    {
        public VideoFormat Format { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int VerticalAspect { get; set; }

        public int HorizontalAspect { get; set; }

        public int Bitrate { get; set; }

        public static VideoConversionOptions FromConvertRequest(ConvertRequest request)
        {
            return new VideoConversionOptions
            {
                Bitrate = request.Bitrate,
                Format = (VideoFormat)Enum.Parse(typeof(VideoFormat), request.Format, true),
                Height = request.Height,
                Width = request.Width,
            };
        }
    }
}
