using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using RecipeBox.Controllers;

namespace RecipeBox.Controllers
{
    public class HomeController : Controller
    {
      [HttpGet("/")]
      public ActionResult Index()
      {
        return View();
      }

    }
}
