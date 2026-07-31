namespace P1Onken.TypeP.Engine.Core;

// xoroshiro128+ based
internal struct RandomNumberGenerator
{
    private ulong _stateVariable1;
    private ulong _stateVariable2;

    internal RandomNumberGenerator(uint seed)
    {
        uint normalisedSeed = seed == 0 ? 64 : seed;

        _stateVariable1 = normalisedSeed;
        _stateVariable2 = normalisedSeed ^ Constants.XoroshiroConstant;
    }

    internal float NextSingle()
    {
        var localStateVariable1 = _stateVariable1;
        var localStateVariable2 = _stateVariable2;
        var result = localStateVariable1 + localStateVariable2;

        localStateVariable2 ^= localStateVariable1;
        _stateVariable1 =
            ((localStateVariable1 << 24) | (localStateVariable1 >> 40))
            ^ localStateVariable2
            ^ (localStateVariable2 << 16);
        _stateVariable2 = (localStateVariable2 << 37) | (localStateVariable2 >> 27);

        var resultHighBits = (uint)(result >> 40);

        return resultHighBits / Constants.MaxFloat;
    }
}
