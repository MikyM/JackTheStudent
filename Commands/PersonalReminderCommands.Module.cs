using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Serilog;
using System.Linq;
using DSharpPlus.Entities;

namespace JackTheStudent.Commands
{
public class PersonalReminderCommandsModule : Base​Command​Module
{
    [Command("reminder")]
    [Description("Sets a reminder for a user calling it for the specified time and date. 'About' is optional.\n" +
    "As always multiple words must be wrapped with \"\"." +
    "!reminder <date> <time> <about>\n" +
    "Example:\n" +
    "!reminder 20/11/2020 19:23" +
    "!reminder 20/11/2020 19:23 \"finally reach gold in league\"")]
    public async Task ReminderLog(CommandContext ctx, 
        [Description("Takes date in dd/mm/yyyy format, accepts different separators.")] string date = "", 
        [Description("Takes time in hh:mm format.")] string time = "", 
        [Description("Takes anything you want it to take, it's that awesome, just wrap multiple words within \"\".")] string about = "")
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();

        if (date == "") {
            await ctx.RespondAsync("You forgot to provide a date dummy!");
            return;
        } else if (!DateTime.TryParse(date, out parsedEventDate)) {
            await ctx.RespondAsync("That's not a valid date!");
            return;
        } else if (time == "") {
            await ctx.RespondAsync("And maybe add the time you dumbass? Am I a fairy?");
            return;
        } else if (!DateTime.TryParse(time, out parsedEventTime)) {
            await ctx.RespondAsync("That's not a valid time!");
            return;
        } else if (about == "") {
            await ctx.RespondAsync("You forgot to add what will the alarm be about.");
            return;
        } else if(JackTheStudent.Program.reminderList.Any(r => r.SetForDate == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay))) {
            await ctx.RespondAsync("You already have a reminder logged for this exact time.");
            return;
        }
        
        PersonalReminder reminder = new PersonalReminder{
            SetForDate = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
            LogById = ctx.Message.Author.Id,
            LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
            About = about,
            UserMention = ctx.Message.Author.Mention
        };
        
        JackTheStudent.Program.reminderList.Add(reminder);

        try {
            using (var db = new JackTheStudentContext()) {
                db.PersonalReminder.Add(reminder);
                await db.SaveChangesAsync();
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Log failed");
            return;
        }
        Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {reminder.Id}");
        await ctx.RespondAsync($"Reminder about \"{about}\" set for {date} {time}");
    }


    [Command("reminders")]
    [Description("Shows all set reminders logged by a user calling it.")]
    public async Task ReminderLogs(CommandContext ctx)
    {
        string result = String.Empty;
        try {
            bool isEmpty = true;
            foreach (PersonalReminder reminder in JackTheStudent.Program.reminderList) {
                if (reminder.LogById == ctx.Message.Author.Id && DateTime.Now < reminder.SetForDate) {
                    result = $"{result} \nYou have a reminder about \"{reminder.About}\" set for {reminder.SetForDate.ToString().Trim()}.";
                    isEmpty = false;
                }
            }
            if(isEmpty) {
                await ctx.RespondAsync("You don't have any reminders set.");
                return;
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":alarm_clock:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Reminders:",
            Description = result,
            Color = new DiscordColor(0x6b2810) 
        };
        await ctx.RespondAsync("", embed: embed);
            
        
    }

}
}