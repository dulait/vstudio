# vstudio

A .NET global tool that opens Visual Studio solution files from the command line.

## Installation

```
dotnet tool install --global vstudio
```

## Usage

```
vstudio .                        # find and open a solution in the current directory
vstudio ./path/to/dir            # find and open a solution in a given directory
vstudio ./path/to/MyApp.sln      # open a specific solution file directly
```

## Uninstall

```
dotnet tool uninstall --global vstudio
```
