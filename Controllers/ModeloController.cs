using Microsoft.AspNetCore.Mvc;
using CarManagement.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CarManagement.Helpers;

[SessionCheck]
public class ModeloController : Controller
{
    private readonly ModeloDAO modeloDAO;
    private readonly MarcaDAO marcaDAO;
    private readonly TipoUsuarioDAO tipoUsuarioDAO;

    public ModeloController(IConfiguration configuration)
    {
        modeloDAO = new ModeloDAO(configuration);
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
            var modelos = modeloDAO.GetAll().ToList();

            if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
            {
                modelos = filterField switch
                {
                    "nome" => modelos.Where(m => m.Nome.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "status" => modelos.Where(m => bool.TryParse(filterValue, out bool status) && m.Status == status).ToList(),
                    _ => modelos
                };
            }

            ViewBag.FilterField = filterField;
            ViewBag.FilterValue = filterValue;

            return View(modelos);
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
            ViewBag.Marcas = marcaDAO.GetAll();
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Create(Modelo modelo)
    {
        try
        {
            if (modeloDAO.NomeExists(modelo.Nome))
            {
                ModelState.AddModelError("Nome", "Já existe um modelo com este nome.");
                ViewBag.Marcas = marcaDAO.GetAll();
                return View(modelo);
            }

            modeloDAO.Add(modelo);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            ViewBag.Marcas = marcaDAO.GetAll();
            return View("Create", modelo);
        }
    }

    public ActionResult Edit(int id)
    {
        try
        {
            SetViewBags();
            var modelo = modeloDAO.GetById(id);
            ViewBag.Marcas = marcaDAO.GetAll();
            return View(modelo);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Edit(Modelo modelo)
    {
        try
        {
            SetViewBags();

            if (modeloDAO.NomeExists(modelo.Nome, modelo.Id))
            {
                ModelState.AddModelError("Nome", "Já existe um modelo com este nome.");
                ViewBag.Marcas = marcaDAO.GetAll();
                return View(modelo);
            }

            modeloDAO.Update(modelo);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            ViewBag.Marcas = marcaDAO.GetAll();
            return View("Edit", modelo);
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            SetViewBags();
            var modelo = modeloDAO.GetById(id);
            return View(modelo);
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
            modeloDAO.Delete(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            var modelo = modeloDAO.GetById(id);
            return View("Delete", modelo);
        }
    }

    public ActionResult Details(int id)
    {
        try
        {
            SetViewBags();
            var modelo = modeloDAO.GetById(id);
            return View(modelo);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }
}
