using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.API.Data;
using PCM.API.DTOs;
using PCM.API.Models;
using PCM.API.Services;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound();
        return Ok(member);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMemberDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var member = await _memberService.UpdateMemberAsync(id, dto, userId);
        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpGet("top-ranking")]
    public async Task<IActionResult> GetTopRanking([FromQuery] int limit = 5)
    {
        var topMembers = await _memberService.GetTopRankingAsync(limit);
        return Ok(topMembers);
    }
}

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isPinned = null)
    {
        var query = _context.News.AsQueryable();

        if (isPinned.HasValue)
            query = query.Where(n => n.IsPinned == isPinned.Value);

        var news = await query
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.CreatedDate)
            .Select(n => new NewsDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                IsPinned = n.IsPinned,
                CreatedDate = n.CreatedDate
            })
            .ToListAsync();

        return Ok(news);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null)
            return NotFound();

        return Ok(new NewsDto
        {
            Id = news.Id,
            Title = news.Title,
            Content = news.Content,
            IsPinned = news.IsPinned,
            CreatedDate = news.CreatedDate
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateNewsDto dto)
    {
        var news = new News
        {
            Title = dto.Title,
            Content = dto.Content,
            IsPinned = dto.IsPinned
        };

        _context.News.Add(news);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateNewsDto dto)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null)
            return NotFound();

        news.Title = dto.Title;
        news.Content = dto.Content;
        news.IsPinned = dto.IsPinned;
        news.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(news);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null)
            return NotFound();

        _context.News.Remove(news);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourtsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CourtsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var courts = await _context.Courts
            .Select(c => new CourtDto
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive,
                Description = c.Description
            })
            .ToListAsync();

        return Ok(courts);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourtDto dto)
    {
        var court = new Court
        {
            Name = dto.Name,
            IsActive = dto.IsActive,
            Description = dto.Description
        };

        _context.Courts.Add(court);
        await _context.SaveChangesAsync();

        return Ok(court);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCourtDto dto)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
            return NotFound();

        court.Name = dto.Name;
        court.IsActive = dto.IsActive;
        court.Description = dto.Description;

        await _context.SaveChangesAsync();
        return Ok(court);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
            return NotFound();

        _context.Courts.Remove(court);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var booking = await _bookingService.CreateBookingAsync(dto, userId);
        if (booking == null)
            return BadRequest(new { message = "Time slot is not available or invalid" });

        return Ok(booking);
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminId)) return Unauthorized();

        var result = await _bookingService.ApproveBookingAsync(id, adminId);
        if (!result) return NotFound();
        return Ok(new { message = "Approved" });
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminId)) return Unauthorized();

        var result = await _bookingService.RejectBookingAsync(id, adminId);
        if (!result) return NotFound();
        return Ok(new { message = "Rejected" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _bookingService.DeleteBookingAsync(id, userId);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("available-slots")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckAvailability([FromQuery] int courtId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        var isAvailable = await _bookingService.CheckAvailabilityAsync(courtId, startTime, endTime);
        return Ok(new { isAvailable });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChallengesController : ControllerBase
{
    private readonly IChallengeService _challengeService;

    public ChallengesController(IChallengeService challengeService)
    {
        _challengeService = challengeService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var challenges = await _challengeService.GetAllChallengesAsync();
        return Ok(challenges);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var challenge = await _challengeService.GetChallengeByIdAsync(id);
        if (challenge == null)
            return NotFound();

        return Ok(challenge);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChallengeDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var challenge = await _challengeService.CreateChallengeAsync(dto, userId);
        if (challenge == null)
            return BadRequest();

        return Ok(challenge);
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> Join(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _challengeService.JoinChallengeAsync(id, userId);
        if (!result)
            return BadRequest(new { message = "Cannot join this challenge" });

        return Ok(new { message = "Joined successfully" });
    }

    [HttpPost("{id}/auto-divide-teams")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<IActionResult> AutoDivideTeams(int id)
    {
        var result = await _challengeService.AutoDivideTeamsAsync(id);
        if (!result)
            return BadRequest(new { message = "Cannot divide teams" });

        return Ok(new { message = "Teams divided successfully" });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var matches = await _matchService.GetAllMatchesAsync();
        return Ok(matches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var match = await _matchService.GetMatchByIdAsync(id);
        if (match == null)
            return NotFound();

        return Ok(match);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
    {
        var match = await _matchService.CreateMatchAsync(dto);
        if (match == null)
            return BadRequest();

        return Ok(match);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var transaction = await _transactionService.CreateTransactionAsync(dto, userId);
        if (transaction == null)
            return BadRequest();

        return Ok(transaction);
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<IActionResult> Approve(int id)
    {
        var approverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(approverId)) return Unauthorized();

        var result = await _transactionService.ApproveTransactionAsync(id, approverId);
        if (!result) return NotFound();
        return Ok(new { message = "Approved" });
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<IActionResult> Reject(int id)
    {
        var approverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(approverId)) return Unauthorized();

        var result = await _transactionService.RejectTransactionAsync(id, approverId);
        if (!result) return NotFound();
        return Ok(new { message = "Rejected" });
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _transactionService.GetSummaryAsync();
        return Ok(summary);
    }
}

[ApiController]
[Route("api/transaction-categories")]
[Authorize]
public class TransactionCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransactionCategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.TransactionCategories
            .Select(c => new TransactionCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type.ToString()
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCategoryDto dto)
    {
        var category = new TransactionCategory
        {
            Name = dto.Name,
            Type = Enum.Parse<TransactionType>(dto.Type)
        };

        _context.TransactionCategories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTransactionCategoryDto dto)
    {
        var category = await _context.TransactionCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        category.Name = dto.Name;
        category.Type = Enum.Parse<TransactionType>(dto.Type);

        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.TransactionCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        _context.TransactionCategories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
