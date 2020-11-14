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
public class TestCommandsModule : Base​Command​Module
{
    
    [Command("test")]
    [Description("Command logging a test, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!test <groupId> <classShortName> <testDate> <testTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!test 3 mat 05-05-2021 13:30" + 
        "\n!test 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!test 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!test 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"")]
    public async Task TestLog(CommandContext ctx,
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
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !test <group> <class> <testDate> <testTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Contains(groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !test <group> <class> <testDate> <testTime> Try again!");
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
                var test = new Test {Class = classType,
                                                Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                                                GroupId = groupId,
                                                LogById = ctx.Message.Author.Id.ToString(),
                                                LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                                                AdditionalInfo = additionalInfo,
                                                Materials = materials};
                JackTheStudent.Program.testList.Add(test);
                db.Test.Add(test);
                await db.SaveChangesAsync();
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Test logged successfully");     
        return;
        }   
    }

    [Command("tests")]
    [Description("Command retrieving logged test based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!tests <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!tests - will retrieve all PLANNED tests for all the groups and all the classes" + 
        "\n!tests 1 - will retrieve all PLANNED tests for group 1 for all the classes" +
        "\n!tests 1 mat - will retrieve all PLANNED tests for group 1 for Maths class" +
        "\n!tests 1 mat planned - will retrieve all PLANNED tests for group 1 for Maths class" +
        "\n!tests 1 mat . - will retrieve all LOGGED tests for group 1 for Maths class" +
        "\n!tests 1 . . - will retrieve all LOGGED tests for group 1 for ALL classes" + 
        "\n!tests . . . - will retrieve all LOGGED tests for ALL groups for ALL classes" +
        "\n!tests . mat . - will retrieve all LOGGED tests for ALL groups for MAths class" +
        "\n!tests . . planned - will retrieve all PLANNED tests for ALL groups for ALL classes")]
    public async Task TestLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve test for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve test for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED test, \"planned\" retrieves only future events.\n")] string span = "planned")
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
                var tests = db.Test
                            .Where( x => x.Date > DateTime.Now)
                            .ToList();
                    if (tests.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There are literally no tests planned at all!");
                    } else {
                        string result = String.Empty;
                        foreach (Test test in tests) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == test.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " test for group " + test.GroupId + ", will happen on " + test.Date;
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
                var tests = db.Test
                    .Where( x => x.GroupId == group)
                    .ToList();
                    if (tests.Count == 0) {
                            await ctx.RespondAsync("There are no tests logged for group " + group + "!");
                    } else {
                        string result = String.Empty;
                        foreach (Test test in tests) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == test.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " " + test.Date;
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
        } else if (classType == "." && span == "planned") {
            try {
                using (var db = new JackTheStudentContext()){
                var tests = db.Test
                    .Where(x => x.Date > DateTime.Now && x.GroupId == group)
                    .ToList();
                    if (tests.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There are no tests planned for any class for group " + group + "!");
                    } else {
                        string result = String.Empty;
                        foreach (Test test in tests) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == test.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " test for group " + test.GroupId + ", will happen on " + test.Date;
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
        } else if (classType != "." && span == "planned" && group != ".") {

            if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                try {
                    using (var db = new JackTheStudentContext()){
                        var tests = db.Test
                            .Where(x => x.Date > DateTime.Now && x.Class == classType && x.GroupId == group)
                            .ToList();                     

                        if (tests.Count == 0) {
                            string response = "There is no " + JackTheStudent.Program.classList
                                                                .Where( c => c.ShortName == classType)
                                                                .Select( c => c.Name)
                                                                .FirstOrDefault() + " test planned for group " + group + "!";
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            foreach (Test test in tests) {
                                result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                            .ToTitleCase(JackTheStudent.Program.classList
                                                            .Where( c => c.ShortName == test.Class)
                                                            .Select( c => c.Name)
                                                            .FirstOrDefault()) + " test for group " + test.GroupId + ", will happen on " + test.Date;
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
                await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !tests <group> <group> <testDate> <testTime> Try again!");
                return;
            }                    
        } else if (classType != "." && span == "." && group != ".") {
            if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                try {
                    using (var db = new JackTheStudentContext()){
                        var tests = db.Test
                            .Where(x => x.Class == classType && x.GroupId == group)
                            .ToList();                     

                        if (tests.Count == 0) {
                            string response = "There is no test logged for " + JackTheStudent.Program.classList
                                                                                    .Where( c => c.ShortName == classType)
                                                                                    .Select( c => c.Name)
                                                                                    .FirstOrDefault() + " class " + "for group " + group + "!";;
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            foreach (Test test in tests) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == test.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " test for group " + test.GroupId + ", will happen / happened on " + test.Date;
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
                    var tests = db.Test.ToList();                     

                    if (tests.Count == 0) {
                        string response = "There aren no tests logged!";
                        await ctx.RespondAsync(response);
                        return;
                    } else {
                        string result = String.Empty;
                        foreach (Test test in tests) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == test.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " test for group " + test.GroupId + ", will happen / happened " + test.Date;
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