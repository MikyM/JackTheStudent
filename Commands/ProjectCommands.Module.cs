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
public class ProjectCommandsModule : IModule
{
    
    [Command("project")]
    [Description("Command logging a project, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!project <groupId> <classShortName> <projectDate> <projectTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!project 3 mat 05-05-2021 13:30" + 
        "\n!project 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!project 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!project 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"")]
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
                var project = new Project {Class = classType,
                                                Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                                                GroupId = groupId,
                                                isGroup = Convert.ToBoolean(Convert.ToInt16(isGroup)),
                                                LogById = ctx.Message.Author.Id.ToString(),
                                                LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                                                AdditionalInfo = additionalInfo};
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
            var intr = ctx.Client.GetInteractivityModule(); // Grab the interactivity module
            var response = await intr.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
                TimeSpan.FromSeconds(5) // Wait 60 seconds for a response instead of the default 30 we set earlier!
            );

            short membersCount = 1;    
            while (!Int16.TryParse(response.Message.Content, out membersCount) && membersCount <= 2) {
                await ctx.RespondAsync("Ever heard of intigers? Try again.");
                var interactivity = ctx.Client.GetInteractivityModule(); // Grab the interactivity module
                var response1 = await intr.WaitForMessageAsync(
                    c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
                    TimeSpan.FromSeconds(5) // Wait 60 seconds for a response instead of the default 30 we set earlier!
                );
            }

            try {
                using (var db = new JackTheStudentContext()){
                var project = new Project {Class = classType,
                                                Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                                                GroupId = groupId,
                                                isGroup = Convert.ToBoolean(Convert.ToInt16(isGroup)),
                                                LogById = ctx.Message.Author.Id.ToString(),
                                                LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                                                AdditionalInfo = additionalInfo,
                                                GroupProjectMembers = new List<GroupProjectMember>()};
                for (int i = 1; i <= membersCount; i++) {
                    await ctx.RespondAsync("What's the name of the " + i + " participant?");
                    var interactivity = ctx.Client.GetInteractivityModule(); // Grab the interactivity module
                    var participant = await intr.WaitForMessageAsync(
                        c => c.Author.Id == ctx.Message.Author.Id, // Make sure the response is from the same person who sent the command
                        TimeSpan.FromSeconds(5) // Wait 60 seconds for a response instead of the default 30 we set earlier!
                    );
                    var groupProjectMember = new GroupProjectMember { Member = participant.Message.Content};
                    project.GroupProjectMembers.Add(groupProjectMember);
                }
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
    [Description("Command retrieving logged project based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!projects <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!projects - will retrieve all PLANNED projects for all the groups and all the classes" + 
        "\n!projects 1 - will retrieve all PLANNED projects for group 1 for all the classes" +
        "\n!projects 1 mat - will retrieve all PLANNED projects for group 1 for Maths class" +
        "\n!projects 1 mat planned - will retrieve all PLANNED projects for group 1 for Maths class" +
        "\n!projects 1 mat . - will retrieve all LOGGED projects for group 1 for Maths class" +
        "\n!projects 1 . . - will retrieve all LOGGED projects for group 1 for ALL classes" + 
        "\n!projects . . . - will retrieve all LOGGED projects for ALL groups for ALL classes" +
        "\n!projects . mat . - will retrieve all LOGGED projects for ALL groups for MAths class" +
        "\n!projects . . planned - will retrieve all PLANNED projects for ALL groups for ALL classes")]
    public async Task ProjectLogs(CommandContext ctx, 
        [Description("\nTakes group IDs or \".\", type !group to retrieve all groups, usage of \".\" will tell Jack to retrieve project for ALL groups.\n")] string group = ".",
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve project for ALL classes.\n")] string classType = ".",
        [Description("\nTakes 0 for only individual projects, 1 for group projects or \".\" for all projects\n")] string isGroup = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve all LOGGED project, \"planned\" retrieves only future events.\n")] string span = "planned")
    {      

        bool isParticipants = false;

        if (!JackTheStudent.Program.groupList.Contains(group) && group != ".") {
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType) && classType != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (isGroup != "." && isGroup != "0" && isGroup != "1") {
            await ctx.RespondAsync("... isGroup only takes . , 0 i 1");
            return;
        }
        if (group == "." && classType == "." && span == "planned") {
            try {
                using (var db = new JackTheStudentContext()){
                var projects = db.Project
                            .Where(x => x.Date > DateTime.Now)
                            .ToList();
                    if (projects.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There are literally no projects planned at all!");
                    } else {
                        string result = String.Empty;
                        string participantsString = String.Empty;
                        isParticipants = await ParticipantsQuestion(ctx);

                        foreach (Project project in projects) {                           
                            if (project.isGroup && isParticipants) {
                                participantsString = await project.GetParticipantsString();
                            }
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == project.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " project for group " + project.GroupId + ", deadline is " + project.Date + participantsString;
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
                var projects = db.Project
                    .Where( x => x.GroupId == group)
                    .ToList();
                    if (projects.Count == 0) {
                            await ctx.RespondAsync("There are no projects logged for group " + group + "!");
                    } else {
                        string result = String.Empty;
                        string participantsString = String.Empty;

                        if (isGroup == "1" || isGroup == ".") {
                            isParticipants = await ParticipantsQuestion(ctx);
                        }

                        foreach (Project project in projects) {
                            if (project.isGroup && isParticipants) {
                                participantsString = await project.GetParticipantsString();
                            }
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == project.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " project for group " + project.GroupId + ", deadline is/was " + project.Date + participantsString;
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
                var projects = db.Project
                    .Where(x => x.Date > DateTime.Now && x.GroupId == group)
                    .ToList();
                    if (projects.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There are no projects planned for any class for group " + group + "!");
                    } else {
                        string result = String.Empty;
                        string participantsString = String.Empty;

                        if (isGroup == "1" || isGroup == ".") {
                            isParticipants = await ParticipantsQuestion(ctx);
                        }
            
                        foreach (Project project in projects) {
                            if (project.isGroup && isParticipants) {
                                participantsString = await project.GetParticipantsString();
                            }

                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == project.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " project for group " + project.GroupId + ", deadline is " + project.Date + participantsString;
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
                        var projects = db.Project
                            .Where(x => x.Date > DateTime.Now && x.Class == classType && x.GroupId == group)
                            .ToList();                     

                        if (projects.Count == 0) {
                            string response = "There are no " + JackTheStudent.Program.classList
                                                                .Where( c => c.ShortName == classType)
                                                                .Select( c => c.Name)
                                                                .FirstOrDefault() + " projects planned for group " + group + "!";
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            string participantsString = String.Empty;

                            if (isGroup == "1" || isGroup == ".") {
                                isParticipants = await ParticipantsQuestion(ctx);
                            }

                            foreach (Project project in projects) {
                                if (project.isGroup && isParticipants) {
                                    participantsString = await project.GetParticipantsString();
                                }
                                result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                            .ToTitleCase(JackTheStudent.Program.classList
                                                            .Where( c => c.ShortName == project.Class)
                                                            .Select( c => c.Name)
                                                            .FirstOrDefault()) + " project for group " + project.GroupId + ", deadline is " + project.Date + participantsString;
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
                await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !projects <group> <group> <projectDate> <projectTime> Try again!");
                return;
            }                    
        } else if (classType != "." && span == "." && group != ".") {
            if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                try {
                    using (var db = new JackTheStudentContext()){
                        var projects = db.Project
                            .Where(x => x.Class == classType && x.GroupId == group)
                            .ToList();                     

                        if (projects.Count == 0) {
                            string response = "There is no project logged for " + JackTheStudent.Program.classList
                                                                                    .Where( c => c.ShortName == classType)
                                                                                    .Select( c => c.Name)
                                                                                    .FirstOrDefault() + " class " + "for group " + group + "!";;
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            string participantsString = String.Empty;

                            if (isGroup == "1" || isGroup == ".") {
                                isParticipants = await ParticipantsQuestion(ctx);
                            }

                            foreach (Project project in projects) {
                                if (project.isGroup && isParticipants) {
                                    participantsString = await project.GetParticipantsString();
                                }

                                result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                            .ToTitleCase(JackTheStudent.Program.classList
                                                            .Where( c => c.ShortName == project.Class)
                                                            .Select( c => c.Name)
                                                            .FirstOrDefault()) + " project for group " + project.GroupId + ", will happen / happened on " + project.Date + participantsString;
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
                    var projects = db.Project.ToList();                     

                    if (projects.Count == 0) {
                        string response = "There aren no projects logged!";
                        await ctx.RespondAsync(response);
                        return;
                    } else {
                        string result = String.Empty;
                        string participantsString = String.Empty;

                        if (isGroup == "1" || isGroup == ".") {
                            isParticipants = await ParticipantsQuestion(ctx);
                        }

                        
                        foreach (Project project in projects) {
                            if (project.isGroup && isParticipants) {
                                    participantsString = await project.GetParticipantsString();
                            }
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == project.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " project for group " + project.GroupId + ", will happen / happened " + project.Date + participantsString;
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
    public async Task<bool> ParticipantsQuestion(CommandContext ctx) 
    {
        await ctx.RespondAsync("Would you like to see the participants of each project? Answer with yes or no.");
        var interactivity = ctx.Client.GetInteractivityModule();
        var response = await interactivity.WaitForMessageAsync(
            c => c.Author.Id == ctx.Message.Author.Id, 
            TimeSpan.FromSeconds(5));

        while (response.Message.Content.ToLower() != "yes" && response.Message.Content.ToLower() != "no"){
            await ctx.RespondAsync("Answer with either yes or no you moron...");
            response = await interactivity.WaitForMessageAsync(
                c => c.Author.Id == ctx.Message.Author.Id, 
                TimeSpan.FromSeconds(5));
        }
        
        return ("yes".Equals(response.Message.Content.ToLower()) ? true : false);
    }
}
}