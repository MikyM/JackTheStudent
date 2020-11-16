using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;

namespace JackTheStudent.Models
{
    public partial class Exam
    {
        public int Id { get; set; }
        public string Class { get; set; }
        public string ClassShortName { get; set; }
        public DateTime Date { get; set; }
        public string LogById { get; set; }
        public string LogByUsername { get; set; }
        public string AdditionalInfo { get; set; }
        public bool wasReminded { get; set; } = false;

        public async Task Ping(DiscordClient client) 
        {
            if(!this.wasReminded) {
                var channel = await client.GetChannelAsync(777157335885414411);
                await channel.SendMessageAsync($"There is a {this.Class} exam in a week, exact date is: {this.Date}");  
            }     
        }

    }
}
