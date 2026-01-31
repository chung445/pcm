using Microsoft.EntityFrameworkCore;
using PCM.API.Data;
using PCM.API.DTOs;

namespace PCM.API.Services;

public class MemberService : IMemberService
{
    private readonly ApplicationDbContext _context;

    public MemberService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberDto>> GetAllMembersAsync()
    {
        return await _context.Members
            .Select(m => new MemberDto
            {
                Id = m.Id,
                FullName = m.FullName,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                DateOfBirth = m.DateOfBirth,
                JoinDate = m.JoinDate,
                RankLevel = m.RankLevel,
                Status = m.Status.ToString(),
                TotalMatches = m.TotalMatches,
                WinMatches = m.WinMatches
            })
            .ToListAsync();
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null)
            return null;

        return new MemberDto
        {
            Id = member.Id,
            FullName = member.FullName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            DateOfBirth = member.DateOfBirth,
            JoinDate = member.JoinDate,
            RankLevel = member.RankLevel,
            Status = member.Status.ToString(),
            TotalMatches = member.TotalMatches,
            WinMatches = member.WinMatches
        };
    }

    public async Task<MemberDto?> UpdateMemberAsync(int id, UpdateMemberDto dto, string userId)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null || member.UserId != userId)
            return null;

        if (!string.IsNullOrEmpty(dto.FullName))
            member.FullName = dto.FullName;
        if (!string.IsNullOrEmpty(dto.PhoneNumber))
            member.PhoneNumber = dto.PhoneNumber;
        if (dto.DateOfBirth.HasValue)
            member.DateOfBirth = dto.DateOfBirth;

        member.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetMemberByIdAsync(id);
    }

    public async Task<List<TopRankingDto>> GetTopRankingAsync(int limit)
    {
        return await _context.Members
            .OrderByDescending(m => m.RankLevel)
            .Take(limit)
            .Select(m => new TopRankingDto
            {
                Id = m.Id,
                FullName = m.FullName,
                RankLevel = m.RankLevel,
                TotalMatches = m.TotalMatches,
                WinMatches = m.WinMatches
            })
            .ToListAsync();
    }
}
