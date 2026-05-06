using FitLife.Membership.Api.Models;

namespace FitLife.Membership.Api.DTOs;

public class UpdateUserPreferenceRequest
{
    public List<FitnessGoal> FitnessGoals { get; set; } = new();
    public List<TrainingInterest> TrainingInterests { get; set; } = new();

    public MembershipType MembershipType { get; set; }

    public bool ClassReminders { get; set; }
    public bool LivestreamReminders { get; set; }
    public bool BookingUpdates { get; set; }
    public bool CommunityActivity { get; set; }
    public bool MembershipAndPaymentMessages { get; set; }
}