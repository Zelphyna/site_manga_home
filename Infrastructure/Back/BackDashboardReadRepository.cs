using site_manga_home.Application.Back.Interfaces;
using site_manga_home.Domain.Back;

namespace site_manga_home.Infrastructure.Back;

public sealed class BackDashboardReadRepository : IBackDashboardReadRepository
{
    public AdminDashboardInfo GetDashboard()
    {
        return new AdminDashboardInfo(
            "Back Office",
            "Cette zone est separee du front et reservee a l'administration.");
    }
}
