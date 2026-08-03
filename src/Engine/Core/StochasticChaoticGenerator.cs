namespace P1Onken.TypeP.Engine.Core;

internal static class StochasticChaoticGenerator
{
    private static PseudorandomNumberGenerator _randomGenerator = new(64u);

    internal static float ComputeGaussian(float mean, float spread)
    {
        float uniformVariable1 = 1f - _randomGenerator.NextSingle();
        float uniformVariable2 = 1f - _randomGenerator.NextSingle();

        float standardDistribution =
            MathF.Sqrt(-2.0f * MathF.Log(uniformVariable1))
            * MathF.Sin(2.0f * Constants.Pi * uniformVariable2);

        return mean + (spread * standardDistribution);
    }

    // at >87 averageEvents threshold becomes denormal
    // at >103 averageEvents threshold becomes 0f
    // branching at >50 averageEvents should preserve enough accuracy whilst not
    // spending computation on large averageEvents (Gaussian branch is O(1),
    // Knuth branch calls PseudoandomNumberGenerator.NextSingle() averageEvents times)
    internal static uint ComputePoisson(uint averageEvents)
    {
        if (averageEvents <= 0u)
        {
            return 0u;
        }

        if (averageEvents > 50u)
        {
            float rawNextEventCount = ComputeGaussian(averageEvents, MathF.Sqrt(averageEvents));
            uint roundedNextEventCount = (uint)MathF.Round(rawNextEventCount);
            return Math.Max(roundedNextEventCount, 0u);
        }

        float threshold = MathF.Exp(-averageEvents);
        float probability = 1f;
        uint eventCountAccumulator = 0u;

        do
        {
            eventCountAccumulator++;
            probability *= _randomGenerator.NextSingle();
        } while (probability > threshold);

        return eventCountAccumulator - 1u;
    }

    internal static float ComputeNextLogisticMap(float current, float chaosAmount)
    {
        float normalisedCurrent = Math.Clamp(current, Constants.Epsilon, 1f);
        // move chaosAmount normalisation to config struct setter
        float normalisedChaos = Math.Clamp(chaosAmount, Constants.Epsilon, 4f);

        return normalisedCurrent * normalisedChaos * (1f - normalisedCurrent);
    }
}
