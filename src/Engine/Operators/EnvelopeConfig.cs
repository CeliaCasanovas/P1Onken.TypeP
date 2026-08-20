namespace P1Onken.TypeP.Engine.Operators;

internal unsafe struct EnvelopeConfig
{
    internal float[] TargetLevels;
    internal float[] ExponentialFactors;
    internal int[] LengthsSamples;
    internal float AttackSigmoidFactor;

    // curve = 0f is fully exponential
    // curve = 1f is fully pseudo-sigmoid
    internal float AttackCurve;

    internal EnvelopeProxyIndexer this[EnvelopeStage stage] => new(ref this, stage);
}
