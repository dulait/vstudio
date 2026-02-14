using System.Diagnostics;
using VStudio.Core.Models;
using VStudio.Core.Services;

namespace VStudio.Core.UnitTests;

public class VisualStudioLauncherTests : IDisposable
{
    private readonly string _tempDir;

    public VisualStudioLauncherTests()
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

    [Fact]
    public void Launch_SolutionFileDoesNotExist_ThrowsFileNotFoundException()
    {
        var sut = new VisualStudioLauncher(_ => { });
        var solution = new SolutionFile { FullPath = Path.Combine(_tempDir, "Missing.sln") };

        Assert.Throws<FileNotFoundException>(() => sut.Launch(solution));
    }

    [Fact]
    public void Launch_SolutionFileExists_CallsProcessStartWithCorrectFileName()
    {
        var slnPath = Path.Combine(_tempDir, "MyApp.sln");
        File.WriteAllText(slnPath, "");

        ProcessStartInfo? captured = null;
        var sut = new VisualStudioLauncher(info => captured = info);
        var solution = new SolutionFile { FullPath = slnPath };

        sut.Launch(solution);

        Assert.NotNull(captured);
        Assert.Equal(slnPath, captured.FileName);
    }

    [Fact]
    public void Launch_SolutionFileExists_UsesShellExecute()
    {
        var slnPath = Path.Combine(_tempDir, "MyApp.sln");
        File.WriteAllText(slnPath, "");

        ProcessStartInfo? captured = null;
        var sut = new VisualStudioLauncher(info => captured = info);
        var solution = new SolutionFile { FullPath = slnPath };

        sut.Launch(solution);

        Assert.NotNull(captured);
        Assert.True(captured.UseShellExecute);
    }
}
