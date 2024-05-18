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
        /// Registra a un nuevo usuario.
        /// </summary>
        /// <param name="usuario">El usuario a registrar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> RegistroUsuariosAsync(Usuario usuario);

        /// <summary>
        /// Valida las credenciales de inicio de sesión de un usuario.
        /// </summary>
        /// <param name="correo">El correo electrónico del usuario.</param>
        /// <param name="contrasenya">La contraseña del usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> ValidarUsuariosAsync(string correo, string contrasenya);

        /// <summary>
        /// Añade o actualiza la información de un usuario.
        /// </summary>
        /// <param name="usuario">El usuario a añadir o actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> AnyadirOActualizarUsuario(Usuario usuario);

        /// <summary>
        /// Obtiene la información del usuario registrado.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el usuario registrado.</returns>
        Task<Usuario> ObtenerUsuarioRegistrado();

        /// <summary>
        /// Obtiene la lista de productos asociada a un usuario.
        /// </summary>
        /// <param name="id">El ID del usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de productos asociada al usuario.</returns>
        Task<List<Producto>> ObtenerListaProductos(string id);

        /// <summary>
        /// Obtiene un usuario por el ID de un producto asociado a él.
        /// </summary>
        /// <param name="idProducto">El ID del producto asociado al usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el usuario asociado al producto.</returns>
        Task<Usuario> ObtenerUsuarioPorIdProducto(string idProducto);

        /// <summary>
        /// Obtiene todos los IDs de los usuarios registrados.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de IDs de usuarios.</returns>
        Task<List<string>> ObtenerTodosLosIdsUsuarios();

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el usuario correspondiente al ID proporcionado.</returns>
        Task<Usuario> ObtenerUsuarioPorId(string idUsuario);

        /// <summary>
        /// Agrega un producto a la lista de productos de un usuario.
        /// </summary>
        /// <param name="producto">El producto a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve un mensaje de respuesta.</returns>
        Task<string> AgregarProductoAUsuario(Producto producto);
    }
}
