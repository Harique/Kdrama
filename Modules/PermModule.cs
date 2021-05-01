using System;
using System.Threading.Tasks;
using Discord.Commands;
using Kdrama.Models;
using Kdrama.Services;
using Discord;
using Microsoft.Extensions.Configuration;

namespace Kdrama.Modules
{
    [Group()]
    public class PermModule : ModuleBase
    {
        private readonly IPermService _Permservice;
        private IConfiguration _config;
        public PermModule(IPermService permService,IConfiguration config)
        {
            _config = config;
            _Permservice = permService;
        }
        
        [Command("warn")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task WarnAsync(IGuildUser user, [Remainder] string reason)
        {
            try
            {
                var role = Context.Guild.GetRole(ulong.Parse(_config.GetSection("discord").GetSection("roles").GetSection("Gulag")["id"]));
                var adminName = Context.User.ToString();
                var date = DateTime.UtcNow;
                var WarnedUser = user.ToString();

                if (user == null)
                {
                    throw new UserNotFoundException($"Couldn't Find {user}");
                }

                var DBinfo = new PermissionSchema
                {
                    AdminName = adminName,
                    Date = date,
                    Reason = reason,
                    User = WarnedUser,
                };
                await _Permservice.AddWarnAsync(DBinfo);
                await ReplyAsync("warned");

                var docNum = await _Permservice.GetDocsAsync(user);
                if (docNum == 3)
                {
                    await user.AddRoleAsync(role);
                    await _Permservice.RemoveDocAsync(user.ToString());
                }
                
            }
            catch(UserNotFoundException e)
            {
                await ReplyAsync(e.Message);
            }
            catch (Exception e)
            {
                await ReplyAsync($"Error While Trying to Warn {user}");
                Console.WriteLine(e.ToString());
            }
        }
    }
}