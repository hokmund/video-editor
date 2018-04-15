namespace VE.Web.Contracts
{
    public interface IFfmpegService
    {
        string GetFrame(string inputVideo, int timeInSeconds);

        string Join(string firstVideo, string secondVideo);

        string Convert(string inputVideo, string format);
    }
}
