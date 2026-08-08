namespace P1Onken.TypeP.Engine.Core;

internal static class Constants
{
    internal const float Pi = 3.1415927f;
    internal const float Epsilon = 0.0001f;
    internal const float Ln1000 = 6.907755f;
    internal const float CurveScalingFactor = 6f;
    internal const float SampleRate = 96000f;

    internal const uint FloatMask = 0x3f800000u;
    internal const ulong XoroshiroConstant0 = 0x9e3779b97f4a7c15ul;
    internal const ulong XoroshiroConstant1 = 0xbf58476d1ce4e5b9ul;
    internal const ulong XoroshiroConstant2 = 0x9e3779b97f4a7c15ul;
}
