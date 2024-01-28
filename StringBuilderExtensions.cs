using System.Text;
using static System.IO.Path;

namespace ReadMeGenerator;

public static class StringBuilderExtensions
{
    public static void AppendTableHeader(this StringBuilder builder, dynamic readMeJson, string snippetFilePath)
    {
        switch (GetFileNameWithoutExtension(snippetFilePath))
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