namespace P1Onken.TypeP.Engine.Core;

internal static class FloatExtensions
{
    extension(float f)
    {
        internal float ToRadians() => 2f * Constants.Pi * f;
    }
}
