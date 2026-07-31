namespace P1Onken.TypeP.Engine.Core;

internal struct RandomNumberGenerator
{
    private uint _state;

    internal RandomNumberGenerator(uint seed)
    {
        _state = seed == 0 ? 64 : seed;
    }

    internal float NextSingle()
    {
        uint x = _state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _state = x;

        return (x & 0xFFFFFF) / 16777216.0f;
    }
}
