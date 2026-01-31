namespace PCM.API.Models;

public enum MemberStatus
{
    Active,
    Inactive,
    Suspended
}

public enum TransactionType
{
    Income,
    Expense
}

public enum TransactionStatus
{
    Pending,
    Approved,
    Rejected
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Rejected,
    Cancelled
}

public enum ChallengeType
{
    Duel,
    MiniGame
}

public enum GameMode
{
    None,
    TeamBattle,
    RoundRobin
}

public enum ChallengeStatus
{
    Open,
    Ongoing,
    Finished
}

public enum MatchFormat
{
    Singles,
    Doubles
}

public enum WinningSide
{
    None,
    Team1,
    Team2
}

public enum ParticipantTeam
{
    None,
    TeamA,
    TeamB
}

public enum ParticipantStatus
{
    Pending,
    Confirmed,
    Withdrawn
}

public enum NotificationType
{
    Info,
    Warning,
    Success,
    Error
}
