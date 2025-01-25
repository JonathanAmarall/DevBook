using SharedKernel;

namespace Domain.Users;

public sealed record UserRegisteredDomainEvent(string UserId) : IDomainEvent;
