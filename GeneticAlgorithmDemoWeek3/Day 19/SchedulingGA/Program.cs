
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchedulingGA
{
    public class Program
    {
        public static void Main()
        {
            var rand = new Random();

            var groups = new List<StudentGroup>
            {
                new StudentGroup("GroupA", new[] { "Math", "English" }),
                new StudentGroup("GroupB", new[] { "Biology", "History" }),
                new StudentGroup("GroupC", new[] { "Chemistry", "Physics" })
            };

            var timeSlots = Enumerable.Range(1, 6).ToList();
            var rooms = new[] { "Room1", "Room2" };

            var population = Enumerable.Range(0, 50)
                .Select(_ => Chromosome.Random(groups, timeSlots, rooms, rand))
                .ToList();

            foreach (var chromo in population)
                chromo.EvaluateFitness();

            for (int gen = 0; gen < 100; gen++)
            {
                population = population.OrderByDescending(c => c.Fitness).ToList();
                var nextGen = new List<Chromosome> { population[0] };

                while (nextGen.Count < population.Count)
                {
                    var p1 = Tournament(population, rand);
                    var p2 = Tournament(population, rand);
                    var child = p1.Crossover(p2, rand);
                    child.Mutate(rand, timeSlots, rooms);
                    child.EvaluateFitness();
                    nextGen.Add(child);
                }

                population = nextGen;
                if (gen % 10 == 0)
                    Console.WriteLine($"Generation {gen} Best Fitness: {population[0].Fitness:F4}");
            }

            Console.WriteLine("\nBest Schedule:");
            foreach (var g in population[0].Genes)
                Console.WriteLine($"{g.GroupName} - {g.Subject} in {g.Room} at time slot {g.TimeSlot}");
        }

        static Chromosome Tournament(List<Chromosome> pop, Random rand, int size = 3)
        {
            return pop.OrderBy(_ => rand.Next()).Take(size)
                .OrderByDescending(c => c.Fitness).First();
        }
    }

    public class StudentGroup
    {
        public string Name { get; }
        public List<string> Subjects { get; }

        public StudentGroup(string name, IEnumerable<string> subjects)
        {
            Name = name;
            Subjects = subjects.ToList();
        }
    }

    public class Gene
    {
        public string GroupName { get; set; }
        public string Subject { get; set; }
        public int TimeSlot { get; set; }
        public string Room { get; set; }
    }

    public class Chromosome
    {
        public List<Gene> Genes { get; set; }
        public double Fitness { get; set; }

        public static Chromosome Random(List<StudentGroup> groups, List<int> slots, string[] rooms, Random rand)
        {
            var genes = new List<Gene>();
            foreach (var group in groups)
            {
                foreach (var subject in group.Subjects)
                {
                    genes.Add(new Gene
                    {
                        GroupName = group.Name,
                        Subject = subject,
                        TimeSlot = slots[rand.Next(slots.Count)],
                        Room = rooms[rand.Next(rooms.Length)]
                    });
                }
            }
            return new Chromosome { Genes = genes };
        }

        public void EvaluateFitness()
        {
            double penalty = 0;
            var roomSchedule = new Dictionary<(int, string), List<string>>();
            var groupSchedule = new Dictionary<(int, string), List<string>>();

            foreach (var gene in Genes)
            {
                var roomKey = (gene.TimeSlot, gene.Room);
                var groupKey = (gene.TimeSlot, gene.GroupName);

                if (!roomSchedule.ContainsKey(roomKey))
                    roomSchedule[roomKey] = new List<string>();
                if (!groupSchedule.ContainsKey(groupKey))
                    groupSchedule[groupKey] = new List<string>();

                roomSchedule[roomKey].Add(gene.Subject);
                groupSchedule[groupKey].Add(gene.Subject);
            }

            penalty += roomSchedule.Values.Count(v => v.Count > 1) * 2;
            penalty += groupSchedule.Values.Count(v => v.Count > 1) * 3;

            Fitness = 1.0 / (1 + penalty);
        }

        public Chromosome Crossover(Chromosome other, Random rand)
        {
            var newGenes = new List<Gene>();
            for (int i = 0; i < Genes.Count; i++)
            {
                var gene = rand.NextDouble() < 0.5 ? Genes[i] : other.Genes[i];
                newGenes.Add(new Gene
                {
                    GroupName = gene.GroupName,
                    Subject = gene.Subject,
                    TimeSlot = gene.TimeSlot,
                    Room = gene.Room
                });
            }
            return new Chromosome { Genes = newGenes };
        }

        public void Mutate(Random rand, List<int> timeSlots, string[] rooms)
        {
            int i = rand.Next(Genes.Count);
            Genes[i].TimeSlot = timeSlots[rand.Next(timeSlots.Count)];
            Genes[i].Room = rooms[rand.Next(rooms.Length)];
        }
    }
}
