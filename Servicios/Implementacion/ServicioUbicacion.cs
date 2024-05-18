using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace AlkilaApp
{
    /// <summary>
    /// Clase que proporciona métodos para interactuar con la base de datos Firebase y gestionar la información de ubicación de los usuarios.
    /// </summary>
    public class ServicioUbicacion : IServicioUbicacion
    {

        #region Atributos

        private FirebaseClient firebase; // Cliente Firebase para interactuar con la base de datos

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor de la clase ServicioUbicacion.
        /// </summary>
        public ServicioUbicacion()
        {
            // Inicialización del cliente Firebase con la URL de la base de datos y el token de autenticación
            firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(Setting.FireBaseSeceret)
            });
        }

        #endregion


        #region Metodos

        /// <summary>
        /// Inserta o actualiza la ubicación de un usuario en la base de datos.
        /// </summary>
        /// <param name="ubicacion">Objeto Ubicacion que contiene la información de la ubicación del usuario.</param>
        /// <returns>True si la operación se realiza con éxito, false en caso contrario.</returns>
        public async Task<bool> InsertarOActualizarUbicacion(Ubicacion ubicacion)
        {
            try
            {
                // Verificar que el ID de usuario no sea nulo
                if (ubicacion.IdUsuario != null)
                {
                    // Insertar o actualizar la ubicación en la base de datos
                    await firebase.Child("Ubicacion").Child(ubicacion.IdUsuario).PutAsync(ubicacion);
                    return true; // Operación exitosa
                }
                return false; // ID de usuario nulo
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                Console.WriteLine("Error: " + ex.Message);
                return false; // Operación fallida
            }
        }

        /// <summary>
        /// Obtiene la ubicación de un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del usuario cuya ubicación se quiere obtener.</param>
        /// <returns>Objeto Ubicacion que contiene la información de la ubicación del usuario si se encuentra, o null si no se encuentra o hay un error.</returns>
        public async Task<Ubicacion> ObtenerLocalizacionAsync(string idUsuario)
        {
            try
            {
                // Obtener la ubicación del usuario desde la base de datos
                var location = await firebase.Child("Ubicacion").Child(idUsuario).OnceSingleAsync<Ubicacion>();
                return location; // Retornar la ubicación
            }
            catch (Exception ex)
            {
                // Manejar excepciones
                Console.WriteLine($"Error al obtener la ubicación: {ex.Message}");
                return null; // Retornar null en caso de error
            }
        }

        #endregion
    }
}
