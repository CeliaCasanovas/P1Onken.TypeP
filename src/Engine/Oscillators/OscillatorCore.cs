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
        modulatedPhase %= 1;
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
            return (v * modulatedPhase / d);
        }
        else
        {
            return (((1f - v) * (modulatedPhase - d) / (1f - d)) + v);
        }
    }

    private static float DistortPhaseAntialias(
        in float distortedPhase,
        in TransferFunction transferFunction
    )
    {
        var b = transferFunction.V % 1f;
        b = b == 0f ? Constants.Epsilon : b;

        if (b <= 0.5f)
        {
            return distortedPhase % 1 / (2f * b);
        }
        else
        {
            return distortedPhase % 1 / b;
        }
    }

    internal static float ComputeSignal(
        in float distortedPhase,
        in bool hasAntialias,
        in TransferFunction transferFunction
    )
    {
        if (hasAntialias && distortedPhase > MathF.Floor(transferFunction.V))
        {
            return ComputeAntialiasedSignal(
                DistortPhaseAntialias(in distortedPhase, in transferFunction),
                in transferFunction
            );
        }
        else
        {
            return -MathF.Cos(2 * Constants.Pi * (distortedPhase % 1));
        }
    }

    // needs to be called only when distortedphase BEFORE % 1f > floor(v)
    private static float ComputeAntialiasedSignal(
        in float antialiasedDistortedPhase,
        in TransferFunction transferFunction
    )
    {
        var b = transferFunction.V % 1f;
        var c = MathF.Cos(2f * Constants.Pi * b);

        if (b <= 0.5f)
        {
            return (((1f - c) * -MathF.Cos(2 * Constants.Pi * antialiasedDistortedPhase)) - 1f - c)
                / 2f;
        }
        else if (b > 0.5f && antialiasedDistortedPhase > 0.5f)
        {
            return (((1f + c) * -MathF.Cos(2 * Constants.Pi * antialiasedDistortedPhase)) + 1f - c)
                / 2f;
        }
        else
        {
            return -MathF.Cos(2 * Constants.Pi * antialiasedDistortedPhase);
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
