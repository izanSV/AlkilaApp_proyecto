using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios.Interfaces;
using AlkilaApp.Servicios.Implementacion;




namespace AlkilaApp.Servicios
{
    public class ServicioProducto : IServicioProducto
    {
       
        #region Atributos

        /// <summary>
        /// Cliente Firebase para interactuar con la base de datos.
        /// </summary>
        private FirebaseClient _firebase;

        /// <summary>
        /// ID del _producto.
        /// </summary>
        private string _IdProducto;

        /// <summary>
        /// Propiedad para acceder y establecer el ID del _producto.
        /// </summary>
        public string IdUsuario
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value;
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor de la clase ServicioProducto.
        /// Inicializa una nueva instancia de FirebaseClient con la URL de la base de datos.
        /// </summary>
        public ServicioProducto()
        {
            _firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
        }

        #endregion


        /// <summary>
        /// Inicialización del cliente Firebase con opciones de autenticación
        /// </summary>
        FirebaseClient firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl, new FirebaseOptions
        {
            /// <summary>
            /// Configuración del token de autenticación asincrónica
            /// </summary>
            AuthTokenAsyncFactory = () => Task.FromResult(Setting.FireBaseSeceret)
        });

        /// <summary>
        /// Propiedad para almacenar el último mensaje de error
        /// </summary>
        public string MensajeError { get; private set; }


