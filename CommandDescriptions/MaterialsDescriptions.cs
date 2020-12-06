using System;

namespace JackTheStudent.CommandDescriptions
{
    public class MaterialsDescriptions
    {
    public const string MaterialLogDescription = "Command logging a material, last argument is optional." +
        "\nTo pass with addInfo but without a link use \".\" where a link should be.\n" +
        "Words seperated with spaces must be wrapped with \"\"\n" +
        "\n!material <classShortName> <link> additionalInfo>\n" + 
        "\nExamples:\n" +
        "\n!material " + 
        "\n!material mat https://mylink.com \"something something\"" +
        "\n!material mat https://mylink.com" +
        "\n!material mat . \"something something\"";
    
    public const string MaterialLogsDescription = "Command retrieving logged materials based on passed arguments. There's only one argument and it's optional.\n" +
        "\n!materials <classShortName>\n" + 
        "\nType !classes to retrieve short names." +
        "\nExamples:\n" +
        "\n!materials - will retrieve all logged materials" + 
        "\n!materials mat - will retrieve all logged materials for Math class";

    }
}