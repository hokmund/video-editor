using System;
using VE.Web.Models;

namespace VE.Web.Contracts
{
    public interface IFfmpegService
    {
        string GetFrame(string inputVideo, int timeInSeconds);

        string Join(params string[] inputs);

        string Convert(string inputVideo, VideoConversionOptions options);
    }
}
