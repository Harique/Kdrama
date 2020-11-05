using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using snipetrain_bot.Models;
using snipetrain_bot.Services;

namespace snipetrain_bot.Modules
{
    [Group("event")]
    public class EventModule : ModuleBase
    {
        private readonly IEventService _eventservice;
        public EventModule(IEventService eventService)
        {
            _eventservice = eventService;

        }
        [Command("add")]
        public async Task addEvent(string prize, [Remainder] string message)
        {
            try
            {
                var user = Context.User.ToString();
                var time = DateTime.Now.ToString();
                var events = new EventSchema
                {
                    Prize = prize,
                    Message = message,
                    AnDate = time,
                    Name = user
                };
                await _eventservice.AddEventAsync(events);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                await ReplyAsync($"Error trying to add a event :: Check Logs");
            }

        }
        [Command("delete")]
        public async Task deletEvent([Remainder] string id)
        {
            try
            {
                var docid = await _eventservice.GetDocAsync(id);

                if (docid == null)
                    await ReplyAsync("Event Doesnt Exist in the DB");

                await _eventservice.DeleteDocAsync(id);

                await ReplyAsync("Deleted the Doc from thr DB");
            }
            catch (Exception e)
            {

                System.Console.WriteLine(e.ToString());
                await ReplyAsync("Error While Trying to Delete the Doc from the DB,Please Check the logs");
            }
        }

    }
}