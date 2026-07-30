using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

internal static class OscillatorCore
{
    internal static float ComputeNextPhase(float currentSample, float frequency, float sampleRate)
    {
        return ((frequency / sampleRate) + currentSample) % 1f;
    }

    internal static float DistortPhase(float phase, TransferFunction transferFunction)
    {
        (var d, var v) = transferFunction;

        if (phase <= d)
        {
            return v * phase / d;
        }
        else
        {
            return ((1f - v) * (phase - d) * (1f - d)) + v;
        }
    }

    internal static float AntialiasDistortedPhase(float phase, TransferFunction transferFunction)
    {
        var b = transferFunction.V % 1;
        if (b <= 0.5f)
        {
            return phase % 1f / 2f * b;
        }
        else
        {
            return phase % 1f / b;
        }
    }

    internal static float ComputeSignal(float phase) => -MathF.Cos(2 * Constants.Pi * phase);

    internal static float ComputeAntialiasedSignal(
        float antiAliasedPhase,
        TransferFunction transferFunction
    )
    {
        var b = transferFunction.V % 1f;
        var c = MathF.Cos(2f * Constants.Pi * b);

        if (b <= 0.5f)
        {
            return (((1f - c) * ComputeSignal(antiAliasedPhase)) - 1f - c) / 2f;
        }
        else if (b > 0.5f && antiAliasedPhase > 0.5f)
        {
            return (((1f + c) * ComputeSignal(antiAliasedPhase)) - 1f - c) / 2f;
        }
        else
        {
            return ComputeSignal(antiAliasedPhase);
        }
    }
}
