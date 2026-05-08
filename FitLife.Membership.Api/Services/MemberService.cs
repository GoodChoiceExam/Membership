using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.Services;

public class MemberService : IMemberService
{
    public Member CreateMember(Guid userId, CreateMemberRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required", nameof(userId));

        ValidateRequired(request.FullName, "FullName");
        ValidateRequired(request.Email, "Email");
        ValidateRequired(request.PhoneNumber, "PhoneNumber");

        var member = new Member
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            PrimaryCenter = request.PrimaryCenter,
            MembershipStatus = MembershipStatus.Active,
            StartDate = DateTime.UtcNow,
            CancellationDate = null
        };

        member.UserPreference = new UserPreference
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id,
            FitnessGoals = request.FitnessGoals,
            TrainingInterests = request.TrainingInterests,
            MembershipType = request.MembershipType,
            ClassReminders = true,
            LivestreamReminders = true,
            BookingUpdates = true,
            CommunityActivity = false,
            MembershipAndPaymentMessages = true
        };

        return member;
    }

    public Member UpdateMember(Member member, UpdateMemberRequest request)
    {
        ValidateRequired(request.FullName, "FullName");
        ValidateRequired(request.Email, "Email");
        ValidateRequired(request.PhoneNumber, "PhoneNumber");

        member.FullName = request.FullName.Trim();
        member.Email = request.Email.Trim();
        member.PhoneNumber = request.PhoneNumber.Trim();
        member.PrimaryCenter = request.PrimaryCenter;

        return member;
    }

    public Member UpdateUserPreference(Member member, UpdateUserPreferenceRequest request)
    {
        member.UserPreference ??= new UserPreference
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id
        };

        member.UserPreference.FitnessGoals = request.FitnessGoals;
        member.UserPreference.TrainingInterests = request.TrainingInterests;
        member.UserPreference.MembershipType = request.MembershipType;
        member.UserPreference.ClassReminders = request.ClassReminders;
        member.UserPreference.LivestreamReminders = request.LivestreamReminders;
        member.UserPreference.BookingUpdates = request.BookingUpdates;
        member.UserPreference.CommunityActivity = request.CommunityActivity;
        member.UserPreference.MembershipAndPaymentMessages = request.MembershipAndPaymentMessages;

        return member;
    }

    public Member PauseMembership(Member member)
    {
        if (member.MembershipStatus == MembershipStatus.Cancelled)
            throw new InvalidOperationException("Cancelled members cannot be paused");

        member.MembershipStatus = MembershipStatus.Paused;
        member.CancellationDate = null;

        return member;
    }

    public Member CancelMembership(Member member)
    {
        member.MembershipStatus = MembershipStatus.Cancelled;
        member.CancellationDate = DateTime.UtcNow;

        return member;
    }
    
    private static void ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required", fieldName);
    }
}