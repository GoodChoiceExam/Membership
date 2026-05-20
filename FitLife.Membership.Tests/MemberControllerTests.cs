using System.Security.Claims;
using FitLife.Membership.Api.Controllers;
using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.IntegrationEvents;
using FitLife.Membership.Api.Messaging;
using FitLife.Membership.Api.Models;
using FitLife.Membership.Api.Repositories;
using FitLife.Membership.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace FitLife.Membership.Tests.Unit;

[TestFixture]
public class MemberControllerTests
{
    [Test]
    public async Task Create_WhenMemberDoesNotExist_ShouldCreateMemberForAuthenticatedUser()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMemberRepository();
        var controller = CreateController(repository, userId);

        var request = CreateValidRequest();

        var result = await controller.CreateMember(request);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());

        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.Value, Is.TypeOf<MemberResponse>());

        var response = (MemberResponse)createdResult.Value!;
        Assert.That(response.UserId, Is.EqualTo(userId));
        Assert.That(response.FullName, Is.EqualTo("Sarah Nielsen"));

        var savedMember = await repository.GetByUserIdAsync(userId);
        Assert.That(savedMember, Is.Not.Null);
    }

    [Test]
    public async Task Create_WhenMemberAlreadyExists_ShouldReturnConflict()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMemberRepository();

        await repository.CreateAsync(new Member
        {
            UserId = userId,
            FullName = "Sarah Nielsen",
            Email = "sarah@fitlife.dk",
            PhoneNumber = "12345678",
            PrimaryCenter = PrimaryCenter.Vesterbro
        });

        var controller = CreateController(repository, userId);

        var result = await controller.CreateMember(CreateValidRequest());

        Assert.That(result.Result, Is.TypeOf<ConflictObjectResult>());
    }

    [Test]
    public async Task GetMe_WhenMemberExists_ShouldReturnAuthenticatedUsersMember()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMemberRepository();

        var member = new Member
        {
            UserId = userId,
            FullName = "Sarah Nielsen",
            Email = "sarah@fitlife.dk",
            PhoneNumber = "12345678",
            PrimaryCenter = PrimaryCenter.Vesterbro,
            MembershipStatus = MembershipStatus.Active
        };

        await repository.CreateAsync(member);

        var controller = CreateController(repository, userId);

        var result = await controller.GetMember();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.Value, Is.TypeOf<MemberResponse>());

        var response = (MemberResponse)okResult.Value!;
        Assert.That(response.UserId, Is.EqualTo(userId));
        Assert.That(response.FullName, Is.EqualTo("Sarah Nielsen"));
    }

    [Test]
    public async Task Cancel_WhenMemberExists_ShouldSetMembershipStatusToCancelled()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMemberRepository();

        var member = new Member
        {
            UserId = userId,
            FullName = "Sarah Nielsen",
            Email = "sarah@fitlife.dk",
            PhoneNumber = "12345678",
            PrimaryCenter = PrimaryCenter.Vesterbro,
            MembershipStatus = MembershipStatus.Active
        };

        await repository.CreateAsync(member);

        var controller = CreateController(repository, userId);

        var result = await controller.Cancel();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());

        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.Value, Is.TypeOf<MemberResponse>());

        var response = (MemberResponse)okResult.Value!;
        Assert.That(response.MembershipStatus, Is.EqualTo(MembershipStatus.Cancelled));
        Assert.That(response.CancellationDate, Is.Not.Null);
    }

    private static MembersController CreateController(FakeMemberRepository repository, Guid userId)
    {
        var controller = new MembersController(
            repository,
            new MemberService(),
            NullLogger<MembersController>.Instance,
            new FakeMemberEventPublisher());

        var claims = new List<Claim>
        {
            new("sub", userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        return controller;
    }

    private static CreateMemberRequest CreateValidRequest()
    {
        return new CreateMemberRequest
        {
            FullName = "Sarah Nielsen",
            Email = "sarah@fitlife.dk",
            PhoneNumber = "12345678",
            PrimaryCenter = PrimaryCenter.Vesterbro,
            FitnessGoals = new List<FitnessGoal>
            {
                FitnessGoal.Strength,
                FitnessGoal.ProgressOverview
            },
            TrainingInterests = new List<TrainingInterest>
            {
                TrainingInterest.Classes,
                TrainingInterest.Swimming
            },
            MembershipType = MembershipType.Premium
        };
    }

    private class FakeMemberRepository : IMemberRepository
    {
        private readonly List<Member> _members = new();

        public Task<Member?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_members.FirstOrDefault(member => member.Id == id));
        }

        public Task<Member?> GetByUserIdAsync(Guid userId)
        {
            return Task.FromResult(_members.FirstOrDefault(member => member.UserId == userId));
        }

        public Task CreateAsync(Member member)
        {
            _members.Add(member);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Member member)
        {
            var index = _members.FindIndex(existing => existing.Id == member.Id);

            if (index >= 0)
                _members[index] = member;

            return Task.CompletedTask;
        }

        public Task<List<Member>> GetAllAsync()
        {
            return Task.FromResult(_members.ToList());
        }
    }

    private class FakeMemberEventPublisher : IMemberEventPublisher
    {
        public Task PublishMemberCreatedAsync(MemberCreatedEvent memberCreatedEvent)
        {
            return Task.CompletedTask;
        }
    }
}
