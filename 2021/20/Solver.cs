using System;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._20
{
    [ProblemDate(2021, 20)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            ParseInput(input, out int[] algorithm, out Image image);
            //PrintImage(image);
            image = Enhance(algorithm, image);
            //PrintImage(image);
            image = Enhance(algorithm, image);
            //PrintImage(image);

            int count = 0;
            foreach (int[] row in image.Pixels)
                count += row.Count(px => px == 1);
            return count.ToString();
        }

        public string SolveSecond(string input)
        {
            ParseInput(input, out int[] algorithm, out Image image);

            for (int i = 0; i < 50; i++)
                image = Enhance(algorithm, image);

            //PrintImage(image);

            int count = 0;
            foreach (int[] row in image.Pixels)
                count += row.Count(px => px == 1);
            return count.ToString();
        }

        private void ParseInput(string input, out int[] algorithm, out Image image)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            string algoString = lines[0];
            algorithm = algoString.Select(IsLightPixel).ToArray();

            image = new Image()
            {
                Pixels = new int[lines.Length - 2][], //excluding the first two lines
                Border = 0
            };

            for (int i = 0; i < image.Pixels.Length; i++)
            {
                string imageRow = lines[i + 2];
                image.Pixels[i] = imageRow.Select(IsLightPixel).ToArray();
            }
        }

        private Image Enhance(int[] algorithm, Image image)
        {
            var enhancedPixels = new int[image.Pixels.Length + 2][];
            for (int i = 0; i < image.Pixels.Length + 2; i++)
                enhancedPixels[i] = new int[image.Pixels.Length + 2];

            for (int y = 0; y < enhancedPixels.Length; y++)
            {
                for (int x = 0; x < enhancedPixels.Length; x++)
                {
                    int algoIndex = 0;

                    for (int sy = y - 2; sy <= y; sy++) // sy and sx: search indeces for original image
                                                        // (original image is shifted 1 px down and to the right, 
                                                        // therefore sy points to the same pixel as y - 1 etc.)
                    {
                        for (int sx = x - 2; sx <= x; sx++)
                        {
                            algoIndex <<= 1;

                            if (sy < 0 || sy >= image.Pixels.Length ||
                                sx < 0 || sx >= image.Pixels[0].Length)
                                algoIndex |= image.Border;
                            else
                                algoIndex |= image.Pixels[sy][sx];
                        }
                    }

                    enhancedPixels[y][x] = algorithm[algoIndex];
                }
            }

            int newBorder = image.Border == 0 ? algorithm[0] : algorithm[511];
            return new Image()
            {
                Pixels = enhancedPixels,
                Border = newBorder
            };
        }

        private int IsLightPixel(char pixel) => pixel == '#' ? 1 : 0;

        private void PrintImage(Image image)
        {
            foreach (int[] row in image.Pixels)
            {
                foreach (int pixel in row)
                    Console.Write(pixel == 1 ? '#' : '.');
                Console.WriteLine();
            }
        }

        private struct Image
        {
            public int[][] Pixels { get; set; }
            public int Border { get; set; }
        }
    }
}
