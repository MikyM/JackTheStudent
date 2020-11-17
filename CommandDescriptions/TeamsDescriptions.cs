using System;

namespace JackTheStudent.CommandDescriptions
{
    public class TeamsDescriptions
    {
    public const string teamslinkLogDescription = "Command logging a teamsLink, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!teamsLink <groupId> <classShortName> <teamsLinkDate> <teamsLinkTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!teamsLink 3 mat 05-05-2021 13:30" + 
        "\n!teamsLink 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!teamsLink 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!teamsLink 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"";

    public const string teamslinkLogsDescription = "Command retrieving logged teamsLink based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!teamsLinks <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!teamsLinks - will retrieve all PLANNED teamsLinks for all the groups and all the classes" + 
        "\n!teamsLinks 1 - will retrieve all PLANNED teamsLinks for group 1 for all the classes" +
        "\n!teamsLinks 1 mat - will retrieve all PLANNED teamsLinks for group 1 for Maths class" +
        "\n!teamsLinks 1 mat planned - will retrieve all PLANNED teamsLinks for group 1 for Maths class" +
        "\n!teamsLinks 1 mat . - will retrieve all LOGGED teamsLinks for group 1 for Maths class" +
        "\n!teamsLinks 1 . . - will retrieve all LOGGED teamsLinks for group 1 for ALL classes" + 
        "\n!teamsLinks . . . - will retrieve all LOGGED teamsLinks for ALL groups for ALL classes" +
        "\n!teamsLinks . mat . - will retrieve all LOGGED teamsLinks for ALL groups for MAths class" +
        "\n!teamsLinks . . planned - will retrieve all PLANNED teamsLinks for ALL groups for ALL classes";
    }

}