using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators;

internal record struct EnvelopeConfig()
{
    internal float DelayTime;

    // dAttackPhase = 1000 / (AttackTime * SampleRate)
    // f(AttackPhase) = (1 - e^(-AttackCurve * AttackPhase)) / (1 - e^(-AttackCurve))
    internal float AttackTime
    {
        get;
        set
        {
            field = value < Constants.Epsilon ? Constants.Epsilon : value;
            AttackRate = 1000f / (field * Constants.SampleRate);
        }
    }
    internal float HoldTime;

    // amplitude = Decay1Level + (amplitude - Decay1Level) * Decay1Factor
    internal float Decay1Time
    {
        get;
        set
        {
            field = value < Constants.Epsilon ? Constants.Epsilon : value;
            Decay1Factor = MathF.Exp(-Constants.Ln1000 * 1000f / (field * Constants.SampleRate));
        }
    }

    // amplitude = amplitude * Decay2Factor
    internal float Decay2Time
    {
        get;
        set
        {
            field = value < Constants.Epsilon ? Constants.Epsilon : value;
            Decay2Factor = MathF.Exp(-Constants.Ln1000 * 1000f / (field * Constants.SampleRate));
        }
    }

    // f(t) = initialAmplitude * e^(-t/damping)
    // -t/damping = -1/(sampleRate * damping)
    // approximate damping is the time it takes to drop 60dB
    // damping = timeToDrop60dB / ln(1000) = timeToDrop60dB / 6.907755
    internal float ReleaseTime
    {
        get;
        set
        {
            field = value < Constants.Epsilon ? Constants.Epsilon : value;
            ReleaseFactor = MathF.Exp(-Constants.Ln1000 * 1000f / (field * Constants.SampleRate));
        }
    }

    internal float MaximumLevel = 1f;
    internal float Decay1Level;

    // dAttackPhase = 1000 / (AttackTime * SampleRate)
    // f(AttackPhase) = (1 - e^(-AttackCurve * AttackPhase)) / (1 - e^(-AttackCurve))
    internal float AttackCurve
    {
        get;
        set
        {
            field = value;
            if (MathF.Abs(field) > Constants.Epsilon)
            {
                AttackMultiplier = 1f / (1f - MathF.Exp(-field));
            }
        }
    }

    internal readonly float DelayFactor = 1f;
    internal readonly float HoldFactor = 1f;
    internal float AttackMultiplier;
    internal float AttackRate;
    internal float Decay1Factor;
    internal float Decay2Factor;
    internal float ReleaseFactor;
    internal bool IsLooping;
}
