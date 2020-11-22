using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using Serilog;
using System.Text;
using System.Globalization;
using JackTheStudent.CommandDescriptions;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace JackTheStudent.Commands
{

public class TeamsLinksCommandsModule : Base​Command​Module
{
    private string groupGuid = Environment.GetEnvironmentVariable("BITLY_GROUPGUID");
    private string bitlyToken = Environment.GetEnvironmentVariable("BITLY_TOKEN");

    [Command("teamslink")]
    [Description(TeamsDescriptions.teamslinkLogDescription)]
    public async Task TeamsLinkLog(CommandContext ctx,
        [Description ("\nTakes class' short names, type !classes to retrive all classes.\n")] string uniClass = "",
        [Description ("\nTakes class' type short names, type !classtypes to retrive all classes.\n")] string classType = "",   
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDayOfWeek = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "",
        [Description ("\nTakes group IDs OR \".\" which says that the link is for everyone, type !groups to retrieve all groups.\n")] string groupId = ".", 
        [Description ("\nTakes time in hh:mm format.\n")] string link = "",
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        DayOfWeek parsedEventDayOfWeek = new DayOfWeek();
        DateTime parsedEventTime = new DateTime();

        if (uniClass == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !teamsLink <classShortName> <classType> <dayOfWeek> <time> <group> <link> <addInfo> Try again!");
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
        } else if(JackTheStudent.Program.teamsLinkList
            .Any(t => 
                t.Date == $"{parsedEventDayOfWeek} {eventTime}" && 
                t.Class == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == uniClass)
                    .Select(c => c.Name)
                    .FirstOrDefault() &&
                t.GroupId == groupId && 
                t.ClassType == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                    .FirstOrDefault() && 
                t.Link == link)) {
            await ctx.RespondAsync("Someone has already logged this link.");
            return;   
        } else if(JackTheStudent.Program.teamsLinkList.Any(t => t.Date == $"{parsedEventDayOfWeek} {eventTime}" && t.GroupId == groupId)) {
            await ctx.RespondAsync("There's a class logged for this group/everyone that takes place same time.");
            return;         
        } else {
            try {
                string shortenedUrl = await ShortenUrl(link, ctx);
                
                using (var db = new JackTheStudentContext()){
                var teamsLink = new TeamsLink { 
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == uniClass).Select(c => c.Name).FirstOrDefault(),
                    ClassType = JackTheStudent.Program.classTypeList.Where(c => c.ShortName == classType).Select(c => c.Name).FirstOrDefault(),
                    Date = $"{parsedEventDayOfWeek} {eventTime}",
                    GroupId = groupId,
                    ShortenedLink = shortenedUrl,
                    Link = link,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.teamsLinkList.Add(teamsLink);
                db.TeamsLink.Add(teamsLink);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {teamsLink.Id}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Teams link logged successfully");     
        return;
        }   
    }

    [Command("teamslinks")]
    [Description(TeamsDescriptions.teamslinkLogsDescription)]
    public async Task TeamsLinkLogs(CommandContext ctx,
        [Description("\nTakes class' short names or \".\", type !classes to retrieve all classes, usage of \".\" will tell Jack to retrieve teams links for ALL classes.\n")] string uniClass = ".",
        [Description("\nTakes class' short names or \".\", type !classtypes to retrieve all class types, usage of \".\" will tell Jack to retrieve teams links for ALL class types.\n")] string classType = ".",
        [Description("\nTakes group IDs or \".\", type !groups to retrieve all groups, usage of \".\" will tell Jack to retrieve teams links for ALL groups.\n")] string group = ".")
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

        string chosenUniClass = JackTheStudent.Program.classList
                    .Where(c => c.ShortName == uniClass)
                    .Select(c => c.Name)
                .FirstOrDefault();
        string chosenClassType = JackTheStudent.Program.classTypeList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                .FirstOrDefault();
        string chosenGroupString = String.Empty;
        string result = String.Empty;
        var teamsLinks = JackTheStudent.Program.teamsLinkList;

        try {
            if (uniClass == "." && classType == "." && group == ".") {              
                teamsLinks = teamsLinks.ToList();
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync("There are no teams links logged!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";                       
                    }
                }
            } else if(uniClass != "." && classType == "." && group == ".") {
                teamsLinks = teamsLinks
                    .Where(t => 
                        t.Class == chosenUniClass)
                    .ToList();
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenUniClass} teams links logged.");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";                                   
                    }
                }
            } else if (uniClass != "." && classType != "." && group == ".") {
                teamsLinks = teamsLinks
                    .Where(t => 
                        t.Class == chosenUniClass &&
                        t.ClassType == chosenClassType)
                    .ToList();
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenUniClass} {chosenClassType} teams links logged.");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";                         
                    }
                }
            } else if (uniClass == "." && classType != "." && group == ".") {
                teamsLinks = teamsLinks
                    .Where(t => t.ClassType == chosenClassType)
                    .ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClassType} teams links logged!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        if (teamsLink.GroupId == ".") {
                            chosenGroupString = "everyone";
                        } else {
                            chosenGroupString = $"group {teamsLink.GroupId}";
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";
                    }
                }                                            
            } else if (uniClass == "." && classType != "." && group != ".") {
                chosenGroupString = $"group {group}";
                teamsLinks = teamsLinks
                    .Where(t => 
                        t.ClassType == chosenClassType && 
                        t.GroupId == group)
                    .ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClassType} teams links logged for group {group}!");
                    return;
                } else {
                    result = String.Empty;
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";
                    }
                }                           
            } else if (uniClass != "." && classType == "." && group != ".") {
                chosenGroupString = $"group {group}";
                teamsLinks = teamsLinks
                    .Where(t => 
                        t.Class == chosenUniClass &&
                        t.GroupId == group)
                    .ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenUniClass} teams links logged for group {group}!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";
                    }
                }
            } else if (uniClass == "." && classType == "." && group != ".") {
                chosenGroupString = $"group {group}";
                teamsLinks = teamsLinks
                    .Where(t => t.GroupId == group)
                    .ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no teams links logged for group {group}!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";
                    }
                }                                     
            } else {
                chosenGroupString = $"group {group}";
                teamsLinks = teamsLinks
                    .Where(t => 
                        t.Class == chosenUniClass && 
                        t.ClassType == chosenClassType && 
                        t.GroupId == group)
                    .ToList();                     
                if (teamsLinks.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenUniClass} {chosenClassType}s for group {group} logged!");
                    return;
                } else {
                    foreach (TeamsLink teamsLink in teamsLinks) {
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(teamsLink.Class)} {teamsLink.ClassType} teams link for {chosenGroupString}, takes place on {teamsLink.Date.ToString().Trim()}. Link: {teamsLink.ShortenedLink}{(teamsLink.AdditionalInfo.Equals("") ? "" : $"\nAdditional info: {teamsLink.AdditionalInfo}.")}";
                    }
                }                          
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":microphone:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Found links:",
            Description = result,
            Color = new DiscordColor(0x1e0f91) 
        };
        await ctx.RespondAsync("", embed: embed);       
    }

    [Command("changelink")]
    [Description(TeamsDescriptions.teamslinkLogDescription)]
    public async Task ChangeTeamsLink(CommandContext ctx,
        [Description ("\nTakes class' short names, type !classes to retrive all classes.\n")] string oldShortLink = "",
        [Description ("\nTakes class' type short names, type !classtypes to retrive all classes.\n")] string newLink = "")
    {
        if (oldShortLink == "") {
            await ctx.RespondAsync("Learn to read...");
            return;      
        } else if (newLink == "") {
            await ctx.RespondAsync("Learn to read...");
            return;
        } else if (!newLink.Contains("teams.microsoft.com/l/meetup-join/19%3ameeting")) {
            await ctx.RespondAsync("That's not a valid teams meetup link!");
            return;
        } else if(!JackTheStudent.Program.teamsLinkList.Any(t => t.ShortenedLink == oldShortLink)) {  
            await ctx.RespondAsync("Such link hasn't been logged.");
            return;      
        } else {
            try {
                string shortenedUrl = await ShortenUrl(newLink, ctx);

                using (var db = new JackTheStudentContext()){
                    var teamsLink = db.TeamsLink.ToList().Where(t => t.ShortenedLink == oldShortLink).FirstOrDefault();
                    teamsLink.ShortenedLink = shortenedUrl;
                    teamsLink.Link = newLink;
                    await db.SaveChangesAsync();

                    var teamsLinkFromList = JackTheStudent.Program.teamsLinkList.Where(t => t.ShortenedLink == oldShortLink).FirstOrDefault();
                    teamsLinkFromList.Link = newLink;
                    teamsLinkFromList.ShortenedLink = shortenedUrl;
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} changed the link of ID: {teamsLink.Id} with '{ctx.Command.QualifiedName}' command");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Teams link changed successfully");     
        return;
        }   
    }

    public async Task<string> ShortenUrl(string newLink, CommandContext ctx) 
    {
        string shortenedUrl = "";
        try {
            string baseUrl = "https://api-ssl.bitly.com/v4/shorten";
            var content = new BitlyCall{
                long_url = newLink,
                group_guid = groupGuid
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bitlyToken);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, baseUrl);
                request.Content = jsonContent;
                var response = await client.SendAsync(request);
                JObject rss = JObject.Parse(await response.Content.ReadAsStringAsync());                           
                shortenedUrl = (string)rss["link"];
            }
        } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return String.Empty;
        }
        return shortenedUrl;
    }
}
}