using AlkilaApp.Modelos;

namespace AlkilaApp.Interfaces
{
    /// <summary>
    /// Interfaz que define los métodos para el servicio de alquiler de productos.
    /// </summary>
    public interface IServicioAlquiler
    {
        /// <summary>
        /// Inserta o actualiza un registro de alquiler en la base de datos.
        /// </summary>
        /// <param name="alquiler">El objeto Alquiler a insertar o actualizar.</param>
        /// <returns>True si la operación se realiza con éxito, de lo contrario, false.</returns>
        Task<bool> InsertarOAlquilarAlquiler(Alquiler alquiler);

        /// <summary>
        /// Obtiene una lista de alquileres basados en el ID del usuario comprador o vendedor.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario comprador o vendedor.</param>
        /// <returns>Una lista de objetos Alquiler asociados al usuario especificado.</returns>
        Task<List<Alquiler>> GetAlquileresByUsuarioCompradorYVendedorId(string idUsuario);

        /// <summary>
        /// Obtiene un objeto Alquiler basado en el ID del producto.
        /// </summary>
        /// <param name="idProducto">El ID del producto.</param>
        /// <returns>Un objeto Alquiler asociado al producto especificado.</returns>
        Task<Alquiler> ObtenterAlquilerporProductoId(string idProducto);

        /// <summary>
        /// Elimina un registro de alquiler de la base de datos basado en su ID.
        /// </summary>
        /// <param name="idAlquiler">El ID del alquiler a eliminar.</param>
        /// <returns>True si la operación se realiza con éxito, de lo contrario, false.</returns>
        Task<bool> EliminarAlquilerPorId(string idAlquiler);
    }
}
