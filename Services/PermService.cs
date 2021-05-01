using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Kdrama.Models;
using Discord;


namespace Kdrama.Services
{
    public class PermService : IPermService
    {
        
        private readonly IMongoCollection<PermissionSchema> _Warn;
        public PermService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetSection("connectionStrings")["snipetrain"]);
            var database = client.GetDatabase("userInteractions");
            
            _Warn = database.GetCollection<PermissionSchema>("warn");
        }
        public async Task AddWarnAsync(PermissionSchema model)
        {
            await _Warn.InsertOneAsync(model);
        }
        public async Task<List<PermissionSchema>> GetDocsAsync()
        {
            return (await _Warn.FindAsync(s => true)).ToList();
        }
        public async Task<long> GetDocsAsync(IGuildUser user)
        {
            return await _Warn.CountDocumentsAsync(x => x.User == user.ToString());
        }

        public async Task RemoveDocAsync(string user)
        {
            await _Warn.DeleteManyAsync(s => s.User == user);
        }
    }
}