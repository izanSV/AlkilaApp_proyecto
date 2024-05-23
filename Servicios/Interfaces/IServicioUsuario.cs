using AlkilaApp.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlkilaApp.Servicios.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de gestión de usuarios.
    /// </summary>
    public interface IServicioUsuario
    {
        /// <summary>
        /// Registra a un nuevo _usuario.
        /// </summary>
        /// <param name="usuario">El _usuario a registrar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> RegistroUsuariosAsync(Usuario usuario);

        /// <summary>
        /// Valida las credenciales de inicio de sesión de un _usuario.
        /// </summary>
        /// <param name="correo">El correo electrónico del _usuario.</param>
        /// <param name="contrasenya">La contraseña del _usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> ValidarUsuariosAsync(string correo, string contrasenya);

        /// <summary>
        /// Añade o actualiza la información de un _usuario.
        /// </summary>
        /// <param name="usuario">El _usuario a añadir o actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> AnyadirOActualizarUsuario(Usuario usuario);

        /// <summary>
        /// Obtiene la información del _usuario registrado.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el _usuario registrado.</returns>
        Task<Usuario> ObtenerUsuarioRegistrado();

        /// <summary>
        /// Obtiene la lista de productos asociada a un _usuario.
        /// </summary>
        /// <param name="id">El ID del _usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de productos asociada al _usuario.</returns>
        Task<List<Producto>> ObtenerListaProductos(string id);

        /// <summary>
        /// Obtiene un _usuario por el ID de un _producto asociado a él.
        /// </summary>
        /// <param name="idProducto">El ID del _producto asociado al _usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el _usuario asociado al _producto.</returns>
        Task<Usuario> ObtenerUsuarioPorIdProducto(string idProducto);

        /// <summary>
        /// Obtiene todos los IDs de los usuarios registrados.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de IDs de usuarios.</returns>
        Task<List<string>> ObtenerTodosLosIdsUsuarios();

        /// <summary>
        /// Obtiene un _usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">El ID del _usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el _usuario correspondiente al ID proporcionado.</returns>
        Task<Usuario> ObtenerUsuarioPorId(string idUsuario);

        /// <summary>
        /// Agrega un _producto a la lista de productos de un _usuario.
        /// </summary>
        /// <param name="producto">El _producto a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> AgregarProductoAUsuario(Producto producto);
    }
}
