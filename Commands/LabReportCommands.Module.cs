using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;
namespace JackTheStudent.Commands
{
public class LabReportCommandsModule : Base​Command​Module
{
    
    [Command("labreport")]
    [Description("Command logging a lab report, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!labreport <groupId> <classShortName> <labReportDate> <labReportTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!labreport 3 mat 05-05-2021 13:30" + 
        "\n!labreport 1 ele 05-05-2021 12:30 \"Calculator required\"")]
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
        } else if (!JackTheStudent.Program.groupList.Contains(groupId)){
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
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var labReport = new LabReport {
                    ClassShortName = classType,
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
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Lab report logged successfully");     
        return;
        }   
    }

    [Command("labreports")]
    [Description("Command retrieving logged lab report based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!labreports <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!labreports - will retrieve all PLANNED lab reports for all the groups and all the classes" + 
        "\n!labreports 1 - will retrieve all PLANNED lab reports for group 1 for all the classes" +
        "\n!labreports 1 mat - will retrieve all PLANNED lab reports for group 1 for Maths class" +
        "\n!labreports 1 mat planned - will retrieve all PLANNED lab reports for group 1 for Maths class" +
        "\n!labreports 1 mat . - will retrieve all LOGGED lab reports for group 1 for Maths class" +
        "\n!labreports 1 . . - will retrieve all LOGGED lab reports for group 1 for ALL classes" + 
        "\n!labreports . . . - will retrieve all LOGGED lab reports for ALL groups for ALL classes" +
        "\n!labreports . mat . - will retrieve all LOGGED lab reports for ALL groups for MAths class" +
        "\n!labreports . . planned - will retrieve all PLANNED lab reports for ALL groups for ALL classes")]
    public async Task LabReportLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve lab report for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve lab report for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED lab report, \"planned\" retrieves only future events.\n")] string span = "planned")
    {      
        if (!JackTheStudent.Program.groupList.Contains(group) && group != ".") {
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
        string result = String.Empty;

        if (group == "." && classType == "." && span == "planned") {
            try {
            labReports = labReports.Where(l => l.Date > DateTime.Now).ToList();
                if (labReports.Count == 0) {
                        await ctx.RespondAsync("Wait what!? There are literally no lab reports planned at all!");
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is {labReport.Date}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if(classType == "." && span == "." && group != "." ) {
            try {
            labReports = labReports.Where(l => l.GroupId == group).ToList();
                if (labReports.Count == 0) {
                        await ctx.RespondAsync($"There are no lab reports logged for group {group}!");
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (classType == "." && span == "planned") {
            try {
            labReports = labReports.Where(l => l.Date > DateTime.Now && l.GroupId == group).ToList();
                if (labReports.Count == 0) {
                        await ctx.RespondAsync($"Wait what!? There are no lab reports planned for any class for group {group}!");
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is {labReport.Date}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (classType != "." && span == "planned" && group != ".") {
            try {
                labReports = labReports.Where(l => l.Date > DateTime.Now && l.Class == classType && l.GroupId == group).ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync($"There are no {labReports.Select(l => l.Class).FirstOrDefault()} lab reports planned for group {group}!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is {labReport.Date}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                    return;
                }                           
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }                 
        } else if (classType != "." && span == "." && group != ".") {
            try {
                labReports = labReports.Where(l => l.Class == classType && l.GroupId == group).ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync($"There are no lab reports logged for {labReports.Select( c => c.Class).FirstOrDefault()} class for group {group}!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                    result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                    return;
                }                           
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }                 
        } else {
            try {
                labReports = labReports.ToList();                     
                if (labReports.Count == 0) {
                    await ctx.RespondAsync("There aren no lab reports logged!");
                    return;
                } else {
                    foreach (LabReport labReport in labReports) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(labReport.Class)} lab report for group {labReport.GroupId}, deadline is/was {labReport.Date}.{(labReport.AdditionalInfo.Equals("") ? "" : $"Additional info: {labReport.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                    return;
                }                        
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        }    
    }
}
}