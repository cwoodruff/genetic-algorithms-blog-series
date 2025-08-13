
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridMemeticGA
{
    class Program
    {
        static Random random = new Random();
        const string target = "HELLO WORLD";
        const int populationSize = 100;
        const int generations = 1000;
        const double mutationRate = 0.01;

        static void Main()
        {
            var population = InitializePopulation();
            for (int generation = 0; generation < generations; generation++)
            {
                foreach (var individual in population)
                {
                    individual.Fitness = CalculateFitness(individual.Genes);
                }

                var newPopulation = new List<Individual>();

                var sorted = population.OrderByDescending(x => x.Fitness).ToList();
                newPopulation.Add(sorted[0]); // Elitism

                while (newPopulation.Count < populationSize)
                {
                    var parent1 = TournamentSelection(population);
                    var parent2 = TournamentSelection(population);
                    var child = Crossover(parent1, parent2);
                    if (random.NextDouble() < mutationRate)
                        child = Mutate(child);

                    child = HillClimb(child);
                    newPopulation.Add(child);
                }

                population = newPopulation;

                var best = population.OrderByDescending(x => x.Fitness).First();
                Console.WriteLine($"Gen {generation}: {best.Genes} ({best.Fitness})");

                if (best.Fitness >= target.Length)
                    break;
            }
        }

        static List<Individual> InitializePopulation()
        {
            var population = new List<Individual>();
            for (int i = 0; i < populationSize; i++)
            {
                var genes = new string(Enumerable.Range(0, target.Length)
                    .Select(_ => (char)random.Next(32, 127)).ToArray());
                population.Add(new Individual { Genes = genes });
            }
            return population;
        }

        static double CalculateFitness(string genes)
        {
            int score = 0;
            for (int i = 0; i < genes.Length; i++)
            {
                if (genes[i] == target[i])
                    score++;
            }
            return score;
        }

        static Individual Crossover(Individual parent1, Individual parent2)
        {
            var childGenes = new StringBuilder();
            for (int i = 0; i < parent1.Genes.Length; i++)
            {
                childGenes.Append(random.NextDouble() < 0.5 ? parent1.Genes[i] : parent2.Genes[i]);
            }
            return new Individual { Genes = childGenes.ToString() };
        }

        static Individual Mutate(Individual individual)
        {
            var genes = individual.Genes.ToCharArray();
            int index = random.Next(genes.Length);
            genes[index] = (char)random.Next(32, 127);
            return new Individual { Genes = new string(genes) };
        }

        static Individual HillClimb(Individual individual)
        {
            var best = individual;
            for (int i = 0; i < 5; i++) // Local search depth
            {
                var neighbor = Mutate(best);
                neighbor.Fitness = CalculateFitness(neighbor.Genes);
                if (neighbor.Fitness > best.Fitness)
                    best = neighbor;
            }
            return best;
        }

        static Individual TournamentSelection(List<Individual> population)
        {
            int tournamentSize = 5;
            var tournament = new List<Individual>();
            for (int i = 0; i < tournamentSize; i++)
            {
                tournament.Add(population[random.Next(population.Count)]);
            }
            return tournament.OrderByDescending(x => x.Fitness).First();
        }
    }

    class Individual
    {
        public string Genes { get; set; }
        public double Fitness { get; set; }
    }
}
