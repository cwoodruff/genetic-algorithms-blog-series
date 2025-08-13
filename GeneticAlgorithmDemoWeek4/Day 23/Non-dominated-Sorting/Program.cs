
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSGA2Demo
{
    public class Individual
    {
        public double[] Objectives { get; set; }
        public int Rank { get; set; }
        public double CrowdingDistance { get; set; }

        public Individual(double[] objectives)
        {
            Objectives = objectives;
        }

        public override string ToString()
        {
            return $"Obj: ({string.Join(", ", Objectives)}) | Rank: {Rank} | CD: {CrowdingDistance:F2}";
        }
    }

    public static class NSGA2
    {
        public static List<List<Individual>> FastNonDominatedSort(List<Individual> population)
        {
            var fronts = new List<List<Individual>>();
            var S = new Dictionary<Individual, List<Individual>>();
            var n = new Dictionary<Individual, int>();

            var rank = new Dictionary<Individual, int>();
            var front = new List<Individual>();

            foreach (var p in population)
            {
                S[p] = new List<Individual>();
                n[p] = 0;

                foreach (var q in population)
                {
                    if (Dominates(p, q))
                        S[p].Add(q);
                    else if (Dominates(q, p))
                        n[p]++;
                }

                if (n[p] == 0)
                {
                    rank[p] = 0;
                    p.Rank = 0;
                    front.Add(p);
                }
            }

            fronts.Add(front);
            int i = 0;
            while (i < fronts.Count)
            {
                var next = new List<Individual>();
                foreach (var p in fronts[i])
                {
                    foreach (var q in S[p])
                    {
                        n[q]--;
                        if (n[q] == 0)
                        {
                            q.Rank = i + 1;
                            next.Add(q);
                        }
                    }
                }

                if (next.Count > 0)
                    fronts.Add(next);
                i++;
            }

            return fronts;
        }

        private static bool Dominates(Individual a, Individual b)
        {
            bool betterInOne = false;
            for (int i = 0; i < a.Objectives.Length; i++)
            {
                if (a.Objectives[i] > b.Objectives[i]) return false;
                if (a.Objectives[i] < b.Objectives[i]) betterInOne = true;
            }
            return betterInOne;
        }

        public static void AssignCrowdingDistance(List<Individual> front)
        {
            int n = front[0].Objectives.Length;
            foreach (var ind in front)
                ind.CrowdingDistance = 0;

            for (int m = 0; m < n; m++)
            {
                front.Sort((a, b) => a.Objectives[m].CompareTo(b.Objectives[m]));
                front[0].CrowdingDistance = front[^1].CrowdingDistance = double.PositiveInfinity;

                double min = front[0].Objectives[m];
                double max = front[^1].Objectives[m];

                if (max == min) continue;

                for (int i = 1; i < front.Count - 1; i++)
                {
                    front[i].CrowdingDistance += 
                        (front[i + 1].Objectives[m] - front[i - 1].Objectives[m]) / (max - min);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var population = new List<Individual>
            {
                new Individual(new double[] { 1.0, 2.0 }),
                new Individual(new double[] { 1.5, 1.8 }),
                new Individual(new double[] { 2.0, 1.5 }),
                new Individual(new double[] { 2.5, 1.0 }),
                new Individual(new double[] { 0.5, 2.5 }),
                new Individual(new double[] { 1.1, 1.9 }),
                new Individual(new double[] { 1.7, 1.2 }),
                new Individual(new double[] { 0.9, 2.0 }),
            };

            var fronts = NSGA2.FastNonDominatedSort(population);

            for (int i = 0; i < fronts.Count; i++)
            {
                Console.WriteLine($"Front {i + 1}:");
                NSGA2.AssignCrowdingDistance(fronts[i]);
                foreach (var ind in fronts[i])
                {
                    Console.WriteLine(ind);
                }
                Console.WriteLine();
            }

            Console.WriteLine("NSGA-II completed.");
        }
    }
}
