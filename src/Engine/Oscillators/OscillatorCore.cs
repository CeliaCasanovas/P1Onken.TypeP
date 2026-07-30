using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

internal static class OscillatorCore
{
    internal static float ComputeNextRawPhase(
        float currentSample,
        float frequency,
        float sampleRate
    ) => ((frequency / sampleRate) + currentSample) % 1f;

    // calculates phase modulation. comes before phase distortion.
    internal static float ModulatePhase(in float rawPhase, in float modulatorAmplitude)
    {
        var modulatedPhase = (rawPhase + modulatorAmplitude) % 1f;
        return modulatedPhase < 0f ? modulatedPhase + 1f : modulatedPhase;
    }

    // calculates phase distortion. comes after phase modulation.
    internal static float DistortPhase(
        in float modulatedPhase,
        in TransferFunction transferFunction
    )
    {
        var (d, v) = transferFunction;

        if (modulatedPhase <= d)
        {
            return (v * modulatedPhase / d) % 1f;
        }
        else
        {
            return (((1f - v) * (modulatedPhase - d) / (1f - d)) + v) % 1f;
        }
    }

    internal static float DistortPhaseAntialias(
        in float modulatedPhase,
        in TransferFunction transferFunction
    )
    {
        var b = transferFunction.V % 1f;
        b = b == 0f ? b : Constants.Epsilon;

        if (b <= 0.5f)
        {
            return DistortPhase(in modulatedPhase, in transferFunction) / (2f * b);
        }
        else
        {
            return DistortPhase(in modulatedPhase, in transferFunction) / b;
        }
    }

    internal static float ComputeSignal(in float distortedPhase) =>
        -MathF.Cos(2 * Constants.Pi * distortedPhase);

    internal static float ComputeAntialiasedSignal(
        in float antialiasedDistortedPhase,
        in TransferFunction transferFunction
    )
    {
        var b = transferFunction.V % 1f;
        var c = MathF.Cos(2f * Constants.Pi * b);

        if (b <= 0.5f)
        {
            return (((1f - c) * ComputeSignal(in antialiasedDistortedPhase)) - 1f - c) / 2f;
        }
        else if (b > 0.5f && antialiasedDistortedPhase > 0.5f)
        {
            return (((1f + c) * ComputeSignal(in antialiasedDistortedPhase)) - 1f - c) / 2f;
        }
        else
        {
            return ComputeSignal(in antialiasedDistortedPhase);
        }
    }

    internal static TransferFunction LerpTransferFunctionTowardsIdentity(
        in float lerpWeight,
        in TransferFunction transferFunction
    ) =>
        new(
            MathF.FusedMultiplyAdd(0.5f - transferFunction.D, lerpWeight, transferFunction.D),
            MathF.FusedMultiplyAdd(0.5f - transferFunction.V, lerpWeight, transferFunction.V)
        );
}
