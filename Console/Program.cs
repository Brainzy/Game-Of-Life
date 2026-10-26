using System;
using System.IO;
using GameOfLife;

internal static class Program
{
    private static int Main()
    {
        try
        {
            LifeFile.RunFile("input.txt", "output.txt");
            return 0;
        }
        catch (Exception error) when (error is IOException || error is FormatException ||
                                      error is UnauthorizedAccessException)
        {
            System.Console.Error.WriteLine(error.Message);
            return 1;
        }
    }
}

