using System;
using System.Collections.Generic;
using System.Linq;
using GateBallDemo.Helpers;
using GateBallDemo.Model;

namespace GateBallDemo
{
    public class Tree
    {
        public Tree(INode rootNode, List<Container> containers)
        {
            RootNode = rootNode;
            Containers = containers;
        }

        public INode RootNode { get; }
        public List<Container> Containers { get; }

        public static Tree Create(int branch, int depth)
        {
            var containers = new List<Container>();
            var currentLayer = Enumerable.Range(0, int.MaxValue)
                .Select(id => new Container(id))
                .Do(containers.Add)
                .Cast<INode>();

            var random = new Random();
            var randoms = MissingLinq.RepeatCall(() => random.Next(branch));

            for (var i = 0; i < depth; ++i)
            {
                currentLayer = currentLayer.Batch(branch)
                    .Zip(randoms, (children, direction) => new Gate(direction, children));
            }

            var rootNode = currentLayer.First();
            return new Tree(rootNode, containers);
        }

        public Container Predict(int ballNum)
        {
            var node = RootNode;
            while (node is Gate gate)
            {
                node = gate.PredictNthBall(ballNum, out ballNum);
            }

            if (node is Container container)
            {
                return container;
            }

            throw new Exception("Invalid tree, found path that does not lead to a container");
        }

        public Container RunBall(Ball ball)
        {
            var node = RootNode;
            while (node is Gate gate)
            {
                node = gate.RunBall();
            }

            if (node is Container container)
            {
                container.ReceiveBall(ball);
                return container;
            }
            throw new Exception("Invalid tree, found path that does not lead to a container");
        }

        public IEnumerable<Container> RunBalls(int ballCount)
        {
            foreach (var ball in Enumerable.Range(0, ballCount).Select(id => new Ball(id)))
            {
                yield return RunBall(ball);
            }
        }
    }
}