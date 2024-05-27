using CarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CarManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TipoUsuarioDAO tipoUsuarioDAO;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            tipoUsuarioDAO = new TipoUsuarioDAO(configuration);
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.TipoUsuarios = tipoUsuarioDAO.GetAll();
            ViewBag.TipoUsuarioId = HttpContext.Session.GetInt32("TipoUsuarioId");
            ViewBag.UsuarioEmail = HttpContext.Session.GetString("UsuarioEmail");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
