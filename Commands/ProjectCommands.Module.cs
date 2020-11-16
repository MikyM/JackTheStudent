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
using System.IO;
using DSharpPlus.Interactivity.Extensions;
using JackTheStudent.CommandDescriptions;

/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class ProjectCommandsModule : Base​Command​Module
{
    [Command("project")]
    [Description(ProjectDescriptions.projectLogDescription)]
    public async Task ProjectLog(CommandContext ctx,
        [Description ("\nTakes either 1 (true) or 0 (false)\n")] string isGroup = "", 
        [Description ("\nTakes group IDs, type !group to retrieve all groups.\n")] string groupId = "", 
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string eventDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string eventTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "") 
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();
        if(isGroup == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !project <isGroup> <group> <class> <projectDate> <projectTime> Try again!");
            return;
        } else if (isGroup != "0" && isGroup != "1"){
            await ctx.RespondAsync("How stupid are you, really? isGroup argument takes either 1 (true) or 0 (false)!");
            return;
        } else if(groupId == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !project <isGroup> <group> <class> <projectDate> <projectTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Contains(groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !project <isGroup> <group> <class> <projectDate> <projectTime> Try again!");
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
        } else if (isGroup == "0") {
            try {
                using (var db = new JackTheStudentContext()){
                var project = new Project {
                    ClassShortName = classType,
                    Class = JackTheStudent.Program.classList.Where(e => e.ShortName == classType).Select(e => e.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    isGroup = Convert.ToBoolean(Convert.ToInt16(isGroup)),
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.projectList.Add(project);
                db.Project.Add(project);
                await db.SaveChangesAsync();
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
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
                TimeSpan.FromSeconds(5) 
            );

            short membersCount = 1;    
            while (!Int16.TryParse(response.Result.Content, out membersCount) && membersCount <= 2) {
                await ctx.RespondAsync("Ever heard of intigers? Try again.");
                response = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, 
                    TimeSpan.FromSeconds(5) 
                );
            }

            try {
                using (var db = new JackTheStudentContext()){
                var project = new Project {
                    ClassShortName = classType,
                    Class = JackTheStudent.Program.classList.Where(e => e.ShortName == classType).Select(e => e.Name).FirstOrDefault(),
                    Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                    GroupId = groupId,
                    isGroup = Convert.ToBoolean(Convert.ToInt16(isGroup)),
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo,
                    GroupProjectMembers = new List<GroupProjectMember>()
                };
                for (int i = 1; i <= membersCount; i++) {
                    await ctx.RespondAsync("What's the name of the " + i + " participant?");
                    var participant = await intr.WaitForMessageAsync(
                        c => c.Author.Id == ctx.Message.Author.Id, 
                        TimeSpan.FromSeconds(5) 
                    );
                    var groupProjectMember = new GroupProjectMember { Member = participant.Result.Content};
                    project.GroupProjectMembers.Add(groupProjectMember);
                }
                JackTheStudent.Program.projectList.Add(project);
                db.Project.Add(project);
                await db.SaveChangesAsync();
            }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
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
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve project for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve project for ALL classes.\n")] string classType = ".",
        [Description("\nTakes 0 for only individual projects, 1 for group projects or \".\" for all projects\n")] string isGroup = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED project, \"planned\" retrieves only future events.\n")] string span = "planned")
    {      
        if (!JackTheStudent.Program.groupList.Contains(group) && group != ".") {
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (isGroup != "." && isGroup != "0" && isGroup != "1") {
            await ctx.RespondAsync("... isGroup only takes . , 0 i 1");
            return;
        } else if (span != "." && span != "planned") {
            await ctx.RespondAsync("Span only accepts . and planned values");
            return;
        }

        var projects = JackTheStudent.Program.projectList;
        bool isParticipants = false;
        string result = String.Empty;
        string participantsString = String.Empty;

        if (group == "." && classType == "." && span == "planned") {
            try {
                projects = projects.Where(p => p.Date > DateTime.Now).ToList();
                    if (projects.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There are literally no projects planned at all!");
                    } else {
                        isParticipants = await ParticipantsQuestion(ctx);
                        foreach (Project project in projects) {                           
                            if (project.isGroup && isParticipants) {
                                participantsString = await GetParticipantsString(await project.GetParticipants());
                            }
                            result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} project for group {project.GroupId}, deadline is {project.Date}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}")}.{participantsString}";
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
                projects = projects.Where(p => p.GroupId == group)
                    .ToList();
                    if (projects.Count == 0) {
                            await ctx.RespondAsync($"There are no projects logged for group {group}!");
                    } else {
                        if (isGroup == "1" || isGroup == ".") {
                            isParticipants = await ParticipantsQuestion(ctx);
                        }
                        foreach (Project project in projects) {
                            if (project.isGroup && isParticipants) {
                                participantsString = await GetParticipantsString(await project.GetParticipants());
                            }
                            result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} project for group {project.GroupId}, deadline is/was {project.Date}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}")}.{participantsString}";
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
                projects = projects.Where(p => p.Date > DateTime.Now && p.GroupId == group).ToList();
                if (projects.Count == 0) {
                        await ctx.RespondAsync($"Wait what!? There are no projects planned for any class for group {group}!");
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }     
                    foreach (Project project in projects) {
                        if (project.isGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }

                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} project for group {project.GroupId}, deadline is {project.Date}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}")}.{participantsString}";
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
                projects = projects.Where(p => p.Date > DateTime.Now && p.Class == classType && p.GroupId == group).ToList();                     
                if (projects.Count == 0) {
                    await ctx.RespondAsync($"There are no {projects.Select(p => p.Class).FirstOrDefault()} projects planned for group {group}!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (project.isGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} project for group {project.GroupId}, deadline is {project.Date}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}")}.{participantsString}";
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
                projects = projects.Where(p => p.Class == classType && p.GroupId == group).ToList();                     
                if (projects.Count == 0) {
                    await ctx.RespondAsync($"There are no projects logged for {projects.Select(p => p.Class).FirstOrDefault()} class for group {group}!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }
                    foreach (Project project in projects) {
                        if (project.isGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} project for group {project.GroupId}, will happen / happened on {project.Date}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}")}.{participantsString}";
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
                projects = projects.ToList();                     
                if (projects.Count == 0) {
                    await ctx.RespondAsync("There are no projects logged!");
                    return;
                } else {
                    if (isGroup == "1" || isGroup == ".") {
                        isParticipants = await ParticipantsQuestion(ctx);
                    }     
                    foreach (Project project in projects) {
                        if (project.isGroup && isParticipants) {
                            participantsString = await GetParticipantsString(await project.GetParticipants());
                        }
                        result = $"{result} \n{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(project.Class)} project for group {project.GroupId}, will happen / happened on {project.Date}.{(project.AdditionalInfo.Equals("") ? "" : $"Additional info: {project.AdditionalInfo}")}.{participantsString}";
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
    public async Task<bool> ParticipantsQuestion(CommandContext ctx) 
    {
        await ctx.RespondAsync("Would you like to see the participants of each project? Answer with yes or no.");
        var interactivity = ctx.Client.GetInteractivity();
        var response = await interactivity.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(5));

        while (response.Result.Content.ToLower() != "yes" && response.Result.Content.ToLower() != "no"){
            await ctx.RespondAsync("Answer with either yes or no you moron...");
            response = await interactivity.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(5));
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