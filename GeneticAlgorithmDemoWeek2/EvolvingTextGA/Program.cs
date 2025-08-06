
using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolvingTextGA
{
    public class Program
    {
        public static void Main()
        {
            var target = "HELLO WORLD";
            int populationSize = 100;
            double mutationRate = 0.01;
            int maxGenerations = 1000;
            var rand = new Random();

            var population = Enumerable.Range(0, populationSize)
                .Select(_ => Chromosome.CreateRandom(target.Length, rand))
                .ToList();

            for (int gen = 0; gen < maxGenerations; gen++)
            {
                foreach (var c in population)
                    c.EvaluateFitness(target);

                population = population.OrderByDescending(c => c.Fitness).ToList();
                var best = population.First();

                Console.WriteLine($"Gen {gen + 1}: {best.Genes} (Fitness: {best.Fitness:F4})");

                if (best.Genes == target)
                    break;

                var nextGen = new List<Chromosome> { best }; // Elitism

                while (nextGen.Count < populationSize)
                {
                    var parent1 = Tournament(population, rand);
                    var parent2 = Tournament(population, rand);
                    var child = parent1.Crossover(parent2, rand);
                    child.Mutate(mutationRate, rand);
                    nextGen.Add(child);
                }

                population = nextGen;
            }
        }

        private static Chromosome Tournament(List<Chromosome> population, Random rand, int size = 5)
        {
            return population.OrderBy(_ => rand.Next())
                             .Take(size)
                             .OrderByDescending(c => c.Fitness)
                             .First();
        }
    }

    public class Chromosome
    {
        private static readonly string Charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
        public string Genes { get; set; }
        public double Fitness { get; set; }

        public static Chromosome CreateRandom(int length, Random rand)
        {
            var genes = new string(Enumerable.Range(0, length)
                .Select(_ => Charset[rand.Next(Charset.Length)])
                .ToArray());
            return new Chromosome { Genes = genes };
        }

        public void EvaluateFitness(string target)
        {
            int score = Genes.Zip(target, (a, b) => a == b ? 1 : 0).Sum();
            Fitness = (double)score / target.Length;
        }

        public Chromosome Crossover(Chromosome other, Random rand)
        {
            var childGenes = new char[Genes.Length];
            for (int i = 0; i < Genes.Length; i++)
                childGenes[i] = rand.NextDouble() < 0.5 ? Genes[i] : other.Genes[i];

            return new Chromosome { Genes = new string(childGenes) };
        }

        public void Mutate(double rate, Random rand)
        {
            var genes = Genes.ToCharArray();
            for (int i = 0; i < genes.Length; i++)
            {
                if (rand.NextDouble() < rate)
                    genes[i] = Charset[rand.Next(Charset.Length)];
            }
            Genes = new string(genes);
        }
    }
}
