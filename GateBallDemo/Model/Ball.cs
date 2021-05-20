namespace GateBallDemo.Model
{
    public record Ball(int Id)
    {
        public override string ToString() => Id.ToString();
    }
}