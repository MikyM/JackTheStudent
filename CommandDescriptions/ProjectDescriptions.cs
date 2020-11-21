namespace JackTheStudent.CommandDescriptions
{
    public class ProjectDescriptions
    {
    public const string projectLogDescription = "Command logging a project, last two arguments are optional." +
        "\nTo log without addInfo but with materials use \".\" where addInfo should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!project <isGroup> <groupId> <classShortName> <projectDate> <projectTime> <additionalInfo>\n" + 
        "\nExamples:\n" +
        "\n!project 1 3 mat 05-05-2021 13:30" + 
        "\n!project 0 1 ele 05-05-2021 12:30 \"Calculator required\"" +
        "\n!project 1 3 mat 05-05-2021 13:30" + 
        "\n!project 0 1 eng 05-05-2021 13:30";
    

    public const string projectLogsDescription = "Command retrieving logged project based on passed arguments, ALL arguments are optional and the command has default settings.\n" +
        "\n!projects <groupId> <classShortName> <isGroup> <alreadyTookPlace?>\n" + 
        "\nType !classes to retrieve short names and !groups to retrieve group IDs" +
        "\nUse \".\" to retrieve ALL possible entries for each argument, <alreadyTookPlace?> takes \"planned\" or \".\"\n" +
        "\nExamples:\n" +
        "\n!projects - will retrieve all PLANNED projects for all the groups and all the classes" + 
        "\n!projects 1 - will retrieve all PLANNED projects for group 1 for all the classes" +
        "\n!projects 1 mat - will retrieve all PLANNED projects for group 1 for Maths class" +
        "\n!projects 1 mat 1 - will retrieve all PLANNED projects for group 1 for Maths class and only GROUP ones" +
        "\n!projects 1 mat 0 planned - will retrieve all PLANNED projects for group 1 for Maths class and only NON-GROUP ones" +
        "\n!projects 1 mat . . - will retrieve all LOGGED projects for group 1 for Maths class";
    }

}