using AlkilaApp.Interfaces;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using Firebase.Database;
using Firebase.Database.Query;

namespace AlkilaApp
{

    /// <summary>
    /// Clase que se utilizará para gestionar los productos alquilados
    /// </summary>

    public class ServicioAlquiler : IServicioAlquiler
    {

        #region Atributos
        private FirebaseClient firebase;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase ServicioAlquiler.
        /// Inicializa una nueva instancia de FirebaseClient con la URL de la base de datos Firebase.
        /// </summary>
        public ServicioAlquiler()
        {
            firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
        }

        #endregion


        #region Metodos

        /// <summary>
        /// Inserta o actualiza un alquiler en la base de datos.
        /// </summary>
        /// <param name="alquiler">El objeto Alquiler a insertar o actualizar.</param>
        /// <returns>True si la operación se realizó con éxito; de lo contrario, false.</returns>
        public async Task<bool> InsertarOAlquilarAlquiler(Alquiler alquiler)
        {
            try
            {
                if (alquiler.IdAlquiler != null)
                {
                    await firebase.Child("Alquiler").Child(alquiler.IdAlquiler).PutAsync(alquiler);
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
        /// Obtiene una lista de alquileres por ID de usuario comprador o vendedor.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario comprador o vendedor.</param>
        /// <returns>Una lista de alquileres que coinciden con el ID de usuario proporcionado.</returns>
        public async Task<List<Alquiler>> GetAlquileresByUsuarioCompradorYVendedorId(string idUsuario)
        {
            try
            {
                // Obtener todos los alquileres
                var alquileres = await firebase.Child("Alquiler").OnceAsync<Alquiler>();

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
        /// Obtiene un alquiler por el ID de un producto asociado a él.
        /// </summary>
        /// <param name="idProducto">El ID del producto asociado al alquiler.</param>
        /// <returns>El alquiler correspondiente al ID de producto proporcionado.</returns>
        public async Task<Alquiler> ObtenterAlquilerporProductoId(string idProducto)
        {
            try
            {
                // Obtener todos los alquileres
                var alquileres = await firebase.Child("Alquiler").OnceAsync<Alquiler>();

                // Buscar el alquiler que tenga el producto con el id proporcionado
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
        /// Elimina un alquiler por su ID.
        /// </summary>
        /// <param name="idAlquiler">El ID del alquiler a eliminar.</param>
        /// <returns>True si la operación se realizó con éxito; de lo contrario, false.</returns>
        public async Task<bool> EliminarAlquilerPorId(string idAlquiler)
        {
            try
            {
                if (idAlquiler != null)
                {
                    // Eliminar el alquiler 
                    await firebase.Child("Alquiler").Child(idAlquiler).DeleteAsync();
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
