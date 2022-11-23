using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MarkdownLog;
using static System.Console;
using static System.IO.Directory;
using static System.IO.Path;
using static Newtonsoft.Json.JsonConvert;

namespace ReadMeGenerator
{
    public class Generator
    {
        public void Generate()
        {
            foreach (var directory in GetSnippetsDirectory())
            {
                if (!Directory.Exists(directory)) continue;
                
                var builder = new StringBuilder();
                dynamic readMeJson;

                using (var reader = new StreamReader(Combine(GetParent(directory).ToString(), "readme.json")))
                {
                    readMeJson = DeserializeObject(reader.ReadToEnd());

                    var title = readMeJson?.title.Value;
                    var animatedGif = readMeJson?.animated_gif.Value;
                    var createIssue = readMeJson?.create_issue.Value;

                    builder.AppendLine(title).AppendLine().AppendLine(animatedGif).AppendLine().AppendLine()
                        .AppendLine(createIssue).AppendLine();
                }

                foreach (var snippetFilePath in GetFiles(directory))
                {
                    using var reader = new StreamReader(snippetFilePath);

                    var fileName = GetFileNameWithoutExtension(snippetFilePath);
                    var tableHeader = readMeJson?.csharp_table_header.Value;

                    switch (fileName)
                    {
                        case "csharp":
                            builder.AppendLine(tableHeader);
                            builder.AppendLine();
                            break;
                        case "razor":
                            builder.AppendLine(tableHeader);
                            builder.AppendLine();
                            break;
                    }

                    dynamic snippets = DeserializeObject(reader.ReadToEnd());
                    var markdownTable = new List<MarkDownTableEntry>();

                    if (snippets != null)
                        foreach (var snippet in snippets)
                            markdownTable.Add(new MarkDownTableEntry(snippet));

                    builder.Append(markdownTable.OrderBy(x => x.Prefix).ToArray().ToMarkdownTable()).AppendLine();
                }

                using var writer = new StreamWriter(Combine(GetParent(directory).ToString(), "README.md"));
                WriteLine(Combine(GetParent(directory).ToString(), $"README.md"));
                writer.WriteLine(builder.ToString());
            }
        }

        private static List<string> GetSnippetsDirectory()
        {
            var currentDirectory = GetCurrentDirectory().Replace("\\bin\\Debug\\netcoreapp3.1", "");
            var parentDirectory = GetParent(currentDirectory);
            return GetDirectories(parentDirectory.ToString()).ToList()
                .Select(e => Combine(e.Replace("\\bin\\Debug\\netcoreapp3.1", ""), "snippets")).ToList();
        }
    }
}