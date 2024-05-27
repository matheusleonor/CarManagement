using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class SessionCheckAttribute : ActionFilterAttribute
{
    private readonly string[] _allowedActions = { "Login", "Register", "Logout" };
    private readonly string[] _allowedControllers = { "Usuario" };

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"].ToString();
        var action = context.RouteData.Values["action"].ToString();

        if (!_allowedControllers.Contains(controller) || !_allowedActions.Contains(action))
        {
            var session = context.HttpContext.Session;
            var usuarioEmail = session.GetString("UsuarioEmail");
            var tipoUsuarioId = session.GetInt32("TipoUsuarioId");

            if (string.IsNullOrEmpty(usuarioEmail))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            // Verificar permissões para cada tipo de usuário
            if (tipoUsuarioId == 2) // TipoUsuarioId == 2 é Cliente
            {
                if (!(controller == "Home" || controller == "Veiculo" && (action == "Index" || action == "Details")))
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }
        }

        base.OnActionExecuting(context);
    }
}
