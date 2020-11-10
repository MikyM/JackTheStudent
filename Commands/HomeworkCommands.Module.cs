using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;
using JackTheStudent;
using System.Globalization;

namespace JackTheStudent.Commands
{
public class HomeworkCommandsModule : IModule
{
    
    [Command("homework")]
    [Description("Command logging a homework, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!homework <groupId> <classShortName> <deadlineDate> <deadlineTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!homework 3 mat 05-05-2021 13:30" + 
        "\n!homework 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!homework 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!homework 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"")]
    public async Task HomeworkLog(CommandContext ctx,
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "", 
        [Description ("\nTakes material links, multiple links must be wrapped with \"\".\n")] string materials = "")
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
                var homeWork = new Homework {Class = classType,
                                                Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                                                GroupId = groupId,
                                                LogById = ctx.Message.Author.Id.ToString(),
                                                LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                                                AdditionalInfo = additionalInfo,
                                                Materials = materials};
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
        }
        if (group == "." && classType == "." && span == "planned") {
            try {
                using (var db = new JackTheStudentContext()){
                var homeworks = db.Homework
                            .Where( x => x.Date > DateTime.Now)
                            .ToList();
                    if (homeworks.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There is literally no homework planned at all!");
                    } else {
                        string result = String.Empty;
                        foreach (Homework homework in homeworks) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is " + homework.Date;
                        }
                        await ctx.RespondAsync(result);
                    }
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if(classType == "." && span == "." && group != "." ) {
            try {
                using (var db = new JackTheStudentContext()){
                var homeworks = db.Homework
                    .Where( x => x.GroupId == group)
                    .ToList();
                    if (homeworks.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There is no homework logged for group " + group + "!");
                    } else {
                        string result = String.Empty;
                        foreach (Homework homework in homeworks) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is/was " + homework.Date;
                        }
                        await ctx.RespondAsync(result);
                    }
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (classType == "." && span == "planned" && group != ".") {
            try {
                using (var db = new JackTheStudentContext()){
                var homeworks = db.Homework
                    .Where(x => x.Date > DateTime.Now && x.GroupId == group)
                    .ToList();
                    if (homeworks.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There is no planned homework for group " + group +", hmm... league?");
                    } else {
                        string result = String.Empty;
                        foreach (Homework homework in homeworks) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is " + homework.Date;
                        }
                        await ctx.RespondAsync(result);
                    }
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (classType != "." && span == "planned" && group !=".") {

            if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                try {
                    using (var db = new JackTheStudentContext()){
                        var homeworks = db.Homework
                            .Where(x => x.Date > DateTime.Now && x.Class == classType && x.GroupId == group)
                            .ToList();                     

                        if (homeworks.Count == 0) {
                            string response = "There is no " + JackTheStudent.Program.classList
                                                                .Where( c => c.ShortName == classType)
                                                                .Select( c => c.Name)
                                                                .FirstOrDefault() + " homework planned for group " + group + " at all!";
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            foreach (Homework homework in homeworks) {
                                result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                            .ToTitleCase(JackTheStudent.Program.classList
                                                            .Where( c => c.ShortName == homework.Class)
                                                            .Select( c => c.Name)
                                                            .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is " + homework.Date;
                            }
                            await ctx.RespondAsync(result);
                            return;
                        }                           
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            } else {
                await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !homeworks <group> <group> <eventDate> <eventTime> Try again!");
                return;
            }                    
        } else if (classType != "." && span == "." && group !=".") {
            if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                try {
                    using (var db = new JackTheStudentContext()){
                        var homeworks = db.Homework
                            .Where(x => x.Class == classType && x.GroupId == group)
                            .ToList();                     

                        if (homeworks.Count == 0) {
                            string response = "There is no homework logged for " + JackTheStudent.Program.classList
                                                                                    .Where( c => c.ShortName == classType)
                                                                                    .Select( c => c.Name)
                                                                                    .FirstOrDefault() + " class " + "for group " + group + "!";
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            foreach (Homework homework in homeworks) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is/was " + homework.Date;
                            }
                            await ctx.RespondAsync(result);
                            return;
                        }                           
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            } else {
                await ctx.RespondAsync("Ya know there's only either all possible events or the ones that didn't happen right? Get yo facts straight negro!");
                return;
            }                   
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                    var homeworks = db.Homework.ToList();                     

                    if (homeworks.Count == 0) {
                        string response = "There is no logged homework!";
                        await ctx.RespondAsync(response);
                        return;
                    } else {
                        string result = String.Empty;
                        foreach (Homework homework in homeworks) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is/was " + homework.Date;
                        }
                        await ctx.RespondAsync(result);
                        return;
                    }
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
