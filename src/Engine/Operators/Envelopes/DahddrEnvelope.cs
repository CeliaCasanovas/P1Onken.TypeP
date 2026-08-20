namespace P1Onken.TypeP.Engine.Operators.Envelopes;

internal unsafe class DahddrEnvelope
{
    private EnvelopeConfig _config;
    private float _sampleAccumulator = 0f;
    private EnvelopeStage _stage;

    internal EnvelopeProxyIndexer this[EnvelopeStage stage] => new(ref _config, stage);

    internal float AttackCurve
    {
        get => _config.AttackCurve;
        set => _config.AttackCurve = value;
    }

    internal float AttackSigmoidFactor
    {
        get => _config.AttackSigmoidFactor;
    }

    // gate behaviour, release and loop behaviour missing
    internal float ComputeNextAmplitude(float currentAmplitude)
    {
        _sampleAccumulator += this[_stage].StepLength;
        if (_sampleAccumulator >= this[_stage].LengthSamples)
        {
            _stage++;
            _sampleAccumulator = 0f;
        }
        return EnvelopeComputation.Computations[(int)_stage](currentAmplitude, this, _stage);
    }
}
