using System;
using System.Collections.Generic;

namespace GateBallDemo.Model
{
    public class Gate : INode
    {
        public Gate(int direction, List<INode> children)
        {
            if (direction <0 || direction >= children.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), "direction must be a valid index of children");
            }
            _direction = direction;
            _children = children;
        }

        private readonly List<INode> _children;
        private int _direction;

        public INode RunBall()
        {
            var nextNode = _children[_direction];
            _direction = (_direction + 1) % _children.Count;
            return nextNode;
        }

        public INode PredictNthBall(int ballNumber, out int newBallNumber)
        {
            newBallNumber = ballNumber / _children.Count;
            var predictedDirection = (_direction + ballNumber) % _children.Count;
            return _children[predictedDirection];
        }
    }
}