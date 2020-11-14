using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus;

namespace JackTheStudent.Models
{
    public partial class PersonalReminder
    {
        public int Id { get; set; }
        public ulong LogById { get; set; }
        public string LogByUsername { get; set; }
        public string UserMention { get; set; }
        public DateTime SetForDate { get; set; }
        public ulong ChannelId { get; set; }
        public string About { get; set; }
        public async Task Ping(DiscordClient client) 
        {
            var channel = await client.GetChannelAsync(777157335885414411);
            await channel.SendMessageAsync($"Wake up {this.UserMention}, it's time for {this.About}.");            
        }
    }
}
