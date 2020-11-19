using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using Serilog;
using DSharpPlus.Entities;

/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class StudentCommandsModule : Base​Command​Module
{
    [Command("groups")]
    [Description("Command retrieving group IDs")]
    public async Task GroupLogs(CommandContext ctx)
    {
        try {
            var groups = JackTheStudent.Program.groupList.ToList();
            if (groups.Count == 0) {
                    await ctx.RespondAsync("No groups logged!");
            } else {
                string result = String.Empty;
                foreach (Group group in groups) {
                    result = $"{result} \nID: {group.GroupId}";
                }
                var emoji = DiscordEmoji.FromName(ctx.Client, ":grey_exclamation:");
                var embed = new DiscordEmbedBuilder {
                    Title = $"{emoji} Groups:",
                    Description = result,
                    Color = new DiscordColor(0x0f9175) 
                };
                await ctx.RespondAsync("", embed: embed);
            }
        } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Group logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
        }
        return; 
    }

    [Command("classes")]
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
                    result = $"{result} \nClass - {uniClass.Name}, short version - {uniClass.ShortName}";
                }
                var emoji = DiscordEmoji.FromName(ctx.Client, ":grey_exclamation:");
                var embed = new DiscordEmbedBuilder {
                    Title = $"{emoji} Classes:",
                    Description = result,
                    Color = new DiscordColor(0x0f9175) 
                };
                await ctx.RespondAsync("", embed: embed); 
            }
        } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Class logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
        }
        return; 
    }

    [Command("classtypes")]
    [Description("Command retrieving class types")]
    public async Task ClassTypesLogs(CommandContext ctx)
    {
        try {
            var classTypes = JackTheStudent.Program.classTypeList.ToList();
            if (classTypes.Count == 0) {
                    await ctx.RespondAsync("No class types logged!");
            } else {
                string result = String.Empty;
                foreach (ClassType classType in classTypes) {
                    result = $"{result} \nClass type - {classType.Name}, short version - {classType.ShortName}";
                }
                var emoji = DiscordEmoji.FromName(ctx.Client, ":grey_exclamation:");
                var embed = new DiscordEmbedBuilder {
                    Title = $"{emoji} Class types:",
                    Description = result,
                    Color = new DiscordColor(0x0f9175) 
                };
                await ctx.RespondAsync("", embed: embed);    
            }
        } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Class types logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
        }
        return; 
    }
}
}