using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;

namespace JackTheStudent.Commands
{

public class TeamsLinksCommandsModule : Base​Command​Module
{
    
    [Command("teamslink")]
    [Description("Command logging a teamsLink, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!teamsLink <groupId> <classShortName> <teamsLinkDate> <teamsLinkTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!teamsLink 3 mat 05-05-2021 13:30" + 
        "\n!teamsLink 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!teamsLink 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!teamsLink 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"")]
    public async Task TeamsLinkLog(CommandContext ctx,
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string uniClass = "",
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "",   
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDayOfWeek = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "",
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = ".", 
        [Description ("\nTakes time in hh:mm format.\n")] string link = "",
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        DayOfWeek parsedEventDayOfWeek = new DayOfWeek();
        DateTime parsedEventTime = new DateTime();
        if (uniClass == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !teamsLink <group> <class> <teamsLinkDate> <teamsLinkTime> Try again!");
            return;      
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == uniClass)) {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Class type is missing!");
            return;
        } else if (!JackTheStudent.Program.classTypeList.Any(c => c.ShortName == classType)) {
            await ctx.RespondAsync("There's no such class type -_- . Try again!");
            return;
        } else if (eventDayOfWeek == ""){
            await ctx.RespondAsync("There's date missing, fix it!");
            return;
        } else if (!DayOfWeek.TryParse(eventDayOfWeek, out parsedEventDayOfWeek)) {
            await ctx.RespondAsync("That's not a valid date you retard, learn to type!");
            return;
        } else if (eventTime == ""){
            await ctx.RespondAsync("There's time missing, fix it!");
            return;
        } else if (!DateTime.TryParse(eventTime, out parsedEventTime)) {
            await ctx.RespondAsync("That's not a valid time you retard, learn to type!");
            return;
        } else if (link == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !teamsLink <group> <class> <teamsLinkDate> <teamsLinkTime> Try again!");
            return;
        } else if (!link.Contains("teams.microsoft.com/l/meetup-join/19%3ameeting")) {
            await ctx.RespondAsync("That's not a valid teams meetup link!");
            return;
        } else if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == groupId) && groupId != "."){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;      
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var teamsLink = new TeamsLink { 
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    ClassShortName = uniClass,
                    ClassType = classType,
                    Date = $"{parsedEventDayOfWeek} {eventTime}",
                    GroupId = groupId,
                    Link = link,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.teamsLinkList.Add(teamsLink);
                db.TeamsLink.Add(teamsLink);
                await db.SaveChangesAsync();
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Teams link logged successfully");     
        return;
        }   
    }

    [Command("teamslinks")]
    [Description("Command retrieving logged teamsLink based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!teamsLinks <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!teamsLinks - will retrieve all PLANNED teamsLinks for all the groups and all the classes" + 
        "\n!teamsLinks 1 - will retrieve all PLANNED teamsLinks for group 1 for all the classes" +
        "\n!teamsLinks 1 mat - will retrieve all PLANNED teamsLinks for group 1 for Maths class" +
        "\n!teamsLinks 1 mat planned - will retrieve all PLANNED teamsLinks for group 1 for Maths class" +
        "\n!teamsLinks 1 mat . - will retrieve all LOGGED teamsLinks for group 1 for Maths class" +
        "\n!teamsLinks 1 . . - will retrieve all LOGGED teamsLinks for group 1 for ALL classes" + 
        "\n!teamsLinks . . . - will retrieve all LOGGED teamsLinks for ALL groups for ALL classes" +
        "\n!teamsLinks . mat . - will retrieve all LOGGED teamsLinks for ALL groups for MAths class" +
        "\n!teamsLinks . . planned - will retrieve all PLANNED teamsLinks for ALL groups for ALL classes")]
    public async Task TeamsLinkLogs(CommandContext ctx,
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve teamsLink for ALL classes.\n")] string uniClass = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve teamsLink for ALL classes.\n")] string classType = ".",
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve teamsLink for ALL groups.\n")] string group = ".")
    {  
        if (!JackTheStudent.Program.classList.Any(c => c.ShortName == uniClass) && uniClass != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (!JackTheStudent.Program.classTypeList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class type!");
            return;
        } else if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == group) && group != ".") {
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        }

