namespace GeneticAlgorithmDemoWeek1;

public class GeneticAlgorithm(
    string target,
    int populationSize = 100,
    double mutationRate = 0.01,
    int maxGenerations = 1000,
    bool useTournament = false)
{
    private List<Chromosome> _population = new();

    public void Run()
    {
        InitializePopulation();

        for (int generation = 0; generation < maxGenerations; generation++)
        {
            EvaluateFitness();

            var best = _population.OrderByDescending(c => c.Fitness).First();
            Console.WriteLine($"Gen {generation}: {best.Phrase} (Fitness: {best.Fitness:0.00})");

            if (best.Fitness >= 1.0)
            {
                Console.WriteLine("Target phrase evolved!");
                break;
            }

            var newPopulation = new List<Chromosome>();

            for (int i = 0; i < populationSize; i++)
            {
                var parentA = useTournament
                    ? SelectionStrategies.TournamentSelection(_population)
                    : SelectionStrategies.RouletteSelection(_population);

                var parentB = useTournament
                    ? SelectionStrategies.TournamentSelection(_population)
                    : SelectionStrategies.RouletteSelection(_population);

                var child = parentA.Crossover(parentB);
                child.Mutate(mutationRate);
                newPopulation.Add(child);
            }

            _population = newPopulation;
        }
    }

    private void InitializePopulation()
    {
        _population.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            _population.Add(new Chromosome(target.Length));
        }
    }

    private void EvaluateFitness()
    {
        foreach (var chromosome in _population)
        {
            chromosome.Fitness = FitnessCalculator.CalculateFitness(target, chromosome);
        }
    }
}