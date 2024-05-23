using AlkilaApp.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlkilaApp.Servicios.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de gestión de productos.
    /// </summary>
    public interface IServicioProducto
    {
        /// <summary>
        /// Obtiene una lista de todos los productos.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de productos si tiene éxito, de lo contrario, devuelve null.</returns>
        Task<List<Producto>> ObtenerProductos();


        /// <summary>
        /// Obtiene un _producto por su ID.
        /// </summary>
        /// <param name="idProducto">El ID del _producto a obtener.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve el _producto si se encuentra, de lo contrario, devuelve null.</returns>
        Task<Producto> ObtenerProductoPorId(string idProducto);

        /// <summary>
        /// Elimina un _producto por su ID.
        /// </summary>
        /// <param name="idProducto">El ID del _producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve true si se elimina el _producto con éxito, de lo contrario, devuelve false.</returns>
        Task<bool> EliminarProductoPorId(string idProducto);

        /// <summary>
        /// Obtiene una lista de productos de usuarios que no están logeados actualmente.
        /// </summary>
        /// <param name="idUsuarioActual">El ID del _usuario actualmente logeado.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de productos si tiene éxito, de lo contrario, devuelve null.</returns>
        Task<List<Producto>> ObtenerProductosUsuariosNoLogeados(string idUsuarioActual);

        /// <summary>
        /// Obtiene una lista de productos de un determinado tipo.
        /// </summary>
        /// <param name="tipoProducto">El tipo de _producto.</param>
        /// <param name="idUsuarioActual">El ID del _usuario actualmente logeado.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve una lista de productos si tiene éxito, de lo contrario, devuelve null.</returns>
        Task<List<Producto>> ObtenerListaProductosPorTipo(TipoProducto tipoProducto, string idUsuarioActual);

        /// <summary>
        /// Actualiza un _producto asociado a un _usuario.
        /// </summary>
        /// <param name="producto">El _producto a actualizar.</param>
        /// <param name="idUsuario">El ID del _usuario asociado al _producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica. La tarea devuelve true si se actualiza el _producto con éxito, de lo contrario, devuelve false.</returns>
        Task<bool> ActualizarProductoAUsuario(Producto producto, string idUsuario);
    }
}
