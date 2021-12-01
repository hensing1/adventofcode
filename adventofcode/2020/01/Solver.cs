using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2020._01
{
    [ProblemDate(2020, 1)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int[] nums = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                nums[i] = int.Parse(lines[i]);

            for (int i = 0; i < nums.Length; i++)
                for (int j = i + 1; j < nums.Length; j++)
                    if (nums[i] + nums[j] == 2020)
                        return $"{nums[i] * nums[j]}";

            return "No solution found.";
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int[] nums = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                nums[i] = int.Parse(lines[i]);

            for (int i = 0; i < nums.Length; i++)
            /**/for (int j = i + 1; j < nums.Length; j++)
             /*   */if (nums[i] + nums[j] < 2020)
              /*      */for (int k = j + 1; k < nums.Length; k++)
              /*   *      */if (nums[i] + nums[j] + nums[k] == 2020)
            /***         *    */return $"{nums[i] * nums[j] * nums[k]}";
              /*    *     */
              /*      */
             /*   */
            /**/

            //Christmas tree!

            return "No solution found.";
        }
    }
}
