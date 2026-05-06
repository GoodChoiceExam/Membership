namespace FitLife.Membership.Api.Models;

public class UserPreference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MemberId { get; set; }

    //Interesser, mål og medlemsskab
    public List<FitnessGoal> FitnessGoals { get; set; } = new();
    public List<TrainingInterest> TrainingInterests { get; set; } = new();
    public MembershipType MembershipType { get; set; }

    //Notifikationer
    public bool ClassReminders { get; set; } = true;
    public bool LivestreamReminders { get; set; } = true;
    public bool BookingUpdates { get; set; } = true;
    public bool CommunityActivity { get; set; } = false;
    public bool MembershipAndPaymentMessages { get; set; } = true;
}