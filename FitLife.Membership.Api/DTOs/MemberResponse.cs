using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.DTOs;

public class MemberResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public PrimaryCenter PrimaryCenter { get; set; }
    public MembershipStatus MembershipStatus { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? CancellationDate { get; set; }

    public UserPreference? UserPreference { get; set; }
}