using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.Models;
using FitLife.Membership.Api.Services;
using FluentAssertions;

namespace FitLife.Membership.Tests.Unit;

public class MemberServiceTests
{
    [Fact]
    public void CreateMember_WhenValidData_ShouldCreateMember()
    {
        var service = new MemberService();
        var userId = Guid.NewGuid();
        var request = CreateValidRequest();

        var result = service.CreateMember(userId, request);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.FullName.Should().Be("Sarah Nielsen");
        result.Email.Should().Be("sarah@fitlife.dk");
        result.PhoneNumber.Should().Be("12345678");
        result.PrimaryCenter.Should().Be(PrimaryCenter.Vesterbro);
    }

    [Fact]
    public void CreateMember_WhenValidData_ShouldSetDefaultMembershipStatusToActive()
    {
        var service = new MemberService();
        var request = CreateValidRequest();

        var result = service.CreateMember(Guid.NewGuid(), request);

        result.MembershipStatus.Should().Be(MembershipStatus.Active);
        result.CancellationDate.Should().BeNull();
        result.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateMember_WhenUserIdIsEmpty_ShouldThrowArgumentException()
    {
        var service = new MemberService();
        var request = CreateValidRequest();

        Action act = () => service.CreateMember(Guid.Empty, request);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*UserId*");
    }

    [Fact]
    public void CreateMember_WhenFullNameIsMissing_ShouldThrowArgumentException()
    {
        var service = new MemberService();
        var request = CreateValidRequest();
        request.FullName = "";

        Action act = () => service.CreateMember(Guid.NewGuid(), request);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*FullName*");
    }

    [Fact]
    public void CreateMember_WhenEmailIsMissing_ShouldThrowArgumentException()
    {
        var service = new MemberService();
        var request = CreateValidRequest();
        request.Email = "";

        Action act = () => service.CreateMember(Guid.NewGuid(), request);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email*");
    }

    [Fact]
    public void CreateMember_WhenValidData_ShouldCreateUserPreference()
    {
        var service = new MemberService();
        var request = CreateValidRequest();

        var result = service.CreateMember(Guid.NewGuid(), request);

        result.UserPreference.Should().NotBeNull();
        result.UserPreference!.FitnessGoals.Should().Contain(FitnessGoal.Strength);
        result.UserPreference.FitnessGoals.Should().Contain(FitnessGoal.ProgressOverview);
        result.UserPreference.TrainingInterests.Should().Contain(TrainingInterest.Classes);
        result.UserPreference.MembershipType.Should().Be(MembershipType.Premium);
    }

    [Fact]
    public void UpdateMember_WhenValidData_ShouldUpdateBasicProfile()
    {
        var service = new MemberService();
        var member = service.CreateMember(Guid.NewGuid(), CreateValidRequest());

        var request = new UpdateMemberRequest
        {
            FullName = "Sarah Jensen",
            PhoneNumber = "87654321",
            PrimaryCenter = PrimaryCenter.Østerbro
        };

        var result = service.UpdateMember(member, request);

        result.FullName.Should().Be("Sarah Jensen");
        result.PhoneNumber.Should().Be("87654321");
        result.PrimaryCenter.Should().Be(PrimaryCenter.Østerbro);
    }

    [Fact]
    public void UpdateUserPreference_WhenValidData_ShouldUpdatePreferences()
    {
        var service = new MemberService();
        var member = service.CreateMember(Guid.NewGuid(), CreateValidRequest());

        var request = new UpdateUserPreferenceRequest
        {
            FitnessGoals = new List<FitnessGoal>
            {
                FitnessGoal.WeightLoss,
                FitnessGoal.FlexibleTraining
            },
            TrainingInterests = new List<TrainingInterest>
            {
                TrainingInterest.HomeTraining
            },
            MembershipType = MembershipType.Plus,
            ClassReminders = false,
            LivestreamReminders = true,
            BookingUpdates = true,
            CommunityActivity = true,
            MembershipAndPaymentMessages = true
        };

        var result = service.UpdateUserPreference(member, request);

        result.UserPreference!.FitnessGoals.Should().Contain(FitnessGoal.WeightLoss);
        result.UserPreference.TrainingInterests.Should().Contain(TrainingInterest.HomeTraining);
        result.UserPreference.MembershipType.Should().Be(MembershipType.Plus);
        result.UserPreference.ClassReminders.Should().BeFalse();
        result.UserPreference.CommunityActivity.Should().BeTrue();
    }

    [Fact]
    public void PauseMembership_WhenMemberIsActive_ShouldSetStatusToPaused()
    {
        var service = new MemberService();
        var member = service.CreateMember(Guid.NewGuid(), CreateValidRequest());

        var result = service.PauseMembership(member);

        result.MembershipStatus.Should().Be(MembershipStatus.Paused);
        result.CancellationDate.Should().BeNull();
    }

    [Fact]
    public void CancelMembership_WhenMemberExists_ShouldSetStatusToCancelledAndCancellationDate()
    {
        var service = new MemberService();
        var member = service.CreateMember(Guid.NewGuid(), CreateValidRequest());

        var result = service.CancelMembership(member);

        result.MembershipStatus.Should().Be(MembershipStatus.Cancelled);
        result.CancellationDate.Should().NotBeNull();
        result.CancellationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
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
}