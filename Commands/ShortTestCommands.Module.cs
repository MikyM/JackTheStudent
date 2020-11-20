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
public class ShortTestCommandsModule : Base​Command​Module
{
    
    [Command("shorttest")]
    [Description(ShortTestDescriptions.shorttestLogDescription)]
    public async Task ShortTestLog(CommandContext ctx,
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();

        if(groupId == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !shorttest <group> <class> <shortTestDate> <shortTestTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !shorttest <group> <class> <shortTestDate> <shortTestTime> Try again!");
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
        } else if(JackTheStudent.Program.shortTestList
            .Any(s => 
                s.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && 
                s.Class == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                    .FirstOrDefault() && 
                s.GroupId == groupId)) {
            await ctx.RespondAsync("Someone has already logged this short test.");
            return;
        } else if(JackTheStudent.Program.shortTestList.Any(s => s.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && s.GroupId == groupId)) {
            await ctx.RespondAsync("There's a short test logged for this group that takes place same time.");
            return;         
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var shortTest = new ShortTest {
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.shortTestList.Add(shortTest);
                db.ShortTest.Add(shortTest);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {shortTest.Id}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Short test logged successfully");     
        return;
        }   
    }

    [Command("shorttests")]
    [Description(ShortTestDescriptions.shorttestLogsDescription)]
    public async Task ShortTestLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve short test for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve short test for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED short test, \"planned\" retrieves only future events.\n")] string span = "planned")
    {       
        if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == group) && group != ".") {
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (span != "." && span != "planned") {
            await ctx.RespondAsync("Span only accepts ''. and 'planned' values");
            return;
        }

        var shortTests = JackTheStudent.Program.shortTestList;
        string chosenClass = JackTheStudent.Program.classList
            .Where(c => c.ShortName == classType)
            .Select(c => c.Name)
            .FirstOrDefault();
        string result = String.Empty;
        try {
            if (group == "." && classType == "." && span == "planned") {        
                shortTests = shortTests
                    .Where(s => 
                        s.Date > DateTime.Now)
                    .ToList();
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync("Wait what!? There are literally no short tests planned at all!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }
            } else if(classType == "." && span == "." && group != "." ) {
                shortTests = shortTests
                    .Where(s => 
                        s.GroupId == group)
                    .ToList();
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync($"There are no short tests logged for group {group}!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen/happened on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }
            } else if (classType == "." && span == "planned" && group != ".") {
                shortTests = shortTests
                    .Where(s => 
                        s.Date > DateTime.Now &&
                        s.GroupId == group)
                    .ToList();
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync($"Wait what!? There are no short tests planned for any class for group {group}!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }
            } else if (classType != "." && span == "planned" && group != ".") {
                shortTests = shortTests
                    .Where(s => 
                        s.Date > DateTime.Now && 
                        s.Class == chosenClass && 
                        s.GroupId == group)
                    .ToList();                     
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClass} short tests planned for group {group}!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }                                            
            } else if (classType != "." && span == "." && group != ".") {
                shortTests = shortTests
                    .Where(s => 
                        s.Class == chosenClass && 
                        s.GroupId == group)
                    .ToList();                     
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync($"There are no short tests logged for {chosenClass} class for group {group}!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen/happened on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }  
            } else if (classType != "." && span == "planned" && group == ".") {
                shortTests = shortTests
                    .Where(s => 
                        s.Class == chosenClass && 
                        s.Date > DateTime.Now)
                    .ToList();                     
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync($"There are no short tests logged for {chosenClass} class for any of the groups!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen/happened on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }                                        
            } else {
                shortTests = shortTests.ToList();                     
                if (shortTests.Count == 0) {
                    await ctx.RespondAsync("There are no short tests logged!");
                    return;
                } else {
                    foreach (ShortTest shortTest in shortTests) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shortTest.Class)} short test for group {shortTest.GroupId}, will happen/happened on {shortTest.Date.ToString().Trim()}.{(shortTest.AdditionalInfo.Equals("") ? "" : $"Additional info: {shortTest.AdditionalInfo}")}";
                    }
                }                          
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":pencil:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Found short tests:",
            Description = result,
            Color = new DiscordColor(0x4c910f) 
        };
        await ctx.RespondAsync("", embed: embed);
            
    }
}
}