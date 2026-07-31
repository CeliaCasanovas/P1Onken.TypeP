namespace P1Onken.TypeP.Engine.Core;

internal static class StochasticChaoticGenerator
{
    private static PseudorandomNumberGenerator _randomGenerator = new(64);

    internal static float ComputeGaussian(in float mean, in float spread)
    {
        float uniformVariable1 = 1f - _randomGenerator.NextSingle();
        float uniformVariable2 = 1f - _randomGenerator.NextSingle();

        float standardDistribution =
            MathF.Sqrt(-2.0f * MathF.Log(uniformVariable1))
            * MathF.Sin(2.0f * Constants.Pi * uniformVariable2);

        return mean + (spread * standardDistribution);
    }

    // at >87 averageEvents threshold becomes denormal
    // at >103 averageEvents threshold becomes 0
    // branching at >50 averageEvents should maintain accuracy well whilst not
    // spending computation on large averageEvents (Gaussian branch is O(1),
    // Knuth branch calls RandomGenerator.NextSingle() averageEvents times)
    internal static int ComputePoisson(in int averageEvents)
    {
        if (averageEvents <= 0)
        {
            return 0;
        }

        if (averageEvents > 50)
        {
            float rawNextEventCount = ComputeGaussian(averageEvents, MathF.Sqrt(averageEvents));
            int roundedNextEventCount = (int)MathF.Round(rawNextEventCount);
            return Math.Max(roundedNextEventCount, 0);
        }

        float threshold = MathF.Exp(-averageEvents);
        float probability = 1f;
        int eventCountAccumulator = 0;

        do
        {
            eventCountAccumulator++;
            probability *= _randomGenerator.NextSingle();
        } while (probability > threshold);

        return eventCountAccumulator - 1;
    }

    internal static float ComputeNextLogisticMap(in float current, in float chaosAmount)
    {
        float normalisedCurrent = Math.Clamp(current, 0f, 1f);
        if (normalisedCurrent == 0)
        {
            normalisedCurrent += Constants.Epsilon;
        }
        return normalisedCurrent * chaosAmount * (1f - normalisedCurrent);
    }
}
