using System;
using System.IO;
using System.Linq;
using System.Windows;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020
{
    class Program
    {
        static readonly string Root = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("Required args: \n\t[# of Day] [# of Puzzle (1 or 2)] [Input file name (optional, must be contained in folder corresponding with # of Day)]");
                Console.ReadLine();
                return;
            }

            var day = int.Parse(args[0]);
            var numOfPuzzle = int.Parse(args[1]);
            var path = 
                args.Length == 3 ? 
                    Path.Combine(Root, "days", args[0], args[2]) :
                    Path.Combine(Root, "days", args[0], "input.txt");


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
                    Console.WriteLine("2nd argument (# of Puzzle) must either be 1 or 2");
                    Console.ReadLine();
                    return;
            }

            var solverType =
            (
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from type in a.GetTypes()
                where 
                    type.IsDefined(typeof(ProblemDate), false) &&
                    ((ProblemDate)type.GetCustomAttributes(typeof(ProblemDate), false)[0]).Date == day
                select type
            ).Single();
            
            var solver = (ISolver)Activator.CreateInstance(solverType);

            Console.WriteLine("Calculating...");
            var solution = (string)solverType.GetMethod(methodName).Invoke(solver, new[] { path });

            Console.WriteLine($"Solution: {solution}");
            Clipboard.SetText(solution);
            Console.WriteLine("Copied to clipboard");
            Console.ReadLine();
        }
    }
}
