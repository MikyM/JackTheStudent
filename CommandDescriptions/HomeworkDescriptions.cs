using System;

namespace JackTheStudent.CommandDescriptions
{
    public class HomeworkDescriptions
    {
    public const string homeworkLogDescription = "Command logging a homework, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!homework <groupId> <classShortName> <deadlineDate> <deadlineTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!homework 3 mat 05-05-2021 13:30" + 
        "\n!homework 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!homework 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!homework 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"";
    

    public const string homeworkLogsDescription = "Command retrieving logged homework based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!homeworks <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!homeworks - will retrieve all PLANNED homework for all the groups and all the classes" + 
        "\n!homeworks 1 - will retrieve all PLANNED homework for group 1 for all the classes" +
        "\n!homeworks 1 mat - will retrieve all PLANNED homework for group 1 for Maths class" +
        "\n!homeworks 1 mat planned - will retrieve all PLANNED homework for group 1 for Maths class" +
        "\n!homeworks 1 mat . - will retrieve all LOGGED homework for group 1 for Maths class" +
        "\n!homeworks 1 . . - will retrieve all LOGGED homework for group 1 for ALL classes" + 
        "\n!homeworks . . . - will retrieve all LOGGED homework for ALL groups for ALL classes" +
        "\n!homeworks . mat . - will retrieve all LOGGED homework for ALL groups for MAths class" +
        "\n!homeworks . . planned - will retrieve all PLANNED homework for ALL groups for ALL classes";
    }

}