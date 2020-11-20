using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using DSharpPlus;
using System.Collections.Generic;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
using DSharp​Plus.Entities;
using System.Text.RegularExpressions;
using Serilog;

namespace JackTheStudent.Commands
{
public class AdminCommandsModule : Base​Command​Module
{
    [RequireOwner]
    [Hidden]
    [Command("newsemesterclass")]
    [Description("Semester group class table rebuild")]
    public async Task SemesterClass(CommandContext ctx)
    {
        List<Class> backupClasses = new List<Class>();

        await ctx.RespondAsync("Are you absolutely sure you want to do it?");
        var intr = ctx.Client.GetInteractivity(); 
        var sureResponse = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(5)
        );
   
        if(sureResponse.Result.Content.ToLower() != "yes" || sureResponse.Result == null)
        {
            await ctx.RespondAsync("Aborting, all good");
            return;
        }

        using (var context = new JackTheStudentContext()) {
            backupClasses = context.Class.ToList();
            var truncateText = "truncate table class";
            try {
                await context.Database.ExecuteSqlRawAsync(truncateText);
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] New semester classes, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Truncate failed");
                return;
            }
            JackTheStudent.Program.classList.Clear();
            await context.SaveChangesAsync();
            await ctx.RespondAsync("Truncate successful");
        }
        
        

        using (var db = new JackTheStudentContext()){
            await ctx.RespondAsync("Let's populate the table with new data, how many classes are there for the new semester?");
        
            var amountOfClassesResponse = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(10) 
            );

            if (amountOfClassesResponse.TimedOut) {
                await RestoreClasses(backupClasses, ctx);
                return;
            }
            short classesCount = 0;
            while (!Int16.TryParse(amountOfClassesResponse.Result.Content, out classesCount) || classesCount < 2)
            {
                await ctx.RespondAsync("That's not a valid number, try again.");
                amountOfClassesResponse = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, 
                    TimeSpan.FromSeconds(10) 
                );

