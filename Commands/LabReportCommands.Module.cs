using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using Serilog;
using System.Globalization;
using JackTheStudent.CommandDescriptions;
using DSharpPlus.Entities;

namespace JackTheStudent.Commands
{
public class LabReportCommandsModule : Base​Command​Module
{
    
    [Command("labreport")]
    [Description(LabReportDescriptions.labreportLogDescription)]
    public async Task LabReportLog(CommandContext ctx,
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();

        if(groupId == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !labreport <group> <class> <labReportDate> <labReportTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !labreport <group> <class> <labReportDate> <labReportTime> Try again!");
            return;      
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (eventDate == ""){
            await ctx.RespondAsync("There's date missing, fix it!");
            return;
        } else if (!DateTime.TryParse(eventDate, out parsedEventDate)) {
            await ctx.RespondAsync("That's not a valid date you retard, learn to type!");
            return;
        } else if (eventTime == ""){
            await ctx.RespondAsync("There's time missing, fix it!");
            return;
        } else if (!DateTime.TryParse(eventTime, out parsedEventTime)) {
            await ctx.RespondAsync("That's not a valid time you retard, learn to type!");
            return;
        } else if(JackTheStudent.Program.labReportList
            .Any(l => 
                l.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && 
                l.Class == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                    .FirstOrDefault() && 
                l.GroupId == groupId)) {
            await ctx.RespondAsync("Someone has already logged this lab report.");
            return;
        } else if(JackTheStudent.Program.labReportList.Any(l => l.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && l.GroupId == groupId)) {
            await ctx.RespondAsync("There's a lab report logged for this group that takes place same time.");
            return;         
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var labReport = new LabReport {
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.labReportList.Add(labReport);
                db.LabReport.Add(labReport);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {labReport.Id}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Lab report logged successfully");     
        return;
        }   
    }

    [Command("labreports")]
    [Description(LabReportDescriptions.labreportLogsDescription)]
    public async Task LabReportLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve lab report for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve lab report for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED lab report, \"planned\" retrieves only future events.\n")] string span = "planned")
    {      
        if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == group) && group != ".") {
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (span != "." && span != "planned") {
            await ctx.RespondAsync("Span only accepts . and planned values");
            return;
        }

        var labReports = JackTheStudent.Program.labReportList;
        string chosenClass = JackTheStudent.Program.classList
            .Where(c => c.ShortName == classType)
            .Select(c => c.Name)
            .FirstOrDefault();
        string result = String.Empty;
        try {
            if (group == "." && classType == "." && span == "planned") {
                labReports = labReports
                    .Where(l => 
                        l.Date > DateTime.Now)
                    .ToList();
                if (labReports.Count == 0) {
                        await ctx.RespondAsync("Wait what!? There are literally no lab reports planned at all!");
                        return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                }
            } else if(classType == "." && span == "." && group != "." ) {
                labReports = labReports
                    .Where(l => 
                        l.GroupId == group)
                    .ToList();
                if (labReports.Count == 0) {
                        await ctx.RespondAsync($"There are no lab reports logged for group {group}!");
                        return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                }
            } else if (classType == "." && span == "planned") {
                labReports = labReports
                    .Where(l => 
                        l.Date > DateTime.Now && 
                        l.GroupId == group)
                    .ToList();
                if (labReports.Count == 0) {
                        await ctx.RespondAsync($"Wait what!? There are no lab reports planned for any class for group {group}!");
                        return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }

                }
            } else if (classType != "." && span == "planned" && group != ".") {
                labReports = labReports
                    .Where(l => 
                        l.Date > DateTime.Now && 
                        l.Class == chosenClass && 
                        l.GroupId == group)
                    .ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync($"There are no {labReports.Select(l => l.Class).FirstOrDefault()} lab reports planned for group {group}!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                }                                           
            } else if (classType != "." && span == "." && group != ".") {
                labReports = labReports
                    .Where(l => 
                        l.Class == chosenClass && 
                        l.GroupId == group)
                    .ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync($"There are no lab reports logged for {labReports.Select( c => c.Class).FirstOrDefault()} class for group {group}!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                    result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                } 
            } else if (classType != "." && span == "planned" && group == ".") {
                labReports = labReports
                    .Where(l => 
                        l.Class == chosenClass && 
                        l.Date > DateTime.Now)
                    .ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync($"There are no lab reports logged for {chosenClass} class for any of the groups!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                    result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                }                                                          
            } else {
                labReports = labReports.ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync("There aren no lab reports logged!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date.ToString().Trim()}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                }                        
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }

        var emoji = DiscordEmoji.FromName(ctx.Client, ":books:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Found lab reports:",
            Description = result,
            Color = new DiscordColor(0x106b2b) 
        };
        await ctx.RespondAsync("", embed: embed);       
    }
}
}