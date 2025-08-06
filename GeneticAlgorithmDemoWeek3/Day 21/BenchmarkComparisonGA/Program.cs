
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BenchmarkComparisonGA
{
    class Program
    {
        static void Main()
        {
            var cities = GenerateCities(11);
            var distanceMatrix = CalculateDistanceMatrix(cities);

            Console.WriteLine("Running Genetic Algorithm...");
            var gaWatch = Stopwatch.StartNew();
            var gaBest = RunGA(distanceMatrix);
            gaWatch.Stop();
            Console.WriteLine($"GA Best Path: {string.Join(" -> ", gaBest.Path)}");
            Console.WriteLine($"GA Distance: {gaBest.Distance:F2}");
            Console.WriteLine($"GA Time: {gaWatch.ElapsedMilliseconds}ms");

            Console.WriteLine();

            Console.WriteLine("Running Brute Force...");
            var bruteWatch = Stopwatch.StartNew();
            var bruteBest = RunBruteForce(distanceMatrix);
            bruteWatch.Stop();
            Console.WriteLine($"Brute Best Path: {string.Join(" -> ", bruteBest.Path)}");
            Console.WriteLine($"Brute Distance: {bruteBest.Distance:F2}");
            Console.WriteLine($"Brute Time: {bruteWatch.ElapsedMilliseconds}ms");
        }

        static double[,] CalculateDistanceMatrix((double X, double Y)[] cities)
        {
            int n = cities.Length;
            var matrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] = Math.Sqrt(Math.Pow(cities[i].X - cities[j].X, 2) + Math.Pow(cities[i].Y - cities[j].Y, 2));
            return matrix;
        }

        static (double X, double Y)[] GenerateCities(int count)
        {
            var rand = new Random();
            return Enumerable.Range(0, count)
                .Select(_ => (rand.NextDouble() * 100, rand.NextDouble() * 100))
                .ToArray();
        }

        static (List<int> Path, double Distance) RunBruteForce(double[,] matrix)
        {
            var cities = Enumerable.Range(0, matrix.GetLength(0)).ToArray();
            double bestDistance = double.MaxValue;
            List<int> bestPath = null;

            foreach (var perm in Permute(cities, 0))
            {
                double dist = 0;
                for (int i = 0; i < perm.Count - 1; i++)
                    dist += matrix[perm[i], perm[i + 1]];
                dist += matrix[perm[^1], perm[0]];

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestPath = new List<int>(perm);
                }
            }

            return (bestPath, bestDistance);
        }

        static IEnumerable<List<int>> Permute(int[] arr, int start)
        {
            if (start == arr.Length - 1)
                yield return arr.ToList();
            else
            {
                for (int i = start; i < arr.Length; i++)
                {
                    (arr[start], arr[i]) = (arr[i], arr[start]);
                    foreach (var perm in Permute(arr, start + 1))
                        yield return perm;
                    (arr[start], arr[i]) = (arr[i], arr[start]);
                }
            }
        }

        static (List<int> Path, double Distance) RunGA(double[,] matrix)
        {
            var rand = new Random();
            var population = Enumerable.Range(0, 100)
                .Select(_ => Chromosome.Random(matrix.GetLength(0), rand))
                .ToList();

            foreach (var chromo in population)
                chromo.Evaluate(matrix);

            for (int gen = 0; gen < 300; gen++)
            {
                population = population.OrderByDescending(c => c.Fitness).ToList();
                var nextGen = new List<Chromosome> { population[0] };

                while (nextGen.Count < population.Count)
                {
                    var p1 = Tournament(population, rand);
                    var p2 = Tournament(population, rand);
                    var child = p1.Crossover(p2, rand);
                    child.Mutate(rand);
                    child.Evaluate(matrix);
                    nextGen.Add(child);
                }

                population = nextGen;
            }

            var best = population.OrderByDescending(c => c.Fitness).First();
            return (best.Genes.ToList(), best.GetDistance(matrix));
        }

        static Chromosome Tournament(List<Chromosome> population, Random rand, int size = 5)
        {
            return population.OrderBy(_ => rand.Next())
                .Take(size)
                .OrderByDescending(c => c.Fitness)
                .First();
        }
    }

    class Chromosome
    {
        public int[] Genes { get; set; }
        public double Fitness { get; set; }

        public void Evaluate(double[,] matrix)
        {
            double dist = GetDistance(matrix);
            Fitness = 1.0 / dist;
        }

        public double GetDistance(double[,] matrix)
        {
            double total = 0;
            for (int i = 0; i < Genes.Length - 1; i++)
                total += matrix[Genes[i], Genes[i + 1]];
            total += matrix[Genes[^1], Genes[0]];
            return total;
        }

        public static Chromosome Random(int length, Random rand)
        {
            var genes = Enumerable.Range(0, length).OrderBy(_ => rand.Next()).ToArray();
            return new Chromosome { Genes = genes };
        }

        public Chromosome Crossover(Chromosome other, Random rand)
        {
            int n = Genes.Length;
            var child = new int[n];
            var used = new HashSet<int>();

            int start = rand.Next(n);
            int end = rand.Next(start, n);

            for (int i = start; i <= end; i++)
            {
                child[i] = Genes[i];
                used.Add(Genes[i]);
            }

            int fillIdx = 0;
            for (int i = 0; i < n; i++)
            {
                if (!used.Contains(other.Genes[i]))
                {
                    while (fillIdx >= start && fillIdx <= end)
                        fillIdx++;
                    child[fillIdx++] = other.Genes[i];
                }
            }

            return new Chromosome { Genes = child };
        }

        public void Mutate(Random rand)
        {
            int a = rand.Next(Genes.Length);
            int b = rand.Next(Genes.Length);
            (Genes[a], Genes[b]) = (Genes[b], Genes[a]);
        }
    }
}
