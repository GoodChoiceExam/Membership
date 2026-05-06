using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.DTOs;

public class UpdateMemberRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public PrimaryCenter PrimaryCenter { get; set; }
}