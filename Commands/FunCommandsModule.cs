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
        await ctx.RespondAsync("In scale of 1 to 10, what's your skill?");

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
                await ctx.RespondAsync("Low skills, low luck, I'm not sure about this dude... That's the worst you could get.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Low skills, under average luck mean some trouble.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Low skills, above average luck, but do you really want to trust on that?");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Low skills, super high luck, I'm not sure about this dude. ");
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
                await ctx.RespondAsync("Average skills, low luck, will be tough.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Average skills, under average luck, not going to be easy.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Average skills, above average luck, stable and boring.");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Average skills, super high luck, might be good.");
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
                await ctx.RespondAsync("Above average skills, low luck means you need to be careful.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("Above average skills, under average luck, don't worry, just focus.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("Above average skills, above average luck, you got this for sure.");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("Above average skills, super high luck. Nothing can go wrong.");
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
                await ctx.RespondAsync("Super high skills, low luck, I guess you still got it.");
            } else if (luck > 25 && luck <= 50) {
                await ctx.RespondAsync("You know everything, under average luck, it's so insignificant.");
            } else if (luck > 50 && luck <= 75) {
                await ctx.RespondAsync("You know everything, above average luck, go and write it already!");
            } else if (luck > 75 && luck <= 100) {
                await ctx.RespondAsync("You know everything and have super high luck. Daaaaamn.");
            }    
        }    

        await ctx.RespondAsync("Despite the result, I hope you pass it anyway. Good luck!");

        }
    }
}