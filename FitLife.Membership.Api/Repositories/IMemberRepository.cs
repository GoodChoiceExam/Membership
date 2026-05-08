using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(Guid id);
    Task<Member?> GetByUserIdAsync(Guid userId);
    Task CreateAsync(Member member);
    Task UpdateAsync(Member member);
}