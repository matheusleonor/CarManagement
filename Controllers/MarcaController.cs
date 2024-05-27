using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using CarManagement.Helpers;

[SessionCheck]
public class MarcaController : Controller
{
    private readonly MarcaDAO marcaDAO;
    private readonly TipoUsuarioDAO tipoUsuarioDAO;

    public MarcaController(IConfiguration configuration)
    {
        marcaDAO = new MarcaDAO(configuration);
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
            var marcas = marcaDAO.GetAll().ToList();

            if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
            {
                marcas = filterField switch
                {
                    "nome" => marcas.Where(m => m.Nome.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "status" => marcas.Where(m => bool.TryParse(filterValue, out bool status) && m.Status == status).ToList(),
                    _ => marcas
                };
            }

            ViewBag.FilterField = filterField;
            ViewBag.FilterValue = filterValue;

            return View(marcas);
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
    public ActionResult Create(Marca marca)
    {
        try
        {
            SetViewBags();

            if (marcaDAO.NomeExists(marca.Nome))
            {
                ModelState.AddModelError("Nome", "Já existe uma marca com este nome.");
                return View(marca);
            }

            marcaDAO.Add(marca);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View("Create", marca);
        }
    }

    public ActionResult Edit(int id)
    {
        try
        {
            SetViewBags();
            var marca = marcaDAO.GetById(id);
            return View(marca);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Edit(Marca marca)
    {
        try
        {
            SetViewBags();

            if (marcaDAO.NomeExists(marca.Nome, marca.Id))
            {
                ModelState.AddModelError("Nome", "Já existe uma marca com este nome.");
                return View(marca);
            }

            marcaDAO.Update(marca);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View("Edit", marca);
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            SetViewBags();
            var marca = marcaDAO.GetById(id);
            return View(marca);
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
            marcaDAO.Delete(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            var marca = marcaDAO.GetById(id);
            return View("Delete", marca);
        }
    }

    public ActionResult Details(int id)
    {
        try
        {
            SetViewBags();
            var marca = marcaDAO.GetById(id);
            return View(marca);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }
}
