namespace P1Onken.TypeP.Engine.Core;

internal static class IntExtensions
{
    extension(int i)
    {
        internal int ToSamples() => Constants.SampleRateMs * i;
    }
}
