namespace P1Onken.TypeP.Engine.Core;

internal static class Constants
{
    internal const float Pi = 3.1415927f;
    internal const float TwoPi = 6.2831853f;
    internal const float Epsilon = 0.0001f;

    // Ln10000 is how many cycles of exponential decay it takes to get within Epsilon of the target level
    // 10000 = 1 / Epsilon
    internal const float Ln10000 = 9.2103403f;
    internal const float SampleRate = 96000f;
    internal const int SampleRateMs = (int)SampleRate / 1000;
    internal const float SampleRateSquared = 9216000000f;
    internal const float Log2E = 1.44269504f;
    internal const float MineiroA = 121.2740575f;
    internal const float MineiroB = 27.7280233f;
    internal const float MineiroC = 4.84252568f;
    internal const float MineiroD = 1.49012907f;

    // internal const float SchraudolphMultiplier = 12102203f; // 2^23 / ln(2)
    // internal const float SchraudolphCorrection = 1064866805f; // 127*2^23 - 486411
    internal const float MinimaxMinusCosCoefficient0 = -0.9999933f;
    internal const float MinimaxMinusCosCoefficient2 = 0.4999124f;
    internal const float MinimaxMinusCosCoefficient4 = -0.0414877f;
    internal const float MinimaxMinusCosCoefficient6 = 0.0012712f;
    internal static readonly float[] MinimaxMinusCosCoefficient6s;
    internal const ulong XoroshiroConstant0 = 0x9e3779b97f4a7c15ul;
    internal const ulong XoroshiroConstant1 = 0xbf58476d1ce4e5b9ul;
    internal const ulong XoroshiroConstant2 = 0x9e3779b97f4a7c15ul;
    internal const uint FloatMask = 0x3f800000u;
    internal const int BlockSize = 128;

    static Constants()
    {
        MinimaxMinusCosCoefficient6s = new float[BlockSize];
        Array.Fill(MinimaxMinusCosCoefficient6s, MinimaxMinusCosCoefficient6);
    }
}
