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
        public bool WasReminded { get; set; }
        public string About { get; set; }
        public async Task Ping(DiscordClient client, ulong channelId) 
        {
            if(!this.WasReminded) {
                var channel = await client.GetChannelAsync(channelId);
                await channel.SendMessageAsync($"Wake up {this.UserMention}, you slack! It's time for/to \"{this.About}\".");   
            }         
        }
    }
}
