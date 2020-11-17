using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Serilog;

namespace JackTheStudent.Commands
{
public class PersonalReminderCommandsModule : Base​Command​Module
{
    [Command("reminder")]
    [Description("Semester group class table rebuild")]
    public async Task ReminderLog(CommandContext ctx, string date, string time, string about)
    {
        DateTime parsedEventDate;
        DateTime parsedEventTime;
        DateTime.TryParse(time, out parsedEventTime);
        DateTime.TryParse(date, out parsedEventDate);
        
        PersonalReminder reminder = new PersonalReminder{
            SetForDate = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
            LogById = ctx.Message.Author.Id,
            LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
            About = about,
            UserMention = ctx.Message.Author.Mention,
            ChannelId = ctx.Channel.Id
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
        Log.Logger.Information($"[Jack] {ctx.Message.Author.Id} logged new personal reminder with ID: {reminder.Id}");
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
                    await ctx.RespondAsync($"You have a reminder about \"{reminder.About}\" set for {reminder.SetForDate}.");
                    isEmpty = false;
                }
            }
            if(isEmpty) {
                await ctx.RespondAsync("You don't have any reminders set.");
                return;
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Reminder logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
            await ctx.RespondAsync("Log failed");
            return;
        }
        
    }

}
}