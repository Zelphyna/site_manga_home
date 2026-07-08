using Microsoft.AspNetCore.Mvc.RazorPages;
using site_manga_home.Application.Back.UseCases;

namespace site_manga_home.Areas.Back.Pages;

public sealed class IndexModel(GetBackDashboardUseCase getBackDashboardUseCase) : PageModel
{
    public string Heading { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public void OnGet()
    {
        var dashboard = getBackDashboardUseCase.Execute();
        Heading = dashboard.Heading;
        Description = dashboard.Description;
    }
}
