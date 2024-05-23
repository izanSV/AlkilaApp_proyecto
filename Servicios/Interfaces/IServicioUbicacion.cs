using AlkilaApp.Modelos;
using System.Threading.Tasks;

namespace AlkilaApp.Servicios
{
    /// <summary>
    /// Interfaz que define los métodos para el servicio de ubicación.
    /// </summary>
    public interface IServicioUbicacion
    {
        /// <summary>
        /// Inserta o actualiza la ubicación en la base de datos.
        /// </summary>
        /// <param name="ubicacion">La ubicación a insertar o actualizar.</param>
        /// <returns>True si la operación se realiza con éxito, false en caso contrario.</returns>
        Task<bool> InsertarOActualizarUbicacion(Ubicacion ubicacion);

        /// <summary>
        /// Obtiene la ubicación de un _usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">El ID del _usuario cuya ubicación se quiere obtener.</param>
        /// <returns>La ubicación del _usuario si se encuentra, o null si no se encuentra o hay un error.</returns>
        Task<Ubicacion> ObtenerLocalizacionAsync(string idUsuario);
    }
}
