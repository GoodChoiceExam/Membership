using FitLife.Membership.Api.DTOs;
using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.Services;

public class MemberService : IMemberService
{
    public Member CreateMember(Guid userId, CreateMemberRequest request)
    {
        throw new NotImplementedException();
    }

    public Member UpdateMember(Member member, UpdateMemberRequest request)
    {
        throw new NotImplementedException();
    }

    public Member UpdateUserPreference(Member member, UpdateUserPreferenceRequest request)
    {
        throw new NotImplementedException();
    }

    public Member PauseMembership(Member member)
    {
        throw new NotImplementedException();
    }

    public Member CancelMembership(Member member)
    {
        throw new NotImplementedException();
    }
}