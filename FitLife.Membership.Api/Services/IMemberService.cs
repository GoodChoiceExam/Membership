using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.Services;

public interface IMemberService
{
    Member CreateMember(Guid userId, CreateMemberRequest request);
    Member UpdateMember(Member member, UpdateMemberRequest request);
    Member UpdateUserPreference(Member member, UpdateUserPreferenceRequest request);
    Member PauseMembership(Member member);
    Member CancelMembership(Member member);
}