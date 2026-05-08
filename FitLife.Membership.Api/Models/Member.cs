using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitLife.Membership.Api.Models;

public class Member
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid UserId { get; set; }

    //Personlige oplysninger
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    //Medlemskabsoplysninger
    public PrimaryCenter PrimaryCenter { get; set; }
    public MembershipStatus MembershipStatus { get; set; } = MembershipStatus.Active;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? CancellationDate { get; set; }

    public UserPreference? UserPreference { get; set; }
}