using System.Collections.Generic;
using System.Threading.Tasks;
using JimmyDore.Models;

namespace JimmyDore.Service.Podcasts
{
    public interface IPodcastService
    {
        Task<List<Podcast>> RssParse(string url);
    }
}