        string chosenUniClass = String.Empty;
        string chosenGroupString = String.Empty;
        string chosenClassType = String.Empty;
        string result = String.Empty;
        var teamsLinks = JackTheStudent.Program.teamsLinkList;

        if (uniClass == "." && classType == "." && group == ".") {
            try {               
                teamsLinks = teamsLinks.ToList();
                if (teamsLinks.Count == 0) {
                        await ctx.RespondAsync("There are no teams links logged!");
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        chosenClassType = teamsLink.GetFullClassType();
                        chosenUniClass = teamsLink.GetFullClassName();

                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";                         
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if(uniClass != "." && classType == "." && group == ".") {
            try {
                chosenUniClass = JackTheStudent.Program.classList.Where(c => c.ShortName == uniClass).Select(c => c.Name).FirstOrDefault();
                teamsLinks = teamsLinks.Where(t => t.Class == uniClass).ToList();
                if (teamsLinks.Count == 0) {
                        await ctx.RespondAsync($"There are no {chosenUniClass} teams links logged.");
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        chosenClassType = teamsLink.GetFullClassType();
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";                                     
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (uniClass != "." && classType != "." && group == ".") {
            try {
                chosenUniClass = JackTheStudent.Program.classList.Where(c => c.ShortName == uniClass).Select(c => c.Name).FirstOrDefault();
                chosenClassType = JackTheStudent.Program.classTypeList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault();
                teamsLinks = teamsLinks.Where(t => t.Class == uniClass && t.ClassType == classType).ToList();
                if (teamsLinks.Count == 0) {
                        await ctx.RespondAsync($"There are no {chosenUniClass} {chosenClassType} teams links logged.");
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";                          
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;
        } else if (uniClass == "." && classType != "." && group == ".") {
            chosenClassType = JackTheStudent.Program.classTypeList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault();
            try {
                teamsLinks = teamsLinks.Where(t => t.ClassType == classType).ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClassType}s teams links logged!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        chosenUniClass = teamsLink.GetFullClassName();
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";          
                    }
                    await ctx.RespondAsync(result);
                    return;
                }                           
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }                   
        } else if (uniClass == "." && classType != "." && group != ".") {
            chosenClassType = JackTheStudent.Program.classTypeList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault();
            chosenGroupString = $"group {group}";
            try {
                teamsLinks = teamsLinks.Where(t => t.ClassType == classType && t.GroupId == group).ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClassType}s teams links logged for group {group}!");
                    return;
                } else {
                    result = String.Empty;
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        chosenUniClass = teamsLink.GetFullClassName();
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";
                    }
                    await ctx.RespondAsync(result);
                    return;
                }                           
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        } else if (uniClass != "." && classType == "." && group != ".") {
            chosenUniClass = JackTheStudent.Program.classList.Where(c => c.ShortName == uniClass).Select(c => c.Name).FirstOrDefault();
            chosenGroupString = $"group {group}";
            try {
                teamsLinks = teamsLinks.Where(t => t.Class == uniClass && t.GroupId == group).ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenUniClass} teams links logged for group {group} logged!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        teamsLink.GetFullClassType();
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";
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
            chosenUniClass = JackTheStudent.Program.classList.Where(c => c.ShortName == uniClass).Select(c => c.Name).FirstOrDefault();
            chosenClassType = JackTheStudent.Program.classTypeList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault();
            chosenGroupString = $"group {group}";
            try {
                teamsLinks = teamsLinks.Where(t => t.Class == uniClass && t.ClassType == classType && t.GroupId == group).ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenUniClass} {chosenClassType}s for group {group} logged!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(chosenUniClass)} {chosenClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date}. Link: {teamsLink.Link}";
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