namespace GeneticAlgorithmDemoWeek1;

public static class FitnessCalculator
{
    public static double CalculateFitness(string target, Chromosome chromosome)
    {
        int score = 0;
        for (int i = 0; i < target.Length; i++)
        {
            if (chromosome.Genes[i].Value == target[i])
                score++;
        }
        return (double)score / target.Length;
    }
}