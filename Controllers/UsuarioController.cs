using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarManagement.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using CarManagement.Helpers;

public class UsuarioController : Controller
{
    private readonly UsuarioDAO usuarioDAO;
    private readonly TipoUsuarioDAO tipoUsuarioDAO;

    public UsuarioController(IConfiguration configuration)
    {
        usuarioDAO = new UsuarioDAO(configuration);
        tipoUsuarioDAO = new TipoUsuarioDAO(configuration);
    }

    private void SetViewBags()
    {
        ViewBag.TipoUsuarios = tipoUsuarioDAO.GetAll();
        ViewBag.TipoUsuarioId = HttpContext.Session.GetInt32("TipoUsuarioId");
        ViewBag.UsuarioEmail = HttpContext.Session.GetString("UsuarioEmail");
    }

    public IActionResult Index()
    {
        try
        {
            SetViewBags();
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public IActionResult Login(string email, string senha)
    {
        try
        {
            string hashedSenha = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(senha)));
            var usuario = usuarioDAO.GetUsuarioByEmailAndSenha(email, hashedSenha);
            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
                HttpContext.Session.SetInt32("TipoUsuarioId", usuario.TipoUsuarioId);

                // Redirecionar com base no tipo de usuário
                if (usuario.TipoUsuarioId == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Veiculo");
                }
            }
            TempData["LoginError"] = "Email ou senha inválidos.";
            TempData["ShowLoginModal"] = true;
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["LoginError"] = ex.Message;
            TempData["ShowLoginModal"] = true;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    public IActionResult Register(Usuario usuario, string ConfirmarSenha)
    {
        try
        {
            SetViewBags();

            if (usuarioDAO.EmailExists(usuario.Email))
            {
                TempData["RegisterError"] = "Este email já está cadastrado.";
                TempData["ShowRegisterModal"] = true;
                return RedirectToAction("Index", "Home");
            }

            if (usuario.Senha != ConfirmarSenha)
            {
                TempData["RegisterError"] = "A senha e a confirmação de senha não coincidem.";
                TempData["ShowRegisterModal"] = true;
                return RedirectToAction("Index", "Home");
            }

            usuario.Senha = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(usuario.Senha))); // Criptografar a senha
            usuarioDAO.AddUsuario(usuario);

            // Autenticar automaticamente após registro
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetInt32("TipoUsuarioId", usuario.TipoUsuarioId);

            // Redirecionar com base no tipo de usuário
            if (usuario.TipoUsuarioId == 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Veiculo");
            }
        }
        catch (Exception ex)
        {
            TempData["RegisterError"] = ex.Message;
            TempData["ShowRegisterModal"] = true;
            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult Logout()
    {
        try
        {
            HttpContext.Session.Remove("UsuarioEmail");
            HttpContext.Session.Remove("TipoUsuarioId");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return RedirectToAction("Index", "Home");
        }
    }
}
