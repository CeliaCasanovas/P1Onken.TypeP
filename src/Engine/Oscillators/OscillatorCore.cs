using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

internal static class OscillatorCore
{
    internal static float ComputeNextRawPhase(
        float currentSample,
        float frequency,
        float sampleRate
    ) => ((frequency / sampleRate) + currentSample) % 1f;

    internal static float DistortPhase(float rawPhase, in TransferFunction transferFunction)
    {
        var (d, v) = transferFunction;

        if (rawPhase <= d)
        {
            return v * rawPhase / d;
        }

        return ((1f - v) * (rawPhase - d) / (1f - d)) + v;
    }

    private static float ComputePhaseModulation(float modulationIndex, float modulatorSignal) =>
        Constants.Pi * 0.5f * modulationIndex * modulatorSignal;

    private static float ComputePhaseModulationFeedback(
        float previousSignal,
        float feedbackFactor
    ) =>
        feedbackFactor < 0f
            ? ComputePhaseModulation(feedbackFactor, MathF.Abs(previousSignal))
            : ComputePhaseModulation(feedbackFactor, previousSignal);

    internal static float ModulatePhase(
        float distortedPhase,
        float modulationIndex,
        float modulatorSignal,
        float previousSignal,
        float feedbackFactor
    ) =>
        distortedPhase.ToRadians()
        + ComputePhaseModulation(modulationIndex, modulatorSignal)
        + ComputePhaseModulationFeedback(previousSignal, feedbackFactor);

    private static float AntialiasPhase(float distortedPhase, float b)
    {
        var normalisedB = b == 0f ? Constants.Epsilon : b;

        if (b <= 0.5f)
        {
            return distortedPhase % 1f / (2f * normalisedB);
        }

        return distortedPhase % 1f / normalisedB;
    }

    private static float ComputeAntialiasedSignal(float rawDistortedModulatedPhase, float v)
    {
        var b = v % 1f;
        var antialiasedDistortedPhase = AntialiasPhase(rawDistortedModulatedPhase, b);
        var c = MathF.Cos(b.ToRadians());
        var rawAntialiasedSignal = -MathF.Cos(antialiasedDistortedPhase.ToRadians());

        if (b <= 0.5f)
        {
            return (((1f - c) * rawAntialiasedSignal) - 1f - c) / 2f;
        }

        if (b > 0.5f && antialiasedDistortedPhase > Constants.Pi)
        {
            return (((1f + c) * rawAntialiasedSignal) + 1f - c) / 2f;
        }

        return rawAntialiasedSignal;
    }

    internal static float ComputeSignal(
        float distortedModulatedPhase,
        bool hasAntialias,
        in TransferFunction transferFunction
    )
    {
        var v = transferFunction.V;

        if (hasAntialias && distortedModulatedPhase > MathF.Floor(v))
        {
            return ComputeAntialiasedSignal(distortedModulatedPhase, v);
        }

        return -MathF.Cos(distortedModulatedPhase % 1f.ToRadians());
    }

    // wavefolder
    // fractalFactor > 0f
    // feedbackFactor
    internal static float ComputeFractalFeedbackSignal(
        float phase,
        float fractalFactor,
        float feedbackFactor
    )
    {
        while (feedbackFactor >= Constants.Epsilon)
        {
            phase += Constants.Pi * 0.5f * feedbackFactor * -MathF.Cos(phase % 1f.ToRadians());

            float currentFractal = fractalFactor;

            fractalFactor = currentFractal * currentFractal;
            feedbackFactor = currentFractal * feedbackFactor;
        }

        return -MathF.Cos(phase % 1f.ToRadians());
    }

    // calculating xenakis
    // set stochastic grain trigger in motion <- GrainTriggerFrequency (Poisson)
    // distort/modulate transfer phase
    // calculate current band <- GrainCentralBand (Gauss or Logistic)
    // calculate starting phase <- GrainCentralStartingPhase (Gauss or Logistic)
    // calculate length in samples <- GrainLength, GrainPitch
    // apply local distortion to v and d <- GrainTransferFunctionVDistortion, GrainTransferFunctionDDistortion
    // get lengthSamples target phases <- accumulate phase if needed
    // calculate lengthSamples signals
    // scale grain amplitude <- GrainAmplitude
    // apply amplitude window <- GrainWindowSharpness <- GrainPhaseAccumulator/lengthSamples
    // send to mix

    // this is very likely not the ideal way to do this
    internal static TransferFunction LerpTransferFunctionTowardsNoHarmonics(
        float lerpWeight,
        in TransferFunction transferFunction
    ) =>
        new(
            MathF.FusedMultiplyAdd(0.5f - transferFunction.D, lerpWeight, transferFunction.D),
            MathF.FusedMultiplyAdd(0.5f - transferFunction.V, lerpWeight, transferFunction.V)
        );
}
