using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarManagement.Models;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Text.Json;
using CarManagement.Helpers;

[SessionCheck]
public class VeiculoController : Controller
{
    private readonly VeiculoDAO veiculoDAO;
    private readonly MarcaDAO marcaDAO;
    private readonly ModeloDAO modeloDAO;
    private readonly CombustivelDAO combustivelDAO;
    private readonly CorDAO corDAO;
    private readonly TipoUsuarioDAO tipoUsuarioDAO;

    public VeiculoController(IConfiguration configuration)
    {
        veiculoDAO = new VeiculoDAO(configuration);
        marcaDAO = new MarcaDAO(configuration);
        modeloDAO = new ModeloDAO(configuration);
        combustivelDAO = new CombustivelDAO(configuration);
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
            var veiculos = veiculoDAO.GetAll().ToList();

            if (!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
            {
                veiculos = filterField switch
                {
                    "placa" => veiculos.Where(v => v.Placa.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "modelo" => veiculos.Where(v => v.Modelo.Nome.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "marca" => veiculos.Where(v => v.Marca.Nome.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "cor" => veiculos.Where(v => v.Cor.Descricao.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "combustivel" => veiculos.Where(v => v.Combustivel.Descricao.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "status" => veiculos.Where(v => bool.TryParse(filterValue, out bool status) && v.Status == status).ToList(),
                    _ => veiculos
                };
            }

            ViewBag.FilterField = filterField;
            ViewBag.FilterValue = filterValue;

            return View(veiculos);
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
            ViewBag.Modelos = modeloDAO.GetAll();
            ViewBag.Combustiveis = combustivelDAO.GetAll();
            ViewBag.Cores = corDAO.GetAll();
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Create(Veiculo veiculo, IEnumerable<IFormFile> Fotos)
    {
        try
        {
            SetViewBags();
            bool hasError = false;

            if (veiculoDAO.PlacaExists(veiculo.Placa))
            {
                ModelState.AddModelError("Placa", "Já existe um veículo com esta placa.");
                hasError = true;
            }
            if (veiculoDAO.RenavamExists(veiculo.Renavam))
            {
                ModelState.AddModelError("Renavam", "Já existe um veículo com este RENAVAM.");
                hasError = true;
            }
            if (veiculoDAO.NumeroChassiExists(veiculo.NumeroChassi))
            {
                ModelState.AddModelError("NumeroChassi", "Já existe um veículo com este número de chassi.");
                hasError = true;
            }
            if (veiculoDAO.NumeroMotorExists(veiculo.NumeroMotor))
            {
                ModelState.AddModelError("NumeroMotor", "Já existe um veículo com este número de motor.");
                hasError = true;
            }

            if (hasError)
            {
                ViewBag.Marcas = marcaDAO.GetAll();
                ViewBag.Modelos = modeloDAO.GetAll();
                ViewBag.Combustiveis = combustivelDAO.GetAll();
                ViewBag.Cores = corDAO.GetAll();
                return View(veiculo);
            }

            var fotosList = new List<string>();
            if (Fotos != null && Fotos.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var foto in Fotos)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + foto.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        foto.CopyTo(stream);
                    }
                    fotosList.Add("/uploads/" + uniqueFileName);
                }
            }

            veiculo.Fotos = fotosList;

            veiculoDAO.Add(veiculo);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            ViewBag.Marcas = marcaDAO.GetAll();
            ViewBag.Modelos = modeloDAO.GetAll();
            ViewBag.Combustiveis = combustivelDAO.GetAll();
            ViewBag.Cores = corDAO.GetAll();
            return View("Create", veiculo);
        }
    }



    public ActionResult Edit(int id)
    {
        try
        {
            SetViewBags();
            var veiculo = veiculoDAO.GetById(id);
            ViewBag.Marcas = marcaDAO.GetAll();
            ViewBag.Modelos = modeloDAO.GetAll();
            ViewBag.Combustiveis = combustivelDAO.GetAll();
            ViewBag.Cores = corDAO.GetAll();
            return View(veiculo);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }

    [HttpPost]
    public ActionResult Edit(int id, Veiculo veiculo, IEnumerable<IFormFile> Fotos, string RemovedPhotos)
    {
        try
        {
            SetViewBags();
            bool hasError = false;

            if (veiculoDAO.PlacaExists(veiculo.Placa, id))
            {
                ModelState.AddModelError("Placa", "Já existe um veículo com esta placa.");
                hasError = true;
            }
            if (veiculoDAO.RenavamExists(veiculo.Renavam, id))
            {
                ModelState.AddModelError("Renavam", "Já existe um veículo com este RENAVAM.");
                hasError = true;
            }
            if (veiculoDAO.NumeroChassiExists(veiculo.NumeroChassi, id))
            {
                ModelState.AddModelError("NumeroChassi", "Já existe um veículo com este número de chassi.");
                hasError = true;
            }
            if (veiculoDAO.NumeroMotorExists(veiculo.NumeroMotor, id))
            {
                ModelState.AddModelError("NumeroMotor", "Já existe um veículo com este número de motor.");
                hasError = true;
            }

            if (hasError)
            {
                ViewBag.Marcas = marcaDAO.GetAll();
                ViewBag.Modelos = modeloDAO.GetAll();
                ViewBag.Combustiveis = combustivelDAO.GetAll();
                ViewBag.Cores = corDAO.GetAll();
                return View(veiculo);
            }

            var fotosList = veiculoDAO.GetById(id).Fotos ?? new List<string>();

            if (!string.IsNullOrEmpty(RemovedPhotos))
            {
                var removedPhotosArray = JsonSerializer.Deserialize<List<string>>(RemovedPhotos);
                fotosList = fotosList.Except(removedPhotosArray).ToList();
            }

            if (Fotos != null && Fotos.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var foto in Fotos)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + foto.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        foto.CopyTo(stream);
                    }
                    fotosList.Add("/uploads/" + uniqueFileName);
                }
            }

            veiculo.Fotos = fotosList;

            veiculoDAO.Update(veiculo);
            return RedirectToAction("Index");
        }
        catch
        {
            return View();
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            SetViewBags();
            var veiculo = veiculoDAO.GetById(id);
            if (veiculo == null)
            {
                return NotFound();
            }
            return View(veiculo);
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
            veiculoDAO.Delete(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            var veiculo = veiculoDAO.GetById(id);
            return View("Delete", veiculo);
        }
    }

    public ActionResult Details(int id)
    {
        try
        {
            SetViewBags();
            var veiculo = veiculoDAO.GetById(id);
            if (veiculo == null)
            {
                return NotFound();
            }
            return View(veiculo);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ExceptionHelper.GetFriendlyErrorMessage(ex);
            return View();
        }
    }
}
