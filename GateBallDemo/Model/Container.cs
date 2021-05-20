using System.Collections.Generic;

namespace GateBallDemo.Model
{
    public record Container(int Id) : INode
    {
        public List<Ball> Balls { get; } = new();
    

        public void ReceiveBall(Ball ball)
        {
            Balls.Add(ball);
        }

        public override string ToString() => Id.ToString();
    }
}