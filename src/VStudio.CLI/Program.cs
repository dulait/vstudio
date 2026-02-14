using Spectre.Console;
using VStudio.Cli;
using VStudio.Core.Services;

var app = new App(new SolutionFinder(), new VisualStudioLauncher(), AnsiConsole.Console);
return app.Run(args);
