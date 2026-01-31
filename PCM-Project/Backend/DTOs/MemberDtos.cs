namespace PCM.API.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }
    public double RankLevel { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalMatches { get; set; }
    public int WinMatches { get; set; }
    public double WinRate => TotalMatches > 0 ? (double)WinMatches / TotalMatches * 100 : 0;
}

public class UpdateMemberDto
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

public class TopRankingDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public double RankLevel { get; set; }
    public int TotalMatches { get; set; }
    public int WinMatches { get; set; }
}
