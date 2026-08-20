using System.Net.Security;
using System.Numerics.Tensors;

namespace P1Onken.TypeP.Engine.Core;

internal static class Maths
{
    // internal static float FastExp(float x)
    // {
    //     x = x < -87f ? -87f : (x > 88f ? 88f : x);
    //     int bits = (int)(Constants.SchraudolphMultiplier * x + Constants.SchraudolphCorrection);
    //     return BitConverter.Int32BitsToSingle(bits);
    // }

    internal static float FastExp(float x)
    {
        x = x < -87f ? -87f : (x > 88f ? 88f : x);

        float exponent = Constants.Log2E * x;
        exponent = exponent < -126f ? -126f : exponent;

        float signOffset = exponent < 0f ? 1f : 0f;

        int integerPart = (int)exponent;
        float mantissa = exponent - integerPart + signOffset;

        int bits = (int)(
            (1 << 23)
            * (
                exponent
                + Constants.MineiroA
                + Constants.MineiroB / (Constants.MineiroC - mantissa)
                - Constants.MineiroD * mantissa
            )
        );

        return BitConverter.Int32BitsToSingle(bits);
    }

    // x [0,1]
    // with x transposed to [-pi, pi]:
    // -cos(x) = c0 + (x^2)(c2 + (x^2)(c4 + (x^2)c6))
    internal static float FastMinusCos(float x)
    {
        x = MathF.FusedMultiplyAdd(x, Constants.TwoPi, Constants.Pi);
        float xSquared = x * x;

        float scratchBuffer = MathF.FusedMultiplyAdd(
            xSquared,
            Constants.MinimaxMinusCosCoefficient6,
            Constants.MinimaxMinusCosCoefficient4
        );
        scratchBuffer = MathF.FusedMultiplyAdd(
            xSquared,
            scratchBuffer,
            Constants.MinimaxMinusCosCoefficient2
        );
        return MathF.FusedMultiplyAdd(
            xSquared,
            scratchBuffer,
            Constants.MinimaxMinusCosCoefficient0
        );
    }

    internal static Span<float> FastMinusCosTensor(Span<float> xs, Span<float> scratchBuffer)
    {
        TensorPrimitives.Subtract(xs, Constants.Pi, xs);
        TensorPrimitives.Multiply(xs, xs, xs);

        TensorPrimitives.FusedMultiplyAdd(
            xs,
            Constants.MinimaxMinusCosCoefficient6s,
            Constants.MinimaxMinusCosCoefficient4,
            scratchBuffer
        );
        TensorPrimitives.FusedMultiplyAdd(
            xs,
            scratchBuffer,
            Constants.MinimaxMinusCosCoefficient2,
            scratchBuffer
        );
        TensorPrimitives.FusedMultiplyAdd(
            xs,
            scratchBuffer,
            Constants.MinimaxMinusCosCoefficient0,
            scratchBuffer
        );

        return scratchBuffer;
    }
}
