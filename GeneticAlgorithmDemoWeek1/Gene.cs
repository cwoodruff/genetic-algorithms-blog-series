namespace GeneticAlgorithmDemoWeek1;

public class Gene
{
    public char Value { get; private set; }
    private static readonly char[] AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ,!".ToCharArray();
    private static readonly Random Random = new();

    public Gene()
    {
        Value = AllowedChars[Random.Next(AllowedChars.Length)];
    }

    private Gene(char value)
    {
        Value = value;
    }

    public Gene Mutate()
    {
        return new Gene(AllowedChars[Random.Next(AllowedChars.Length)]);
    }
}