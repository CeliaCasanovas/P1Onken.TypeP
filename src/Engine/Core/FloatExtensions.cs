namespace P1Onken.TypeP.Engine.Core;

internal static class FloatExtensions
{
    extension(float f)
    {
        internal float ToRadians() => Constants.TwoPi * f;

        internal float ToSamples() => Constants.SampleRateMs * f;
    }
}
