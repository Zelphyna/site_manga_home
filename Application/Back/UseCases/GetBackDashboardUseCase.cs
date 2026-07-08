using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain.Back;

namespace site_manga_home.Application.Back.UseCases;

public sealed class GetBackDashboardUseCase(IBackDashboardReadRepository repository)
{
    public AdminDashboardInfo Execute()
    {
        return repository.GetDashboard();
    }
}
