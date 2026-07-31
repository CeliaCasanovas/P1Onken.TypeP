using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

internal static class OscillatorCore
{
    internal static float ComputeNextRawPhase(
        in float currentSample,
        in float frequency,
        in float sampleRate
    ) => ((frequency / sampleRate) + currentSample) % 1f;

    // calculates phase modulation. comes before phase distortion.
    internal static float ModulatePhase(
        in float rawPhase,
        in float modulatorAmplitude,
        in float feedbackIndex,
        in float carrierAmplitude
    )
    {
        var modulatedPhase = (rawPhase + modulatorAmplitude);
        modulatedPhase += carrierAmplitude * feedbackIndex;
        modulatedPhase %= 1f;
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
            return v * modulatedPhase / d;
        }
        else
        {
            return ((1f - v) * (modulatedPhase - d) / (1f - d)) + v;
        }
    }

    private static float DistortPhaseAntialias(in float distortedPhase, in float b)
    {
        var normalisedB = b == 0f ? Constants.Epsilon : b;

        if (b <= 0.5f)
        {
            return distortedPhase % 1f / (2f * normalisedB);
        }
        else
        {
            return distortedPhase % 1f / normalisedB;
        }
    }

    internal static float ComputeSignal(
        in float distortedPhase,
        in bool hasAntialias,
        in TransferFunction transferFunction
    )
    {
        var v = transferFunction.V;

        if (hasAntialias && distortedPhase > MathF.Floor(v))
        {
            var b = v % 1f;
            return ComputeAntialiasedSignal(DistortPhaseAntialias(in distortedPhase, in b), in b);
        }
        else
        {
            return -MathF.Cos(2f * Constants.Pi * (distortedPhase % 1f));
        }
    }

    private static float ComputeAntialiasedSignal(in float antialiasedDistortedPhase, in float b)
    {
        var c = MathF.Cos(2f * Constants.Pi * b);
        var rawAntialiasedSignal = -MathF.Cos(2f * Constants.Pi * antialiasedDistortedPhase);

        if (b <= 0.5f)
        {
            return (((1f - c) * rawAntialiasedSignal) - 1f - c) / 2f;
        }
        else if (b > 0.5f && antialiasedDistortedPhase > 0.5f)
        {
            return (((1f + c) * rawAntialiasedSignal) + 1f - c) / 2f;
        }
        else
        {
            return rawAntialiasedSignal;
        }
    }

    internal static TransferFunction LerpTransferFunctionTowardsNoHarmonics(
        in float lerpWeight,
        in TransferFunction transferFunction
    ) =>
        new(
            MathF.FusedMultiplyAdd(0.5f - transferFunction.D, lerpWeight, transferFunction.D),
            MathF.FusedMultiplyAdd(0.5f - transferFunction.V, lerpWeight, transferFunction.V)
        );
}
