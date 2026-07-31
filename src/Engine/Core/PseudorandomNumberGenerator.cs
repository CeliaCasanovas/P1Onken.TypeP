namespace P1Onken.TypeP.Engine.Core;

// xoroshiro128++ based
internal struct PseudorandomNumberGenerator
{
    private ulong _stateVariable1;
    private ulong _stateVariable2;

    internal PseudorandomNumberGenerator(in uint seed)
    {
        ulong normalisedSeed =
            seed == 0u ? 64ul + Constants.XoroshiroConstant0 : seed + Constants.XoroshiroConstant0;
        normalisedSeed = (normalisedSeed ^ (normalisedSeed >> 30)) * Constants.XoroshiroConstant1;
        normalisedSeed = (normalisedSeed ^ (normalisedSeed >> 27)) * Constants.XoroshiroConstant2;
        _stateVariable1 = normalisedSeed ^ (normalisedSeed >> 31);

        normalisedSeed = _stateVariable1 + Constants.XoroshiroConstant0;
        normalisedSeed = (normalisedSeed ^ (normalisedSeed >> 30)) * Constants.XoroshiroConstant1;
        normalisedSeed = (normalisedSeed ^ (normalisedSeed >> 27)) * Constants.XoroshiroConstant2;
        _stateVariable2 = normalisedSeed ^ (normalisedSeed >> 31);
    }

    // range is [0f,1f)
    internal float NextSingle()
    {
        ulong localStateVariable1 = _stateVariable1;
        ulong localStateVariable2 = _stateVariable2;
        ulong sum = localStateVariable1 + localStateVariable2;
        ulong rotatedSum = (sum << 17) | (sum >> 47);
        ulong result = rotatedSum + localStateVariable1;

        localStateVariable2 ^= localStateVariable1;
        _stateVariable1 =
            ((localStateVariable1 << 24) | (localStateVariable1 >> 40))
            ^ localStateVariable2
            ^ (localStateVariable2 << 16);
        _stateVariable2 = (localStateVariable2 << 37) | (localStateVariable2 >> 27);

        uint mantissaBits = (uint)(result >> 41);
        uint resultBits = Constants.FloatMask | mantissaBits;

        return BitConverter.UInt32BitsToSingle(resultBits) - 1f;
    }
}
