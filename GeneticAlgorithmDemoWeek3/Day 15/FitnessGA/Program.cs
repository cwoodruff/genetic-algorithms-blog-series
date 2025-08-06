
using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessGA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var target = "EVOLVE";
            var populationSize = 100;
            var generations = 1000;
            var rand = new Random();

            var population = Enumerable.Range(0, populationSize)
                .Select(_ => Chromosome.Random(target.Length, rand))
                .ToList();

            foreach (var chromo in population)
                chromo.EvaluateFitness(target);

            for (int gen = 0; gen < generations; gen++)
            {
                population = population.OrderByDescending(c => c.Fitness).ToList();
                Console.WriteLine($"Gen {gen + 1}: {population[0].Genes} ({population[0].Fitness})");

                if (population[0].Genes == target)
                    break;

                var nextGen = new List<Chromosome>();

                // Elitism
                nextGen.Add(population[0]);

                // Generate rest of the next generation
                while (nextGen.Count < populationSize)
                {
                    var parent1 = TournamentSelection(population, rand);
                    var parent2 = TournamentSelection(population, rand);
                    var child = parent1.Crossover(parent2, rand);
                    child.Mutate(rand);
                    child.EvaluateFitness(target);
                    nextGen.Add(child);
                }

                population = nextGen;
            }
        }

        public static Chromosome TournamentSelection(List<Chromosome> population, Random rand, int tournamentSize = 5)
        {
            var sample = new List<Chromosome>();
            for (int i = 0; i < tournamentSize; i++)
            {
                sample.Add(population[rand.Next(population.Count)]);
            }
            return sample.OrderByDescending(c => c.Fitness).First();
        }
    }

    public class Chromosome
    {
        public string Genes { get; set; }
        public double Fitness { get; set; }

        private static char RandomChar(Random rand)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return chars[rand.Next(chars.Length)];
        }

        public static Chromosome Random(int length, Random rand)
        {
            var genes = new string(Enumerable.Range(0, length)
                .Select(_ => RandomChar(rand)).ToArray());
            return new Chromosome { Genes = genes };
        }

        public void EvaluateFitness(string target)
        {
            int score = 0;
            for (int i = 0; i < Genes.Length; i++)
            {
                if (Genes[i] == target[i])
                    score++;
            }

            Fitness = (double)score / target.Length;
        }

        public Chromosome Crossover(Chromosome other, Random rand)
        {
            var result = new char[Genes.Length];
            for (int i = 0; i < Genes.Length; i++)
            {
                result[i] = rand.NextDouble() < 0.5 ? Genes[i] : other.Genes[i];
            }

            return new Chromosome { Genes = new string(result) };
        }

        public void Mutate(Random rand, double mutationRate = 0.1)
        {
            var chars = Genes.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (rand.NextDouble() < mutationRate)
                {
                    chars[i] = RandomChar(rand);
                }
            }
            Genes = new string(chars);
        }
    }
}
