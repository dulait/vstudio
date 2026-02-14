using Spectre.Console;
using VStudio.Core.Abstractions;
using VStudio.Core.Models;

namespace VStudio.Cli;

public class App(ISolutionFinder solutionFinder, IVisualStudioLauncher launcher, IAnsiConsole console)
{
    public int Run(string[] args)
    {
        try
        {
            var inputPath = args.Length > 0 ? args[0] : ".";
            var fullPath = Path.GetFullPath(inputPath);

            SolutionFile solution;

            if (File.Exists(fullPath))
            {
                var extension = Path.GetExtension(fullPath);
                if (!extension.Equals(".sln", StringComparison.OrdinalIgnoreCase)
                    && !extension.Equals(".slnx", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"File is not a solution file (.sln or .slnx): {fullPath}");
                }

                solution = new SolutionFile { FullPath = fullPath };
            }
            else if (Directory.Exists(fullPath))
            {
                var solutions = solutionFinder.FindSolutions(fullPath);

                solution = solutions.Count switch
                {
                    0 => throw new FileNotFoundException(
                        $"No solution files found in: {fullPath}"),
                    1 => solutions[0],
                    _ => console.Prompt(
                        new SelectionPrompt<SolutionFile>()
                            .Title("Multiple solutions found. Which one?")
                            .UseConverter(s => s.FileName)
                            .AddChoices(solutions)),
                };
            }
            else
            {
                throw new FileNotFoundException($"Path not found: {fullPath}");
            }

            launcher.Launch(solution);

            console.MarkupLine($"[green]Opened {Markup.Escape(solution.FileName)}[/]");
            return 0;
        }
        catch (Exception ex) when (ex is FileNotFoundException
                                       or DirectoryNotFoundException
                                       or InvalidOperationException)
        {
            console.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
            return 1;
        }
        catch (Exception ex)
        {
            console.MarkupLine($"[red]Unexpected error:[/] {Markup.Escape(ex.Message)}");
            return 2;
        }
    }
}
