using System.Diagnostics;

namespace VStudio.IntegrationTests;

public class ProgramTests : IDisposable
{
    private static readonly string CliProjectPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..",
            "src", "VStudio.CLI", "VStudio.CLI.csproj"));

    private readonly string _tempDir;

    public ProgramTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (!Directory.Exists(_tempDir))
        {
            return;
        }

        Directory.Delete(_tempDir, recursive: true);
    }

    private static async Task<(int ExitCode, string Output)> RunCliAsync(
        string workingDirectory, params string[] args)
    {
        var quotedArgs = string.Join(" ", args.Select(a => $"\"{a}\""));
        var arguments = $"run --project \"{CliProjectPath}\" -- {quotedArgs}";

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        process.Start();
        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return (process.ExitCode, stdout + stderr);
    }

    [Fact]
    public async Task NoArgs_EmptyDirectory_ReturnsExitCode1WithError()
    {
        var (exitCode, output) = await RunCliAsync(_tempDir);

        Assert.Equal(1, exitCode);
        Assert.Contains("No solution files found", output);
    }

    [Fact]
    public async Task NonExistentPath_ReturnsExitCode1WithError()
    {
        var fakePath = Path.Combine(_tempDir, "nonexistent");

        var (exitCode, output) = await RunCliAsync(_tempDir, fakePath);

        Assert.Equal(1, exitCode);
        Assert.Contains("Path not found", output);
    }

    [Fact]
    public async Task NonSolutionFile_ReturnsExitCode1WithError()
    {
        var txtFile = Path.Combine(_tempDir, "readme.txt");
        await File.WriteAllTextAsync(txtFile, "");

        var (exitCode, output) = await RunCliAsync(_tempDir, txtFile);

        Assert.Equal(1, exitCode);
        Assert.Contains("not a solution file", output);
    }

    [Fact]
    public async Task ExplicitEmptyDirectory_ReturnsExitCode1WithError()
    {
        var emptyDir = Path.Combine(_tempDir, "empty");
        Directory.CreateDirectory(emptyDir);

        var (exitCode, output) = await RunCliAsync(_tempDir, emptyDir);

        Assert.Equal(1, exitCode);
        Assert.Contains("No solution files found", output);
    }
}
