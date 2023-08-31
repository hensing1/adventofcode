### C# solutions to the [Advent of Code](https://adventofcode.com) puzzles from 2020 and 2021

----------

## Usage (Linux/Windows/macOS)

Prerequisites: installation of .NET 5.0 (or higher)

1. clone repository
2. open command line in directory
2. run `dotnet build`
3. run `dotnet run YEAR DAY {1|2} [your_input_file.txt]`

Example (second puzzle from day 11 in 2021): 
```
C:\Users\henry-dv\c#\adventofcode>dotnet run 2021 1 1

Getting solver instance...
Calculating...

Solution: 1711
Time elapsed: 2ms
Press enter to exit.
```

If you're on windows, you can also just clone this into Visual Studio and specify the command line arguments there.

----------

By default, the program reads from `/[year]/[day]/input.txt`. You can specify your own puzzle input by adding a file in the corresponding directory and referencing it as a command line argument.
