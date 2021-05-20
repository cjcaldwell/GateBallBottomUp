using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GateBallDemo.Helpers;
using GateBallDemo.Model;
using Xunit;

namespace GateBallDemo.Test
{
    public class TreeTests
    {
        [Fact]
        public void GivenDepthZero_WhenTreeCreated_RootNodeShouldBeContainer()
        {
            var tree = Tree.Create(1, 0);
            tree.RootNode.Should().BeOfType<Container>();
        }

        [Fact]
        public void GivenZeroDepthTree_WhenPredicting_AlwaysReturnsRootNode()
        {
            var tree = Tree.Create(1, 0);
            new[] {0, 10, 5000}.Select(tree.Predict).Should().AllBeEquivalentTo(tree.RootNode);
        }

        [Fact]
        public void GivenDeepTreeWithBranchFactorOne_WhenPredicting_AlwaysReturnsOnlyContainer()
        {
            var tree = Tree.Create(1, 10);
            tree.Containers.Should().HaveCount(1);
            var onlyContainer = tree.Containers.First();
            new[] {0, 10, 5000}.Select(tree.Predict).Should().AllBeEquivalentTo(onlyContainer);
        }

        [Fact]
        public void GivenDeepTreeWithBranchFactorOne_WhenRunning_AlwaysReturnsOnlyContainer()
        {
            var tree = Tree.Create(1, 100);
            tree.Containers.Should().HaveCount(1);
            var onlyContainer = tree.Containers.First();
            var balls = new[] {0, 10, 5000}.Select(id => new Ball(id)).ToList();
            var runResults = balls.Select(tree.RunBall).ToList();
            runResults.Should().AllBeEquivalentTo(onlyContainer);
            onlyContainer.Balls.Should().BeEquivalentTo(balls, config => config.WithStrictOrdering());
        }

        [Fact]
        public void GivenTreeWithDepthOne_WhenPredicting_ReturnsCorrectPrediction()
        {
            var left = new Container(0);
            var middle = new Container(1);
            var right = new Container(2);
            var containers = new List<Container> {left, middle, right};
            var gate = new Gate(0, containers.Cast<INode>().ToList());
            var tree = new Tree(gate, containers);
            tree.Predict(0).Should().Be(left);
            tree.Predict(1).Should().Be(middle);
            tree.Predict(2).Should().Be(right);
            tree.Predict(9999).Should().Be(left);
        }

        [Fact]
        public void GivenTreeWithDepthTwo_WhenPredicting_ReturnsCorrectPrediction()
        {
            var containers = Enumerable.Range(0, 4).Select(id => new Container(id)).ToList();
            //build 2 layers of 2-branch gates
            var rootNode = containers
                .Cast<INode>().Batch(2).Select(children => new Gate(1, children))
                .Cast<INode>().Batch(2).Select(children => new Gate(1, children))
                .First();
            var tree = new Tree(rootNode, containers);
            tree.Predict(0).Should().Be(containers[3]);
            tree.Predict(1).Should().Be(containers[1]);
            tree.Predict(2).Should().Be(containers[2]);
            tree.Predict(3).Should().Be(containers[0]);
            tree.Predict(13).Should().Be(containers[1]);
        }

        [Fact]
        public void GivenTreeWithDepthTwo_WhenRunning_BallsArriveInCorrectContainers()
        {
            var containers = Enumerable.Range(0, 4).Select(id => new Container(id)).ToList();
            //build 2 layers of 2-branch gates
            var rootNode = containers
                .Cast<INode>().Batch(2).Select(children => new Gate(1, children))
                .Cast<INode>().Batch(2).Select(children => new Gate(1, children))
                .First();
            var tree = new Tree(rootNode, containers);
            var runResult = tree.RunBalls(16).ToList();
            runResult.Should().HaveCount(16);
            runResult[0].Should().Be(containers[3]);
            runResult[1].Should().Be(containers[1]);
            runResult[2].Should().Be(containers[2]);
            runResult[3].Should().Be(containers[0]);
            runResult[13].Should().Be(containers[1]);
            containers[3].Balls.Should().Contain(ball => ball.Id == 0);
            containers[1].Balls.Should().Contain(ball => ball.Id == 13);

        }
    }
}
