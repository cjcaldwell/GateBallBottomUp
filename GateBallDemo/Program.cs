using System;
using System.Linq;
using System.Transactions;

namespace GateBallDemo
{
    class Program
    {
        private const uint _MAX_BRANCH = 5;
        private const uint _MAX_DEPTH = 10;

        static void Main(string[] args)
        {
            Console.WriteLine($"Please enter a depth between 0 and {_MAX_DEPTH}");
            var input = Console.ReadLine();
            int depth;
            while (!(int.TryParse(input?.Split().First().Trim(), out depth) && depth <= _MAX_DEPTH))
            {
                Console.WriteLine($"Please enter a valid integer between 0 and {_MAX_DEPTH} for depth (or Ctrl+C to quit)");
                input = Console.ReadLine();
            }
            Console.WriteLine($"Please enter a branching factor between 1 and {_MAX_BRANCH}");
            input = Console.ReadLine();
            int branch;
            while (!(int.TryParse(input?.Split().First().Trim(), out branch) && branch <= _MAX_BRANCH))
            {
                Console.WriteLine($"Please enter a valid integer between 1 and {_MAX_BRANCH} for branching factor (or Ctrl+C to quit)");
                input = Console.ReadLine();
            }

            var tree = Tree.Create(branch, depth);

            var defaultBallCount = tree.Containers.Count - 1;
            var predictedEmpty = tree.Predict(defaultBallCount);
            Console.WriteLine($"Predicting that container {predictedEmpty} will be empty after {defaultBallCount} balls");

            Console.WriteLine("Get another prediction by entering a ball number, or hit enter to move on");
            input = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(input))
            {
                if (int.TryParse(input?.Split().First().Trim(), out var ballNum))
                {
                    var prediction = tree.Predict(ballNum);
                    Console.WriteLine($"Ball {ballNum} is predicted to land in container {prediction}");
                }
                else
                {
                    Console.WriteLine("Invalid ball number");
                }
                Console.WriteLine("Get another prediction by entering a ball number, or hit enter to move on");
                input = Console.ReadLine();
            }

            Console.WriteLine($"Enter a number of balls to run, or leave blank for default ({defaultBallCount})");
            input = Console.ReadLine();
            int ballCount;
            while (!(int.TryParse(input?.Split().First().Trim(), out ballCount) && ballCount >=0))
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    ballCount = defaultBallCount;
                    break;
                }
                Console.WriteLine($"Please enter a valid non-negative integer for number of balls to run, or leave blank for default ({defaultBallCount}");
                input = Console.ReadLine();
            }

            Console.WriteLine("Running balls...");
            foreach (var container in tree.RunBalls(ballCount))
            {
                Console.WriteLine($"Container {container.Id} now contains ball(s) {string.Join(",",container.Balls)}");
            }
            
            Console.WriteLine("Run complete.");
            var emptyContainers = tree.Containers.Where(container => !container.Balls.Any()).ToList();
            if (emptyContainers.Any())
            {
                Console.WriteLine($"Empty containers were: {string.Join(",", emptyContainers)}");
            }
            else
            {
                Console.WriteLine("No containers were empty");
            }
        }
    }
}
