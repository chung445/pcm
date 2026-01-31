namespace PCM.API.DTOs;

// News DTOs
public class NewsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateNewsDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; } = false;
}

// Transaction DTOs
public class TransactionDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedDate { get; set; }
}

public class CreateTransactionDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}

public class TransactionSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public bool IsNegative => Balance < 0;
}

// Booking DTOs
public class BookingDto
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateBookingDto
{
    public int CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}

// Challenge DTOs
public class ChallengeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string GameMode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Config_TargetWins { get; set; }
    public int CurrentScore_TeamA { get; set; }
    public int CurrentScore_TeamB { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public int ParticipantCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateChallengeDto
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string GameMode { get; set; } = string.Empty;
    public int? Config_TargetWins { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// Match DTOs
public class MatchDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsRanked { get; set; }
    public int? ChallengeId { get; set; }
    public string? ChallengeTitle { get; set; }
    public string MatchFormat { get; set; } = string.Empty;
    public string Team1_Player1Name { get; set; } = string.Empty;
    public string? Team1_Player2Name { get; set; }
    public string Team2_Player1Name { get; set; } = string.Empty;
    public string? Team2_Player2Name { get; set; }
    public string WinningSide { get; set; } = string.Empty;
}

public class CreateMatchDto
{
    public DateTime Date { get; set; }
    public bool IsRanked { get; set; } = true;
    public int? ChallengeId { get; set; }
    public string MatchFormat { get; set; } = string.Empty;
    public int Team1_Player1Id { get; set; }
    public int? Team1_Player2Id { get; set; }
    public int Team2_Player1Id { get; set; }
    public int? Team2_Player2Id { get; set; }
    public string WinningSide { get; set; } = string.Empty;
}

// Court DTOs
public class CourtDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class CreateCourtDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}

// Category DTOs
public class TransactionCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class CreateTransactionCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
