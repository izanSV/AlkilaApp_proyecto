using AlkilaApp.Interfaces;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;

namespace AlkilaApp
{

    /// <summary>
    /// Clase que se utilizará para gestionar los productos alquilados
    /// </summary>

    public class ServicioAlquiler : IServicioAlquiler
    {

        #region Atributos
        private FirebaseClient _firebase;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase ServicioAlquiler.
        /// Inicializa una nueva instancia de FirebaseClient con la URL de la base de datos Firebase.
        /// </summary>
        public ServicioAlquiler()
        {
            _firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
        }

        #endregion


        #region Metodos

        /// <summary>
        /// Inserta o actualiza un _alquiler en la base de datos.
        /// </summary>
        /// <param name="alquiler">El objeto Alquiler a insertar o actualizar.</param>
        /// <returns>True si la operación se realizó con éxito; de lo contrario, false.</returns>
        public async Task<bool> InsertarOAlquilarAlquiler(Alquiler alquiler)
        {
            try
            {
                if (alquiler.IdAlquiler != null)
                {
                    await _firebase.Child("Alquiler").Child(alquiler.IdAlquiler).PutAsync(alquiler);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene una lista de alquileres por ID de _usuario comprador o vendedor.
        /// </summary>
        /// <param name="idUsuario">El ID del _usuario comprador o vendedor.</param>
        /// <returns>Una lista de alquileres que coinciden con el ID de _usuario proporcionado.</returns>
        public async Task<List<Alquiler>> GetAlquileresByUsuarioCompradorYVendedorId(string idUsuario)
        {
            try
            {
                // Obtener todos los alquileres
                var alquileres = await _firebase.Child("Alquiler").OnceAsync<Alquiler>();

                // Filtrar alquileres que coincidan con el idUsuarioComprador o el idUsuarioVendedor
                var alquileresCoincidentes = alquileres
                    .Select(alquiler => alquiler.Object)
                    .Where(alquiler => alquiler.IdUsuarioComprador == idUsuario || alquiler.IdUsuarioVendedor == idUsuario)
                    .ToList();

                // Si no se encontraron alquileres coincidentes
                if (alquileresCoincidentes.Count == 0)
                {
                    Console.WriteLine("No se encontraron alquileres con el IdUsuarioComprador especificado.");
                }

                return alquileresCoincidentes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene un _alquiler por el ID de un _producto asociado a él.
        /// </summary>
        /// <param name="idProducto">El ID del _producto asociado al _alquiler.</param>
        /// <returns>El _alquiler correspondiente al ID de _producto proporcionado.</returns>
        public async Task<Alquiler> ObtenterAlquilerporProductoId(string idProducto)
        {
            try
            {
                // Obtener todos los alquileres
                var alquileres = await _firebase.Child("Alquiler").OnceAsync<Alquiler>();

                // Buscar el _alquiler que tenga el _producto con el id proporcionado
                var alquiler = alquileres
                    .Select(alquiler => alquiler.Object)
                    .FirstOrDefault(alquiler => alquiler.IdProducto == idProducto);

                return alquiler;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Elimina un _alquiler por su ID.
        /// </summary>
        /// <param name="idAlquiler">El ID del _alquiler a eliminar.</param>
        /// <returns>True si la operación se realizó con éxito; de lo contrario, false.</returns>
        public async Task<bool> EliminarAlquilerPorId(string idAlquiler)
        {
            try
            {
                if (idAlquiler != null)
                {
                    // Eliminar el _alquiler 
                    await _firebase.Child("Alquiler").Child(idAlquiler).DeleteAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        #endregion


    }
}
