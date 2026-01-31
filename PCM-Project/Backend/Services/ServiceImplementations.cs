using Microsoft.EntityFrameworkCore;
using PCM.API.Data;
using PCM.API.DTOs;
using PCM.API.Models;

namespace PCM.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionDto>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Description = t.Description,
                CategoryId = t.CategoryId,
                CategoryName = t.Category!.Name,
                CategoryType = t.Category.Type.ToString(),
                Status = t.Status.ToString(),
                ApprovedByUserId = t.ApprovedByUserId,
                ApprovedDate = t.ApprovedDate
            })
            .ToListAsync();
    }

    public async Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto dto, string userId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
            return null;

        var transaction = new Transaction
        {
            Date = dto.Date,
            Amount = dto.Amount,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
                CreatedBy = member.Id,
                Status = TransactionStatus.Pending
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Id == transaction.Id)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Date = t.Date,
                    Amount = t.Amount,
                    Description = t.Description,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category!.Name,
                    CategoryType = t.Category.Type.ToString(),
                    Status = t.Status.ToString(),
                    ApprovedByUserId = t.ApprovedByUserId,
                    ApprovedDate = t.ApprovedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ApproveTransactionAsync(int id, string approverUserId)
        {
            var tx = await _context.Transactions.FindAsync(id);
            if (tx == null) return false;

            tx.Status = TransactionStatus.Approved;
            tx.ApprovedByUserId = approverUserId;
            tx.ApprovedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTransactionAsync(int id, string approverUserId)
        {
            var tx = await _context.Transactions.FindAsync(id);
            if (tx == null) return false;

            tx.Status = TransactionStatus.Rejected;
            tx.ApprovedByUserId = approverUserId;
            tx.ApprovedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TransactionSummaryDto> GetSummaryAsync()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.Category!.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.Category!.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            return new TransactionSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense
            };
        }
    }


public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;

    public BookingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookingDto>> GetAllBookingsAsync()
    {
        return await _context.Bookings
            .Include(b => b.Court)
            .Include(b => b.Member)
            .OrderByDescending(b => b.StartTime)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                CourtId = b.CourtId,
                CourtName = b.Court!.Name,
                MemberId = b.MemberId,
                MemberName = b.Member!.FullName,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                Notes = b.Notes
            })
            .ToListAsync();
    }

    public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string userId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
            return null;

        // Check availability
        var isAvailable = await CheckAvailabilityAsync(dto.CourtId, dto.StartTime, dto.EndTime);
        if (!isAvailable)
            return null;

        var booking = new Booking
        {
            CourtId = dto.CourtId,
            MemberId = member.Id,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = BookingStatus.Pending,
            Notes = dto.Notes
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return await _context.Bookings
            .Include(b => b.Court)
            .Include(b => b.Member)
            .Where(b => b.Id == booking.Id)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                CourtId = b.CourtId,
                CourtName = b.Court!.Name,
                MemberId = b.MemberId,
                MemberName = b.Member!.FullName,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                Notes = b.Notes
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ApproveBookingAsync(int id, string adminUserId)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Confirmed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectBookingAsync(int id, string adminUserId)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        booking.Status = BookingStatus.Rejected;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBookingAsync(int id, string userId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
            return false;

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null || booking.MemberId != member.Id)
            return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckAvailabilityAsync(int courtId, DateTime startTime, DateTime endTime)
    {
        var overlapping = await _context.Bookings
            .Where(b => b.CourtId == courtId &&
                       b.Status != BookingStatus.Cancelled &&
                       ((b.StartTime < endTime && b.EndTime > startTime)))
            .AnyAsync();

        return !overlapping;
    }
}

public class ChallengeService : IChallengeService
{
    private readonly ApplicationDbContext _context;

    public ChallengeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChallengeDto>> GetAllChallengesAsync()
    {
        return await _context.Challenges
            .Include(c => c.Participants)
            .Select(c => new ChallengeDto
            {
                Id = c.Id,
                Title = c.Title,
                Type = c.Type.ToString(),
                GameMode = c.GameMode.ToString(),
                Status = c.Status.ToString(),
                Config_TargetWins = c.Config_TargetWins,
                CurrentScore_TeamA = c.CurrentScore_TeamA,
                CurrentScore_TeamB = c.CurrentScore_TeamB,
                EntryFee = c.EntryFee,
                PrizePool = c.PrizePool,
                ParticipantCount = c.Participants.Count,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            })
            .ToListAsync();
    }

