namespace Lightspeed.Utilities;

/// <summary>
/// Provides utility methods for floating-point math comparisons.
/// </summary>
public static class MathUtilities
{
    private const double Epsilon = 1e-10;

    /// <summary>
    /// Determines whether two double values are close enough to be considered equal,
    /// accounting for floating-point precision issues.
    /// </summary>
    /// <param name="value1">The first value to compare.</param>
    /// <param name="value2">The second value to compare.</param>
    /// <returns>True if the values are within epsilon of each other; otherwise, false.</returns>
    public static bool AreClose(double value1, double value2)
    {
        if (value1 == value2) return true;
        double diff = value1 - value2;
        return diff > -Epsilon && diff < Epsilon;
    }

    /// <summary>
    /// Determines whether a double value is close enough to zero to be considered zero,
    /// accounting for floating-point precision issues.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is within epsilon of zero; otherwise, false.</returns>
    public static bool IsZero(double value)
    {
        return value > -Epsilon && value < Epsilon;
    }
}
