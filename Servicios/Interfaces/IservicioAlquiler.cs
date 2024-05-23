using AlkilaApp.Modelos;

namespace AlkilaApp.Interfaces
{
    /// <summary>
    /// Interfaz que define los métodos para el servicio de _alquiler de productos.
    /// </summary>
    public interface IServicioAlquiler
    {
        /// <summary>
        /// Inserta o actualiza un registro de _alquiler en la base de datos.
        /// </summary>
        /// <param name="alquiler">El objeto Alquiler a insertar o actualizar.</param>
        /// <returns>True si la operación se realiza con éxito, de lo contrario, false.</returns>
        Task<bool> InsertarOAlquilarAlquiler(Alquiler alquiler);

        /// <summary>
        /// Obtiene una lista de alquileres basados en el ID del _usuario comprador o vendedor.
        /// </summary>
        /// <param name="idUsuario">El ID del _usuario comprador o vendedor.</param>
        /// <returns>Una lista de objetos Alquiler asociados al _usuario especificado.</returns>
        Task<List<Alquiler>> GetAlquileresByUsuarioCompradorYVendedorId(string idUsuario);

        /// <summary>
        /// Obtiene un objeto Alquiler basado en el ID del _producto.
        /// </summary>
        /// <param name="idProducto">El ID del _producto.</param>
        /// <returns>Un objeto Alquiler asociado al _producto especificado.</returns>
        Task<Alquiler> ObtenterAlquilerporProductoId(string idProducto);

        /// <summary>
        /// Elimina un registro de _alquiler de la base de datos basado en su ID.
        /// </summary>
        /// <param name="idAlquiler">El ID del _alquiler a eliminar.</param>
        /// <returns>True si la operación se realiza con éxito, de lo contrario, false.</returns>
        Task<bool> EliminarAlquilerPorId(string idAlquiler);
    }
}
