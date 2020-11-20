using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Top1000.Models;

namespace Top1000.Services
{
    public class StackExchangeClient : IStackExchangeClient
    {
        private readonly IHttpClientFactory _clientFactory;

        public StackExchangeClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<Tag>> GetMostPopularTagsAsync(int amount = 1000, string site = "stackoverflow")
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "\"amount\" must be greater than 0.");
            if (site.Equals(string.Empty))
                throw new ArgumentException(nameof(site), "\"site\" must not be string.Empty.");
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            var client = _clientFactory.CreateClient("StackExchange");
            List<Tag> tags = new List<Tag>();
            int pages = (int)Math.Ceiling(amount / 100.0);
            for (int i = 1; i <= pages; i++)
            {
                var request = string.Format("tags?page={0}&pagesize={1}&order=desc&sort=popular&site={2}", i, 100, site);
                var result = await client.GetAsync(request);
                if (!result.IsSuccessStatusCode)
                {
                    if (result.Content != null)
                    {
                        using (JsonDocument document = await JsonDocument.ParseAsync(await result.Content.ReadAsStreamAsync()))
                        {
                            throw new HttpRequestException(string.Format("{0}: {1}", document.RootElement.GetProperty("error_id").GetRawText(), document.RootElement.GetProperty("error_name").GetString()));
                        }
                    }
                    else
                    {
                        throw new HttpRequestException(string.Format("An unexpected error has occured during the request for the {0}th page.", i));
                    }
                }
                using (JsonDocument document = await JsonDocument.ParseAsync(await result.Content.ReadAsStreamAsync()))
                {
                    var items = document.RootElement.GetProperty("items").GetRawText();
                    tags.AddRange(JsonSerializer.Deserialize<IEnumerable<Tag>>(items, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    }));
                }
            }
            if (tags.Count > amount)
                tags.RemoveRange(amount, tags.Count - amount);
            return tags;
        }
    }
}
