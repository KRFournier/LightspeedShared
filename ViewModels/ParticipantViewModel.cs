using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using System.ComponentModel;

namespace Lightspeed.ViewModels;

#region Messages

public sealed class AddMajorViolationMessage(ParticipantViewModel participant)
{
    public ParticipantViewModel Participant => participant;
}

public sealed class EjectParticipantMessage(ParticipantViewModel participant)
{
    public ParticipantViewModel Participant => participant;
}

public sealed class ParticipantHonorChangedMessage(ParticipantViewModel participant)
{
    public ParticipantViewModel Participant => participant;
}

public sealed class ParticipantDisqualifedChanged(ParticipantViewModel participant)
{
    public ParticipantViewModel Participant => participant;
}

#endregion

public abstract partial class ParticipantViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    public abstract string Name { get; }
    public virtual string? Subtitle => null;
    public abstract int PowerLevel { get; }
    public abstract bool IsBye { get; }
    public abstract bool IsEmpty { get; }
    public virtual PlayerViewModel? CurrentPlayer => null;

    public Guid Guid { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisqualified))]
    public partial Card Card { get; set; } = Card.None;

    [ObservableProperty]
    public partial int Honor { get; set; } = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisqualified))]
    public partial bool IsEjected { get; set; } = false;

    /// <summary>
    /// Determines if the Player is disqualified either by card or ejection
    /// </summary>
    public bool IsDisqualified => Card == Card.Black || IsEjected;

    #endregion

    #region Commands

    [RelayCommand]
    public void GiveCard()
    {
        // send a message to the client to give a card to this player.
        // this allows the client to handle how cards are given, which may involve showing a confirmation dialog or allowing the user to specify the reason for the violation.
        if (Card != Card.Black)
            Send(new AddMajorViolationMessage(this));
    }

    [RelayCommand]
    public void Eject()
    {
        // send a message to the client to eject this player
        // this allows the client to handle how ejection is done, which may involve showing a confirmation dialog or allowing the user to specify the reason for the violation.
        if (!IsEjected)
            Send(new EjectParticipantMessage(this));
    }

    [RelayCommand]
    public void AddHonor()
    {
        Honor++;
        Send(new ParticipantHonorChangedMessage(this));
    }

    [RelayCommand]
    public void RemoveHonor()
    {
        if (Honor > 0)
        {
            Honor--;
            Send(new ParticipantHonorChangedMessage(this));
        }
    }

    #endregion

    public abstract IParticipant ToModel();

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        // we need to let others know when a participant's disqualification status changes so that matches can update accordingly
        if (e.PropertyName == nameof(IsDisqualified))
            Send(new ParticipantDisqualifedChanged(this), Guid);
        base.OnPropertyChanged(e);
    }

    public override string ToString() => Name;

    public virtual ParticipantState ToState() => throw new NotImplementedException("ToState not implemented.");
    public virtual void UpdateState(ParticipantState? state) => throw new NotImplementedException("UpdateState not implemented.");
}

/// <summary>
/// A placeholder for matches in which a participant has a bye
/// </summary>
public sealed partial class ByeViewModel : ParticipantViewModel
{
    public ByeViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Guid = ByeParticipant.ByeGuid;
    }

    public override string Name => string.Empty;
    public override int PowerLevel => 0;
    public override ByeParticipant ToModel() => new();
    public override bool IsBye => true;
    public override bool IsEmpty => false;
    public override string ToString() => "BYE";
}

/// <summary>
/// A placeholder for matches in which a participant has a bye
/// </summary>
public sealed partial class EmptyParticipantViewModel : ParticipantViewModel
{
    public EmptyParticipantViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Guid = Guid.Empty;
    }

    public override string Name => string.Empty;
    public override int PowerLevel => 0;
    public override EmptyParticipant ToModel() => new();
    public override bool IsBye => false;
    public override bool IsEmpty => true;
    public override string ToString() => "EMPTY";
}
