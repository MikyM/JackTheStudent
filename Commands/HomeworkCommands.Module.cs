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
/* Create our class and extend from IModule */
namespace JackTheStudent.Commands
{
public class HomeworkCommandsModule : IModule
{
    //private List<string> classList = new List<string>() {"eng", "mat", "ele", "db", "prog"};
    [Command("homework")]
    [Description("Command logging an event of specified type")]
    public async Task HomeworkLog(CommandContext ctx, string groupId = "", string classType = "", string eventDate = "", string eventTime = "", string additionalInfo = "", string materials = "")
    {
        
        //List<string> classList = GetClassNames();

        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();


        if(groupId == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !homework <class> <group> <eventDate> <eventTime> Try again!");
            return;
        } else if (!JackTheStudent.Program.groupList.Contains(groupId)){
            await ctx.RespondAsync("There's no such group dumbass. Try again!");
            return;
        } else if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !homework <class> <group> <eventDate> <eventTime> Try again!");
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
                                              LogByUsername = ctx.Message.Author.Username,
                                              AdditionalInfo = additionalInfo,
                                              Materials = materials};
                db.Homework.Add(homeWork);
                await db.SaveChangesAsync();
                }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Homework log failed");
                    return;
                }
        await ctx.RespondAsync("Homework logged successfully");     
        return;
        }   
    }

    [Command("homeworks")]
    [Description("Command retrieving all logged events of specified type")]
    public async Task HomeworkLogs(CommandContext ctx, string group = ".", string classType = ".", string span = "planned")
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
                            foreach (Homework homework in homeworks) {
                                await ctx.RespondAsync(CultureInfo.CurrentCulture.TextInfo
                                    .ToTitleCase(JackTheStudent.Program.classList
                                    .Where( c => c.ShortName == homework.Class)
                                    .Select( c => c.Name)
                                    .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is " + homework.Date);
                            }
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
                                await ctx.RespondAsync("Wait what!? There is no homework for group " + group + "!");
                        } else {
                            foreach (Homework homework in homeworks) {
                                await ctx.RespondAsync(CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " " + homework.Date);
                            }
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
                    var homeworks = db.Homework
                        .Where(x => x.Date > DateTime.Now && x.GroupId == group)
                        .ToList();
                        if (homeworks.Count == 0) {
                                await ctx.RespondAsync("Wait what!? There is no homework, hmm... league?");
                        } else {
                            foreach (Homework homework in homeworks) {
                                await ctx.RespondAsync(CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is " + homework.Date);
                            }
                        }
                    }
                } catch(Exception ex) {
                    Console.Error.WriteLine("[Jack] " + ex.ToString());
                    await ctx.RespondAsync("Show logs failed");
                    return;
                }
            return;
            } else if (classType != "." && span == "planned") {

                if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                    try {
                        using (var db = new JackTheStudentContext()){
                            var homeworks = db.Homework
                                .Where(x => x.Date > DateTime.Now && x.Class == classType && x.GroupId == group)
                                .ToList();                     

                            if (homeworks.Count == 0) {
                                string response = "There is no " + CultureInfo.CurrentCulture.TextInfo
                                                                    .ToTitleCase(JackTheStudent.Program.classList
                                                                    .Where( c => c.ShortName == classType)
                                                                    .Select( c => c.Name)
                                                                    .FirstOrDefault()) + " homework planned!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Homework homework in homeworks) {
                                    await ctx.RespondAsync(CultureInfo.CurrentCulture.TextInfo
                                                            .ToTitleCase(JackTheStudent.Program.classList
                                                            .Where( c => c.ShortName == homework.Class)
                                                            .Select( c => c.Name)
                                                            .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is " + homework.Date);
                                }
                            return;
                            }                           
                        }
                        } catch(Exception ex) {
                            Console.Error.WriteLine("[Jack] " + ex.ToString());
                            await ctx.RespondAsync("Show logs failed");
                            return;
                        }
                } else {
                    await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !homeworks <class> <group> <eventDate> <eventTime> Try again!");
                    return;
                }                    
            } else if (classType != "." && span == ".") {
                if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                    try {
                        using (var db = new JackTheStudentContext()){
                            var homeworks = db.Homework
                                .Where(x => x.Class == classType && x.GroupId == group)
                                .ToList();                     

                            if (homeworks.Count == 0) {
                                string response = "There is no homework logged for " + CultureInfo.CurrentCulture.TextInfo
                                                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                                                        .Where( c => c.ShortName == classType)
                                                                                        .Select( c => c.Name)
                                                                                        .FirstOrDefault()) + "class!";
                                await ctx.RespondAsync(response);
                                return;
                            } else {
                                foreach (Homework homework in homeworks) {
                                    await ctx.RespondAsync(homework.Class + " homework for group " + homework.GroupId + ", deadline is " + homework.Date);
                                }
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
                        var homeworks = db.Homework
                            .ToList();                     

                        if (homeworks.Count == 0) {
                            string response = "There is no logged homework!";
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            foreach (Homework homework in homeworks) {
                                await ctx.RespondAsync(CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == homework.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " homework for group " + homework.GroupId + ", deadline is/was " + homework.Date);
                            }
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
