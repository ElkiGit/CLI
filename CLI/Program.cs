// See https://aka.ms/new-console-template for more information

using System.CommandLine;
var outputOption = new Option<FileInfo>("--output");
var languageOption = new Option<string>("--language");
var noteOption = new Option<bool>("--note");
var sortOption = new Option<string>("--sort");
var removeEmptyLineOption = new Option<bool>("--remove-empty-line");
var authorOption = new Option<string>("--author");
outputOption.AddAlias("--o");
languageOption.AddAlias("--l");
noteOption.AddAlias("--n");
removeEmptyLineOption.AddAlias("--rel");
authorOption.AddAlias("--a");
sortOption.AddAlias("--s");
var bundleCommand = new Command("bundle", "bundle command");
outputOption.IsRequired = true;
languageOption.IsRequired = true;
bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(removeEmptyLineOption);
bundleCommand.AddOption(authorOption);
bundleCommand.SetHandler((output, language, note, sort, removeEmptyLine, author) =>
{
    try
    {
        var gitIgnorePath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
        var excludePatterns = new List<string>();

        if (File.Exists(gitIgnorePath))
        {
            var gitIgnoreLines = File.ReadAllLines(gitIgnorePath);
            foreach (var line in gitIgnoreLines)
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                {
                    
                    excludePatterns.AddRange(line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(pattern => pattern.Trim()));
                }
            }
        }
        string[] files = { };
        if (language.ToLower() != "all")
        {
            string[] optionLanguages = language.Split(',');
            foreach (string optionLanguage in optionLanguages)
            {
                string option = optionLanguage.Trim();
                if (option != "all")
                {
                    var languageFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"*.{option}", SearchOption.AllDirectories);

                    languageFiles = languageFiles.Where(file =>
                    {
                        var extension = Path.GetExtension(file);
                        return !excludePatterns.Any(pattern => extension.Equals(pattern, StringComparison.OrdinalIgnoreCase));
                    }).ToArray();

                    files = files.Concat(languageFiles).ToArray();
                }
                else
                {
                    files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories).ToArray();
                }
            }
        }
        else
        {
            files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories).ToArray();
        }
        if (files.Length == 0)
        {
            Console.WriteLine($"No files found for language: {language}");
            return;
        }

        else
        {
            files = files.Where(file => !file.Contains("obj")).ToArray();
            files = files.Where(file => !file.Contains("bin")).ToArray();
            files = files.Where(file => !file.Contains(".vs")).ToArray();
            files = files.Where(file => !file.Contains("Debug")).ToArray();
            files = files.Where(file => !file.Contains("node_modules")).ToArray();
        }
        if (!string.IsNullOrEmpty(sort))
        {
            if (sort.ToLower() == "name")
            {
                files = files.OrderBy(file => Path.GetFileName(file)).ToArray();
            }
            else if (sort.ToLower() == "type")
            {
                files = files.OrderBy(file => Path.GetExtension(file)).ToArray();
            }
        }
        else
            files = files.OrderBy(file => Path.GetFileName(file)).ToArray();

        using (var outputFile = File.CreateText(output.FullName))
        {

            if (!string.IsNullOrEmpty(author))
            {
                outputFile.WriteLine($"----------Name: {author}------------");

            }
            foreach (var file in files)
            {
                if (note)
                    outputFile.WriteLine($"----------Source Code Location: {file.Replace(Directory.GetCurrentDirectory(), "")}------------");
                var fileContent = File.ReadAllText(file);
                if (removeEmptyLine)
                {
                    fileContent = string.Join("", fileContent.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)));
                }
                outputFile.WriteLine(fileContent);
            }

            Console.WriteLine($"Bundle complete. All contents written to {output.FullName}");
        }
    }
    catch (DirectoryNotFoundException e)
    {
        Console.WriteLine("Error: the path of file is not correct!!");
    }
    catch (IOException e)
    {
        Console.WriteLine("Error: the path not valid!!");
    }
    catch (UnauthorizedAccessException e)
    {
        Console.WriteLine("Error: you forget to write the name of file");
    }

}, outputOption, languageOption, noteOption, sortOption, removeEmptyLineOption, authorOption);
var createRspCommand = new Command("create-rsp");
createRspCommand.SetHandler(() =>
{
    string temp;
    using (StreamWriter responseFile = new StreamWriter("response.rsp"))
    {
        responseFile.Write("bundle --o ");
        Console.Write("Enter the path - ");
        responseFile.Write($"\"{Console.ReadLine()}\" ");
        responseFile.Write("--l ");
        Console.Write("Enter the ending of languages you want or enter all (enter , between the languages) - ");
        responseFile.Write($"{Console.ReadLine()} ");
        Console.Write("Enter how to sort the files (name/type) - ");
        responseFile.Write($"--s {Console.ReadLine()} ");
        Console.Write("Enter the name if you want it to be listed in a note in the file - ");
        temp = Console.ReadLine();
        if (temp != "")
            responseFile.Write($"--a {temp} ");
        Console.Write("Enter if you want to write the path in the file (Y,N) - ");
        if (Console.ReadLine() == "Y")
            responseFile.Write("--n ");
        Console.Write("Enter if you want to remove empty lines (Y,N) - ");
        if (Console.ReadLine() == "Y")
            responseFile.Write("--rel");
    }

});


var rootCommand = new RootCommand("Root command");
rootCommand.AddCommand(createRspCommand);
rootCommand.AddCommand(bundleCommand);
rootCommand.InvokeAsync(args);
