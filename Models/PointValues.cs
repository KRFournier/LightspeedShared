namespace Lightspeed;

public static class PointValues
{
    public const int WhiteCard = 0;
    public const int YellowCard = 3;
    public const int RedCard = 3;
    public const int Clean = 3;
    public const int Conceded = 1;
    public const int Disarm = 3;
    public const int FirstContact = 1;
    public const int Headshot = 3;
    public const int HeadshotOverride = 1;
    public const int Indirect = 1;
    public const int OutOfBounds = 3;
    public const int FirstMinorViolation = 0;
    public const int SubsequentMinorViolation = 3;
    public const int Priority = 3;
    public const int Return = 5;

    public static int Max => Return;
}
