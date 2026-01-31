using Microsoft.AspNetCore.Identity;
using PCM.API.Models;

namespace PCM.API.Data;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if already seeded
        if (context.Members.Any())
        {
            return; // DB has been seeded
        }

        // Create Roles
        string[] roleNames = { "Admin", "Treasurer", "Referee", "Member" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create Admin User
        var adminUser = new ApplicationUser
        {
            UserName = "admin@pcm.com",
            Email = "admin@pcm.com",
            FullName = "Admin PCM",
            EmailConfirmed = true
        };

        var adminPassword = "Admin@123";
        var adminResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Create Member Users
        var memberData = new[]
        {
            new { Name = "Nguyễn Văn An", Email = "an@pcm.com", Rank = 4.5, Dob = new DateTime(1990, 5, 15) },
            new { Name = "Trần Thị Bình", Email = "binh@pcm.com", Rank = 4.2, Dob = new DateTime(1992, 8, 20) },
            new { Name = "Lê Hoàng Cường", Email = "cuong@pcm.com", Rank = 4.8, Dob = new DateTime(1988, 3, 10) },
            new { Name = "Phạm Thị Dung", Email = "dung@pcm.com", Rank = 4.0, Dob = new DateTime(1995, 11, 25) },
            new { Name = "Hoàng Văn Em", Email = "em@pcm.com", Rank = 3.8, Dob = new DateTime(1993, 7, 30) },
            new { Name = "Võ Thị Phương", Email = "phuong@pcm.com", Rank = 4.3, Dob = new DateTime(1991, 2, 14) },
            new { Name = "Đặng Minh Quân", Email = "quan@pcm.com", Rank = 4.6, Dob = new DateTime(1989, 9, 5) },
            new { Name = "Bùi Thị Hoa", Email = "hoa@pcm.com", Rank = 3.9, Dob = new DateTime(1994, 6, 18) }
        };

        var members = new List<Member>();
        foreach (var data in memberData)
        {
            var user = new ApplicationUser
            {
                UserName = data.Email,
                Email = data.Email,
                FullName = data.Name,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Member@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Member");

                var member = new Member
                {
                    UserId = user.Id,
                    FullName = data.Name,
                    Email = data.Email,
                    PhoneNumber = $"09{new Random().Next(10000000, 99999999)}",
                    DateOfBirth = data.Dob,
                    RankLevel = data.Rank,
                    JoinDate = DateTime.UtcNow.AddMonths(-6),
                    Status = MemberStatus.Active,
                    TotalMatches = 0,
                    WinMatches = 0
                };
                members.Add(member);
            }
        }

        await context.Members.AddRangeAsync(members);
        await context.SaveChangesAsync();

        // Create Admin Member
        var adminMember = new Member
        {
            UserId = adminUser.Id,
            FullName = "Admin PCM",
            Email = "admin@pcm.com",
            PhoneNumber = "0901234567",
            DateOfBirth = new DateTime(1985, 1, 1),
            RankLevel = 5.0,
            JoinDate = DateTime.UtcNow.AddYears(-2),
            Status = MemberStatus.Active
        };
        await context.Members.AddAsync(adminMember);
        await context.SaveChangesAsync();

        // Seed Courts
        var courts = new List<Court>
        {
            new Court { Name = "Sân 1", IsActive = true, Description = "Sân chính, đèn chiếu sáng tốt" },
            new Court { Name = "Sân 2", IsActive = true, Description = "Sân phụ, view đẹp" }
        };
        await context.Courts.AddRangeAsync(courts);
        await context.SaveChangesAsync();

        // Seed Transaction Categories
        var categories = new List<TransactionCategory>
        {
            new TransactionCategory { Name = "Tiền sân", Type = TransactionType.Income },
            new TransactionCategory { Name = "Quỹ tháng", Type = TransactionType.Income },
            new TransactionCategory { Name = "Nước", Type = TransactionType.Expense },
            new TransactionCategory { Name = "Phạt", Type = TransactionType.Expense },
            new TransactionCategory { Name = "Thưởng giải", Type = TransactionType.Expense }
        };
        await context.TransactionCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Seed Transactions
        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Date = DateTime.UtcNow.AddMonths(-1),
                Amount = 1000000,
                Description = "Thu quỹ tháng 12",
                CategoryId = categories.First(c => c.Name == "Quỹ tháng").Id,
                CreatedBy = adminMember.Id
            },
            new Transaction
            {
                Date = DateTime.UtcNow.AddDays(-15),
                Amount = 500000,
                Description = "Thu tiền sân",
                CategoryId = categories.First(c => c.Name == "Tiền sân").Id,
                CreatedBy = adminMember.Id
            },
            new Transaction
            {
                Date = DateTime.UtcNow.AddDays(-10),
                Amount = 200000,
                Description = "Mua nước uống",
                CategoryId = categories.First(c => c.Name == "Nước").Id,
                CreatedBy = adminMember.Id
            },
            new Transaction
            {
                Date = DateTime.UtcNow.AddDays(-5),
                Amount = 150000,
                Description = "Chi phí khác",
                CategoryId = categories.First(c => c.Name == "Phạt").Id,
                CreatedBy = adminMember.Id
            }
        };
        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();

        // Seed News
        var news = new List<News>
        {
            new News
            {
                Title = "Thông báo giải đấu tháng 1",
                Content = "CLB thông báo tổ chức giải đấu Mini-game vào cuối tháng 1. Mọi người tham gia đông đủ nhé!",
                IsPinned = true,
                CreatedDate = DateTime.UtcNow.AddDays(-3)
            },
            new News
            {
                Title = "Lịch nghỉ Tết Nguyên Đán",
                Content = "CLB nghỉ hoạt động từ 28 Tết đến mùng 5 Tết. Chúc mọi người năm mới vui vẻ!",
                IsPinned = true,
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            },
            new News
            {
                Title = "Vinh danh thành viên xuất sắc",
                Content = "Chúc mừng anh Lê Hoàng Cường đạt rank 4.8 - cao nhất CLB!",
                IsPinned = false,
                CreatedDate = DateTime.UtcNow.AddDays(-10)
            }
        };
        await context.News.AddRangeAsync(news);
        await context.SaveChangesAsync();

        // Seed Challenge (Mini-game Team Battle)
        var challenge = new Challenge
        {
            Title = "Mini-game Tết 2026 - Team Battle",
            Type = ChallengeType.MiniGame,
            GameMode = GameMode.TeamBattle,
            Status = ChallengeStatus.Ongoing,
            Config_TargetWins = 5,
            CurrentScore_TeamA = 2,
            CurrentScore_TeamB = 1,
            EntryFee = 50000,
            PrizePool = 500000,
            CreatedBy = adminMember.Id,
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(10)
        };
        await context.Challenges.AddAsync(challenge);
        await context.SaveChangesAsync();

        // Seed Participants
        var participants = new List<Participant>();
        for (int i = 0; i < members.Count && i < 10; i++)
        {
            participants.Add(new Participant
            {
                ChallengeId = challenge.Id,
                MemberId = members[i].Id,
                Team = i % 2 == 0 ? ParticipantTeam.TeamA : ParticipantTeam.TeamB,
                EntryFeePaid = true,
                EntryFeeAmount = 50000,
                JoinedDate = DateTime.UtcNow.AddDays(-4),
                Status = ParticipantStatus.Confirmed
            });
        }
        await context.Participants.AddRangeAsync(participants);
        await context.SaveChangesAsync();

        // Seed Matches
        var matches = new List<Match>
        {
            // Match 1: Singles
            new Match
            {
                Date = DateTime.UtcNow.AddDays(-3),
                IsRanked = true,
                ChallengeId = challenge.Id,
                MatchFormat = MatchFormat.Singles,
                Team1_Player1Id = members[0].Id,
                Team2_Player1Id = members[1].Id,
                WinningSide = WinningSide.Team1
            },
            // Match 2: Doubles
            new Match
            {
                Date = DateTime.UtcNow.AddDays(-2),
                IsRanked = true,
                ChallengeId = challenge.Id,
                MatchFormat = MatchFormat.Doubles,
                Team1_Player1Id = members[2].Id,
                Team1_Player2Id = members[4].Id,
                Team2_Player1Id = members[3].Id,
                Team2_Player2Id = members[5].Id,
                WinningSide = WinningSide.Team1
            },
            // Match 3: Singles (friendly match - no challenge)
            new Match
            {
                Date = DateTime.UtcNow.AddDays(-1),
                IsRanked = true,
                ChallengeId = null,
                MatchFormat = MatchFormat.Singles,
                Team1_Player1Id = members[6].Id,
                Team2_Player1Id = members[7].Id,
                WinningSide = WinningSide.Team2
            }
        };
        await context.Matches.AddRangeAsync(matches);
        await context.SaveChangesAsync();

        // Update member stats
        members[0].TotalMatches = 1;
        members[0].WinMatches = 1;
        members[1].TotalMatches = 1;
        members[1].WinMatches = 0;
        members[2].TotalMatches = 1;
        members[2].WinMatches = 1;
        members[3].TotalMatches = 1;
        members[3].WinMatches = 0;
        members[4].TotalMatches = 1;
        members[4].WinMatches = 1;
        members[5].TotalMatches = 1;
        members[5].WinMatches = 0;
        members[6].TotalMatches = 1;
        members[6].WinMatches = 0;
        members[7].TotalMatches = 1;
        members[7].WinMatches = 1;

        await context.SaveChangesAsync();
    }
}
