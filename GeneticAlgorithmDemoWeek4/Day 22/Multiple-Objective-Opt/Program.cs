
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiObjectiveGA
{
    public class Individual
    {
        public double[] Objectives { get; set; }
        public double CombinedFitness => Objectives.Sum();
        public string Genome { get; set; }

        public override string ToString()
        {
            return $"Genome: {Genome}, Objectives: [{string.Join(", ", Objectives.Select(x => x.ToString("F2")))}], CombinedFitness: {CombinedFitness:F2}";
        }
    }

    class Program
    {
        static Random random = new Random();
        const int PopulationSize = 10;
        const int Generations = 50;

        static void Main(string[] args)
        {
            var population = InitializePopulation();

            for (int gen = 0; gen < Generations; gen++)
            {
                foreach (var ind in population)
                {
                    ind.Objectives = EvaluateObjectives(ind.Genome);
                }

                population = population.OrderByDescending(i => i.CombinedFitness).ToList();

                Console.WriteLine($">>> Generation {gen}");
                foreach (var ind in population)
                    Console.WriteLine(ind);

                var newPopulation = new List<Individual> { population[0] }; // Elitism

                while (newPopulation.Count < PopulationSize)
                {
                    var p1 = TournamentSelection(population);
                    var p2 = TournamentSelection(population);
                    var child = Crossover(p1, p2);
                    child = Mutate(child);
                    newPopulation.Add(child);
                }

                population = newPopulation;
            }
        }

        static List<Individual> InitializePopulation()
        {
            var population = new List<Individual>();
            for (int i = 0; i < PopulationSize; i++)
            {
                string genome = new string(Enumerable.Range(0, 10)
                    .Select(_ => (char)random.Next(65, 91)).ToArray());
                population.Add(new Individual { Genome = genome });
            }
            return population;
        }

        static double[] EvaluateObjectives(string genome)
        {
            double objective1 = genome.Count(c => "AEIOU".Contains(c)); // vowel count
            double objective2 = genome.Distinct().Count(); // diversity
            return new[] { objective1, objective2 };
        }

        static Individual Crossover(Individual p1, Individual p2)
        {
            char[] child = new char[p1.Genome.Length];
            for (int i = 0; i < child.Length; i++)
            {
                child[i] = random.NextDouble() < 0.5 ? p1.Genome[i] : p2.Genome[i];
            }
            return new Individual { Genome = new string(child) };
        }

        static Individual Mutate(Individual individual)
        {
            char[] genes = individual.Genome.ToCharArray();
            if (random.NextDouble() < 0.1)
            {
                int index = random.Next(genes.Length);
                genes[index] = (char)random.Next(65, 91);
            }
            return new Individual { Genome = new string(genes) };
        }

        static Individual TournamentSelection(List<Individual> population)
        {
            int size = 3;
            return population
                .OrderBy(_ => random.Next())
                .Take(size)
                .OrderByDescending(i => i.CombinedFitness)
                .First();
        }
    }
}
