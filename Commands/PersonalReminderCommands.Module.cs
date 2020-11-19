using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Serilog;
using System.Linq;

namespace JackTheStudent.Commands
{
public class PersonalReminderCommandsModule : Base​Command​Module
{
    [Command("reminder")]
    [Description("Semester group class table rebuild")]
    public async Task ReminderLog(CommandContext ctx, string date = "", string time = "", string about = "")
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
            Log.Logger.Error($"[Jack] New reminder log, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
            await ctx.RespondAsync("Log failed");
            return;
        }
        Log.Logger.Information($"[Jack] Logged new personal reminder with ID: {reminder.Id}");
        await ctx.RespondAsync($"Reminder about \"{about}\" set for {date} {time}");
    }


    [Command("reminders")]
    [Description("Semester group class table rebuild")]
    public async Task ReminderLogs(CommandContext ctx)
    {
        try {
            bool isEmpty = true;
            foreach (PersonalReminder reminder in JackTheStudent.Program.reminderList) {
                if (reminder.LogById == ctx.Message.Author.Id) {
                    await ctx.RespondAsync($"You have a reminder about \"{reminder.About}\" set for {reminder.SetForDate.ToString().Trim()}.");
                    isEmpty = false;
                }
            }
            if(isEmpty) {
                await ctx.RespondAsync("You don't have any reminders set.");
                return;
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Reminder logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        
    }

}
}