using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        public IRepositorioTiposCuentas RepositorioTiposCuentas { get; }
        public IServicioUsuarios ServicioUsuarios { get; }

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            RepositorioTiposCuentas = repositorioTiposCuentas;
            ServicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = ServicioUsuarios.ObtenerUsuarioId();
            var tipoCuentas = await RepositorioTiposCuentas.Obtener(usuarioId);
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = tipoCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
            return View(modelo);
        }
    }
}
