
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeuristicGA
{
    class Program
    {
        static void Main()
        {
            var cities = GenerateCities(10);
            var distanceMatrix = ComputeDistanceMatrix(cities);
            var rand = new Random();

            var greedyChromo = Chromosome.Greedy(distanceMatrix);
            greedyChromo.EvaluateFitness(distanceMatrix);
            Console.WriteLine("Greedy Chromosome: " + string.Join(" -> ", greedyChromo.Genes));
            Console.WriteLine($"Total Distance: {1.0 / greedyChromo.Fitness:F2}");

            var randomChromo = Chromosome.Random(cities.Length, rand);
            randomChromo.EvaluateFitness(distanceMatrix);
            Console.WriteLine("Random Chromosome: " + string.Join(" -> ", randomChromo.Genes));
            Console.WriteLine($"Total Distance: {1.0 / randomChromo.Fitness:F2}");
        }

        static PointF[] GenerateCities(int count)
        {
            var rand = new Random();
            return Enumerable.Range(0, count)
                .Select(_ => new PointF(rand.Next(100), rand.Next(100)))
                .ToArray();
        }

        static double[,] ComputeDistanceMatrix(PointF[] cities)
        {
            int n = cities.Length;
            var matrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] = Distance(cities[i], cities[j]);
            return matrix;
        }

        static double Distance(PointF a, PointF b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    class Chromosome
    {
        public int[] Genes { get; set; }
        public double Fitness { get; set; }

        public void EvaluateFitness(double[,] matrix)
        {
            double dist = 0;
            for (int i = 0; i < Genes.Length - 1; i++)
                dist += matrix[Genes[i], Genes[i + 1]];
            dist += matrix[Genes[^1], Genes[0]];
            Fitness = 1.0 / dist;
        }

        public static Chromosome Random(int length, Random rand)
        {
            var genes = Enumerable.Range(0, length).OrderBy(_ => rand.Next()).ToArray();
            return new Chromosome { Genes = genes };
        }

        public static Chromosome Greedy(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            var visited = new HashSet<int>();
            var route = new List<int> { 0 };
            visited.Add(0);

            while (route.Count < n)
            {
                int last = route[^1];
                double minDist = double.MaxValue;
                int next = -1;

                for (int i = 0; i < n; i++)
                {
                    if (!visited.Contains(i) && matrix[last, i] < minDist)
                    {
                        minDist = matrix[last, i];
                        next = i;
                    }
                }

                route.Add(next);
                visited.Add(next);
            }

            return new Chromosome { Genes = route.ToArray() };
        }
    }
}
