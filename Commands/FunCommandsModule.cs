using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;
using JackTheStudent;
using System.Globalization;

namespace JackTheStudent.Commands
{
public class FunCommandsModule : IModule
{


    [Command("roll")]
    [Description("Rolls a random number from 1 to 100 which's considered your lucky number.")]
    public async Task Roll(CommandContext ctx)
    {
        Random r = new Random();
        int rnd = r.Next(1,100);
        await ctx.RespondAsync("Your lucky number is: " + rnd);
    }


    [Command("chances")]
    [Description("A complex algorithm used to calculate your chances of passing an exam or any other thing you wish to pass.")]
    public async Task Chances(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
        await ctx.RespondAsync("On a scale from 1 to 10, how do you rate your skills?");

        var intr = ctx.Client.GetInteractivityModule();
        var response = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(15));

        if(response == null) {
            await ctx.RespondAsync("If you not gonna answer, dont bother me dude...");
            
        } else if (response.Message.Content == "1" || response.Message.Content == "2" || response.Message.Content == "3") {
            await ctx.RespondAsync("That's kinda bad... What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);

            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("Low skills and low luck, I'm not sure about this dude... That's the worst you could get.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Low skills + under average luck = some trouble.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Low skills and above average luck, but do you really want to trust on that?");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Low skills and high luck, let the gamble begin!");
            }
             
        } else if (response.Message.Content == "4" || response.Message.Content == "5" || response.Message.Content == "6") {
            await ctx.RespondAsync("Not bad. What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);
            
            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("Average skills and low luck, this will be tough.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Average skills and under average luck, won't be easy.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Average skills and above average luck, kinda boring.");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Average skills and high luck, has some potential to be good.");
            }
             
        } else if (response.Message.Content == "7" || response.Message.Content == "8" || response.Message.Content == "9") {
            await ctx.RespondAsync("Looks promising! What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);

            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("Above average skills and low luck means you need to be careful.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Above average skills and under average luck. Don't worry, just focus.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Above average skills and above average luck. You got this for sure.");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Above average skills and high luck. Nothing can go wrong.");
            }
            
        } else if (response.Message.Content == "10") {
            await ctx.RespondAsync("Wow, you really need this? What's your lucky number?");
            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id,
                TimeSpan.FromSeconds(15));
            int luck = 0; Int32.TryParse(response1.Message.Content, out luck);

            if(response1 == null) {
            await ctx.RespondAsync("You might want to use !roll command to get your lucky number.");
            } else if (luck > 0 && luck <= 25) {
                await ctx.RespondAsync("You know everything and have low luck. Kinda like an uphill bike climb.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("You know everything and have under average luck, I guess you still got it.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("You know everything and have above average luck. Just go and write it already!");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("You know everything and have super high luck. Daaaaamn. This really can't go wrong.");
            }    
        }    

        await ctx.RespondAsync("Despite my answer, I hope you pass it anyway. Good luck!");

        }

    
    [Command("inspire")]
    [Description("Throws a random inspirational quote straight at you.")]
    public async Task Inspire(CommandContext ctx)    
    {
        var random = new Random();
        int index = random.Next(JackTheStudent.Program.quotes.Count);
        await ctx.RespondAsync(JackTheStudent.Program.quotes[index]);
    }
    }
}