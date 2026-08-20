using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators;

internal static class OscillatorComputation
{
    internal static float ComputeNextRawPhase(
        float currentPhase,
        float frequency,
        float sampleRate
    ) => ((frequency / sampleRate) + currentPhase) % 1f;

    internal static float DistortPhase(float rawPhase, TransferFunction transferFunction)
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
        (
            distortedPhase
            + ComputePhaseModulation(modulationIndex, modulatorSignal)
            + ComputePhaseModulationFeedback(previousSignal, feedbackFactor)
        ) % 1f;

    // probably needs to become a Core helper, also for modulators
    // if not, it can be folded into the oscillator pipeline
    internal static float ComputeSignal(float phase)
    {
        return Maths.FastMinusCos(phase);
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

    // work in progress: config structs need to be defined
    // partconfig, oscillatorconfig, spectralconfig, phasemodulationconfig, xenakisconfig, modulationconfig?
    // should these be bundled in a single object, statically allocated for every oscillator?
    // write frequency calculation helper (part frequency * multiplier)
    // pipeline
    // modulate part config
    // modulate oscillator config <- should it include phase modulation sources? or should those be a separate struct
    // get raw phase
    // distort phase
    // modulate phase
    // lerp if harmonicsWeight > 0f, < 1f
    // compute signal
    // modulate Xenakis config <- should it be separate from oscillator config?
    // compute Xenakis signal
    // return mix of xenakis signal and pdpm signal
    //
    // internal static float ComputeOscillatorSignal(
    //  in PartConfig partConfig,
    //  in OscillatorConfig oscillatorConfig,
    //  in SpectralConfig spectralConfig,
    //  in PhaseModulationConfig phaseModulationConfig,
    //  in XenakisConfig xenakisConfig
    //  in ModulationConfig modulationConfig,
    //  float currentSample)
    // {
    //     float rawPhase = ComputeNextRawPhase()
    // }
}
