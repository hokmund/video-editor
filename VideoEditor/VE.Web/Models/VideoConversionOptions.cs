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
    }
}
