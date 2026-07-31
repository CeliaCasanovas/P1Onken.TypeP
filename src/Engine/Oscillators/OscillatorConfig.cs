namespace P1Onken.TypeP.Engine.Oscillators;

internal struct OscillatorConfig
{
    // phase distortion params
    internal TransferFunction TransferFunction;
    internal bool HasAntialias;

    // phase modulation params
    internal float AudioAmplitudeMultiplier;
    internal float PhaseModulationAmplitudeMultiplier;
    internal float Feedback;
    internal float FrequencyMultiplier;
    internal float InharmonicityMultiplier;
    internal float CoarseDetune;
    internal float FineDetune;
    internal bool HasFixedFrequency;

    // xenakis params
    internal float XenakisAudioAmplitudeMultiplier;
    internal float GrainTriggerFrequency;
    internal float GrainLength;
    internal float GrainWindowSharpness;
    internal float GrainTransferFunctionVDistortion;
    internal float GrainTransferFunctionDDistortion;
    internal float GrainPitch;
    internal float GrainAmplitude;
    internal float GrainCentralStartingPhase;
    internal float GrainCentralBand; // 0f is band1, 0.33f band2, 0.66f band3, 1f band1

    // spectral params

    // modifies TransferFunction.V:
    // FormantFrequency/FundamentalFrequency = 2 * TransferFunction.V - 1f
    // (see VPS paper)
    // bandwidth param is not needed as it's simply moving TransferFunction.D towards 0
    internal float Band1FormantFrequency;
    internal float Band2FormantFrequency;
    internal float Band3FormantFrequency;
    internal float Band4FormantFrequency;
    internal float Band1AmplitudeMultiplier;
    internal float Band2AmplitudeMultiplier;
    internal float Band3AmplitudeMultiplier;
    internal float Band4AmplitudeMultiplier;

    // multiply signal by (Inharmonicity + FundamentalFrequency^Assymetry)
    internal float Band1Assymetry;
    internal float Band2Assymetry;
    internal float Band3Assymetry;
    internal float Band4Assymetry;
    internal float Band1Inharmonicity;
    internal float Band2Inharmonicity;
    internal float Band3Inharmonicity;
    internal float Band4Inharmonicity;
}
