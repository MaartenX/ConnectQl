#r "..\..\tools\Coco.exe"

using System.Linq;
using System.IO;
using Coco;

var atgFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.atg");

try
{
    foreach (var file in atgFiles)
    {
        var args = new[]
        {
            file
        };

        var deps = CocoBuilder.GetDependencies(args);

        var lastInputWritten = deps.Inputs.Concat(new[] { @"..\..\tools\Coco.exe" }).Select(o => File.GetLastWriteTime(o)).Max();
        var lastOutputWritten = deps.Outputs.Select(o => File.GetLastWriteTime(o)).Min();

        var line = $"Processing {Path.GetFileName(file)}.";

        Context.Log.Info(line);
        Context.Log.Info(new string('-', line.Length));
        Context.Log.Info($"Last input written: {lastInputWritten}, last output written: {lastOutputWritten}.");

        if (lastInputWritten > lastOutputWritten)
        { 
            CocoBuilder.Transform(
                args, 
                fname =>
            {
                return Context.Output[fname]; 
            },
            (f, a) => Context.Log.Info(string.Format(f, a)));
        }
        else
        {
            Context.Log.Info("Skipping generation.");
        }
    }
}
catch (Exception e)
{
    Context.Log.Error($"{e.Message}\n{e.StackTrace} ".Replace("\n", $"{Environment.NewLine}"));
}