using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using Serilog;
using DSharpPlus.Interactivity.Extensions;



namespace JackTheStudent.Commands
{
public class BasicCommandsModule : Base​Command​Module
{
    [Command("alive")]
    [Description("Simple command to test if the bot is running!")]
    public async Task Alive(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();
        await ctx.RespondAsync("I'm alive!");
    }

    [Command("dead")]
    [Description("Simple command to test if the bot is dead!")]
    public async Task Dead(CommandContext ctx)
    {
        
        DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":thinking:");
        await ctx.TriggerTypingAsync();
        await ctx.RespondAsync(emoji);
        Log.Logger.Information($"User {ctx.Message.Author.Mention} used dead command");
    }


    [Command("interact")]
    [Description("Simple command to test interaction!")]
    public async Task Interact(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();

        await ctx.RespondAsync("How I am today?");

        var intr = ctx.Client.GetInteractivity();
        var response = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(15) 
        );

        if(response.Result == null) {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
        } else if (response.ToString() == "bad") {
            await ctx.RespondAsync("Okay, how are you then?");

            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(15)
            );
            if(response.Result == null) {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
            } else if (response1.Result.Content.ToLower() == "bad") {
                await ctx.RespondAsync("Loser!");
            } else if (response1.Result.Content.ToLower() == "good") {
                await ctx.RespondAsync("I'm glad!");
            } 
        } else if (response.Result.Content.ToLower() == "good") {
            await ctx.RespondAsync("Okay, how are you then?");
            var response2 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(15)
            );
            if(response.Result == null) {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
            } else if (response2.Result.Content.ToLower() == "bad") {
                await ctx.RespondAsync("Aww, that's too bad!");
            } else if (response2.Result.Content.ToLower() == "good") {
                await ctx.RespondAsync("I'm glad!");
            }
        } 
        
        await ctx.RespondAsync("Thank you for telling me how you are!");
    }
    
}
}