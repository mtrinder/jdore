using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using FeedParserCore;
using JimmyDore.Models;

namespace JimmyDore.Service.Podcasts
{
    public class PodcastService : IPodcastService
    {
        private readonly HttpClient _httpClient;

        public PodcastService(HttpClient httpClient)
        {
            CurrentPods = new List<Podcast>();
            _httpClient = httpClient;
        }

        public List<Podcast> CurrentPods { get; set; }

        public async Task<List<Podcast>> RssParse(string url)
        {
            if (!CurrentPods.Any())
            {
                try
                {
                    var stream = await _httpClient.GetStreamAsync(url);

                    var items = await FeedParser.ParseAsync(stream,
                        xDocument => xDocument.Root
                            .Descendants()
                            .Where(i => i.Name.LocalName == "channel")
                            .Elements()
                            .Where(i => i.Name.LocalName == "item"),
                        item => new Podcast
                        {
                            Title = item.GetElementValue<string>("title"),
                            Date = item.GetElementValue<DateTime>("pubDate"),
                            Description = item.GetElementValue<string>("description"),
                            Link = (item.Nodes().ToList().FirstOrDefault(n => n.ToString().Contains("enclosure")) as XElement).Attribute("url").Value,
                        });

                    CurrentPods = items.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return CurrentPods;
        }
    }
}
