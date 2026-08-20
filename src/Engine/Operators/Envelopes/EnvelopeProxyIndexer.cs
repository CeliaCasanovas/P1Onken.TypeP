using Microsoft.VisualBasic;

namespace P1Onken.TypeP.Engine.Operators.Envelopes;

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
    }
    internal float LengthSamples
    {
        get => _config.LengthsSamples[_stage];
        set
        {
            _config.LengthsSamples[_stage] = value;
            _config.StepLengths[_stage] = EnvelopeComputation.ComputeStep(value);
            _config.ExponentialFactors[_stage] = EnvelopeComputation.ComputeExponentialFactor(
                value
            );
            if (_stage == (int)EnvelopeStage.Attack)
            {
                _config.AttackSigmoidFactor = EnvelopeComputation.ComputeSigmoidFactor(value);
            }
        }
    }
    internal float StepLength
    {
        get => _config.StepLengths[_stage];
    }
}
