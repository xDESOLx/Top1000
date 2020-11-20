using System.Collections.Generic;
using System.Threading.Tasks;
using Top1000.Models;

namespace Top1000.Services
{
    public interface IStackExchangeClient
    {
        Task<IEnumerable<Tag>> GetMostPopularTagsAsync(int amount = 1000, string site = "stackoverflow");
    }
}