using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using System.Linq;
using JackTheStudent.Commands;
using DSharpPlus.Interactivity.Extensions;
using JackTheStudent.Models;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;


namespace JackTheStudent.Commands
{
/* Create our class and extend from IModule */
public class BasicCommandsModule : Base​Command​Module
{
    /* Commands in DSharpPlus.CommandsNext are identified by supplying a Command attribute to a method in any class you've loaded into it. */
    /* The description is just a string supplied when you use the help command included in CommandsNext. */
    [Command("alive")]
    [Description("Simple command to test if the bot is running!")]
    public async Task Alive(CommandContext ctx)
    {
        /* Trigger the Typing... in discord */
        await ctx.TriggerTypingAsync();

        /* Send the message "I'm Alive!" to the channel the message was recieved from */
        await ctx.RespondAsync("I'm alive!");
    }

    [Command("dead")]
    [Description("Simple command to test if the bot is dead!")]
    public async Task Dead(CommandContext ctx)
    {
        /* Trigger the Typing... in discord */
        await ctx.TriggerTypingAsync();

        /* Send the message "I'm Alive!" to the channel the message was recieved from */
        await ctx.RespondAsync("I'm dead!");
    }

    [Command("truncate")]
    [Description("Simple command to test if the bot is dead!")]
    public async Task Truncate(CommandContext ctx)
    {
        
        using (var db = new JackTheStudentContext()){
            
        }                    

        /* Send the message "I'm Alive!" to the channel the message was recieved from */
        await ctx.RespondAsync("All tables have been cleared!");
    }

    [Command("interact")]
    [Description("Simple command to test interaction!")]
    public async Task Interact(CommandContext ctx)
    {
    /* Trigger the Typing... in discord */
        await ctx.TriggerTypingAsync();

    /* Send the message "I'm Alive!" to the channel the message was recieved from */
        await ctx.RespondAsync("How I am today?");

        var intr = ctx.Client.GetInteractivity(); // Grab the interactivity module
        var response = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
            TimeSpan.FromSeconds(15) // Wait 60 seconds for a response instead of the default 30 we set earlier!
        );
    // You can also check for a specific message by doing something like
        
    // Null if the user didn't respond before the timeout
        if(response.Result == null) {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
        } else if (response.ToString() == "bad") {
            await ctx.RespondAsync("Okay, how are you then?");

            var response1 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
                TimeSpan.FromSeconds(15)
            );
            if(response.Result == null) {
            await ctx.RespondAsync("Sorry, I didn't get a response!");
            return;
            } else if (response1.Result.Content.ToLower() == "bad") {
                await ctx.RespondAsync("Loser!");
            } else if (response1.Result.Content.ToLower() == "good") {
                await ctx.RespondAsync("I'm glad!");
            } // Wait 60 seconds for a response instead of the default 30 we set earlier!
        } else if (response.Result.Content.ToLower() == "good") {
            await ctx.RespondAsync("Okay, how are you then?");
            var response2 = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
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