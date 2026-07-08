using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Front.UseCases;

namespace site_manga_home.Areas.Front.Pages;

public sealed class IndexModel(GetFrontLandingUseCase getFrontLandingUseCase) : PageModel
{
    public string SiteTitle { get; private set; } = string.Empty;

    public void OnGet()
    {
        var landing = getFrontLandingUseCase.Execute();
        SiteTitle = landing.Title;
    }
}
