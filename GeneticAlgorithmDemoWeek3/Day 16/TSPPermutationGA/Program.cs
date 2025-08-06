
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace TSPPermutationGA
{
    class Program
    {
        static void Main()
        {
            var cities = GenerateCities(10);
            var distanceMatrix = CalculateDistanceMatrix(cities);
            var rand = new Random();

            int populationSize = 100;
            int generations = 300;

            var population = Enumerable.Range(0, populationSize)
                .Select(_ => Chromosome.Random(cities.Length, rand))
                .ToList();

            foreach (var chromo in population)
                chromo.EvaluateFitness(distanceMatrix);

            for (int gen = 0; gen < generations; gen++)
            {
                var nextGen = new List<Chromosome>();

                population = population.OrderByDescending(c => c.Fitness).ToList();
                Console.WriteLine($"Gen {gen + 1} Best Distance: {1.0 / population[0].Fitness:F2}");

                nextGen.Add(population[0]); // elitism

                while (nextGen.Count < populationSize)
                {
                    var parent1 = Tournament(population, rand);
                    var parent2 = Tournament(population, rand);

                    var child = parent1.Crossover(parent2, rand);
                    child.Mutate(rand);
                    child.EvaluateFitness(distanceMatrix);
                    nextGen.Add(child);
                }

                population = nextGen;
            }

            var best = population.OrderByDescending(c => c.Fitness).First();
            Console.WriteLine("Best Route:");
            Console.WriteLine(string.Join(" -> ", best.Genes));
        }

        static PointF[] GenerateCities(int count)
        {
            var rand = new Random();
            return Enumerable.Range(0, count)
                .Select(_ => new PointF(rand.Next(100), rand.Next(100)))
                .ToArray();
        }

        static double[,] CalculateDistanceMatrix(PointF[] cities)
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

        static Chromosome Tournament(List<Chromosome> population, Random rand, int size = 5)
        {
            var group = new List<Chromosome>();
            for (int i = 0; i < size; i++)
                group.Add(population[rand.Next(population.Count)]);
            return group.OrderByDescending(c => c.Fitness).First();
        }
    }

    class Chromosome
    {
        public int[] Genes { get; set; }
        public double Fitness { get; set; }

        public static Chromosome Random(int length, Random rand)
        {
            var genes = Enumerable.Range(0, length).OrderBy(_ => rand.Next()).ToArray();
            return new Chromosome { Genes = genes };
        }

        public void EvaluateFitness(double[,] distances)
        {
            double dist = 0;
            for (int i = 0; i < Genes.Length - 1; i++)
                dist += distances[Genes[i], Genes[i + 1]];
            dist += distances[Genes[^1], Genes[0]];

            Fitness = 1.0 / dist;
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