                if(amountOfClassesResponse.TimedOut) {
                    await RestoreClasses(backupClasses, ctx);
                return;
                }
            }

            if (amountOfClassesResponse.TimedOut) {
                await RestoreClasses(backupClasses, ctx);
                return;
            }

            for (int i = 1; i <= classesCount; i++) {
                await ctx.RespondAsync($"What's the name and shortname of the {i} class? Answer with NAME SHORTNAME");
                var newClassNames = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, 
                    TimeSpan.FromSeconds(30) 
                );

                if (newClassNames.TimedOut) {
                    await RestoreClasses(backupClasses, ctx);
                    return;
                }

                string[] classArray = newClassNames.Result.Content.Split(new Char[]{' '});

                while (classArray.Count() != 2) {
                    await ctx.RespondAsync("Try again -.-");
                    newClassNames = await intr.WaitForMessageAsync(
                        c => c.Author.Id == ctx.Message.Author.Id, 
                        TimeSpan.FromSeconds(30) 
                    );

                    classArray = newClassNames.Result.Content.Split(new Char[]{' '});

                    if (newClassNames.TimedOut) {
                        await RestoreClasses(backupClasses, ctx);
                        return;
                    }
                } 
 
                var newClass = new Class {
                    Name = classArray[0],
                    ShortName = classArray[1]
                };
                JackTheStudent.Program.classList.Add(newClass);
                db.Class.Add(newClass);
                await ctx.RespondAsync($"Added class: {newClass.Name} with short name: {newClass.ShortName}");
                Task.Delay(1000).Wait();          
            }

            try {
                await db.SaveChangesAsync();      
            } catch (Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await RestoreClasses(backupClasses, ctx);
                return;
            } 
            Log.Logger.Information("[Jack} Class table repopulated successfully");             
            await ctx.RespondAsync("Class table repopulated successfully");          
        }
    }

    [RequireOwner]
    [Hidden]
    [Command("newsemestergroup")]
    [Description("Semester group class table rebuild")]
    public async Task SemesterGroup(CommandContext ctx)
    {
        List<Models.Group> backupGroups = new List<Models.Group>();

        await ctx.RespondAsync("Are you absolutely sure you wanna do it?");
        var intr = ctx.Client.GetInteractivity(); 
        var sureResponse = await intr.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(5)
        );
   
        if(sureResponse.Result.Content.ToLower() != "yes" || sureResponse.Result == null)
        {
            await ctx.RespondAsync("Aborting, all good");
            return;
        }

        using (var context = new JackTheStudentContext()) {
            backupGroups = context.Group.ToList();
            var truncateText = "truncate table `group`";
            try {
                await context.Database.ExecuteSqlRawAsync(truncateText);
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] New semester groups, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Truncate failed");
                return;
            }
            JackTheStudent.Program.groupList.Clear();
            await context.SaveChangesAsync();
            await ctx.RespondAsync("Truncate successful");
        }
        
        using (var db = new JackTheStudentContext()){
            await ctx.RespondAsync("Let's populate the table with new data, how many groups are there for the new semester?");
        
            var amountOfGroupsResponse = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(10) 
            );

            if (amountOfGroupsResponse.TimedOut) {
                await RestoreGroups(backupGroups, ctx);
                return;
            }
            short groupsCount = 0;

            while (!Int16.TryParse(amountOfGroupsResponse.Result.Content, out groupsCount) || groupsCount < 1)
            {
                await ctx.RespondAsync("That's not a valid number, try again.");
                amountOfGroupsResponse = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, 
                    TimeSpan.FromSeconds(10) 
                );

                if(amountOfGroupsResponse.TimedOut) {
                    await RestoreGroups(backupGroups, ctx);
                return;
                }

            }
            

            for (int i = 1; i <= groupsCount; i++) {
                await ctx.RespondAsync($"What's the ID of the {i} group? Answer with shortID fullID");
                var newGroupIds = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, 
                    TimeSpan.FromSeconds(30) 
                );

                if (newGroupIds.TimedOut) {                  
                    await RestoreGroups(backupGroups, ctx);
                    return;
                }

                string[] groupArray = newGroupIds.Result.Content.Split(new Char[]{' '});

                while (groupArray.Count() != 2) {
                    await ctx.RespondAsync("Try again -.-");
                    newGroupIds = await intr.WaitForMessageAsync(
                        c => c.Author.Id == ctx.Message.Author.Id, 
                        TimeSpan.FromSeconds(30) 
                    );

                    groupArray = newGroupIds.Result.Content.Split(new Char[]{' '});

                    if (newGroupIds.TimedOut) {
                        await RestoreGroups(backupGroups, ctx);
                        return;
                    }
                }

                var newGroup = new Models.Group {
                    GroupId = groupArray[0],
                    FullGroupId = groupArray[1]
                };
                JackTheStudent.Program.groupList.Add(newGroup);
                db.Group.Add(newGroup);
                await ctx.RespondAsync($"Added group with ID: {newGroup.GroupId}.");
                Task.Delay(1000).Wait();
            }

            try {
                await db.SaveChangesAsync();      
            } catch (Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await RestoreGroups(backupGroups, ctx);
                return;
            }
            Log.Logger.Information("[Jack} Group table repopulated successfully");  
            await ctx.RespondAsync("Group table repopulated successfully");   
        }
        return;
    }

    [RequireOwner]
    [Hidden]
    [Command("ban")]
    [Description("Bans a member from the server")]
    public async Task Ban(CommandContext ctx, string mention, string reason = "")
    {
        string userMention = mention;
        ulong userId = ulong.Parse(Regex.Replace(userMention, @"[><@!]", ""));
        DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
        await member.BanAsync(7, reason);
        await ctx.RespondAsync($"User {mention} has been banned.");
        Log.Logger.Information($"[Jack] {ctx.Message.Author.Username} with ID: {ctx.Message.Author.Id} banned {member.DisplayName} with ID: {userId}");
    }

    [RequireOwner]
    [Hidden]
    [Command("unban")]
    [Description("Unbans a member")]
    public async Task Unban(CommandContext ctx, string mention)
    {
        string userMention = mention;
        ulong userId = ulong.Parse(Regex.Replace(userMention, @"[><@!]", ""));
        DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
        await member.UnbanAsync();
        await ctx.RespondAsync($"User {mention} has been unbanned.");
        Log.Logger.Information($"[Jack] {ctx.Message.Author.Username} with ID: {ctx.Message.Author.Id} unbanned {member.DisplayName} with ID: {userId}");
    }

    [RequireOwner]
    [Hidden]
    [Command("kick")]
    [Description("Kicks a member from the server")]
    public async Task Kick(CommandContext ctx, string mention, string reason = "")
    {
        string userMention = mention;
        ulong userId = ulong.Parse(Regex.Replace(userMention, @"[><@!]", ""));
        DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
        await member.RemoveAsync(reason);
        await ctx.RespondAsync($"User {mention} has been kicked.");
        Log.Logger.Information($"[Jack] {ctx.Message.Author.Username} with ID: {ctx.Message.Author.Id} kicked {member.DisplayName} with ID: {userId}");
    }

    [RequireOwner]
    [Hidden]
    [Command("mutevoice")]
    [Description("Mutes a member for specified time")]
    public async Task MuteVoice(CommandContext ctx, string mention, string reason = "")
    {   
        string userMention = mention;
        ulong userId = ulong.Parse(Regex.Replace(userMention, @"[><@!]", ""));
        DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
        if(member.VoiceState.Channel == null) {
            await ctx.RespondAsync("This user isn't connected to any voice channels.");
            return;
        }
        await member.SetMuteAsync(true, reason);
        await ctx.RespondAsync($"User {mention} has been muted in voice channels.");
        Log.Logger.Information($"[Jack] {ctx.Message.Author.Username} with ID: {ctx.Message.Author.Id} muted {member.DisplayName} with ID: {userId}");
    }

    [RequireOwner]
    [Hidden]
    [Command("unmutevoice")]
    [Description("Unmutes a member")]
    public async Task UnmuteVoice(CommandContext ctx, string mention)
    {
        string userMention = mention;
        ulong userId = ulong.Parse(Regex.Replace(userMention, @"[><@!]", ""));
        DiscordMember member = await ctx.Guild.GetMemberAsync(userId);
        if(member.VoiceState.Channel == null) {
            await ctx.RespondAsync("This user isn't connected to any voice channels.");
            return;
        }
        await member.SetMuteAsync(false);
        await ctx.RespondAsync($"User {mention} has been unmuted in voice channels.");
        Log.Logger.Information($"[Jack] {ctx.Message.Author.Username} with ID: {ctx.Message.Author.Id} unmuted {member.DisplayName} with ID: {userId}");
    }

    public async Task RestoreGroups(List<Models.Group> backupList, CommandContext ctx)
    {
        await ctx.RespondAsync("Failed, aborting and restoring from backup.");
        using(var db = new JackTheStudentContext()) {
            await db.Database.ExecuteSqlRawAsync("truncate table `group`");
            foreach (Models.Group backupGroup in backupList) {
                db.Group.Add(backupGroup);
                JackTheStudent.Program.groupList.Add(backupGroup);
            }
            await db.SaveChangesAsync();
        }
        await ctx.RespondAsync("Groups restored from backup.");
    }

    public async Task RestoreClasses(List<Class> backupList, CommandContext ctx)
    {
        await ctx.RespondAsync("Failed, aborting and restoring from backup.");
        using(var db = new JackTheStudentContext()) {
            await db.Database.ExecuteSqlRawAsync("truncate table class");
            foreach (Class backupClass in backupList) {
                db.Class.Add(backupClass);
                JackTheStudent.Program.classList.Add(backupClass);
            }
            await db.SaveChangesAsync();
        }
        await ctx.RespondAsync("Groups restored from backup.");
    }

}
}