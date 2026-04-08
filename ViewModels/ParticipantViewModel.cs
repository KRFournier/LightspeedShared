using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using System.ComponentModel;

namespace Lightspeed.ViewModels;

#region Messages

public sealed record ParticipantDisqualifedChanged(ParticipantViewModel Participant);

#endregion

public abstract partial class ParticipantViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    public abstract string Name { get; }
    public virtual string? Subtitle => null;
    public abstract int PowerLevel { get; }
    public abstract bool IsDisqualified { get; }
    public abstract bool IsBye { get; }
    public abstract bool IsEmpty { get; }
    public virtual PlayerViewModel? CurrentPlayer => null;

    public Guid Guid { get; set; } = Guid.NewGuid();

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
    public override bool IsDisqualified => true;
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
    public override bool IsDisqualified => false;
    public override string ToString() => "EMPTY";
}
