using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharp​Plus;
using System;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Interactivity.Extensions;
using HtmlAgilityPack;
using JackTheStudent.CommandDescriptions;
using JackTheStudent.Models;
using System.Globalization;
using Serilog;



namespace JackTheStudent.Commands
{
    public class HelpfullLevelModule : Base​Command​Module
    {
        [Command("award")]
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

            await ctx.RespondAsync(userPing + " has been awarded with 1 help point. Now he has a total of " + JackTheStudent.Program.respects[idString] + " points.");
        }


        [Command("points")]
        [Description("Awards specific person with a respect point to rise their rank! Max 2 respects a day.")]
        public async Task Points(CommandContext ctx, string user = "")
        {
            await ctx.RespondAsync(user + " has a total of " + JackTheStudent.Program.respects[user] + " respects.");
        }
    }
}