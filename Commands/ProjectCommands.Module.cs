using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using JackTheStudent.CommandDescriptions;
using Serilog;

namespace JackTheStudent.Commands
{
public class ProjectCommandsModule : Base​Command​Module
{
    [Command("project")]
    [Description(ProjectDescriptions.projectLogDescription)]
    public async Task ProjectLog(CommandContext ctx,
        [Description ("\nTakes either 1 (true) or 0 (false)\n")] string IsGroup = "", 
        [Description ("\nTakes group IDs, type !groups to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !classes to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "") 
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();
        if(IsGroup == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !project <IsGroup> <group> <class> <projectDate> <projectTime> Try again!");
            return;
        } else if (IsGroup != "0" && IsGroup != "1"){
            await ctx.RespondAsync("How stupid are you, really? IsGroup argument takes either 1 (true) or 0 (false)!");
            return;
        } else if(groupId == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !project <IsGroup> <group> <class> <projectDate> <projectTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !project <IsGroup> <group> <class> <projectDate> <projectTime> Try again!");
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
        } else if(JackTheStudent.Program.projectList
            .Any(p => 
                p.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && 
                p.Class == JackTheStudent.Program.classList
                    .Where(c => c.ShortName == classType)
                    .Select(c => c.Name)
                    .FirstOrDefault() && 
                p.GroupId == groupId)) {
            await ctx.RespondAsync("Someone has already logged this project.");
            return;
        } else if(JackTheStudent.Program.projectList.Any(p => p.Date == parsedEventDate.Date.Add(parsedEventTime.TimeOfDay) && p.GroupId == groupId)) {
            await ctx.RespondAsync("There's a project logged for this group that takes place same time.");
            return;
        } else if (IsGroup == "0") {
            try {
                using (var db = new JackTheStudentContext()){
                var project = new Project {
                    Class = JackTheStudent.Program.classList.Where(e => e.ShortName == classType).Select(e => e.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    IsGroup = Convert.ToBoolean(Convert.ToInt16(IsGroup)),
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo,
                    GroupProjectMembers = null                   
                };
                JackTheStudent.Program.projectList.Add(project);
                db.Project.Add(project);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {project.Id}");
                }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Project logged successfully");     
        return;
        } else {
            await ctx.RespondAsync("How many members does the project have?");
            var intr = ctx.Client.GetInteractivity(); 
            var response = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(10) 
            );

            if(response.TimedOut) {
                await ctx.RespondAsync("How long you think I'm gonna wait for you to answer? I'm out.");
                return;
            }

            short membersCount = 1;    
            while (!Int16.TryParse(response.Result.Content, out membersCount) && membersCount <= 2) {
                await ctx.RespondAsync("Ever heard of intigers? Try again.");
                response = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, 
                    TimeSpan.FromSeconds(5) 
                );
            }

            if(response.TimedOut) {
                await ctx.RespondAsync("How long you think I'm gonna wait for you to answer? I'm out.");
                return;
            }

            try {
                using (var db = new JackTheStudentContext()){
                var project = new Project {
                    Class = JackTheStudent.Program.classList.Where(e => e.ShortName == classType).Select(e => e.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    IsGroup = Convert.ToBoolean(Convert.ToInt16(IsGroup)),
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo,
                    GroupProjectMembers = new List<GroupProjectMember>()
                };
                for (int i = 1; i <= membersCount; i++) {
                    await ctx.RespondAsync("What's the name of the " + i + " participant?");
                    var participant = await intr.WaitForMessageAsync(
                        c => c.Author.Id == ctx.Message.Author.Id, 
                        TimeSpan.FromSeconds(15) 
                    );

                    if(participant.TimedOut) {
                        await ctx.RespondAsync("How long you think I'm gonna wait for you to answer? I'm out.");
                        return;
                    }
                    var groupProjectMember = new GroupProjectMember { Member = participant.Result.Content};
                    project.GroupProjectMembers.Add(groupProjectMember);
                }
                JackTheStudent.Program.projectList.Add(project);
                db.Project.Add(project);
                await db.SaveChangesAsync();
                Log.Logger.Information($"[Jack] User {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id} created a new log with '{ctx.Command.QualifiedName}' command and created ID: {project.Id}");  
            }
            } catch(Exception ex) {
                Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Project logged successfully");   
        return;        
        }  
    }

    [Command("projects")]
    [Description(ProjectDescriptions.projectLogsDescription)]
    public async Task ProjectLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !groups to retrieve all groups, usage of \".\" will tell Jack to retrieve project for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !classes to retrieve all classes, usage of \".\" will tell Jack to retrieve project for ALL classes.\n")] string classType = ".",
        [Description("\nTakes 0 for only individual projects, 1 for group projects or \".\" for all projects\n")] string isGroup = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED project, \"planned\" retrieves only future events.\n")] string span = "planned")
    {      
        if (!JackTheStudent.Program.groupList.Any(g => g.GroupId == group) && group != ".") {
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (isGroup != "." && isGroup != "0" && isGroup != "1") {
            await ctx.RespondAsync("... IsGroup only takes . , 0 i 1");
            return;
        } else if (span != "." && span != "planned") {
            await ctx.RespondAsync("Span only accepts '.' and 'planned' values");
            return;
        }

        var projects = JackTheStudent.Program.projectList;
        string chosenClass = JackTheStudent.Program.classList
            .Where(c => c.ShortName == classType)
            .Select(c => c.Name)
            .FirstOrDefault();
        bool isParticipants = false;
        string result = String.Empty;
        string participantsString = String.Empty;
        try {
            if (group == "." && classType == "." && span == "planned") {
                projects = projects
                    .Where(p => 
                        p.Date > DateTime.Now)
                    .ToList();
                if (projects.Count == 0) {
                        await ctx.RespondAsync("Wait what!? There are literally no projects planned at all!");
                        return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        }                      
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        } 
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, deadline is {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;
                    }
                }
            } else if(classType == "." && span == "." && group != "." ) {
                projects = projects
                    .Where(p => 
                        p.GroupId == group)
                    .ToList();
                if (projects.Count == 0) {
                        await ctx.RespondAsync($"There are no projects logged for group {group}!");
                        return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        }
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, deadline is/was {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;                    
                    }
                }
            } else if (classType == "." && span == "planned" && group != ".") {
                projects = projects
                    .Where(p => 
                        p.Date > DateTime.Now && 
                        p.GroupId == group)
                    .ToList();
                if (projects.Count == 0) {
                        await ctx.RespondAsync($"Wait what!? There are no projects planned for any class for group {group}!");
                        return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }     
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        } 
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, deadline is {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;
                    }
                }
            } else if (classType != "." && span == "planned" && group != ".") {
                projects = projects
                    .Where(p => 
                        p.Date > DateTime.Now && 
                        p.Class == chosenClass && 
                        p.GroupId == group)
                    .ToList();                     
                if (projects.Count == 0) {
                    await ctx.RespondAsync($"There are no {chosenClass} projects planned for group {group}!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        }  
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, deadline is {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;
                    }
                }                                          
            } else if (classType != "." && span == "." && group != ".") {
                projects = projects
                    .Where(p => 
                        p.Class == chosenClass && 
                        p.GroupId == group)
                    .ToList();                     
                if (projects.Count == 0) {
                    await ctx.RespondAsync($"There are no projects logged for {chosenClass} class for group {group}!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        } 
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, will happen / happened on {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;
                    }
                } 
            } else if (classType != "." && span == "planned" && group == ".") {
                projects = projects
                    .Where(p => 
                        p.Class == chosenClass && 
                        p.Date > DateTime.Now)
                    .ToList();                     
                if (projects.Count == 0) {
                    await ctx.RespondAsync($"There are no projects logged for {chosenClass} class for group {group}!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        }
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, will happen / happened on {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;
                    }
                }                                          
            } else {                   
                if (projects.Count == 0) {
                    await ctx.RespondAsync("There are no projects logged!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }     
                    foreach (Project project in projects) {
                        if (isGroup == "0" && project.IsGroup) {
                            continue;
                        } else if (isGroup == "1" && !project.IsGroup) {
                            continue;
                        }
                        if (project.IsGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} {(project.IsGroup ? "group project" : "project")} for group {project.GroupId}, will happen / happened on {project.Date.ToString().Trim()}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}.")}{participantsString}";
                        participantsString = String.Empty;
                    }
                }                       
            }
        } catch(Exception ex) {
            Log.Logger.Error($"[Jack] Command {ctx.Command.QualifiedName} was called by user {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator} ID:{ctx.Message.Author.Id}, but it errored: " + ex.ToString());
            await ctx.RespondAsync("Show logs failed");
            return;
        }
        var emoji = DiscordEmoji.FromName(ctx.Client, ":mortar_board:");
        var embed = new DiscordEmbedBuilder {
            Title = $"{emoji} Logged projects:",
            Description = result,
            Color = new DiscordColor(0x8c770a) 
        };
        await ctx.RespondAsync("", embed: embed);       
    }
    public async Task<bool> ParticipantsQuestion(CommandContext ctx) 
    {
        await ctx.RespondAsync("Would you like to see the participants of each project? Answer with yes or no.");
        var interactivity = ctx.Client.GetInteractivity();
        var response = await interactivity.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(7));

        if(response.TimedOut) {
            await ctx.RespondAsync("I ain't gonna wait this long. Not listing participants.");
            return false;
        }

        while (response.Result.Content.ToLower() != "yes" && response.Result.Content.ToLower() != "no"){
            await ctx.RespondAsync("Answer with either yes or no you moron...");
            response = await interactivity.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(7));
            
            if(response.TimedOut) {
                await ctx.RespondAsync("I ain't gonna wait this long. Not listing participants.");
                return false;
            }
        }
        
        return ("yes".Equals(response.Result.Content.ToLower()) ? true : false);
    }

    public async Task <string> GetParticipantsString(List<GroupProjectMember> participants)
    {
        string participantsString = String.Empty;
        foreach (GroupProjectMember participant in participants) {
            participantsString = participantsString + participant.Member + ", ";
        }
        return $"\nMembers: {participantsString.Substring(0, participantsString.Length-2)}";
    }
}
}