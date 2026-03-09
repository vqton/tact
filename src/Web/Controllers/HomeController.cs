using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Trang chủ";
        return View();
    }
}
