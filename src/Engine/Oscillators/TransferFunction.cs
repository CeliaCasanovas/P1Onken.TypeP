using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

public readonly record struct TransferFunction
{
    public TransferFunction(float d, float v)
    {
        D = Math.Clamp(d, Constants.Epsilon, 1f - Constants.Epsilon);
        V = v;
    }

    public float D { get; init; }
    public float V { get; init; }

    public void Deconstruct(out float d, out float v)
    {
        d = this.D;
        v = this.V;
    }
}
