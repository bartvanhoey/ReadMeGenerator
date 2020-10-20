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
      _currentDirectory = Directory.GetCurrentDirectory();
      _parentDirectory = Directory.GetParent(_currentDirectory);
      _directoriesInParentDirectory = Directory.GetDirectories(_parentDirectory.ToString()).ToList();
      _snippetsDirectories = _directoriesInParentDirectory.Select(e => Path.Combine(e, "snippets")).ToList();
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
          dynamic readMeArray;

          using (StreamReader r = new StreamReader(readMeJsonFilePath))
          {
            string json = r.ReadToEnd();
            readMeArray = JsonConvert.DeserializeObject(json);

            var title = readMeArray.title.Value;
            var animated_gif = readMeArray.animated_gif.Value;

            builder.AppendLine(title);
            builder.AppendLine();
            builder.AppendLine(animated_gif);
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
                tableHeader = readMeArray.csharp_table_header.Value;
                builder.AppendLine(tableHeader);
                builder.AppendLine();
              }

              if (fileNameWithoutExtension == "razor")
              {
                tableHeader = readMeArray.razor_table_header.Value;
                builder.AppendLine(tableHeader);
                builder.AppendLine();
              }
              var json = reader.ReadToEnd();
              dynamic array = JsonConvert.DeserializeObject(json);
              var markdownTable = new List<PrefixDescription>();

              foreach (var item in array)
              {
                var v = new { Amount = 108, Message = "Hello" };
                markdownTable.Add(new PrefixDescription { Prefix = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JProperty)item).Value).Last).Value).Value.ToString(), Description = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)item).First).First.Next).First.ToString() });
              }

              builder.Append(markdownTable.ToArray().ToMarkdownTable());
              builder.AppendLine();
            }
          }
          using (StreamWriter writer = new StreamWriter(Path.Combine(parentSnippetsDirectory, $"README.md")))
          {
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