using System.Collections.Generic;
using System.Threading.Tasks;
using Kdrama.Models;
using Discord;

namespace Kdrama.Services
{
    public interface IPermService
    {
        Task AddWarnAsync(PermissionSchema model);
        Task<List<PermissionSchema>> GetDocsAsync();
        Task<long> GetDocsAsync(IGuildUser user);
        Task RemoveDocAsync(string user);

    }
}