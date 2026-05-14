using FitLife.Membership.Api.IntegrationEvents;

namespace FitLife.Membership.Api.Messaging;

public interface IMemberEventPublisher
{
    Task PublishMemberCreatedAsync(MemberCreatedEvent memberCreatedEvent);
}