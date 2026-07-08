using site_manga_home.Domain.Back;

namespace site_manga_home.Application.Back.Interfaces;

public interface IBackDashboardReadRepository
{
    AdminDashboardInfo GetDashboard();
}
