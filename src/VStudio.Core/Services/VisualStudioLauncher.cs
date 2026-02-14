using System.Diagnostics;
using VStudio.Core.Abstractions;
using VStudio.Core.Models;

namespace VStudio.Core.Services;

public class VisualStudioLauncher : IVisualStudioLauncher
{
    private readonly Action<ProcessStartInfo> _startProcess;

    public VisualStudioLauncher()
        : this(info => Process.Start(info))
    {
    }

    internal VisualStudioLauncher(Action<ProcessStartInfo> startProcess)
    {
        _startProcess = startProcess;
    }

    public void Launch(SolutionFile solutionFile)
    {
        if (!File.Exists(solutionFile.FullPath))
        {
            throw new FileNotFoundException(
                $"Solution file not found: {solutionFile.FullPath}",
                solutionFile.FullPath);
        }

        _startProcess(new ProcessStartInfo
        {
            FileName = solutionFile.FullPath,
            UseShellExecute = true,
        });
    }
}
