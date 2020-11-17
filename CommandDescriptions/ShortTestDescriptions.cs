using System;

namespace JackTheStudent.CommandDescriptions
{
    public class ShortTestDescriptions
    {
    public const string shorttestLogDescription = "Command logging a short test, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!shorttest <groupId> <classShortName> <shortTestDate> <shortTestTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!shorttest 3 mat 05-05-2021 13:30" + 
        "\n!shorttest 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!shorttest 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!shorttest 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"";
    

    public const string shorttestLogsDescription = "Command retrieving logged short test based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!shorttests <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!shorttests - will retrieve all PLANNED short tests for all the groups and all the classes" + 
        "\n!shorttests 1 - will retrieve all PLANNED short tests for group 1 for all the classes" +
        "\n!shorttests 1 mat - will retrieve all PLANNED short tests for group 1 for Maths class" +
        "\n!shorttests 1 mat planned - will retrieve all PLANNED short tests for group 1 for Maths class" +
        "\n!shorttests 1 mat . - will retrieve all LOGGED short tests for group 1 for Maths class" +
        "\n!shorttests 1 . . - will retrieve all LOGGED short tests for group 1 for ALL classes" + 
        "\n!shorttests . . . - will retrieve all LOGGED short tests for ALL groups for ALL classes" +
        "\n!shorttests . mat . - will retrieve all LOGGED short tests for ALL groups for MAths class" +
        "\n!shorttests . . planned - will retrieve all PLANNED short tests for ALL groups for ALL classes";
    }

}