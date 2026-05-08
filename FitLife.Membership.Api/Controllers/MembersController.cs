using System.Security.Claims;
using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.Models;
using FitLife.Membership.Api.Repositories;
using FitLife.Membership.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitLife.Membership.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _repository;
    private readonly IMemberService _memberService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberRepository repository, IMemberService memberService, ILogger<MembersController> logger)
    {
        _repository = repository;
        _memberService = memberService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<MemberResponse>> GetMember()
    {
        var userId = GetUserIdFromClaims();
        if (userId is null)
            return Unauthorized();

        var member = await _repository.GetByUserIdAsync(userId.Value);
        if (member is null)
            return NotFound();

        return Ok(ToResponse(member));
    }

    [HttpPost]
    public async Task<ActionResult<MemberResponse>> CreateMember(CreateMemberRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null)
            return Unauthorized();

        var existingMember = await _repository.GetByUserIdAsync(userId.Value);
        if (existingMember is not null)
            return Conflict("Member already exists for this user");

        Member member;

        try
        {
            member = _memberService.CreateMember(userId.Value, request);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        await _repository.CreateAsync(member);

        _logger.LogInformation(
            "Member created with MemberId {MemberId} for UserId {UserId}",
            member.Id,
            member.UserId);

        return CreatedAtAction(nameof(GetById), new { id = member.Id }, ToResponse(member));
    }

    [HttpPut]
    public async Task<ActionResult<MemberResponse>> UpdateMember(UpdateMemberRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null)
            return Unauthorized();

        var member = await _repository.GetByUserIdAsync(userId.Value);
        if (member is null)
            return NotFound();

        try
        {
            _memberService.UpdateMember(member, request);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        await _repository.UpdateAsync(member);

        _logger.LogInformation("Member updated with MemberId {MemberId}", member.Id);

        return Ok(ToResponse(member));
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<MemberResponse>> UpdatePreferences(UpdateUserPreferenceRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId is null)
            return Unauthorized();

        var member = await _repository.GetByUserIdAsync(userId.Value);
        if (member is null)
            return NotFound();

        _memberService.UpdateUserPreference(member, request);

        await _repository.UpdateAsync(member);

        _logger.LogInformation("Preferences updated for MemberId {MemberId}", member.Id);

        return Ok(ToResponse(member));
    }

    [HttpPost("pause")]
    public async Task<ActionResult<MemberResponse>> Pause()
    {
        var userId = GetUserIdFromClaims();
        if (userId is null)
            return Unauthorized();

        var member = await _repository.GetByUserIdAsync(userId.Value);
        if (member is null)
            return NotFound();

        try
        {
            _memberService.PauseMembership(member);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        await _repository.UpdateAsync(member);

        _logger.LogInformation("Membership paused for MemberId {MemberId}", member.Id);

        return Ok(ToResponse(member));
    }

    [HttpPost("cancel")]
    public async Task<ActionResult<MemberResponse>> Cancel()
    {
        var userId = GetUserIdFromClaims();
        if (userId is null)
            return Unauthorized();

        var member = await _repository.GetByUserIdAsync(userId.Value);
        if (member is null)
            return NotFound();

        _memberService.CancelMembership(member);

        await _repository.UpdateAsync(member);

        _logger.LogInformation("Membership cancelled for MemberId {MemberId}", member.Id);

        return Ok(ToResponse(member));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberResponse>> GetById(Guid id)
    {
        var member = await _repository.GetByIdAsync(id);
        if (member is null)
            return NotFound();

        return Ok(ToResponse(member));
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<ActionResult<MemberResponse>> GetByUserId(Guid userId)
    {
        var member = await _repository.GetByUserIdAsync(userId);
        if (member is null)
            return NotFound();

        return Ok(ToResponse(member));
    }
    
    private Guid? GetUserIdFromClaims()
    {
        var value =
            User.FindFirstValue("sub") ??
            User.FindFirstValue("userId") ??
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    private static MemberResponse ToResponse(Member member)
    {
        return new MemberResponse
        {
            Id = member.Id,
            UserId = member.UserId,
            FullName = member.FullName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            PrimaryCenter = member.PrimaryCenter,
            MembershipStatus = member.MembershipStatus,
            StartDate = member.StartDate,
            CancellationDate = member.CancellationDate,
            UserPreference = member.UserPreference
        };
    }
}
    
    
    