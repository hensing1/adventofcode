using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2020._20
{
    [ProblemDate(2020, 20)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            Dictionary<int, Tile> tiles = GetTileDictionary(lines);

            AssembleImage(ref tiles, out int[,] imageIDs);

            long result = 1;
            var imgSize = imageIDs.GetLength(0);
            result *= imageIDs[0, 0];
            result *= imageIDs[0, imgSize - 1];
            result *= imageIDs[imgSize - 1, 0];
            result *= imageIDs[imgSize - 1, imgSize - 1];
            return result.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            Dictionary<int, Tile> tiles = GetTileDictionary(lines);

            AssembleImage(ref tiles, out int[,] imageIDs);

            bool[,] image = RemoveBordersAndCombine(tiles, imageIDs);

            var seaMonster = new string[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            int numMonsters = CountSeaMonsters(image, seaMonster);

            int seaMonsterPixels = seaMonster.Select(line => line.Count(c => c == '#')).Sum();
            int totalPixels = image.Cast<bool>().Count(b => b);
            int wavePixels = totalPixels - (numMonsters * seaMonsterPixels);

            return wavePixels.ToString();
        }

        private Dictionary<int, Tile> GetTileDictionary(string[] lines)
        {
            var tiles = new Dictionary<int, Tile>();

            for (int i = 0; i < lines.Length; i++)
            {
                Match m = Regex.Match(lines[i], @"Tile (\d+)");
                if (m.Success)
                {
                    i++;
                    var ID = int.Parse(m.Groups[1].Value);
                    string[] tileImage = lines.SkipWhile((line, index) => index < i).TakeWhile(line => line.Trim() != String.Empty).ToArray();
                    tiles.Add(ID, new Tile(tileImage));
                    i += tileImage.Length;
                }
            }

            return tiles;
        }

        private void AssembleImage(ref Dictionary<int, Tile> tiles, out int[,] imageIDs)
        {
            var imgSize = (int)Math.Sqrt(tiles.Count);
            var arrSize = imgSize * 2 - 1;
            imageIDs = new int[arrSize, arrSize];

            int firstKey = tiles.Keys.First();
            imageIDs[imgSize - 1, imgSize - 1] = firstKey;
            tiles[firstKey].Pos = (imgSize - 1, imgSize - 1);
            List<int> remaining = tiles.Keys.ToList();
            remaining.Remove(firstKey);

            var tilesUnderConsideration = new Stack<int>();
            tilesUnderConsideration.Push(firstKey);

            while (remaining.Count > 0 && tilesUnderConsideration.Count > 0)
            {
                int id = tilesUnderConsideration.Pop();
                Tile currTile = tiles[id];
                if (NeighbourCount(currTile.Pos, imageIDs) == 4)
                    continue;
                var addedTiles = new List<int>();
                for (int r = 0; r < remaining.Count; r++)
                {
                    Tile remTile = tiles[remaining[r]];
                    for (int b1 = 0; b1 < 4; b1++)
                    {
                        for (int b2 = 0; b2 < 4; b2++)
                        {
                            if (currTile.Borders[b1].SequenceEqual(remTile.Borders[b2].Reverse()))
                            {
                                remTile.Rotate(2 - b2 + b1);
                            }
                            else if (currTile.Borders[b1].SequenceEqual(remTile.Borders[b2]))
                            {
                                remTile.Rotate(2 - b2 + b1);
                                if (b1 % 2 == 0)
                                    remTile.FlipAroundY();
                                else
                                    remTile.FlipAroundX();
                            }
                            else continue;

                            remTile.Parent = id;
                            tilesUnderConsideration.Push(remaining[r]);
                            var newPos = (X: -1, Y: -1);
                            switch (b1)
                            {
                                case 0:
                                    newPos = (currTile.Pos.X, currTile.Pos.Y - 1);
                                    break;
                                case 1:
                                    newPos = (currTile.Pos.X + 1, currTile.Pos.Y);
                                    break;
                                case 2:
                                    newPos = (currTile.Pos.X, currTile.Pos.Y + 1);
                                    break;
                                case 3:
                                    newPos = (currTile.Pos.X - 1, currTile.Pos.Y);
                                    break;
                            }

                            remTile.Pos = newPos;
                            imageIDs[newPos.X, newPos.Y] = remaining[r];
                            addedTiles.Add(remaining[r]);

                            goto tileAdded;
                        }
                    }
                tileAdded:;
                }

                foreach (var addTile in addedTiles)
                    remaining.Remove(addTile);
            }

            imageIDs = TrimImage(imageIDs);
        }

        private int NeighbourCount((int X, int Y) pos, int[,] image)
        {
            int neighbourCount = 0;
            if (pos.X != 0)
                neighbourCount += image[pos.X - 1, pos.Y] == 0 ? 0 : 1;
            if (pos.X != image.GetLength(0) - 1)
                neighbourCount += image[pos.X + 1, pos.Y] == 0 ? 0 : 1;
            if (pos.Y != 0)
                neighbourCount += image[pos.X, pos.Y - 1] == 0 ? 0 : 1;
            if (pos.Y != image.GetLength(1) - 1)
                neighbourCount += image[pos.X, pos.Y + 1] == 0 ? 0 : 1;

            return neighbourCount;
        }

        private int[,] TrimImage(int[,] imageIDs)
        {
            int midpoint = imageIDs.GetLength(0) / 2;
            int startX = -1, startY = -1;

            for (int i = 0; i <= midpoint; i++)
                if (imageIDs[i, midpoint] != 0)
                {
                    startX = i;
                    break;
                }

            for (int i = 0; i <= midpoint; i++)
                if (imageIDs[midpoint, i] != 0)
                {
                    startY = i;
                    break;
                }

            var imgLen = midpoint + 1;
            var trimmed = new int[imgLen, imgLen];

            for (int y = 0; y < imgLen; y++)
                for (int x = 0; x < imgLen; x++)
                    trimmed[x, y] = imageIDs[startX + x, startY + y];

            return trimmed;
        }

        private bool[,] RemoveBordersAndCombine(Dictionary<int, Tile> tiles, int[,] imageIDs)
        {
            var newTileSize = tiles.First().Value.Size - 2;
            var sqrtNumTiles = imageIDs.GetLength(0);
            var imgSize = newTileSize * sqrtNumTiles;
            var image = new bool[imgSize, imgSize];

            for (int idY = 0; idY < sqrtNumTiles; idY++)
                for (int idX = 0; idX < sqrtNumTiles; idX++)
                {
                    Tile currTile = tiles[imageIDs[idX, idY]];
                    for (int tileY = 0; tileY < newTileSize; tileY++)
                        for (int tileX = 0; tileX < newTileSize; tileX++)
                        {
                            int globalX = idX * newTileSize + tileX;
                            int globalY = idY * newTileSize + tileY;

                            image[globalX, globalY] = currTile.Image[1 + tileX][1 + tileY];
                        }
                }

            return image;
        }

        private int CountSeaMonsters(bool[,] image, string[] seaMonster)
        {
            int[] monsterMask = ConvertToBitmask(seaMonster);
            long[][][] imageMasks = new long[8][][]; // image can be flipped/rotated in 8 distinct ways
            int numLongs = 1 + (int)Math.Ceiling(Math.Max(0, image.GetLength(0) - 64) / (64f - seaMonster[0].Length)); //how many longs do we need for one row?
            for (int i = 0; i < 8; i++)
            {
                imageMasks[i] = new long[numLongs][];
                int orientation = i / 2;
                bool flipped = i % 2 == 1;
                for (int j = 0; j < numLongs; j++)
                    imageMasks[i][j] = ConvertToBitmask(image, j * (65 - seaMonster[0].Length), orientation, flipped);
            }

            for (int o = 0; o < 8; o++)
            {
                int numMonsters = 0;

                for (int l = 0; l < numLongs; l++)
                    for (int i = 0; i < imageMasks[0][l].Length - 2; i++)
                        for (int j = 0; j < 65 - seaMonster[0].Length; j++) //nguieh
                            numMonsters +=
                              ((monsterMask[0] & (imageMasks[o][l][i] >> j)) == monsterMask[0] &&
                               (monsterMask[1] & (imageMasks[o][l][i + 1] >> j)) == monsterMask[1] &&
                               (monsterMask[2] & (imageMasks[o][l][i + 2] >> j)) == monsterMask[2]) ? 1 : 0;

                if (numMonsters > 0)
                    return numMonsters;
            }
            return -1;
        }

        private int[] ConvertToBitmask(string[] lines)
        {
            var bitmask = new int[lines.Length];
            for (int y = 0; y < lines.Length; y++)
                for (int x = 0; x < lines[0].Length; x++)
                {
                    int bit = lines[y][x] == '#' ? 1 : 0;
                    bitmask[y] |= bit << (lines[0].Length - 1 - x);
                }

            return bitmask;
        }

        private long[] ConvertToBitmask(bool[,] image, int start, int orientation, bool flipped)
        {
            var imgSize = image.GetLength(0);
            var rowSize = Math.Min(64, imgSize - start);
            var mask = new long[imgSize];
            orientation %= 4;
            switch (orientation)
            {
                case 0:
                    for (int y = 0; y < imgSize; y++)
                        for (int x = 0; x < rowSize; x++)
                        {
                            long bit = image[start + x, y] ? 1 : 0;
                            mask[y] |= bit << (rowSize - 1 - x);
                        }
                    break;
                case 1:
                    for (int x = 0; x < imgSize; x++)
                        for (int y = 0; y < rowSize; y++)
                        {
                            long bit = image[imgSize - 1 - x, start + y] ? 1 : 0;
                            mask[x] |= bit << (rowSize - 1 - y);
                        }
                    break;
                case 2:
                    for (int y = 0; y < imgSize; y++)
                        for (int x = 0; x < rowSize; x++)
                        {
                            long bit = image[imgSize - 1 - start - x, imgSize - 1 - y] ? 1 : 0;
                            mask[y] |= bit << (rowSize - 1 - x);
                        }
                    break;
                case 3:
                    for (int x = 0; x < imgSize; x++)
                        for (int y = 0; y < rowSize; y++)
                        {
                            long bit = image[x, imgSize - 1 - start - y] ? 1 : 0;
                            mask[x] |= bit << (rowSize - 1 - y);
                        }
                    break;
            }

            if (flipped)
                mask = mask.Reverse().ToArray();

            return mask;
        }

        class Tile
        {
            public bool[][] Borders { get; private set; }
            public bool[][] Image { get; private set; }
            public int Size { get { return Image.Length; } }
            public (int X, int Y) Pos { get; set; }
            public int Parent { get; set; }

            public Tile(string[] lines)
            {
                if (lines[0].Length != lines.Length)
                    throw new ArgumentException("Tile must be quadratic");

                Image = new bool[lines.Length][];
                for (int i = 0; i < lines.Length; i++)
                    Image[i] = new bool[lines.Length];

                for (int y = 0; y < lines.Length; y++)
                    for (int x = 0; x < lines[y].Length; x++)
                        Image[x][y] = lines[y][x] == '#';

                SetBorders();
            }

            public void FlipAroundX()
            {
                for (int i = 0; i < Size; i++)
                    Image[i] = Image[i].Reverse().ToArray();

                SetBorders();
            }

            public void FlipAroundY()
            {
                Image = Image.Reverse().ToArray();

                SetBorders();
            }

            public void Rotate(int dir)
            {
                dir %= 4;
                if (dir == 3)
                    dir = -1;
                else if (dir == -3)
                    dir = 1;

                while (dir > 0)
                {
                    RotateCW();
                    dir--;
                }
                while (dir < 0)
                {
                    RotateCCW();
                    dir++;
                }
            }

            private void RotateCW()
            {
                var newImg = new bool[Size][];
                for (int i = 0; i < Size; i++)
                    newImg[i] = Image.Select(col => col[Size - 1 - i]).ToArray();
                Image = newImg;

                //SetBorders();
                bool[] temp = Borders[0];
                Borders[0] = Borders[3];
                Borders[3] = Borders[2];
                Borders[2] = Borders[1];
                Borders[1] = temp;
            }

            private void RotateCCW()
            {
                var newImg = new bool[Size][];
                for (int i = 0; i < Size; i++)
                    newImg[i] = Image.Select(col => col[i]).Reverse().ToArray();
                Image = newImg;

                //SetBorders();
                bool[] temp = Borders[0];
                Borders[0] = Borders[1];
                Borders[1] = Borders[2];
                Borders[2] = Borders[3];
                Borders[3] = temp;
            }

            private void SetBorders()
            {
                Borders = new bool[4][];
                Borders[0] = new bool[this.Size];
                Borders[1] = new bool[this.Size];
                Borders[2] = new bool[this.Size];
                Borders[3] = new bool[this.Size];

                for (int i = 0; i < Size; i++)
                {
                    Borders[0][i] = Image[i][0];
                    Borders[1][i] = Image[Size - 1][i];
                    Borders[2][i] = Image[Size - 1 - i][Size - 1];
                    Borders[3][i] = Image[0][Size - 1 - i];
                }
            }
        }
    }
}
