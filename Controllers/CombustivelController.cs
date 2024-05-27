using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using System.Collections.Generic;
using System.Linq;
using CarManagement.Helpers;

[SessionCheck]
public class CombustivelController : Controller
{
    private readonly CombustivelDAO combustivelDAO;
    private readonly TipoUsuarioDAO tipoUsuarioDAO;

    public CombustivelController(IConfiguration configuration)
    {
        combustivelDAO = new CombustivelDAO(configuration);
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
            var combustiveis = combustivelDAO.GetAll().ToList();

            if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
            {
                combustiveis = filterField switch
                {
                    "descricao" => combustiveis.Where(c => c.Descricao.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "status" => combustiveis.Where(c => bool.TryParse(filterValue, out bool status) && c.Status == status).ToList(),
                    _ => combustiveis
                };
            }

            ViewBag.FilterField = filterField;
            ViewBag.FilterValue = filterValue;

            return View(combustiveis);
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
    public ActionResult Create(Combustivel combustivel)
    {
        try
        {
            SetViewBags();

            if (combustivelDAO.NomeExists(combustivel.Descricao))
            {
                ModelState.AddModelError("Descricao", "Já existe um combustível com esta descrição.");
                return View(combustivel);
            }

            combustivelDAO.Add(combustivel);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View("Create", combustivel);
        }
    }

    public ActionResult Edit(int id)
    {
        try
        {
            SetViewBags();
            var combustivel = combustivelDAO.GetById(id);
            return View(combustivel);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Edit(Combustivel combustivel)
    {
        try
        {
            SetViewBags();

            if (combustivelDAO.NomeExists(combustivel.Descricao, combustivel.Id))
            {
                ModelState.AddModelError("Descricao", "Já existe um combustível com esta descrição.");
                return View(combustivel);
            }

            combustivelDAO.Update(combustivel);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View("Edit", combustivel);
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            SetViewBags();
            var combustivel = combustivelDAO.GetById(id);
            return View(combustivel);
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
            combustivelDAO.Delete(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            var combustivel = combustivelDAO.GetById(id);
            return View("Delete", combustivel);
        }
    }

    public ActionResult Details(int id)
    {
        try
        {
            SetViewBags();
            var combustivel = combustivelDAO.GetById(id);
            return View(combustivel);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }
}
