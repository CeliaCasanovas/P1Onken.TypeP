namespace P1Onken.TypeP.Engine.Oscillators;

internal struct XenakisGrain
{
    internal float CarrierPhase; // stochastic start, then accumulates normally
    internal float WindowPhase; // 0..1, tracks duration, unrelated to CarrierPhase
    internal float Frequency; // centerFreq × 2^(pitchOffset/12)
    internal float DurationSamples;
    internal float Amplitude;
    internal bool Active;
}


// calculating xenakis

// set stochastic grain trigger in motion <- GrainTriggerFrequency (Poisson)
// distort/modulate transfer phase
// calculate current band <- GrainCentralBand (Gauss or Logistic)
// calculate starting phase <- GrainCentralStartingPhase (Gauss or Logistic)
// calculate length in samples <- GrainLength, GrainPitch
// apply local distortion to v and d <- GrainTransferFunctionVDistortion, GrainTransferFunctionDDistortion
// get lengthSamples target phases <- accumulate phase if needed
// calculate lengthSamples signals
// scale grain amplitude <- GrainAmplitude
// apply amplitude window <- GrainWindowSharpness <- GrainPhaseAccumulator/lengthSamples
// send to mix

// idea for lissajous transferfunction drift
// float d = Clamp(baseD + dSpread * 0.5f * MathF.Cos(2f*Constants.Pi*omegad*distortedPhase + theta), epsilon, 1f-epsilon);
// float v = baseV + vSpread * 0.5f * MathF.Cos(2f*Constants.Pi*omegav*distortedPhase);
// var grainTf = new TransferFunction(d, v);
// omegad:omegav ratio 1:1, 2:1, 3:1, 1:3... vps paperfigure 7
// theta 0, pi/4, pi/2...

// stochastic tf drift for general function
// float d (or grain brightness) = Clamp0..1(shapeCenterD + GaussianSample(prng) * shapeSpread);
// float v (or grain formant) = shapeCenterV + GaussianSample(prng) * shapeSpread * vRangeScale;
// grain.Tf = new TransferFunction(d, v);

// per active grain, per sample:

// grain.CarrierPhase = Prng.NextFloat();
// grain.WindowPhase = 0f;
// grain.PitchOffset = GaussianSample(prng) * pitchSpreadSemitones;
// grain.Frequency = centerFreq * MathF.Pow(2f, grain.PitchOffset / 12f);
// grain.DurationSamples = SampleDuration(prng, grainSizeMean);  // needs a floor clamp
// grain.Amplitude = GetGrainAmplitude(prng, ampSpread, etc);

// grain.CarrierPhase = OscillatorCore.ComputeNextRawPhase(
//     grain.CarrierPhase, grain.Frequency, sampleRate);

// float modulated = OscillatorCore.ModulatePhase(
//     grain.CarrierPhase, fmInput[i] * fmAmount, feedbackIndex, slotFeedbackSample);

// float distorted = OscillatorCore.DistortPhase(modulated, baseTf);

// float folded = once;
// for (int k = 0; k < foldIterations; k++)
//     folded = OscillatorCore.DistortPhase(folded, baseTf);

// float rawSignal = OscillatorCore.ComputeSignal(distorted);

// float window = ComputeWindow(grain.WindowPhase, windowSharpness);
// output[i] += window * grain.Amplitude * raw;

// grain.WindowPhase += 1f / grain.DurationSamples;
// if (grain.WindowPhase >= 1f) grain.Active = false;
