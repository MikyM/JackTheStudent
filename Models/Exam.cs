using System;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using DSharpPlus;

namespace JackTheStudent.Models
{
    public partial class Exam
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public bool WasReminded { get; set; } = false;

        public async Task Ping(DiscordClient client, ulong channelId, ulong roleId) 
        {
            var emoji = DiscordEmoji.FromName(client, ":timer:");
            if(!this.WasReminded) {
                var channel = await client.GetChannelAsync(channelId);
                await channel.SendMessageAsync($"{emoji} There is a {this.Class} exam in a week, exact date is: {this.Date}. <@&{roleId}>");  
            }     
            
        }

    }
}
