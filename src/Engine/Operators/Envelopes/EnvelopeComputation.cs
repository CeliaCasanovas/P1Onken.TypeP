using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators.Envelopes;

internal static unsafe class EnvelopeComputation
{
    internal static readonly delegate* <float, DahddrEnvelope, EnvelopeStage, float>[] Computations;

    static EnvelopeComputation()
    {
        Computations =
        [
            &ComputeNextAmplitudeHold,
            &ComputeNextAmplitudeRise,
            &ComputeNextAmplitudeHold,
            &ComputeNextAmplitudeFall,
            &ComputeNextAmplitudeFall,
            &ComputeNextAmplitudeFall,
        ];
    }

    internal static float ComputeSigmoidFactor(float samples)
    {
        return Maths.FastExp(2f * Constants.Ln10000 / samples);
    }

    internal static float ComputeExponentialFactor(float samples)
    {
        return 1f - Maths.FastExp(-Constants.Ln10000 / samples);
    }

    // curve = 0f is fully exponential
    // curve = 1f is fully pseudo-sigmoid
    internal static float ComputeNextAmplitudeRise(
        float currentAmplitude,
        DahddrEnvelope envelope,
        EnvelopeStage stage
    )
    {
        var exponentialFactor = envelope[stage].ExponentialFactor;
        var sigmoidFactor = envelope.AttackSigmoidFactor;
        var curve = envelope.AttackCurve;

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
        DahddrEnvelope envelope,
        EnvelopeStage stage
    )
    {
        var targetLevel = envelope[stage].TargetLevel;
        var exponentialFactor = envelope[stage].ExponentialFactor;

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

    internal static float ComputeNextAmplitudeHold(
        float currentAmplitude,
        DahddrEnvelope envelope,
        EnvelopeStage stage
    ) => currentAmplitude;

    internal static float ComputeStep(float stageLengthSamples) =>
        Constants.SampleRateSquared / stageLengthSamples;
}
