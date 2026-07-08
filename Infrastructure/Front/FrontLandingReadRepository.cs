using site_manga_home.Application.Front.Interfaces;
using site_manga_home.Domain.Front;

namespace site_manga_home.Infrastructure.Front;

public sealed class FrontLandingReadRepository : IFrontLandingReadRepository
{
    public SiteInfo GetLanding()
    {
        return new SiteInfo("site_manga_home");
    }
}
