using FitLife.Membership.Api.Models;
using MongoDB.Driver;

namespace FitLife.Membership.Api.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IMongoCollection<Member> _members;

    public MemberRepository(IMongoDatabase database)
    {
        _members = database.GetCollection<Member>("members");
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        return await _members.Find(member => member.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Member?> GetByUserIdAsync(Guid userId)
    {
        return await _members.Find(member => member.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Member member)
    {
        await _members.InsertOneAsync(member);
    }

    public async Task UpdateAsync(Member member)
    {
        await _members.ReplaceOneAsync(
            existing => existing.Id == member.Id,
            member);
    }
}
