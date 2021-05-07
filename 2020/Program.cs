using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020
{
    class Program
    {
        static readonly string Root = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        [STAThread] //needed because of clipboard
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
                Exit("Required args: \n\t[# of Day] [# of Puzzle (1 or 2)] [Input file name (optional, must be contained in folder corresponding with # of Day)]", -1);

            var day = int.Parse(args[0]);
            var numOfPuzzle = int.Parse(args[1]);

            if (args[0].Length == 1)
                args[0] = '0' + args[0];

            var path = 
                args.Length == 3 ? 
                    Path.Combine(Root, "days", args[0], args[2]) :
                    Path.Combine(Root, "days", args[0], "input.txt");

            if (!File.Exists(path))
                Exit($"File '{path}' does not exist", -1);

            var methodName = string.Empty;
            switch (numOfPuzzle)
            {
                case 1:
                    methodName = "SolveFirst";
                    break;
                case 2:
                    methodName = "SolveSecond";
                    break;
                default:
                    Exit("2nd argument (# of Puzzle) must either be 1 or 2", -1);
                    return;
            }

            Console.WriteLine("Getting solver instance...");
            Type solverType =
            (
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from type in a.GetTypes()
                where 
                    type.IsDefined(typeof(ProblemDate), false) &&
                    ((ProblemDate)type.GetCustomAttributes(typeof(ProblemDate), false)[0]).Date == day
                select type
            ).Single();

            var solver = (ISolver)Activator.CreateInstance(solverType);
            MethodInfo solverMethod = solverType.GetMethod(methodName);

            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Calculating...");
            sw.Start();
            var solution = (string)solverMethod.Invoke(solver, new[] { path });
            sw.Stop();

            Console.WriteLine($"\n\nSolution: {solution}");
            Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds}ms");
            Clipboard.SetText(solution);
            Exit("Solution copied to clipboard.");
        }

        static void Exit(string message, int exitCode = 0)
        {
            if (exitCode == -1)
                Console.Write("Error: ");
            Console.WriteLine(message);
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            System.Environment.Exit(exitCode);
        }
    }
}
