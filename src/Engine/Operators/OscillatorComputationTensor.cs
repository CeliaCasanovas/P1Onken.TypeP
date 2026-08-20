using System.Numerics.Tensors;
using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators;

internal static class OscillatorComputationTensor
{
    internal static Span<float> ComputeNextRawPhaseBlock(
        Span<float> phases,
        float frequency,
        Span<float> scratchBuffer
    )
    {
        float step = frequency / Constants.SampleRate;

        TensorPrimitives.Add(phases, step, phases);
        TensorPrimitives.Floor(phases, scratchBuffer);
        TensorPrimitives.Subtract(phases, scratchBuffer, phases);

        return phases;
    }

    internal static Span<float> DistortPhaseBlock(
        ReadOnlySpan<float> rawPhases,
        TransferFunction transferFunction,
        Span<float> scratchBuffer,
        Span<float> distortedPhases
    )
    {
        var (d, v) = transferFunction;
        float firstSegment = v / d;
        float secondSegment = (1f - v) / (1f - d);

        // rawPhase <= d: distortedPhase = (v / d) * rawPhase
        // rawPhase > d: distortedPhase = (rawPhase - d) * (1 - v) / (1 - d) + v

        // min(rawPhase, d) = d when rawPhase > d, and d * (v / d) = v, so branchless version:
        // distortedPhase = min(rawPhase, d) * (v / d) + max(rawPhase - d, 0) * (1 - v) / (1 - d)

        // first term
        TensorPrimitives.Min(rawPhases, d, distortedPhases);
        TensorPrimitives.Multiply(distortedPhases, firstSegment, distortedPhases);
        // [rawPhase * v/d, rawPhase * v/d... v, v, v]
        // second term and addition of the two terms
        // Slice rawPhases at first v
        TensorPrimitives.Subtract(rawPhases, d, scratchBuffer);
        TensorPrimitives.Max(scratchBuffer, 0f, scratchBuffer);
        // [0, 0... rawPhase - d, rawPhase - d, rawPhase - d, rawPhase - d]
        TensorPrimitives.FusedMultiplyAdd(
            scratchBuffer,
            secondSegment,
            distortedPhases,
            distortedPhases
        );

        return distortedPhases;
    }

    internal static Span<float> ModulatePhaseBlock(
        Span<float> distortedPhases,
        float modulationIndex,
        ReadOnlySpan<float> modulatorSignals,
        ReadOnlySpan<float> previousSignals,
        float feedbackIndex,
        Span<float> scratchBuffer
    )
    {
        float modulationFactor = Constants.Pi * 0.5f * modulationIndex;
        float feedbackFactor = Constants.Pi * 0.5f * feedbackIndex;

        TensorPrimitives.Multiply(distortedPhases, Constants.TwoPi, distortedPhases);

        TensorPrimitives.FusedMultiplyAdd(
            modulatorSignals,
            modulationFactor,
            distortedPhases,
            distortedPhases
        );

        if (feedbackIndex < 0f)
        {
            TensorPrimitives.Abs(previousSignals, scratchBuffer);
            TensorPrimitives.FusedMultiplyAdd(
                scratchBuffer,
                feedbackFactor,
                distortedPhases,
                distortedPhases
            );
        }
        else
        {
            TensorPrimitives.FusedMultiplyAdd(
                previousSignals,
                feedbackFactor,
                distortedPhases,
                distortedPhases
            );
        }

        return distortedPhases;
    }

    internal static Span<float> ComputeSignalBlock(
        ReadOnlySpan<float> phases,
        Span<float> signals,
        Span<float> scratchBuffer
    )
    {
        // cos(-x) = cos x, so using -2pi for this FMA trick works
        // signals is acting as a scratch buffer here, and it represents the normalised phases
        TensorPrimitives.Multiply(phases, 1f / Constants.TwoPi, signals);
        TensorPrimitives.Floor(signals, signals);
        TensorPrimitives.FusedMultiplyAdd(signals, -Constants.TwoPi, phases, signals);

        return Maths.FastMinusCosTensor(signals, scratchBuffer);
    }

    internal static Span<float> LerpRawPhaseTowardsDistortedPhaseBlock(
        ReadOnlySpan<float> rawPhases,
        ReadOnlySpan<float> distortedModulatedPhases,
        float harmonicsWeight,
        Span<float> lerpedPhases
    )
    {
        TensorPrimitives.Lerp(rawPhases, distortedModulatedPhases, harmonicsWeight, lerpedPhases);
        return lerpedPhases;
    }
}
