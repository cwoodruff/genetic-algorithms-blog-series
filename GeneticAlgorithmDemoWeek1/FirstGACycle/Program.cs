
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstGACycle
{
    public class Program
    {
        public static void Main()
        {
            var target = "HELLO WORLD";
            var rand = new Random();
            int populationSize = 100;
            double mutationRate = 0.01;
            int generations = 1000;

            var population = Enumerable.Range(0, populationSize)
                .Select(_ => Chromosome.Random(target.Length, rand))
                .ToList();

            for (int gen = 0; gen < generations; gen++)
            {
                foreach (var c in population)
                    c.EvaluateFitness(target);

                population = population.OrderByDescending(c => c.Fitness).ToList();
                Console.WriteLine($"Gen {gen}: {population[0].Genes} (Fitness: {population[0].Fitness:F4})");

                if (population[0].Genes == target)
                    break;

                var nextGen = new List<Chromosome> { population[0] }; // Elitism

                while (nextGen.Count < populationSize)
                {
                    var p1 = Tournament(population, rand);
                    var p2 = Tournament(population, rand);
                    var child = p1.Crossover(p2, rand);
                    child.Mutate(mutationRate, rand);
                    nextGen.Add(child);
                }

                population = nextGen;
            }
        }

        private static Chromosome Tournament(List<Chromosome> pop, Random rand, int size = 5)
        {
            return pop.OrderBy(_ => rand.Next()).Take(size)
                      .OrderByDescending(c => c.Fitness).First();
        }
    }

    public class Chromosome
    {
        public string Genes { get; set; }
        public double Fitness { get; set; }

        private static string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

        public static Chromosome Random(int length, Random rand)
        {
            var genes = new string(Enumerable.Range(0, length)
                .Select(_ => Chars[rand.Next(Chars.Length)]).ToArray());
            return new Chromosome { Genes = genes };
        }

        public void EvaluateFitness(string target)
        {
            int score = Genes.Zip(target, (a, b) => a == b ? 1 : 0).Sum();
            Fitness = (double)score / target.Length;
        }

        public Chromosome Crossover(Chromosome partner, Random rand)
        {
            var childGenes = new char[Genes.Length];
            int midpoint = rand.Next(Genes.Length);
            for (int i = 0; i < Genes.Length; i++)
                childGenes[i] = i < midpoint ? Genes[i] : partner.Genes[i];
            return new Chromosome { Genes = new string(childGenes) };
        }

        public void Mutate(double rate, Random rand)
        {
            var genes = Genes.ToCharArray();
            for (int i = 0; i < genes.Length; i++)
            {
                if (rand.NextDouble() < rate)
                    genes[i] = Chars[rand.Next(Chars.Length)];
            }
            Genes = new string(genes);
        }
    }
}
