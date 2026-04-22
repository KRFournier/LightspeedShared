using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lightspeed.MatchComponents;

/// <summary>
/// Represents a two-sided match with a left and right side.
/// </summary>
public partial class LeftRightViewModel : ObservableObject
{
    #region Properties

    /// <summary>
    /// The left side of the match, representing one participant and their score.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(HasBye))]
    public partial SideViewModel? Left { get; set; }

    /// <summary>
    /// The right side of the match, representing the other participant and their score.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(HasBye))]
    public partial SideViewModel? Right { get; set; }

    /// <summary>
    /// Determines if the left and right sides are swapped. This is used for display purposes and does not affect the underlying data.
    /// </summary>
    [ObservableProperty]
    public partial bool IsSwapped { get; set; } = false;

    /// <summary>
    /// The winner of the match, if there is one
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Loser))]
    [NotifyPropertyChangedFor(nameof(HasWinner))]
    [NotifyPropertyChangedFor(nameof(WinningSide))]
    [NotifyPropertyChangedFor(nameof(IsLeftWinner))]
    [NotifyPropertyChangedFor(nameof(IsRightWinner))]
    public partial SideViewModel? Winner { get; set; }

    /// <summary>
    /// The winner referenced by position in the match
    /// </summary>
    public SideReference WinningSide
    {
        get => this.ToReference(Winner);
        set => Winner = this.ToSide(value);
    }

    /// <summary>
    /// The loser of the match
    /// </summary>
    public SideViewModel? Loser
    {
        get
        {
            if (Winner is not null)
            {
                if (Winner == Left)
                    return Right;
                else if (Winner == Right)
                    return Left;
            }
            return null;
        }
    }

    /// <summary>
    /// Determines if the winner is left
    /// </summary>
    public bool IsLeftWinner => Winner == Left;

    /// <summary>
    /// Determines if the winner is right
    /// </summary>
    public bool IsRightWinner => Winner == Right;

    /// <summary>
    /// Determines if the match is completed based on whether or not there is a winner
    /// </summary>
    public virtual bool HasWinner => Winner is not null;

    /// <summary>
    /// A bye is when one side of the match is empty, meaning that the other side automatically wins.
    /// </summary>
    public bool HasBye => !IsEmpty && (Left is null || Right is null);

    /// <summary>
    /// A match is empty when both sides are null
    /// </summary>
    public bool IsEmpty => Left is null && Right is null;

    #endregion

    #region Commands

    [RelayCommand]
    public void SwapSides() => IsSwapped = !IsSwapped;

    #endregion

    public LeftRightSide ToModel() => new()
    {
        Left = Left?.ToModel(),
        Right = Right?.ToModel(),
        IsSwapped = IsSwapped,
        WinningSide = WinningSide
    };

    public override string ToString() => IsSwapped ? $"{Right} vs. {Left}" : $"{Left} vs. {Right}";
}