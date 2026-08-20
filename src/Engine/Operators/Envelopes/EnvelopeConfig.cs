namespace P1Onken.TypeP.Engine.Operators.Envelopes;

internal struct EnvelopeConfig
{
    internal float[] TargetLevels;
    internal float[] ExponentialFactors;
    internal float[] LengthsSamples;
    internal float[] StepLengths;
    internal float AttackSigmoidFactor;

    // curve = 0f is fully exponential
    // curve = 1f is fully pseudo-sigmoid
    internal float AttackCurve;
    internal bool IsLooping;
}
