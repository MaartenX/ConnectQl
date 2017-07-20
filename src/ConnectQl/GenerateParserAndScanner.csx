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

        var log = Context.Output[Path.GetFileNameWithoutExtension(file) + ".log"];
        var deps = CocoBuilder.GetDependencies(args);

        var lastInputWritten = deps.Inputs.Concat(new[] { @"..\..\tools\Coco.exe" }).Select(o => File.GetLastWriteTime(o)).Max();
        var lastOutputWritten = deps.Outputs.Select(o => File.GetLastWriteTime(o)).Min();

        var line = $"Processing {Path.GetFileName(file)}.";

        log.WriteLine(line);
        log.WriteLine(new string('-', line.Length));
        log.WriteLine($"Last input written: {lastInputWritten}, last output written: {lastOutputWritten}.");

        if (lastInputWritten > lastOutputWritten)
        { 
            CocoBuilder.Transform(
                args, 
                fname =>
            {
                return Context.Output[fname]; 
            },
            (f, a) => log.Write(f, a));
        }
        else
        {
            log.WriteLine("Skipping generation.");
        }

        log.WriteLine(" ");
    }
}
catch (Exception e)
{
    Context.Output["GenerateParserAndScanner.log"].WriteLine($"{e.Message}\n{e.StackTrace} ".Replace("\n", $"{Environment.NewLine}"));
}