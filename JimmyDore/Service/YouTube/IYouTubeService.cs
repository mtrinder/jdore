using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JimmyDore.Models;
using MvvmHelpers;

namespace JimmyDore.Service.YouTube
{
    public interface IYouTubeService
    {
        Task<ObservableRangeCollection<Video>> GetJimmysVideos(bool refresh);
        Task<ObservableRangeCollection<Video>> GetPlaylistForChannel(string channel, int maxCount);
        Task<Stats> GetStatisticsForVideo(string videoId);
    }
}
