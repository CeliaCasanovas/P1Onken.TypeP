namespace P1Onken.TypeP.Engine.Operators;

internal static class EnvelopeCore
{
    internal static float ComputeNextAttackPhase(float attackPhase, ref EnvelopeConfig config) =>
        attackPhase + config.AttackRate;

    internal static float ComputeAttackAmplitudeLinear(float attackPhase, ref EnvelopeConfig config)
    {
        if (attackPhase >= 1f)
        {
            attackPhase = 1f;
            // transition to next stage
        }

        return config.MaximumLevel * attackPhase;
    }

    internal static float ComputeAttackAmplitude(float attackPhase, ref EnvelopeConfig config)
    {
        if (attackPhase >= 1f)
        {
            attackPhase = 1f;
            // transition to next stage
        }

        // f(t) = (1 - e^(-curve * phase)) / (1 - e^(-curve))
        return config.MaximumLevel
            * (1f - MathF.Exp(-config.AttackCurve * attackPhase))
            * config.AttackMultiplier;
    }


}
