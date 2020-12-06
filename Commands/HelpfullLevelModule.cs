using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharp​Plus;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;





namespace JackTheStudent.Commands
{
    public class HelpfullLevelModule : Base​Command​Module
    {
        [Command("respect")]
        [Description("Awards specific person with a respect point to rise their rank! Max 2 respects a day.")]
        public async Task Award(CommandContext ctx, DiscordMember user)
        {
            var idString = user.Id.ToString();
            var idAuthorString = ctx.Message.Author.Id.ToString();
            var userPing = user.Mention;

            var usersList = await ctx.Guild.GetAllMembersAsync();

            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 1 && DateTime.Now.Minute >= 0 && DateTime.Now.Minute < 1){
                await ctx.RespondAsync("Come on, let me take a break! This command will work after 00:01.");
                return;
            }

            if (idString == idAuthorString)
            {
                await ctx.RespondAsync("You can't award yourself, what a shameful attempt yikes.");
                return;
            }

            if (!usersList.Contains(user))
            {
                await ctx.RespondAsync("There is no student called like that!");
                return;
            }

            if (JackTheStudent.Program.limiter.ContainsKey(idAuthorString) == false)
            {
                JackTheStudent.Program.limiter.Add(idAuthorString, 0);
            }

            if (JackTheStudent.Program.respects.ContainsKey(idString) == false)
            {
                JackTheStudent.Program.respects.Add(idString, 0);
            }

            JackTheStudent.Program.limiter[idAuthorString] += 1;

            if (JackTheStudent.Program.limiter[idAuthorString] > 2)
            {
                await ctx.RespondAsync("Sorry, each user can award someone 2 times a day max!");
                return;
            }

            JackTheStudent.Program.respects[idString] += 1;

            //var asd = JackTheStudent.Program.respects[idString].Where(x => x.id = id).FirstOrDefault;

            await ctx.RespondAsync(userPing + " has been awarded with 1 help point. Now he has a total of " + JackTheStudent.Program.respects[idString] + " points.");
        }


        [Command("respects")]
        [Description("Checks your current respects count.")]
        public async Task Points(CommandContext ctx)
        {
            var idAuthorString = ctx.Message.Author.Id.ToString();
            await ctx.RespondAsync("You have " + JackTheStudent.Program.respects[idAuthorString] /* tutaj bedzie odniesienie do db */ + " respects.");
        }

        [Command("leaderboard")]
        [Description("Check who's the boss!")]
        public async Task Leaderboard(CommandContext ctx)
        {
            var ranking = new List<string>();

            foreach (var item in JackTheStudent.Program.respects.OrderByDescending(key => key.Value)){
                for(int i = 1; i <= 5; i++){
                    ranking.Add(item.ToString());
                }
            } 

            var leaderboardEmbed = new DiscordEmbedBuilder
            {
                Title = ":gem: All time best helpers :gem:",
                Description = string.Join("\n", ranking)
            };
            
            await ctx.RespondAsync(embed: leaderboardEmbed);
        }
    }
}