        /// <summary>
        /// Método para obtener todos los productos de la base de datos.
        /// </summary>
        /// <returns>Lista de productos obtenidos.</returns>
        public async Task<List<Producto>> ObtenerProductos()
        {
            try
            { 
                // Obtener todos los productos desde Firebase y mapearlos a una lista de Producto
                return (await firebase.Child(nameof(Producto)).OnceAsync<Producto>()).Select(f => new Producto
                {
                    // Aquí podrías asignar los valores del _producto si es necesario
                }).ToList();
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener los productos: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Método para obtener un _producto por su ID.
        /// </summary>
        /// <param name="idProducto">ID del _producto a obtener.</param>
        /// <returns>El _producto encontrado, o null si no se encuentra.</returns>
        public async Task<Producto> ObtenerProductoPorId(string idProducto)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtener una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Iterar sobre todos los usuarios para encontrar el que tiene el _producto con el ID buscado
                foreach (var usuarioEntry in usuarios)
                {
                    var usuarioDic = usuarioEntry.Object; // Obtiene el diccionario del _usuario
                    var usuario = usuarioDic; // Obtiene el _usuario del diccionario

                    // Verificar si el _usuario tiene una lista de productos y si alguno de los productos tiene el ID buscado
                    if (usuario != null && usuario.ListaProductos != null)
                    {
                        // Buscar el _producto por su ID en la lista de productos del _usuario
                        var producto = usuario.ListaProductos.FirstOrDefault(p => p.IdProducto == idProducto);

                        // Si se encuentra el _producto, devolverlo
                        if (producto != null)
                        {
                            return producto;
                        }
                    }
                }

                // Si no se encuentra ningún _producto con el ID buscado, devolver null
                return null;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener el _producto por ID: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Método para eliminar un _producto por su ID de la base de datos.
        /// </summary>
        /// <param name="idProducto">ID del _producto a eliminar.</param>
        /// <returns>True si se eliminó el _producto correctamente, False si ocurrió un error.</returns>
        public async Task<bool> EliminarProductoPorId(string idProducto)
        {
            try
            {
                Helpers helpers = new Helpers();

                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtiene una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Itera sobre todos los usuarios para encontrar el que tiene el _producto con el ID buscado
                foreach (var usuarioEntry in usuarios)
                {
                    var usuarioDic = usuarioEntry.Object; // Obtiene el diccionario del _usuario
                    var usuario = usuarioDic; // Obtiene el _usuario del diccionario

                    // Verifica si el _usuario tiene una lista de productos y si alguno de los productos tiene el ID buscado
                    if (usuario != null && usuario.ListaProductos != null)
                    {
                        // Busca el _producto por su ID en la lista de productos del _usuario
                        var producto = usuario.ListaProductos.FirstOrDefault(p => p.IdProducto == idProducto);

                        // Si se encuentra el _producto, lo elimina de la lista de productos del _usuario
                        if (producto != null)
                        {
                            usuario.ListaProductos.Remove(producto);

                            // Actualiza el _usuario en la base de datos Firebase
                            await firebaseClient.Child("Usuario").Child(usuarioEntry.Key).PutAsync(usuario);

                            // eliminamos la foto de fireStore
                            await helpers.DeleteFoto(producto.Foto);

                            // Devuelve true para indicar que se ha eliminado el _producto exitosamente
                            return true;
                        }
                    }
                }

                // Si no se encuentra ningún _producto con el ID buscado, devuelve false
                return false;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al eliminar el _producto por ID: " + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Método para obtener los productos de los usuarios que no han iniciado sesión.
        /// </summary>
        /// <param name="idUsuarioActual">ID del _usuario actualmente logeado.</param>
        /// <returns>Lista de productos de los usuarios no logeados.</returns>
        public async Task<List<Producto>> ObtenerProductosUsuariosNoLogeados(string idUsuarioActual)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                // Obtiene una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Crea una lista para almacenar los productos de los usuarios no logeados
                List<Producto> productosUsuarios = new List<Producto>();

                // Itera sobre todos los usuarios y agrega sus productos a la lista si no es el _usuario actual
                foreach (var usuario in usuarios)
                {
                    // Verifica si el _usuario es diferente del _usuario actual
                    if (usuario.Key != idUsuarioActual)
                    {
                        // Verifica si el _usuario tiene productos
                        if (usuario.Object.ListaProductos != null)
                        {
                            // Agrega los productos del _usuario a la lista
                            productosUsuarios.AddRange(usuario.Object.ListaProductos);
                        }
                    }
                }

                return productosUsuarios;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener los productos de los usuarios no logeados: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Método para obtener la lista de productos de un tipo específico de los usuarios no logeados.
        /// </summary>
        /// <param name="tipoProducto">Tipo de _producto a filtrar.</param>
        /// <param name="idUsuarioActual">ID del _usuario actualmente logeado.</param>
        /// <returns>Lista de productos del tipo especificado de los usuarios no logeados.</returns>
        public async Task<List<Producto>> ObtenerListaProductosPorTipo(TipoProducto tipoProducto, string idUsuarioActual)
        {
            try
            {
                // Obtener la lista de productos de los usuarios no logeados
                var listaProductos = await ObtenerProductosUsuariosNoLogeados(idUsuarioActual);

                // Filtrar la lista general de productos por TipoProducto
                listaProductos = listaProductos.Where(p => p.Tipo == tipoProducto).ToList();

                return listaProductos;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener los productos de los usuarios no logeados por tipo: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Método para actualizar un _producto asociado a un _usuario en la base de datos.
        /// </summary>
        /// <param name="producto">Producto a actualizar.</param>
        /// <param name="idUsuario">ID del _usuario al que está asociado el _producto.</param>
        /// <returns>True si se actualizó el _producto correctamente, False si ocurrió un error.</returns>
        public async Task<bool> ActualizarProductoAUsuario(Producto producto, string idUsuario)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtener una referencia al nodo del _usuario en la base de datos
                var usuarioNode = firebaseClient.Child("Usuario").Child(idUsuario);

                // Obtener la lista de productos del _usuario
                var listaProductos = await usuarioNode.Child("ListaProductos").OnceSingleAsync<List<Producto>>();

                // Si la lista de productos no existe, inicializarla como una nueva lista
                if (listaProductos == null)
                {
                    listaProductos = new List<Producto>();
                }

                // Buscar si ya existe un _producto con el mismo ID en la lista
                var productoExistente = listaProductos.FirstOrDefault(p => p.IdProducto == producto.IdProducto);

                if (productoExistente != null)
                {
                    // Actualizar los datos del _producto existente con los datos del nuevo _producto
                    productoExistente.Nombre = producto.Nombre;
                    productoExistente.DescripcionProducto = producto.DescripcionProducto;
                    productoExistente.Precio = producto.Precio;
                    productoExistente.Foto = producto.Foto;
                    productoExistente.Tipo = producto.Tipo;
                    productoExistente.EstaAlquilado = producto.EstaAlquilado;
                    productoExistente.Valoracion = producto.Valoracion;
                    productoExistente.TotalValoracionProductos = producto.TotalValoracionProductos;
                    productoExistente.TotalUsuariosValorados = producto.TotalUsuariosValorados;

                    // Guardar la lista actualizada de productos en la base de datos
                    await usuarioNode.Child("ListaProductos").PutAsync(listaProductos);

                    return true;
                }

                // Si no existe un _producto con el mismo ID en la lista, se considera como un error
                return false;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al agregar _producto al _usuario: " + ex.Message);
                return false;
            }
        }

    }

}
