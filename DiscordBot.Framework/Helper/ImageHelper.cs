using System;
using System.Diagnostics;
using DiscordBot.Framework.Contract.Helper;

namespace DiscordBot.Framework.Helper;

internal class ImageHelper : IImageHelper
{
    public void Screenshot(string url, string selector)
    {
        ExecuteShellCommand($"firefox -screenshot --selector \"{selector}\" -headless \"{url}\"");
        Console.WriteLine("Screenshot taken: " + url);
    }

    private static void ExecuteShellCommand(string arguments)
    {
        // according to: https://stackoverflow.com/a/15262019/637142
        // thanks to this we will pass everything as one command
        arguments = arguments.Replace("\"", "\"\"");

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + arguments + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        proc.Start();
        proc.WaitForExit();
    }
}