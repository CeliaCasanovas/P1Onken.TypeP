using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

internal static class OscillatorCore
{
    internal static float ComputeNextRawPhase(
        float currentSample,
        float frequency,
        float sampleRate
    )
    {
        return ((frequency / sampleRate) + currentSample) % 1f;
    }

    internal static float DistortPhase(float rawPhase, TransferFunction transferFunction)
    {
        (var d, var v) = transferFunction;

        if (rawPhase <= d)
        {
            return v * rawPhase / d;
        }
        else
        {
            return ((1f - v) * (rawPhase - d) * (1f - d)) + v;
        }
    }

    internal static float DistortPhaseAntialias(float rawPhase, TransferFunction transferFunction)
    {
        var b = transferFunction.V % 1;

        if (b <= 0.5f)
        {
            return DistortPhase(rawPhase, transferFunction) % 1f / 2f * b;
        }
        else
        {
            return DistortPhase(rawPhase, transferFunction) % 1f / b;
        }
    }

    internal static float ComputeSignal(float distortedPhase) =>
        -MathF.Cos(2 * Constants.Pi * distortedPhase);

    internal static float ComputeAntialiasedSignal(
        float antialiasedDistortedPhase,
        TransferFunction transferFunction
    )
    {
        var b = transferFunction.V % 1f;
        var c = MathF.Cos(2f * Constants.Pi * b);

        if (b <= 0.5f)
        {
            return (((1f - c) * ComputeSignal(antialiasedDistortedPhase)) - 1f - c) / 2f;
        }
        else if (b > 0.5f && antialiasedDistortedPhase > 0.5f)
        {
            return (((1f + c) * ComputeSignal(antialiasedDistortedPhase)) - 1f - c) / 2f;
        }
        else
        {
            return ComputeSignal(antialiasedDistortedPhase);
        }
    }

    internal static TransferFunction LerpTransferFunctionTowardsIdentity(
        float lerpWeight,
        TransferFunction transferFunction
    ) =>
        (
            MathF.FusedMultiplyAdd(0.5f - transferFunction.D, lerpWeight, transferFunction.D),
            MathF.FusedMultiplyAdd(0.5f - transferFunction.V, lerpWeight, transferFunction.V)
        );
}
