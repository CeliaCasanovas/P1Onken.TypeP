namespace P1Onken.TypeP.Engine.Operators;

internal class OscillatorConfig
{
    // phase distortion params
    internal TransferFunction TransferFunction;

    // phase modulation params
    internal float AmplitudeMultiplier;
    internal float Feedback;
    internal float FrequencyMultiplier;
    internal float InharmonicityMultiplier;
    internal float CoarseDetune;
    internal float FineDetune;
    internal bool IsFixedFrequency;

    // xenakis params
    internal float XenakisAmplitudeMultiplier;
    internal float GrainTriggerFrequency;
    internal float GrainLength;
    internal float GrainWindowSharpness;
    internal float GrainTransferFunctionVDistortion;
    internal float GrainTransferFunctionDDistortion;
    internal float GrainPitch;
    internal float GrainAmplitude;
    internal float GrainStartingPhase;

    // spectral params

    // modifies TransferFunction.V:
    // FormantFrequency/FundamentalFrequency = 2 * TransferFunction.V - 1f
    // (see VPS paper)
    // bandwidth param is not needed as it's simply moving TransferFunction.D towards 0
    internal bool isSpectral;
    internal float FormantFrequency;

    // multiply signal by (Inharmonicity + FundamentalFrequency^Assymetry)
    internal float Assymetry;
    internal float Inharmonicity;

}
