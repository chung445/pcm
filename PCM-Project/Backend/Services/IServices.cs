using PCM.API.DTOs;

namespace PCM.API.Services;

public interface IMemberService
{
    Task<List<MemberDto>> GetAllMembersAsync();
    Task<MemberDto?> GetMemberByIdAsync(int id);
    Task<MemberDto?> UpdateMemberAsync(int id, UpdateMemberDto dto, string userId);
    Task<List<TopRankingDto>> GetTopRankingAsync(int limit);
}

public interface ITransactionService
{
    Task<List<TransactionDto>> GetAllTransactionsAsync();
    Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto dto, string userId);
    Task<TransactionSummaryDto> GetSummaryAsync();
    Task<bool> ApproveTransactionAsync(int id, string approverUserId);
    Task<bool> RejectTransactionAsync(int id, string approverUserId);
}

public interface IBookingService
{
    Task<List<BookingDto>> GetAllBookingsAsync();
    Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string userId);
    Task<bool> DeleteBookingAsync(int id, string userId);
    Task<bool> CheckAvailabilityAsync(int courtId, DateTime startTime, DateTime endTime);
    Task<bool> ApproveBookingAsync(int id, string adminUserId);
    Task<bool> RejectBookingAsync(int id, string adminUserId);
}

public interface IChallengeService
{
    Task<List<ChallengeDto>> GetAllChallengesAsync();
    Task<ChallengeDto?> GetChallengeByIdAsync(int id);
    Task<ChallengeDto?> CreateChallengeAsync(CreateChallengeDto dto, string userId);
    Task<bool> JoinChallengeAsync(int challengeId, string userId);
    Task<bool> AutoDivideTeamsAsync(int challengeId);
}

public interface IMatchService
{
    Task<List<MatchDto>> GetAllMatchesAsync();
    Task<MatchDto?> GetMatchByIdAsync(int id);
    Task<MatchDto?> CreateMatchAsync(CreateMatchDto dto);
}
