
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstraintFitnessGA
{
    class Program
    {
        static void Main()
        {
            var rand = new Random();
            var population = Enumerable.Range(0, 50)
                .Select(_ => Chromosome.Random(rand))
                .ToList();

            foreach (var chromo in population)
                chromo.EvaluateFitness();

            for (int gen = 0; gen < 100; gen++)
            {
                population = population.OrderByDescending(c => c.Fitness).ToList();
                Console.WriteLine($"Gen {gen + 1}: Best fitness = {population[0].Fitness:F4}");

                var nextGen = new List<Chromosome> { population[0] };

                while (nextGen.Count < population.Count)
                {
                    var p1 = Tournament(population, rand);
                    var p2 = Tournament(population, rand);
                    var child = p1.Crossover(p2, rand);
                    child.Mutate(rand);
                    child.EvaluateFitness();
                    nextGen.Add(child);
                }

                population = nextGen;
            }

            Console.WriteLine("Best Chromosome:");
            Console.WriteLine(string.Join(", ", population[0].Genes));
        }

        static Chromosome Tournament(List<Chromosome> pop, Random rand, int size = 3)
        {
            return pop.OrderBy(_ => rand.Next()).Take(size)
                .OrderByDescending(c => c.Fitness).First();
        }
    }

    public class Chromosome
    {
        public int[] Genes { get; set; }
        public double Fitness { get; set; }

        public static Chromosome Random(Random rand)
        {
            var genes = Enumerable.Range(0, 5).Select(_ => rand.Next(0, 10)).ToArray();
            return new Chromosome { Genes = genes };
        }

        public void EvaluateFitness()
        {
            int sum = Genes.Sum();
            bool constraintViolated = sum > 25;

            double baseScore = Genes.Distinct().Count(); // prefer diversity
            double penalty = constraintViolated ? 5.0 : 0.0;

            Fitness = baseScore / (1.0 + penalty);
        }

        public Chromosome Crossover(Chromosome other, Random rand)
        {
            int[] child = new int[Genes.Length];
            for (int i = 0; i < Genes.Length; i++)
            {
                child[i] = rand.NextDouble() < 0.5 ? Genes[i] : other.Genes[i];
            }
            return new Chromosome { Genes = child };
        }

        public void Mutate(Random rand)
        {
            int index = rand.Next(Genes.Length);
            Genes[index] = rand.Next(0, 10);
        }
    }
}
