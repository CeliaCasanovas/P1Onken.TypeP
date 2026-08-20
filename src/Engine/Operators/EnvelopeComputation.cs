using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators;

internal static unsafe class EnvelopeComputation
{
    // pass just currentAmplitude and EnvelopeConfig into the computations
    private static delegate* <float, int, EnvelopeConfig>[] Computations;

    // riseTime is in ms
    private static float ComputeSigmoidFactor(int samples)
    {
        return Maths.FastExp(2f * Constants.Ln10000 / samples);
    }

    // fallTime is in ms
    private static float ComputeExponentialFactor(int samples)
    {
        return 1f - Maths.FastExp(-Constants.Ln10000 / samples);
    }

    // curve = 0f is fully exponential
    // curve = 1f is fully pseudo-sigmoid
    internal static float ComputeNextAmplitudeRise(
        float currentAmplitude,
        EnvelopeConfig config,
        EnvelopeStage stage
    )
    {
        var exponentialFactor = config[stage].ExponentialFactor;
        var sigmoidFactor = config.AttackSigmoidFactor;
        var curve = config.AttackCurve;

        float distance = 1f - currentAmplitude;

        if (distance <= Constants.Epsilon)
        {
            return 1f;
        }

        // what's more efficient, this branchless approach or adding fast paths for curve = 0 and curve = 1?
        float exponentialAmplitude = currentAmplitude + exponentialFactor * distance;
        float scaledExponentialAmplitude = exponentialAmplitude * (1f - curve);

        float sigmoidProgress = sigmoidFactor * currentAmplitude / distance;
        float sigmoidAmplitude = sigmoidProgress / (1f + sigmoidProgress);

        return MathF.Min(
            MathF.FusedMultiplyAdd(sigmoidAmplitude, curve, scaledExponentialAmplitude),
            1f
        );
    }

    internal static float ComputeNextAmplitudeFall(
        float currentAmplitude,
        EnvelopeConfig config,
        EnvelopeStage stage
    )
    {
        var targetLevel = config[stage].TargetLevel;
        var exponentialFactor = config[stage].ExponentialFactor;

        if (currentAmplitude <= targetLevel)
        {
            return targetLevel;
        }

        return MathF.Max(
            MathF.FusedMultiplyAdd(
                exponentialFactor,
                targetLevel - currentAmplitude,
                currentAmplitude
            ),
            Constants.Epsilon
        );
    }

    internal static float ComputeNextAmplitudeHold(float currentAmplitude) => currentAmplitude;
}
