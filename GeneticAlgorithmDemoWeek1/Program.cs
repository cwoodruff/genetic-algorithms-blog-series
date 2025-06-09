using GeneticAlgorithmDemoWeek1;

Console.WriteLine("Enter target phrase:");
var target = Console.ReadLine() ?? "hello world";

var ga = new GeneticAlgorithm(
    target: target.ToLower(),
    populationSize: 200,
    mutationRate: 0.01,
    maxGenerations: 1000,
    useTournament: false
);

ga.Run();