using site_manga_home.Domain.Front;

namespace site_manga_home.Application.Front.Interfaces;

public interface IFrontLandingReadRepository
{
    SiteInfo GetLanding();
}
