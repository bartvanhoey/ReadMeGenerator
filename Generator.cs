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
            foreach (var snippetDirectory in GetSnippetsFromVsCodeExtensionDirectory())
            {
                if (!Exists(snippetDirectory)) continue;
                
                var builder = new StringBuilder();
                dynamic readMeJson;

                using (var reader = new StreamReader(Combine(GetParentDirectory(snippetDirectory), "readme.json")))
                {
                    readMeJson = DeserializeObject(reader.ReadToEnd());

                    var title = readMeJson?.title.Value;
                    var animatedGif = readMeJson?.animated_gif.Value;
                    var createIssue = readMeJson?.create_issue.Value;

                    builder.AppendLine(title)
                        .AppendLine()
                        .AppendLine(animatedGif)
                        .AppendLine()
                        .AppendLine()
                        .AppendLine(createIssue)
                        .AppendLine();
                }

                foreach (var snippetFilePath in GetFiles(snippetDirectory))
                {
                    builder.AppendTableHeader((object)readMeJson, snippetFilePath);
                    builder.Append(GetMarkDownTableFromFile(snippetFilePath)).AppendLine();
                }
                WriteToReadMeFile(snippetDirectory, builder);
            }
        }

        private static Table GetMarkDownTableFromFile(string snippetFile)
        {
            using var snippetFileReader = new StreamReader(snippetFile);
            dynamic snippets = DeserializeObject(snippetFileReader.ReadToEnd());
            var markdownTable = new List<MarkDownTableEntry>();
            if (snippets == null) return new Table();
            foreach (var snippet in snippets)
                markdownTable.Add(new MarkDownTableEntry(snippet));

            return markdownTable.OrderBy(x => x.Prefix).ToMarkdownTable();

        }

        private static void WriteToReadMeFile(string directory, StringBuilder builder)
        {
            var pathReadMeFile = Combine(GetParentDirectory(directory), "README.md");
            using var writer = new StreamWriter(pathReadMeFile);
            WriteLine(pathReadMeFile);
            writer.WriteLine(builder.ToString());
        }

        private static List<string> GetSnippetsFromVsCodeExtensionDirectory()
        {
            var currentDirectory = GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "");
            var parentDirectory = GetParentDirectory(currentDirectory);
            var snippetsDirectory = GetDirectories(parentDirectory).ToList()
                .Select(e => Combine(e.Replace("\\bin\\Debug\\net6.0", ""), "Snippets")).ToList();
            return snippetsDirectory;
        }

        private static string GetParentDirectory(string currentDirectory) => GetParent(currentDirectory)?.ToString();
    }
}