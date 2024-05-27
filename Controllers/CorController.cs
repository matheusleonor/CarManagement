using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using System.Collections.Generic;
using CarManagement.Helpers;

[SessionCheck]
public class CorController : Controller
{
    private readonly CorDAO corDAO;
    private readonly TipoUsuarioDAO tipoUsuarioDAO;

    public CorController(IConfiguration configuration)
    {
        corDAO = new CorDAO(configuration);
        tipoUsuarioDAO = new TipoUsuarioDAO(configuration);
    }

    private void SetViewBags()
    {
        ViewBag.TipoUsuarios = tipoUsuarioDAO.GetAll();
        ViewBag.TipoUsuarioId = HttpContext.Session.GetInt32("TipoUsuarioId");
        ViewBag.UsuarioEmail = HttpContext.Session.GetString("UsuarioEmail");
    }

    public ActionResult Index(string filterField, string filterValue)
    {
        try
        {
            SetViewBags();
            var cores = corDAO.GetAll().ToList();

            if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
            {
                cores = filterField switch
                {
                    "descricao" => cores.Where(c => c.Descricao.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "status" => cores.Where(c => bool.TryParse(filterValue, out bool status) && c.Status == status).ToList(),
                    _ => cores
                };
            }

            ViewBag.FilterField = filterField;
            ViewBag.FilterValue = filterValue;

            return View(cores);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }


    public ActionResult Create()
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
    public ActionResult Create(Cor cor)
    {
        try
        {
            SetViewBags();

            if (corDAO.NomeExists(cor.Descricao))
            {
                ModelState.AddModelError("Descricao", "Já existe uma cor com esta descrição.");
                return View(cor);
            }

            corDAO.Add(cor);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View("Create", cor);
        }
    }

    public ActionResult Edit(int id)
    {
        try
        {
            SetViewBags();
            var cor = corDAO.GetById(id);
            return View(cor);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Edit(Cor cor)
    {
        try
        {
            SetViewBags();

            if (corDAO.NomeExists(cor.Descricao, cor.Id))
            {
                ModelState.AddModelError("Descricao", "Já existe uma cor com esta descrição.");
                return View(cor);
            }

            corDAO.Update(cor);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View("Edit", cor);
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            SetViewBags();
            var cor = corDAO.GetById(id);
            return View(cor);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            SetViewBags();
            corDAO.Delete(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            var cor = corDAO.GetById(id);
            return View("Delete", cor);
        }
    }

    public ActionResult Details(int id)
    {
        try
        {
            SetViewBags();
            var cor = corDAO.GetById(id);
            return View(cor);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }
}
