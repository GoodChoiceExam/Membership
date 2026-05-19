using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.Models;
using FitLife.Membership.Api.Services;

namespace FitLife.Membership.Tests.Unit;

[TestFixture]
public class MemberServiceTests
{
    [Test]
    public void CreateMember_WhenValidData_ShouldCreateMember()
    {
        var service = new MemberService();
        var userId = Guid.NewGuid();
        var request = CreateValidRequest();

        var result = service.CreateMember(userId, request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
        Assert.That(result.FullName, Is.EqualTo("Sarah Nielsen"));
        Assert.That(result.Email, Is.EqualTo("sarah@fitlife.dk"));
        Assert.That(result.PhoneNumber, Is.EqualTo("12345678"));
        Assert.That(result.PrimaryCenter, Is.EqualTo(PrimaryCenter.Vesterbro));
    }

    [Test]
    public void CreateMember_WhenValidData_ShouldSetDefaultMembershipStatusToActive()
    {
        var service = new MemberService();
        var request = CreateValidRequest();

        var result = service.CreateMember(Guid.NewGuid(), request);

        Assert.That(result.MembershipStatus, Is.EqualTo(MembershipStatus.Active));
        Assert.That(result.CancellationDate, Is.Null);
        Assert.That(result.StartDate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
    }

    [Test]
    public void CreateMember_WhenUserIdIsEmpty_ShouldThrowArgumentException()
    {
        var service = new MemberService();
        var request = CreateValidRequest();

        var exception = Assert.Throws<ArgumentException>(() => service.CreateMember(Guid.Empty, request));

        Assert.That(exception!.Message, Does.Contain("UserId"));
    }

    [Test]
    public void CreateMember_WhenFullNameIsMissing_ShouldThrowArgumentException()
    {
        var service = new MemberService();
        var request = CreateValidRequest();
        request.FullName = "";

        var exception = Assert.Throws<ArgumentException>(() => service.CreateMember(Guid.NewGuid(), request));

        Assert.That(exception!.Message, Does.Contain("FullName"));
    }

    [Test]
    public void CreateMember_WhenEmailIsMissing_ShouldThrowArgumentException()
    {
        var service = new MemberService();
        var request = CreateValidRequest();
        request.Email = "";

        var exception = Assert.Throws<ArgumentException>(() => service.CreateMember(Guid.NewGuid(), request));

        Assert.That(exception!.Message, Does.Contain("Email"));
    }

    [Test]
    public void CreateMember_WhenValidData_ShouldCreateUserPreference()
    {
        var service = new MemberService();
        var request = CreateValidRequest();

        var result = service.CreateMember(Guid.NewGuid(), request);

        Assert.That(result.UserPreference, Is.Not.Null);
        Assert.That(result.UserPreference!.FitnessGoals, Does.Contain(FitnessGoal.Strength));
        Assert.That(result.UserPreference.FitnessGoals, Does.Contain(FitnessGoal.ProgressOverview));
        Assert.That(result.UserPreference.TrainingInterests, Does.Contain(TrainingInterest.Classes));
        Assert.That(result.UserPreference.MembershipType, Is.EqualTo(MembershipType.Premium));
    }

    [Test]
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

        Assert.That(result.FullName, Is.EqualTo("Sarah Jensen"));
        Assert.That(result.PhoneNumber, Is.EqualTo("87654321"));
        Assert.That(result.PrimaryCenter, Is.EqualTo(PrimaryCenter.Østerbro));
    }

    [Test]
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

        Assert.That(result.UserPreference!.FitnessGoals, Does.Contain(FitnessGoal.WeightLoss));
        Assert.That(result.UserPreference.TrainingInterests, Does.Contain(TrainingInterest.HomeTraining));
        Assert.That(result.UserPreference.MembershipType, Is.EqualTo(MembershipType.Plus));
        Assert.That(result.UserPreference.ClassReminders, Is.False);
        Assert.That(result.UserPreference.CommunityActivity, Is.True);
    }

    [Test]
    public void PauseMembership_WhenMemberIsActive_ShouldSetStatusToPaused()
    {
        var service = new MemberService();
        var member = service.CreateMember(Guid.NewGuid(), CreateValidRequest());

        var result = service.PauseMembership(member);

        Assert.That(result.MembershipStatus, Is.EqualTo(MembershipStatus.Paused));
        Assert.That(result.CancellationDate, Is.Null);
    }

    [Test]
    public void CancelMembership_WhenMemberExists_ShouldSetStatusToCancelledAndCancellationDate()
    {
        var service = new MemberService();
        var member = service.CreateMember(Guid.NewGuid(), CreateValidRequest());

        var result = service.CancelMembership(member);

        Assert.That(result.MembershipStatus, Is.EqualTo(MembershipStatus.Cancelled));
        Assert.That(result.CancellationDate, Is.Not.Null);
        Assert.That(result.CancellationDate, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
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
