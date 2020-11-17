using System;

namespace JackTheStudent.CommandDescriptions
{
    public class ExamDescriptions
    {
    public const string examLogDescription = "Command logging an exam, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!exam <classShortName> <examDate> <examTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!exam mat 05-05-2021 13:30" + 
        "\n!exam ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!exam mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!exam eng 05-05-2021 13:30 . \"https://yourmaterials.com\"";
    

    public const string examLogsDescription = "Command retrieving logged exam based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!exams <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!exams - will retrieve all PLANNED exam for all the classes" + 
        "\n!exams mat - will retrieve all PLANNED exams for Maths class" +
        "\n!exams mat planned - will retrieve all PLANNED exams for Maths class" +
        "\n!exams mat . - will retrieve all LOGGGED exams for Maths class" +
        "\n!exams . . - will retrieve all LOGGGED exams for ALL classes" + 
        "\n!exams . planned - will retrieve all PLANNED exams for ALL classes";
    }

}