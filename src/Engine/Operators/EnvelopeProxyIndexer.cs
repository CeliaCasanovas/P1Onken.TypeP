namespace P1Onken.TypeP.Engine.Operators;

internal ref struct EnvelopeProxyIndexer
{
    private ref EnvelopeConfig _config;
    private int _stage;

    internal EnvelopeProxyIndexer(ref EnvelopeConfig envelope, EnvelopeStage stage)
    {
        _config = ref envelope;
        _stage = (int)stage;
    }

    internal float TargetLevel
    {
        get => _config.TargetLevels[_stage];
        set => _config.TargetLevels[_stage] = value;
    }
    internal float ExponentialFactor
    {
        get => _config.ExponentialFactors[_stage];
        set => _config.ExponentialFactors[_stage] = value;
    }
    internal int LengthSamples
    {
        get => _config.LengthsSamples[_stage];
        set => _config.LengthsSamples[_stage] = value;
    }
}