    public async Task<ChallengeDto?> GetChallengeByIdAsync(int id)
    {
        return await _context.Challenges
            .Include(c => c.Participants)
            .Where(c => c.Id == id)
            .Select(c => new ChallengeDto
            {
                Id = c.Id,
                Title = c.Title,
                Type = c.Type.ToString(),
                GameMode = c.GameMode.ToString(),
                Status = c.Status.ToString(),
                Config_TargetWins = c.Config_TargetWins,
                CurrentScore_TeamA = c.CurrentScore_TeamA,
                CurrentScore_TeamB = c.CurrentScore_TeamB,
                EntryFee = c.EntryFee,
                PrizePool = c.PrizePool,
                ParticipantCount = c.Participants.Count,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ChallengeDto?> CreateChallengeAsync(CreateChallengeDto dto, string userId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
            return null;

        var challenge = new Challenge
        {
            Title = dto.Title,
            Type = Enum.Parse<ChallengeType>(dto.Type),
            GameMode = Enum.Parse<GameMode>(dto.GameMode),
            Config_TargetWins = dto.Config_TargetWins,
            EntryFee = dto.EntryFee,
            PrizePool = dto.PrizePool,
            CreatedBy = member.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = ChallengeStatus.Open
        };

        _context.Challenges.Add(challenge);
        await _context.SaveChangesAsync();

        return await GetChallengeByIdAsync(challenge.Id);
    }

    public async Task<bool> JoinChallengeAsync(int challengeId, string userId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
            return false;

        var challenge = await _context.Challenges.FindAsync(challengeId);
        if (challenge == null || challenge.Status != ChallengeStatus.Open)
            return false;

        var existing = await _context.Participants
            .AnyAsync(p => p.ChallengeId == challengeId && p.MemberId == member.Id);
        if (existing)
            return false;

        var participant = new Participant
        {
            ChallengeId = challengeId,
            MemberId = member.Id,
            EntryFeePaid = true,
            EntryFeeAmount = challenge.EntryFee,
            Status = ParticipantStatus.Confirmed
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AutoDivideTeamsAsync(int challengeId)
    {
        var challenge = await _context.Challenges
            .Include(c => c.Participants)
            .ThenInclude(p => p.Member)
            .FirstOrDefaultAsync(c => c.Id == challengeId);

        if (challenge == null || challenge.GameMode != GameMode.TeamBattle)
            return false;

        var participants = challenge.Participants
            .OrderByDescending(p => p.Member!.RankLevel)
            .ToList();

        for (int i = 0; i < participants.Count; i++)
        {
            participants[i].Team = i % 2 == 0 ? ParticipantTeam.TeamA : ParticipantTeam.TeamB;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}

public class MatchService : IMatchService
{
    private readonly ApplicationDbContext _context;

    public MatchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MatchDto>> GetAllMatchesAsync()
    {
        return await _context.Matches
            .Include(m => m.Team1_Player1)
            .Include(m => m.Team1_Player2)
            .Include(m => m.Team2_Player1)
            .Include(m => m.Team2_Player2)
            .Include(m => m.Challenge)
            .OrderByDescending(m => m.Date)
            .Select(m => new MatchDto
            {
                Id = m.Id,
                Date = m.Date,
                IsRanked = m.IsRanked,
                ChallengeId = m.ChallengeId,
                ChallengeTitle = m.Challenge != null ? m.Challenge.Title : null,
                MatchFormat = m.MatchFormat.ToString(),
                Team1_Player1Name = m.Team1_Player1!.FullName,
                Team1_Player2Name = m.Team1_Player2 != null ? m.Team1_Player2.FullName : null,
                Team2_Player1Name = m.Team2_Player1!.FullName,
                Team2_Player2Name = m.Team2_Player2 != null ? m.Team2_Player2.FullName : null,
                WinningSide = m.WinningSide.ToString()
            })
            .ToListAsync();
    }

    public async Task<MatchDto?> GetMatchByIdAsync(int id)
    {
        return await _context.Matches
            .Include(m => m.Team1_Player1)
            .Include(m => m.Team1_Player2)
            .Include(m => m.Team2_Player1)
            .Include(m => m.Team2_Player2)
            .Include(m => m.Challenge)
            .Where(m => m.Id == id)
            .Select(m => new MatchDto
            {
                Id = m.Id,
                Date = m.Date,
                IsRanked = m.IsRanked,
                ChallengeId = m.ChallengeId,
                ChallengeTitle = m.Challenge != null ? m.Challenge.Title : null,
                MatchFormat = m.MatchFormat.ToString(),
                Team1_Player1Name = m.Team1_Player1!.FullName,
                Team1_Player2Name = m.Team1_Player2 != null ? m.Team1_Player2.FullName : null,
                Team2_Player1Name = m.Team2_Player1!.FullName,
                Team2_Player2Name = m.Team2_Player2 != null ? m.Team2_Player2.FullName : null,
                WinningSide = m.WinningSide.ToString()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<MatchDto?> CreateMatchAsync(CreateMatchDto dto)
    {
        var match = new Match
        {
            Date = dto.Date,
            IsRanked = dto.IsRanked,
            ChallengeId = dto.ChallengeId,
            MatchFormat = Enum.Parse<MatchFormat>(dto.MatchFormat),
            Team1_Player1Id = dto.Team1_Player1Id,
            Team1_Player2Id = dto.Team1_Player2Id,
            Team2_Player1Id = dto.Team2_Player1Id,
            Team2_Player2Id = dto.Team2_Player2Id,
            WinningSide = Enum.Parse<WinningSide>(dto.WinningSide)
        };

        _context.Matches.Add(match);

        // Update rank if ranked
        if (match.IsRanked && match.WinningSide != WinningSide.None)
        {
            var winners = new List<int> { match.Team1_Player1Id };
            var losers = new List<int> { match.Team2_Player1Id };

            if (match.WinningSide == WinningSide.Team2)
            {
                (winners, losers) = (losers, winners);
            }

            if (match.Team1_Player2Id.HasValue)
                (match.WinningSide == WinningSide.Team1 ? winners : losers).Add(match.Team1_Player2Id.Value);
            if (match.Team2_Player2Id.HasValue)
                (match.WinningSide == WinningSide.Team2 ? winners : losers).Add(match.Team2_Player2Id.Value);

            foreach (var winnerId in winners)
            {
                var player = await _context.Members.FindAsync(winnerId);
                if (player != null)
                {
                    player.RankLevel += 0.1;
                    player.TotalMatches++;
                    player.WinMatches++;
                }
            }

            foreach (var loserId in losers)
            {
                var player = await _context.Members.FindAsync(loserId);
                if (player != null)
                {
                    player.RankLevel = Math.Max(0, player.RankLevel - 0.1);
                    player.TotalMatches++;
                }
            }
        }

        // Update challenge score if applicable
        if (match.ChallengeId.HasValue)
        {
            var challenge = await _context.Challenges.FindAsync(match.ChallengeId.Value);
            if (challenge != null && challenge.GameMode == GameMode.TeamBattle)
            {
                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.ChallengeId == match.ChallengeId.Value && p.MemberId == match.Team1_Player1Id);

                if (participant != null)
                {
                    if (match.WinningSide == WinningSide.Team1 && participant.Team == ParticipantTeam.TeamA)
                        challenge.CurrentScore_TeamA++;
                    else if (match.WinningSide == WinningSide.Team1 && participant.Team == ParticipantTeam.TeamB)
                        challenge.CurrentScore_TeamB++;
                    else if (match.WinningSide == WinningSide.Team2 && participant.Team == ParticipantTeam.TeamA)
                        challenge.CurrentScore_TeamB++;
                    else if (match.WinningSide == WinningSide.Team2 && participant.Team == ParticipantTeam.TeamB)
                        challenge.CurrentScore_TeamA++;

                    if (challenge.Config_TargetWins.HasValue &&
                        (challenge.CurrentScore_TeamA >= challenge.Config_TargetWins.Value ||
                         challenge.CurrentScore_TeamB >= challenge.Config_TargetWins.Value))
                    {
                        challenge.Status = ChallengeStatus.Finished;
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        return await GetMatchByIdAsync(match.Id);
    }
}
