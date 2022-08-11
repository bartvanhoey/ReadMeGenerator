using System.IO;
using System.Text;

namespace ReadMeGenerator;

public static class StringBuilderExtensions
{
    public static void AppendTableHeader(this StringBuilder builder, dynamic readMeJson, string snippetFilePath)
    {
        
        var fileName = Path.GetFileNameWithoutExtension(snippetFilePath);

        switch (fileName)
        {
            case "csharp":
                builder.AppendLine(readMeJson?.csharp_table_header.Value);
                builder.AppendLine();
                return;
            case "razor":
                builder.AppendLine(readMeJson?.razor_table_header.Value);
                builder.AppendLine();
                return;
            case "jsonc":
                builder.AppendLine(readMeJson?.jsonc_table_header.Value);
                builder.AppendLine();
                return;
        }
    }
}