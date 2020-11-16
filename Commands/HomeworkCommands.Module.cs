using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;

namespace JackTheStudent.Commands
{
public class HomeworkCommandsModule : Base​Command​Module
{
    
    [Command("homework")]
    [Description("Command logging a homework, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!homework <groupId> <classShortName> <deadlineDate> <deadlineTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!homework 3 mat 05-05-2021 13:30" + 
        "\n!homework 1 ele 05-05-2021 12:30 \"Calculator required\"")]
    public async Task HomeworkLog(CommandContext ctx,
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();

        if(groupId == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !homework <group> <class> <deadlineDate> <deadlineTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Contains(groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !homework <group> <class> <deadlineDate> <deadlineTime> Try again!");
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
                var homeWork = new Homework {
                    ClassShortName = classType,
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.homeworkList.Add(homeWork);
                db.Homework.Add(homeWork);
                await db.SaveChangesAsync();
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Homework logged successfully");     
        return;
        }   
    }

    [Command("homeworks")]
    [Description("Command retrieving logged homework based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!homeworks <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!homeworks - will retrieve all PLANNED homework for all the groups and all the classes" + 
        "\n!homeworks 1 - will retrieve all PLANNED homework for group 1 for all the classes" +
        "\n!homeworks 1 mat - will retrieve all PLANNED homework for group 1 for Maths class" +
        "\n!homeworks 1 mat planned - will retrieve all PLANNED homework for group 1 for Maths class" +
        "\n!homeworks 1 mat . - will retrieve all LOGGED homework for group 1 for Maths class" +
        "\n!homeworks 1 . . - will retrieve all LOGGED homework for group 1 for ALL classes" + 
        "\n!homeworks . . . - will retrieve all LOGGED homework for ALL groups for ALL classes" +
        "\n!homeworks . mat . - will retrieve all LOGGED homework for ALL groups for MAths class" +
        "\n!homeworks . . planned - will retrieve all PLANNED homework for ALL groups for ALL classes")]
    public async Task HomeworkLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve homework for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve homework for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED homework, \"planned\" retrieves only future events.\n")] string span = "planned")
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

        var homeworks = JackTheStudent.Program.homeworkList;
        string result = String.Empty;   

        if (group == "." && classType == "." && span == "planned") {
            try {
                homeworks = homeworks.Where(h => h.Date > DateTime.Now).ToList();
                    if (homeworks.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There is literally no homework planned at all!");
                    } else {
                        foreach (Homework homework in homeworks) {
                            result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(homework.Class)} homework for group {homework.GroupId} deadline is {homework.Date}.{(homework.AdditionalInfo.Equals("") ? "" : $"Additional info: {homework.AdditionalInfo}")}";
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
                homeworks = homeworks.Where(h => h.GroupId == group).ToList();
                    if (homeworks.Count == 0) {
                            await ctx.RespondAsync($"Wait what!? There is no homework logged for group {group}!");
                    } else {
                        foreach (Homework homework in homeworks) {
                            result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(homework.Class)} homework for group {homework.GroupId} deadline is/was {homework.Date}.{(homework.AdditionalInfo.Equals("") ? "" : $"Additional info: {homework.AdditionalInfo}")}";
                        }
                        await ctx.RespondAsync(result);
                    }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (classType == "." && span == "planned" && group != ".") {
            try {
                homeworks = homeworks.Where(h => h.Date > DateTime.Now && h.GroupId == group).ToList();
                    if (homeworks.Count == 0) {
                            await ctx.RespondAsync($"Wait what!? There is no planned homework for group {group}, hmm... league?");
                    } else {
                        foreach (Homework homework in homeworks) {
                            result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(homework.Class)} homework for group {homework.GroupId} deadline is {homework.Date}.{(homework.AdditionalInfo.Equals("") ? "" : $"Additional info: {homework.AdditionalInfo}")}";
                        }
                        await ctx.RespondAsync(result);
                    }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (classType != "." && span == "planned" && group !=".") {
            try {
                homeworks = homeworks.Where(h => h.Date > DateTime.Now && h.Class == classType && h.GroupId == group).ToList();                  
                if (homeworks.Count == 0) {
                    await ctx.RespondAsync($"There is no {homeworks.Select(h => h.Class).FirstOrDefault()} homework planned for group {group} at all!");
                    return;
                } else {
                    foreach (Homework homework in homeworks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(homework.Class)} homework for group {homework.GroupId}, deadline is {homework.Date}.{(homework.AdditionalInfo.Equals("") ? "" : $"Additional info: {homework.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                    return;
                }                           
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }                
        } else if (classType != "." && span == "." && group !=".") {
            try {
                homeworks = homeworks.Where(h => h.Class == classType && h.GroupId == group).ToList();                     
                if (homeworks.Count == 0) {
                    await ctx.RespondAsync($"There is no homework logged for {homeworks.Select(h => h.Class).FirstOrDefault()} class for group {group}!");
                    return;
                } else {
                    foreach (Homework homework in homeworks) {
                    result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(homework.Class)} homework for group {homework.GroupId}, deadline is/was {homework.Date}.{(homework.AdditionalInfo.Equals("") ? "" : $"Additional info: {homework.AdditionalInfo}")}";
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
                homeworks = homeworks.ToList();                     
                if (homeworks.Count == 0) {
                    await ctx.RespondAsync("There is no logged homework!");
                    return;
                } else {
                    foreach (Homework homework in homeworks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(homework.Class)} homework for group {homework.GroupId}, deadline is/was {homework.Date}.{(homework.AdditionalInfo.Equals("") ? "" : $"Additional info: {homework.AdditionalInfo}")}";
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
