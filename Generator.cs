using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MarkdownLog;
using Newtonsoft.Json;

namespace ReadMeGenerator
{
  public class Generator
  {
    private readonly string _currentDirectory;
    private readonly DirectoryInfo _parentDirectory;
    private readonly List<string> _directoriesInParentDirectory;
    private readonly List<string> _snippetsDirectories;

    public Generator()
    {
      _currentDirectory = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\netcoreapp3.1", "");
      _parentDirectory = Directory.GetParent(_currentDirectory);
      _directoriesInParentDirectory = Directory.GetDirectories(_parentDirectory.ToString()).ToList();
      _snippetsDirectories = _directoriesInParentDirectory.Select(e => Path.Combine(e.Replace("\\bin\\Debug\\netcoreapp3.1", ""), "snippets")).ToList();
    }

    public void Generate()
    {
      foreach (var snippetDirectory in _snippetsDirectories)
      {
        if (Directory.Exists(snippetDirectory))
        {
          var parentSnippetsDirectory = Directory.GetParent(snippetDirectory).ToString();
          var readMeJsonFilePath = Path.Combine(parentSnippetsDirectory, "readme.json");
          var builder = new StringBuilder();
          dynamic readMeJson;

          using (StreamReader r = new StreamReader(readMeJsonFilePath))
          {
            string json = r.ReadToEnd();
            readMeJson = JsonConvert.DeserializeObject(json);

            var title = readMeJson.title.Value;
            var animated_gif = readMeJson.animated_gif.Value;
            var create_issue  = readMeJson.create_issue.Value;

            builder.AppendLine(title);
            builder.AppendLine();
            builder.AppendLine(animated_gif);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(create_issue);
            builder.AppendLine();
          }

          foreach (var snippetFilePath in Directory.GetFiles(snippetDirectory))
          {
            using (StreamReader reader = new StreamReader(snippetFilePath))
            {
              var tableHeader = string.Empty;
              var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(snippetFilePath);

              if (fileNameWithoutExtension == "csharp")
              {
                tableHeader = readMeJson.csharp_table_header.Value;
                builder.AppendLine(tableHeader);
                builder.AppendLine();
              }

              if (fileNameWithoutExtension == "razor")
              {
                tableHeader = readMeJson.razor_table_header.Value;
                builder.AppendLine(tableHeader);
                builder.AppendLine();
              }
              var json = reader.ReadToEnd();
              dynamic jsonArray = JsonConvert.DeserializeObject(json);
              var markdownTable = new List<PrefixDescription>();

              foreach (var item in jsonArray)
              {
                markdownTable.Add(new PrefixDescription { Prefix = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JProperty)item).Value).Last).Value).Value.ToString(), Description = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)item).First).First.Next).First.ToString() });
              }

              builder.Append(markdownTable.ToArray().ToMarkdownTable());
              builder.AppendLine();
            }
          }
          using (StreamWriter writer = new StreamWriter(Path.Combine(parentSnippetsDirectory, $"README.md")))
          {
            System.Console.WriteLine(Path.Combine(parentSnippetsDirectory, $"README.md"));
            writer.WriteLine(builder.ToString());
          }
        }
        else
        {
          System.Console.WriteLine($"{snippetDirectory} does not exist");
        }
      }
    }
  }
}