namespace GeneticAlgorithmDemoWeek1;

public static class SelectionStrategies
{
    private static readonly Random Random = new();

    public static Chromosome RouletteSelection(List<Chromosome> population)
    {
        double totalFitness = population.Sum(c => c.Fitness);
        double pick = Random.NextDouble() * totalFitness;
        double current = 0;

        foreach (var chromosome in population)
        {
            current += chromosome.Fitness;
            if (current >= pick)
                return chromosome;
        }

        return population.Last();
    }

    public static Chromosome TournamentSelection(List<Chromosome> population, int tournamentSize = 5)
    {
        var selected = population.OrderBy(_ => Random.Next()).Take(tournamentSize).ToList();
        return selected.OrderByDescending(c => c.Fitness).First();
    }
}