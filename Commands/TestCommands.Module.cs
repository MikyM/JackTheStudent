using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
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
        "\n!test 3 mat 05-05-2021 13:30 \"Calculator required\"")]
    public async Task TestLog(CommandContext ctx,
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
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
                var test = new Test {
                    ClassShortName = classType,
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
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
        } else if (span != "." && span != "planned") {
            await ctx.RespondAsync("Span only accepts . and planned values");
            return;
        }

        var tests = JackTheStudent.Program.testList;
        string result = String.Empty;

        if (group == "." && classType == "." && span == "planned") {
            try {
                tests = tests.Where(t => t.Date > DateTime.Now).ToList();
                if (tests.Count == 0) {
                        await ctx.RespondAsync("Wait what!? There are literally no tests planned at all!");
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen on {test.Date}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
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
                tests = tests.Where(t => t.GroupId == group).ToList();
                if (tests.Count == 0) {
                        await ctx.RespondAsync($"There are no tests logged for group {group}!");
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen/happened on {test.Date}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
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
                tests = tests.Where(t => t.Date > DateTime.Now && t.GroupId == group).ToList();
                if (tests.Count == 0) {
                        await ctx.RespondAsync($"Wait what!? There are no tests planned for any class for group {group}!");
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen on {test.Date}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
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
                tests = tests.Where(t => t.Date > DateTime.Now && t.Class == classType && t.GroupId == group).ToList();                     
                if (tests.Count == 0) {
                    await ctx.RespondAsync($"There are no {tests.Select(c => c.Class).FirstOrDefault()} tests planned for group {group}!");
                    return;
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen on {test.Date}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
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
                tests = tests.Where(t => t.Class == classType && t.GroupId == group).ToList();                     
                if (tests.Count == 0) {
                    await ctx.RespondAsync($"There are no {tests.Select(c => c.Class).FirstOrDefault()} tests planned for group {group}!");
                    return;
                } else {                   
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen/happened on {test.Date}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
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
                tests = tests.ToList();                     
                if (tests.Count == 0) {
                    await ctx.RespondAsync("There aren no tests logged!");
                    return;
                } else {
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen/happened on {test.Date}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
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