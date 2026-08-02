using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Oscillators;

internal static class OscillatorCore
{
    internal static float ComputeNextRawPhase(
        in float currentSample,
        in float frequency,
        in float sampleRate
    ) => ((frequency / sampleRate) + currentSample) % 1f;

    internal static float DistortPhase(in float rawPhase, in TransferFunction transferFunction)
    {
        var (d, v) = transferFunction;

        if (rawPhase <= d)
        {
            return v * rawPhase / d;
        }

        return ((1f - v) * (rawPhase - d) / (1f - d)) + v;
    }

    private static float ComputePhaseModulation(
        in float modulationIndex,
        in float modulatorSignal
    ) => modulationIndex * modulatorSignal;

    private static float ComputePhaseModuationFeedback(
        in float previousSignal,
        in float feedbackFactor
    ) =>
        feedbackFactor < 0f
            ? ComputePhaseModulation(feedbackFactor, MathF.Abs(previousSignal))
            : ComputePhaseModulation(feedbackFactor, previousSignal);

    internal static float ModulatePhase(
        in float distortedPhase,
        in float modulationIndex,
        in float modulatorSignal,
        in float previousSignal,
        in float feedbackFactor
    ) =>
        distortedPhase
        + ComputePhaseModulation(modulationIndex, modulatorSignal)
        + ComputePhaseModuationFeedback(previousSignal, feedbackFactor);

    private static float AntialiasPhase(in float distortedPhase, in float b)
    {
        var normalisedB = b == 0f ? Constants.Epsilon : b;

        if (b <= 0.5f)
        {
            return distortedPhase % 1f / (2f * normalisedB);
        }

        return distortedPhase % 1f / normalisedB;
    }

    private static float ComputeAntialiasedSignal(in float rawDistortedModulatedPhase, in float v)
    {
        var b = v % 1f;
        var antialiasedDistortedPhase = AntialiasPhase(rawDistortedModulatedPhase, b);
        var c = MathF.Cos(b.ToRadians());
        var rawAntialiasedSignal = -MathF.Cos(antialiasedDistortedPhase.ToRadians());

        if (b <= 0.5f)
        {
            return (((1f - c) * rawAntialiasedSignal) - 1f - c) / 2f;
        }

        if (b > 0.5f && antialiasedDistortedPhase > 0.5f)
        {
            return (((1f + c) * rawAntialiasedSignal) + 1f - c) / 2f;
        }

        return rawAntialiasedSignal;
    }

    internal static float ComputeSignal(
        in float distortedModulatedPhase,
        in bool hasAntialias,
        in TransferFunction transferFunction
    )
    {
        var v = transferFunction.V;

        if (hasAntialias && distortedModulatedPhase > MathF.Floor(v))
        {
            return ComputeAntialiasedSignal(in distortedModulatedPhase, in v);
        }

        return -MathF.Cos((distortedModulatedPhase % 1f).ToRadians());
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
            phase += feedbackFactor * -MathF.Cos((phase % 1f).ToRadians());

            float currentFractal = fractalFactor;

            fractalFactor = currentFractal * currentFractal;
            feedbackFactor = currentFractal * feedbackFactor;
        }

        return -MathF.Cos((phase % 1f).ToRadians());
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

    internal static TransferFunction LerpTransferFunctionTowardsNoHarmonics(
        in float lerpWeight,
        in TransferFunction transferFunction
    ) =>
        new(
            MathF.FusedMultiplyAdd(0.5f - transferFunction.D, lerpWeight, transferFunction.D),
            MathF.FusedMultiplyAdd(0.5f - transferFunction.V, lerpWeight, transferFunction.V)
        );
}
