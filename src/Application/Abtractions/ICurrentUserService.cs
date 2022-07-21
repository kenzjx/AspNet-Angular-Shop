namespace Application.Abtractions;

public interface ICurrentUserService
{
    Guid UserId { get; }

    bool IsAuthenticated { get; }
}