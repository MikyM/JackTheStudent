using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class StudentCommandsModule : Base​Command​Module
{
    [Command("group")]
    [Description("Command retrieving group IDs")]
    public async Task GroupLogs(CommandContext ctx)
    {
        try {
            var groups = JackTheStudent.Program.groupList.ToList();
            if (groups.Count == 0) {
                    await ctx.RespondAsync("No groups logged!");
            } else {
                string result = String.Empty;
                for(int i = 0; i < groups.Count(); i++) {
                    result = $"{result} \n{groups[i]}";
                }
                await ctx.RespondAsync(result);
            }
        } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
        }
        return; 
    }

    [Command("class")]
    [Description("Command retrieving classes")]
    public async Task ClassesLogs(CommandContext ctx)
    {
        try {
            var classes = JackTheStudent.Program.classList.ToList();
            if (classes.Count == 0) {
                    await ctx.RespondAsync("No classes logged!");
            } else {
                string result = String.Empty;
                foreach (Class uniClass in classes) {
                    result = $"{result} \n Class - {uniClass.Name}, short version - {uniClass.ShortName}";
                }
                await ctx.RespondAsync(result);
            }
        } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
        }
        return; 
    }
}
}