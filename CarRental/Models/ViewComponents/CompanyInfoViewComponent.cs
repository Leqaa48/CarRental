using CarRental.Data;
using Microsoft.AspNetCore.Mvc;

public class CompanyInfoViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public CompanyInfoViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var companyInfo = _context.Infos.FirstOrDefault();
        return View(companyInfo);
    }
}
