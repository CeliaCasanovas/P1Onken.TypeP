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

    internal static float ComputeSignal(float phase)
    {
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

    internal static float LerpRawPhaseTowardsDistortedPhase(
        float rawPhase,
        float distortedModulatedPhase,
        float harmonicsWeight
    ) => MathF.FusedMultiplyAdd(harmonicsWeight, distortedModulatedPhase - rawPhase, rawPhase);
}
