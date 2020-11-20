using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;
using JackTheStudent.CommandDescriptions;
using Serilog;
using DSharpPlus.Entities;

namespace JackTheStudent.Commands
{
public class TestCommandsModule : Base​Command​Module
{
    
    [Command("test")]
    [Description(TestDescriptions.testLogDescription)]
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
        } else if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == groupId)){
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
        } else if(JackTheStudent.Program.testList
            .Any(t => 
                t.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && 
                t.Class == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                    .FirstOrDefault() && 
                t.GroupId == groupId)) {
            await ctx.RespondAsync("Someone has already logged this test.");
            return;
        } else if(JackTheStudent.Program.testList.Any(t => t.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && t.GroupId == groupId)) {
            await ctx.RespondAsync("There's a test logged for this group that takes place same time.");
            return;         
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var test = new Test {
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
                Log.Logger.Information($"[Jack] Logged test with ID: {test.Id} {DateTime.Now}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] New test log, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Test logged successfully");     
        return;
        }   
    }

    [Command("tests")]
    [Description(TestDescriptions.testLogsDescription)]
    public async Task TestLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve test for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve test for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED test, \"planned\" retrieves only future events.\n")] string span = "planned")
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

        var tests = JackTheStudent.Program.testList;
        string result = String.Empty;
        try {
            if (group == "." && classType == "." && span == "planned") {
                tests = tests
                    .Where(t => 
                        t.Date > DateTime.Now)
                    .ToList();
                if (tests.Count == 0) {
                        await ctx.RespondAsync("Wait what!? There are literally no tests planned at all!");
                        return;
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen on {test.Date.ToString().Trim()}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
                    }
                }
            } else if(classType == "." && span == "." && group != "." ) {
                tests = tests
                    .Where(t => 
                        t.GroupId == group)
                    .ToList();
                if (tests.Count == 0) {
                        await ctx.RespondAsync($"There are no tests logged for group {group}!");
                        return;
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen/happened on {test.Date.ToString().Trim()}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
                    }
                }
            } else if (classType == "." && span == "planned" && group != ".") {
                tests = tests
                    .Where(t => 
                        t.Date > DateTime.Now && 
                        t.GroupId == group)
                    .ToList();
                if (tests.Count == 0) {
                        await ctx.RespondAsync($"Wait what!? There are no tests planned for any class for group {group}!");
                        return;
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen on {test.Date.ToString().Trim()}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
                    }
                }
            } else if (classType != "." && span == "planned" && group != ".") {
                tests = tests
                    .Where(t => 
                        t.Date > DateTime.Now && 
                        t.Class == JackTheStudent.Program.classList
                            .Where(c => c.ShortName == classType)
                            .Select(c => c.Name)
                            .FirstOrDefault() && 
                        t.GroupId == group)
                    .ToList();                     
                if (tests.Count == 0) {
                    await ctx.RespondAsync($"There are no {tests.Select(c => c.Class).FirstOrDefault()} tests planned for group {group}!");
                    return;
                } else {                  
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen on {test.Date.ToString().Trim()}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
                    }
                }                                          
            } else if (classType != "." && span == "." && group != ".") {
                tests = tests
                    .Where(t => 
                        t.Class == JackTheStudent.Program.classList
                            .Where(c => c.ShortName == classType)
                            .Select(c => c.Name)
                            .FirstOrDefault() && 
                        t.GroupId == group)
                    .ToList();                     
                if (tests.Count == 0) {
                    await ctx.RespondAsync($"There are no {tests.Select(c => c.Class).FirstOrDefault()} tests planned for group {group}!");
                    return;
                } else {                   
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen/happened on {test.Date.ToString().Trim()}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
                    }
                }                                       
            } else {
                tests = tests.ToList();                     
                if (tests.Count == 0) {
                    await ctx.RespondAsync("There aren no tests logged!");
                    return;
                } else {
                    foreach (Test test in tests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(test.Class)} test for group {test.GroupId}, will happen/happened on {test.Date.ToString().Trim()}.{(test.AdditionalInfo.Equals("") ? "" : $"Additional info: {test.AdditionalInfo}")}";
                    }
                }                          
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Test logs, caller - {ctx.Message.Author.Id}, error: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":writing_hand:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Found tests:",
            Description = result,
            Color = new DiscordColor(0x0f4191) 
        };
        await ctx.RespondAsync("", embed: embed);    
    }
}
}