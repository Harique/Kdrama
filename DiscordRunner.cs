using System;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Kdrama.Modules;
using Microsoft.Extensions.Configuration;

namespace Kdrama
{
    public class DiscordRunner
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private IConfiguration _config;
        private SocketGuild Guild;

        public DiscordRunner(IConfiguration config)
        {
            _commands = new CommandService();
            _config = config;
        }

        public async Task StartClient(IServiceProvider services)
        {
            _services = services;

            _client = new DiscordSocketClient();

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _config.GetSection("discord")["token"]);
            await _client.StartAsync();

            await _client.DownloadUsersAsync(_client.Guilds);
            // Guild = _client.GetGuild(ulong.Parse(_config.GetSection("discord")["guildId"]));


            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommand;
            _client.ReactionAdded += OnReactionUp;
            _client.GuildAvailable += OnGuildAvailable;
            
            await _commands.AddModuleAsync<PermModule>(_services);

        }

        public async Task OnGuildAvailable(SocketGuild guild)
        {
            Guild = guild;
        }

        public async Task<IMessage> SendMessage(string message, ulong channelId)
        {
            try
            {
                var socketChannel = _client.GetChannel(channelId) as IMessageChannel;
                return await socketChannel.SendMessageAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while trying to send Message to Channel :: {e.ToString()}");
                return null;
            }
        }

        public async Task<IMessage> SendDMMessage(string message, IUser user)
        {
            try
            {
                return await user.SendMessageAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while trying to send Message to Channel :: {e.ToString()}");
                return null;
            }
        }

        public async Task OnReactionUp(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {

                var messageId = ulong.Parse(_config.GetSection("discord").GetSection("messages")["streamReaction"]);
                var reactionCode = _config.GetSection("discord").GetSection("emotes")["stream"];

                if (message.Id == messageId && reaction.Emote.Name == reactionCode)
                {
                    var role = Guild.GetRole(ulong.Parse(_config.GetSection("discord").GetSection("roles").GetSection("stream")["id"]));
                    var guildUser = Guild.GetUser(reaction.UserId);

                    await (guildUser as IGuildUser).AddRoleAsync(role);
                    await SendDMMessage($"Successfully Added you to the <{role.Name}> Role.", guildUser);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        
        public async Task HandleCommand(SocketMessage messageParam)
        {

            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix(_config["prefixChar"][0], ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var context = new CommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

    }
}
