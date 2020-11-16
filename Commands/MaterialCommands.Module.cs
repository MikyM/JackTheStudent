using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;
using JackTheStudent.Models;
using System.Linq;
using System.Globalization;

namespace JackTheStudent.Commands
{
public class MaterialCommandsModule : Base​Command​Module
{
    
    [Command("material")]
    [Description("Command logging a test, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!test <groupId> <classShortName> <testDate> <testTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!test 3 mat 05-05-2021 13:30" + 
        "\n!test 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!test 3 mat 05-05-2021 13:30 \"Calculator required\"")]
    public async Task TestLog(CommandContext ctx,
        [Description ("\nTakes class' short names, type !class to retrive all classes.\n")] string className = "", 
        [Description ("\nTakes time in hh:mm format.\n")] string link = "", 
        [Description ("\nTakes additional information, multiple words must be wrapped with \"\".\n")] string additionalInfo = "")
    {
        if (className == "") {
            await ctx.RespondAsync("Learn to read you dumbass. The command looks like: !test <group> <class> <testDate> <testTime> Try again!");
            return;      
        } else if (!JackTheStudent.Program.classList.Any(c => c.ShortName == className)) {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } else if (link == ""){
            await ctx.RespondAsync("There's link missing, fix it!");
            return;
        } else {
            try {
                using (var db = new JackTheStudentContext()){
                var material = new ClassMaterial {
                    ClassShortName = className,
                    Class = JackTheStudent.Program.classList.Where(c => c.ShortName == className).Select(c => c.Name).FirstOrDefault(),
                    Link = link,
                    LogById = ctx.Message.Author.Id.ToString(),
                    LogByUsername = ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator,
                    AdditionalInfo = additionalInfo
                };
                JackTheStudent.Program.classMaterialList.Add(material);
                db.ClassMaterial.Add(material);
                await db.SaveChangesAsync();
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Log failed");
                return;
            }
        await ctx.RespondAsync("Material logged successfully");     
        return;
        }   
    }

    [Command("materials")]
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
        [Description("\nTakes class' short names or \".\", type !class to retrieve all classes, usage of \".\" will tell Jack to retrieve test for ALL classes.\n")] string className = ".")
    {       
        if (!JackTheStudent.Program.classList.Any(c => c.ShortName == className) && className != ".") {
            await ctx.RespondAsync("There's no such class, you high bruh?");
            return;
        } 

        var materials = JackTheStudent.Program.classMaterialList;
        string result = String.Empty;

        if (className == ".") {
            try {
                materials = materials.ToList();
                if (materials.Count == 0) {
                        await ctx.RespondAsync("There are no materials logged!");
                } else {                  
                    foreach (ClassMaterial material in materials) {
                        result = $"{result} \nMaterial link for {material.Class} class - {material.Link}. {(material.AdditionalInfo.Equals("") ? "" : $"Additional info: {material.AdditionalInfo}")}";
                    }
                    await ctx.RespondAsync(result);
                }
            } catch(Exception ex) {
                Console.Error.WriteLine("[Jack] " + ex.ToString());
                await ctx.RespondAsync("Show logs failed");
                return;
            }
        return;            
        } else {
            try {
                materials = materials.Where(m => m.ClassShortName == className).ToList();                     
                if (materials.Count == 0) {
                    await ctx.RespondAsync($"There are no materials logged for {materials.Select(m => m.Class)} class!");
                    return;
                } else {
                    foreach (ClassMaterial material in materials) {
                        result = $"{result} \nMaterial link for {material.Class} class - {material.Link}. {(material.AdditionalInfo.Equals("") ? "" : $"Additional info: {material.AdditionalInfo}")}";
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