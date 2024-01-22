using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObternerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                insert into Cuentas (Nombre, TipoCuentaId, Descripcion, Balance)
                values (@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                select SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"
                select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                from Cuentas inner join TiposCuentas tc
                on tc.Id = Cuentas.TipoCuentaId
                where tc.UsuarioId = @UsuarioId
                order by tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObternerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"
                select Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, TipoCuentaId
                from Cuentas inner join TiposCuentas tc
                on tc.Id = Cuentas.TipoCuentaId
                where tc.UsuarioId = @UsuarioId and Cuentas.Id = @id
                order by tc.Orden", new { id, usuarioId });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                update Cuentas
                set Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                where Id = @Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete Cuentas where Id = @id", new { id });
        }
    }
}
