### C# solutions to the `adventofcode` puzzles from 2020 and 2021, posted at [adventofcode.com](https://adventofcode.com)

----------

## Usage

Prerequisites: installation of .NET 5.0 (or higher)

1. clone repository
2. open command line in directory
2. run `dotnet build`
3. run `dotnet run YEAR DAY {1|2} [your_input_file.txt]`

Example (second puzzle from day 11 in 2021): 
```
C:\Users\henry-dv\c#\adventofcode>dotnet run 2021 11 2

Getting solver instance...
Calculating...

Solution: 258
Time elapsed: 6ms
Press enter to exit.
```
By default, the program reads from `/[year]/[day]/input.txt`. You can specify your own puzzle input by adding a file in the corresponding directory and referencing it as a command line argument.