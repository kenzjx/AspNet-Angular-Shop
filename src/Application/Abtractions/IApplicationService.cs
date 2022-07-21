using Application.Models;

namespace Application.Abtractions;

public interface IApplicationService
{
    ApplicationDataViewModel GetApplicationData();
}