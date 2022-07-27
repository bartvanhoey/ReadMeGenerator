using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MarkdownLog;
using Newtonsoft.Json.Linq;
using static System.Console;
using static System.IO.Directory;
using static System.IO.Path;
using static Newtonsoft.Json.JsonConvert;

namespace ReadMeGenerator
{
    public class Generator
    {
        private readonly List<string> _snippetsDirectories;

        public Generator()
        {
            var currentDirectory = GetCurrentDirectory().Replace("\\bin\\Debug\\netcoreapp3.1", "");
            var parentDirectory = GetParent(currentDirectory);
            var directoriesInParentDirectory = GetDirectories(parentDirectory.ToString()).ToList();
            _snippetsDirectories = directoriesInParentDirectory
                .Select(e => Combine(e.Replace("\\bin\\Debug\\netcoreapp3.1", ""), "snippets")).ToList();
        }

        public void Generate()
        {
            foreach (var directory in _snippetsDirectories)
            {
                if (Exists(directory))
                {
                    var parentSnippetsDirectory = GetParent(directory).ToString();
                    var readMeJsonFilePath = Combine(parentSnippetsDirectory, "readme.json");
                    var builder = new StringBuilder();
                    dynamic readMeJson;

                    using (var reader = new StreamReader(readMeJsonFilePath))
                    {
                        readMeJson = DeserializeObject(reader.ReadToEnd());

                        var title = readMeJson?.title.Value;
                        var animatedGif = readMeJson?.animated_gif.Value;
                        var createIssue = readMeJson?.create_issue.Value;

                        builder.AppendLine(title);
                        builder.AppendLine();
                        builder.AppendLine(animatedGif);
                        builder.AppendLine();
                        builder.AppendLine();
                        builder.AppendLine(createIssue);
                        builder.AppendLine();
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
                        var markdownTable = new List<PrefixDescription>();

                        if (snippets != null)
                        {
                            foreach (var snippet in snippets)
                            {
                                markdownTable.Add(new PrefixDescription
                                {
                                    Prefix = ((JValue)((JProperty)((JContainer)((JProperty)snippet).Value).Last)?.Value)?.Value?.ToString(),
                                    Description = ((JContainer)((JContainer)((JContainer)snippet).First)?.First?.Next)?.First?.ToString()
                                });
                            }
                        }

                        builder.Append(markdownTable.OrderByDescending(x => x.Prefix).ToArray().ToMarkdownTable());
                        builder.AppendLine();
                    }

                    using var writer = new StreamWriter(Combine(parentSnippetsDirectory, $"README.md"));
                    WriteLine(Combine(parentSnippetsDirectory, $"README.md"));
                    writer.WriteLine(builder.ToString());
                }
                else
                {
                    WriteLine($"{directory} does not exist");
                }
            }
        }
    }
}