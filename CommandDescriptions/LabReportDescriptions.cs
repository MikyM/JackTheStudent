using System;

namespace JackTheStudent.CommandDescriptions
{
    public class LabReportDescriptions
    {
    public const string labreportLogDescription = "Command logging a lab report, last two arguments are optional." +
        "\nTo pass without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!labreport <groupId> <classShortName> <labReportDate> <labReportTime> <additionalInfo> <materials>\n" + 
        "\nExamples:\n" +
        "\n!labreport 3 mat 05-05-2021 13:30" + 
        "\n!labreport 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!labreport 3 mat 05-05-2021 13:30 \"Calculator required\" \"https://yourmaterials.com\"" +
        "\n!labreport 1 eng 05-05-2021 13:30 . \"https://yourmaterials.com\"";
    

    public const string labreportLogsDescription = "Command retrieving logged lab report based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!labreports <groupId> <classShortName> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!labreports - will retrieve all PLANNED lab reports for all the groups and all the classes" + 
        "\n!labreports 1 - will retrieve all PLANNED lab reports for group 1 for all the classes" +
        "\n!labreports 1 mat - will retrieve all PLANNED lab reports for group 1 for Maths class" +
        "\n!labreports 1 mat planned - will retrieve all PLANNED lab reports for group 1 for Maths class" +
        "\n!labreports 1 mat . - will retrieve all LOGGED lab reports for group 1 for Maths class" +
        "\n!labreports 1 . . - will retrieve all LOGGED lab reports for group 1 for ALL classes" + 
        "\n!labreports . . . - will retrieve all LOGGED lab reports for ALL groups for ALL classes" +
        "\n!labreports . mat . - will retrieve all LOGGED lab reports for ALL groups for MAths class" +
        "\n!labreports . . planned - will retrieve all PLANNED lab reports for ALL groups for ALL classes";
    }

}