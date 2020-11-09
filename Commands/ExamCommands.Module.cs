using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace JackTheStudent.Commands
{
public class ExamCommandsModule : IModule
{
    
    [Command("exam")]
    [Description("Command logging a exam, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!exam <classShortName> <exameDate> <examTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!exam mat 05-05-2021 13:30" + 
        "\n!exam ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!exam mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!exam eng 05-05-2021 13:30 . \"https://yourmaterials.com\"")]
    public async Task ExamLog(CommandContext ctx,
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string classType = "", 
        [Description ("\nTakes dates in dd/mm/yyyy format, accepts different separators.\n")] string examDate = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string examTime = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "", 
        [Description ("\nTakes material links, multiple links must be wrapped with \"\".\n")] string materials = "")
    {
        DateTime parsedEventDate = new DateTime();
        DateTime parsedEventTime = new DateTime();

         if (classType == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !exam <class> <examDate> <examTime> Try again!");
            return;      
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (examDate == ""){
            await ctx.RespondAsync("There's date missing, fix it!");
            return;
        } else if (!DateTime.TryParse(examDate, out parsedEventDate)) {
            await ctx.RespondAsync("That's not a valid date you retard, learn to type!");
            return;
        } else if (examTime == ""){
            await ctx.RespondAsync("There's time missing, fix it!");
            return;
        } else if (!DateTime.TryParse(examTime, out parsedEventTime)) {
            await ctx.RespondAsync("That's not a valid time you retard, learn to type!");
            return;
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var exam = new Exam {Class = classType,
                                              Date = parsedEventDate.Date.Add(parsedEventTime.TimeOfDay),
                                              LogById = ctx.Message.Author.Id.ToString(),
                                              LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                                              AdditionalInfo = additionalInfo,
                                              Materials = materials};
                db.Exam.Add(exam);
                await db.SaveChangesAsync();
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Exam log failed");
                return;
            }
        await ctx.RespondAsync("Exam logged successfully");     
        return;
        }   
    }

    [Command("exams")]
    [Description("Command retrieving logged exam based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!exams <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!exams - will retrieve all PLANNED exam for all the classes" + 
        "\n!exams mat - will retrieve all PLANNED exams for Maths class" +
        "\n!exams mat planned - will retrieve all PLANNED exams for Maths class" +
        "\n!exams mat . - will retrieve all LOGGGED exams for Maths class" +
        "\n!exams . . - will retrieve all LOGGGED exams for ALL classes" + 
        "\n!exams . planned - will retrieve all PLANNED exams for ALL classes")]
    public async Task ExamLogs(CommandContext ctx, 
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve exam for ALL classes.\n")] string classType = ".",
        [Description("\nTakes \".\" or \"planned\", usage of \".\" will tell Jack to retrieve ALL logged exams, \"planned\" retrieves only future events.\n")] string span = "planned")
    {       
        if (classType == "." && span == "planned") {
            try {
                using (var db = new JackTheStudentContext()){
                var exams = db.Exam
                            .Where( x => x.Date > DateTime.Now)
                            .ToList();
                    if (exams.Count == 0) {
                            await ctx.RespondAsync("Wait what!? There are no exams planned, PAAAARTTTIEEEHH TIIIIIIIIMEEEEEEE!");
                    } else {
                        string result = String.Empty;
                        foreach (Exam exam in exams) {
                                result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                .ToTitleCase(JackTheStudent.Program.classList
                                .Where( c => c.ShortName == exam.Class)
                                .Select( c => c.Name)
                                .FirstOrDefault()) + " exam will happen on " + exam.Date;
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
        } else if(classType == "." && span == ".") {
            try {
                using (var db = new JackTheStudentContext()){
                var exams = db.Exam
                    .ToList();
                    if (exams.Count == 0) {
                            await ctx.RespondAsync("There are no exams logged!");
                    } else {
                        string result = String.Empty;
                        foreach (Exam exam in exams) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                    .ToTitleCase(JackTheStudent.Program.classList
                                                    .Where( c => c.ShortName == exam.Class)
                                                    .Select( c => c.Name)
                                                    .FirstOrDefault()) + " exam will happen / happened on " + exam.Date;
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
        } else if (classType != "." && span == "planned") {

            if(JackTheStudent.Program.classList.Any(c => c.ShortName == classType)) {
                try {
                    using (var db = new JackTheStudentContext()){
                        var exams = db.Exam
                            .Where(x => x.Date > DateTime.Now && x.Class == classType)
                            .ToList();                     

                        if (exams.Count == 0) {
                            string response = "There are no " + JackTheStudent.Program.classList
                                                                .Where( c => c.ShortName == classType)
                                                                .Select( c => c.Name)
                                                                .FirstOrDefault() + " exams planned!";
                            await ctx.RespondAsync(response);
                            return;
                        } else {
                            string result = String.Empty;
                            foreach (Exam exam in exams) {
                                result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                        .ToTitleCase(JackTheStudent.Program.classList
                                                        .Where( c => c.ShortName == exam.Class)
                                                        .Select( c => c.Name)
                                                        .FirstOrDefault()) + " exam will happen on " + exam.Date;
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
                await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !exams <class> <group> <examDate> <examTime> Try again!");
                return;
            }                                       
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                    var exams = db.Exam
                        .Where ( c => c.Class == classType)
                        .ToList();                  
                    if (exams.Count == 0) {
                        string response = "There are no logged exams for " + JackTheStudent.Program.classList
                                                                                .Where( c => c.ShortName == classType)
                                                                                .Select( c => c.Name)
                                                                                .FirstOrDefault() + "class!";
                        await ctx.RespondAsync(response);
                        return;
                    } else {
                        string result = String.Empty;
                        foreach (Exam exam in exams) {
                            result = result + "\n" + CultureInfo.CurrentCulture.TextInfo
                                                    .ToTitleCase(JackTheStudent.Program.classList
                                                    .Where( c => c.ShortName == exam.Class)
                                                    .Select( c => c.Name)
                                                    .FirstOrDefault()) + " exam will happen / happened on "+ exam.Date;
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
