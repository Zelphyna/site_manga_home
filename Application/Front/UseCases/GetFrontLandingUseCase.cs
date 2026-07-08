using site_manga_home.Application.Front.Interfaces;
using site_manga_home.Domain.Front;

namespace site_manga_home.Application.Front.UseCases;

public sealed class GetFrontLandingUseCase(IFrontLandingReadRepository repository)
{
    public SiteInfo Execute()
    {
        return repository.GetLanding();
    }
}
