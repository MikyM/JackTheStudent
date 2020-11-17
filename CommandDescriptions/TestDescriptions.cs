using System;

namespace JackTheStudent.CommandDescriptions
{
    public class TestDescriptions
    {
    public const string testLogDescription = "Command logging a test, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!test <groupId> <classShortName> <testDate> <testTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!test 3 mat 05-05-2021 13:30" + 
        "\n!test 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!test 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!test 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"";

    public const string testLogsDescription = "Command retrieving logged test based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
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
        "\n!tests . . planned - will retrieve all PLANNED tests for ALL groups for ALL classes";
    }

}