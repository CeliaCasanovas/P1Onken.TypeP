using P1Onken.TypeP.Engine.Core;

namespace P1Onken.TypeP.Engine.Operators;

internal class OscillatorState
{
    // 1 float is 32 bits = 4 bytes
    // 4 bytes/float * 128 floats/buffer = 512 bytes/buffer
    // 512 bytes/buffer * 5 buffers = 2560 bytes = 2.5kiB per oscillator
    internal float[] RawPhases = new float[Constants.BlockSize];
    internal float[] DistortedModulatedPhases = new float[Constants.BlockSize];
    internal float[] LerpedPhases = new float[Constants.BlockSize];
    internal float[] Signals = new float[Constants.BlockSize];
    internal float[] ScratchBuffer = new float[Constants.BlockSize];
}
