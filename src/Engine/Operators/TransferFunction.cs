using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators;

public record struct TransferFunction
{
    public TransferFunction(float d, float v)
    {
        D = Math.Clamp(d, Constants.Epsilon, 1f - Constants.Epsilon);
        V = MathF.Max(v, 0f);
    }

    public float D { get; set; }
    public float V { get; set; }

    public readonly void Deconstruct(out float d, out float v)
    {
        d = D;
        v = V;
    }
}
