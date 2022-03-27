using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._19
{
    [ProblemDate(2021, 19)]
    class Solver : ISolver
    {
        /** 
         * this is the first time I've seen that a simple test case breaks the program, but the actual input works perfectly fine
         * 
         * this happens because overlapping scanner-reports are aligned by choosing three of their overlapping points, which is enough,
         * AS LONG AS these three points aren't in a straight line
         * 
         * this edge case doesn't happen with the larger inputs, because the numbers are too big (and I probably also got lucky), 
         * but it does happen in the tiny first test case
         * */



        public string SolveFirst(string input)
        {
            List<ScannerReport> reportList = ParseInput(input);
            List<Coords> allBeacons = Assemble(reportList);
            return allBeacons.Count.ToString();
        }

        public string SolveSecond(string input)
        {
            List<ScannerReport> reportList = ParseInput(input);
            Assemble(reportList);

            int maxDistance = 0;
            for (int i = 0; i < reportList.Count; i++)
                for (int j = i + 1; j < reportList.Count; j++)
                    maxDistance = Math.Max(maxDistance, ManhattenDistance(reportList[i].ScannerCoordinates, reportList[j].ScannerCoordinates));

            return maxDistance.ToString();
        }

        private List<ScannerReport> ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var reportList = new List<ScannerReport>();
            for (int i = 1; i < lines.Length; i += 2)
            {
                var beacons = new List<Coords>();
                while (i < lines.Length && lines[i] != string.Empty)
                {
                    var vals = lines[i].Split(',');
                    beacons.Add(new Coords
                    {
                        X = int.Parse(vals[0]),
                        Y = int.Parse(vals[1]),
                        Z = int.Parse(vals[2])
                    });
                    i++;
                }

                var distances = new float[beacons.Count][];
                for (int j = 0; j < beacons.Count; j++)
                    distances[j] = new float[beacons.Count];
                var newReport = new ScannerReport
                {
                    Beacons = beacons.ToArray(),
                    Distances = distances
                };
                CalcAllDistances(newReport);

                reportList.Add(newReport);
            }
            return reportList;
        }

        private List<Coords> Assemble(List<ScannerReport> reportList)
        {
            var allBeacons = new List<Coords>();
            var frontier = new Queue<ScannerReport>();
            var remainingReports = new List<ScannerReport>(reportList);

            var referenceReport = reportList[0];
            referenceReport.ScannerCoordinates = new Coords
            {
                X = 0,
                Y = 0,
                Z = 0
            };

            allBeacons.AddRange(referenceReport.Beacons);
            frontier.Enqueue(referenceReport);
            remainingReports.Remove(referenceReport);

            while (frontier.Count > 0)
            {
                var currentReport = frontier.Dequeue();
                var overlappingReports = new List<ScannerReport>();
                for (int i = 0; i < remainingReports.Count; i++)
                {
                    var newReport = remainingReports[i];
                    if (TryFindOverlap(currentReport, newReport, out Coords[] originalBeacons, out Coords[] correspondingBeacons))
                    {
                        // figure out which way the new report is pointing (and pray that the three overlapping beacons found aren't in a straight line)
                        Orientation orientation = FindRelativeOrientation(originalBeacons, correspondingBeacons, out Coords referenceBeacon);

                        newReport.Beacons = FixOrientation(newReport.Beacons, orientation);
                        newReport.ScannerCoordinates = currentReport.ScannerCoordinates + originalBeacons[0] - referenceBeacon;

                        AddNewBeacons(allBeacons, newReport);

                        overlappingReports.Add(newReport); // keep track of overlapping reports as we can't remove them from the remainder list while iterating over it
                    }
                }

                foreach (var report in overlappingReports)
                {
                    frontier.Enqueue(report);
                    remainingReports.Remove(report);
                }
            }

            return allBeacons;
        }

        private void CalcAllDistances(ScannerReport report)
        {
            for (int i = 0; i < report.Beacons.Length; i++)
            {
                for (int j = i + 1; j < report.Beacons.Length; j++)
                {
                    report.Distances[i][j] = EuclidianDistance(report.Beacons[i], report.Beacons[j]);
                    report.Distances[j][i] = report.Distances[i][j];
                }
            }
        }

        private float EuclidianDistance(Coords point1, Coords point2)
        {
            Coords d = point1 - point2;

            float diag1 = (float)Math.Sqrt(d.X * d.X + d.Y * d.Y);
            float diag2 = (float)Math.Sqrt(diag1 * diag1 + d.Z * d.Z);

            return (float)Math.Round(diag2, 3);
        }

        private int ManhattenDistance(Coords point1, Coords point2)
        {
            int dx = Math.Abs(point1.X - point2.X);
            int dy = Math.Abs(point1.Y - point2.Y);
            int dz = Math.Abs(point1.Z - point2.Z);

            return dx + dy + dz;
        }

        private bool TryFindOverlap(ScannerReport currentReport, ScannerReport newReport, out Coords[] beaconsOfCurrent, out Coords[] correspondingBeaconsOfNew)
        {
            var beaconsCurr = new List<Coords>();
            var beaconsNew = new List<Coords>();
            for (int i = 0; i < currentReport.Distances.Length; i++)
            {
                if (TryFindMatch(currentReport.Distances[i], newReport.Distances, out int matchedRowIndex))
                {
                    beaconsCurr.Add(currentReport.Beacons[i]);
                    beaconsNew.Add(newReport.Beacons[matchedRowIndex]);
                    if (beaconsCurr.Count == 3)
                    {
                        beaconsOfCurrent = beaconsCurr.ToArray();
                        correspondingBeaconsOfNew = beaconsNew.ToArray();
                        return true;
                    }
                }
            }
            beaconsOfCurrent = null;
            correspondingBeaconsOfNew = null;
            return false;
        }

        private bool TryFindMatch(float[] distances, float[][] newDistances, out int matchedRowIndex)
        {
            for (int i = 0; i < newDistances.Length; i++)
            {
                int count = 0;
                foreach (float distance in distances)
                    if (newDistances[i].Contains(distance))
                        count++;

                if (count >= RequiredOverlaps)
                {
                    matchedRowIndex = i;
                    return true;
                }
            }
            matchedRowIndex = -1;
            return false;
        }

        private Orientation FindRelativeOrientation(Coords[] points1, Coords[] points2, out Coords referencePoint)
        {
            var targetDelta = GetDelta(points1);

            foreach (Axis axis in Enum.GetValues<Axis>())
                foreach (Direction direction in Enum.GetValues<Direction>())
                    foreach (Rotation rotation in Enum.GetValues<Rotation>())
                    {
                        var newOrientation = new Orientation
                        {
                            Axis = axis,
                            Direction = direction,
                            Rotation = rotation
                        };
                        var translatedPoints = FixOrientation(points2, newOrientation);

                        if (GetDelta(translatedPoints) == targetDelta)
                        {
                            referencePoint = translatedPoints[0];
                            return newOrientation;
                        }
                    }
            throw new ArgumentException("Second set of points cannot be rotated to match the first set of points.");
        }


        /// <summary>
        /// Returns the coordinates of points 1 and 2 relative to point 0
        /// </summary>
        private (int, int, int, int, int, int) GetDelta(Coords[] points)
        {
            return (
                points[1].X - points[0].X,
                points[1].Y - points[0].Y,
                points[1].Z - points[0].Z,
                points[2].X - points[0].X,
                points[2].Y - points[0].Y,
                points[2].Z - points[0].Z
            );
        }

        private Coords[] FixOrientation(Coords[] beacons, Orientation orientation)
        {
            if (orientation.Equals(StandardOrientation))
                return beacons;

            beacons = (Coords[])beacons.Clone(); //avoid nasty side effects

            // Step 1: Rotate towards x axis
            if (orientation.Axis == Axis.Y)
                for (int i = 0; i < beacons.Length; i++)
                    beacons[i] = ApplyRotation(beacons[i], Rotation.Right, Axis.Z);

            else if (orientation.Axis == Axis.Z)
                for (int i = 0; i < beacons.Length; i++)
                    beacons[i] = ApplyRotation(beacons[i], Rotation.Right, Axis.Y);

            // Step 2: Flip around if neccessairy
            if (orientation.Direction == Direction.Backward)
                for (int i = 0; i < beacons.Length; i++)
                    beacons[i] = ApplyRotation(beacons[i], Rotation.Down, Axis.Z);

            // Step 3: Make it point upwards
            for (int i = 0; i < beacons.Length; i++)
                beacons[i] = ApplyRotation(beacons[i], orientation.Rotation, Axis.X);

            return beacons;
        }

        private Coords ApplyRotation(Coords point, Rotation rotation, Axis axis)
        {
            int[] matrix = RotationMatrix[rotation];
            return axis switch
            {
                Axis.X => new Coords
                {
                    X = point.X,
                    Y = (point.Y * matrix[0]) + (point.Z * matrix[1]),
                    Z = (point.Y * matrix[2]) + (point.Z * matrix[3])
                },
                Axis.Y => new Coords
                {
                    X = (point.X * matrix[0]) + (point.Z * matrix[1]),
                    Y = point.Y,
                    Z = (point.X * matrix[2]) + (point.Z * matrix[3])
                },
                Axis.Z => new Coords
                {
                    X = (point.X * matrix[0]) + (point.Y * matrix[1]),
                    Y = (point.X * matrix[2]) + (point.Y * matrix[3]),
                    Z = point.Z
                },
                _ => throw new ArgumentException("Invalid Axis")
            };
        }

        private void AddNewBeacons(List<Coords> allBeacons, ScannerReport newReport)
        {
            foreach (Coords newBeacon in newReport.Beacons)
            {
                Coords absolutePosition = newReport.ScannerCoordinates + newBeacon;
                if (!allBeacons.Contains(absolutePosition))
                    allBeacons.Add(absolutePosition);
            }
        }

        private class ScannerReport
        {
            public Coords[] Beacons { get; set; }
            public float[][] Distances { get; set; }
            public Coords ScannerCoordinates { get; set; }
        }
        private struct Coords
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public static Coords operator +(Coords a, Coords b) => new()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
            public static Coords operator -(Coords a, Coords b) => new()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
            //public static bool operator ==(Coords a, Coords b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
            //public static bool operator !=(Coords a, Coords b) => !(a == b);
        }
        private struct Orientation
        {
            public Rotation Rotation { get; set; }
            public Axis Axis { get; set; }
            public Direction Direction { get; set; }
        }
        private enum Rotation { Up, Right, Down, Left }
        private enum Axis { X, Y, Z }
        private enum Direction { Forward, Backward }

        private const int RequiredOverlaps = 12;
        private static readonly Orientation StandardOrientation = new()
        {
            Axis = Axis.X,
            Direction = Direction.Forward,
            Rotation = Rotation.Up
        };
        private static readonly Dictionary<Rotation, int[]> RotationMatrix = new()
        {
            { Rotation.Up, new int[] { 1, 0, 0, 1 } },
            { Rotation.Right, new int[] { 0, -1, 1, 0 } },
            { Rotation.Down, new int[] { -1, 0, 0, -1 } },
            { Rotation.Left, new int[] { 0, 1, -1, 0 } }
        };
    }
}
