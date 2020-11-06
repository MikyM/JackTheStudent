using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class StudentCommandsModule : IModule
{
    /* Commands in DSharpPlus.CommandsNext are identified by supplying a Command attribute to a method in any class you've loaded into it. */
    /* The description is just a string supplied when you use the help command included in CommandsNext. */
    [Command("log")]
    [Description("Command logging an event of specified type")]
    public async Task Log(CommandContext ctx, string type)
    {

    // Null if the user didn't respond before the timeout
        if(type == "exam") {
            await ctx.RespondAsync("exam");
            return;
        } else if (type == "homework") {
            await ctx.RespondAsync("homework");
        } else if (type == "lreport") {
            await ctx.RespondAsync("lreport");
        } else if (type == "qtest") {
            await ctx.RespondAsync("qtest");
        } else {
            await ctx.RespondAsync("Unknown log type");
        } 
        
    }

    [Command("logs")]
    [Description("Command retrieving all logged events of specified type")]
    public async Task Logs(CommandContext ctx, string type)
    {
        if(type == "exam") {
            await ctx.RespondAsync("exams");
            return;
        } else if (type == "homework") {
            await ctx.RespondAsync("homeworks");
        } else if (type == "lreport") {
            await ctx.RespondAsync("lreports");
        } else if (type == "qtest") {
            await ctx.RespondAsync("qtests");
        } else {
            await ctx.RespondAsync("Unknown log type");
        }  
    }

}
}