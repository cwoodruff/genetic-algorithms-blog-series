namespace GeneticAlgorithmDemoWeek1;

public class Chromosome
{
    public Gene[] Genes { get; private set; }
    public string Phrase => string.Concat(Genes.Select(g => g.Value));
    public double Fitness { get; set; }

    private static readonly Random Random = new();

    public Chromosome(int length)
    {
        Genes = new Gene[length];
        for (int i = 0; i < length; i++)
            Genes[i] = new Gene();
    }

    private Chromosome(Gene[] genes)
    {
        Genes = genes;
    }

    public Chromosome Crossover(Chromosome partner)
    {
        var childGenes = new Gene[Genes.Length];
        int midpoint = Random.Next(Genes.Length);

        for (int i = 0; i < Genes.Length; i++)
        {
            childGenes[i] = i < midpoint ? Genes[i] : partner.Genes[i];
        }

        return new Chromosome(childGenes);
    }

    public void Mutate(double mutationRate)
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            if (Random.NextDouble() < mutationRate)
                Genes[i] = Genes[i].Mutate();
        }
    }
}