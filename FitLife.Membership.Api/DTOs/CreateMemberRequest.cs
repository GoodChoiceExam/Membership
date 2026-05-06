using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.DTOs;

public class CreateMemberRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public PrimaryCenter PrimaryCenter { get; set; }

    public List<FitnessGoal> FitnessGoals { get; set; } = new();
    public List<TrainingInterest> TrainingInterests { get; set; } = new();

    public MembershipType MembershipType { get; set; }
}