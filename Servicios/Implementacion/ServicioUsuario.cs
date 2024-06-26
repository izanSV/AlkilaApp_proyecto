﻿using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace AlkilaApp
{
    /// <summary>
    /// Clase que proporciona métodos para el registro y validación de usuarios utilizando Firebase Authentication.
    /// </summary>
    public class ServicioUsuario
    {
        /// <summary>
        /// Identificador único del _usuario.
        /// </summary>
        private string _IdUsuario;

        /// <summary>
        /// Propiedad que obtiene o establece el identificador único del _usuario.
        /// </summary>
        public string IdUsuario
        {
            get => _IdUsuario;
            set
            {
                _IdUsuario = value;
            }
        }

        /// <summary>
        /// Registra un nuevo _usuario utilizando Firebase Authentication.
        /// </summary>
        /// <param name="usuario">Datos del _usuario a registrar.</param>
        /// <returns>Una cadena de respuesta indicando el resultado del registro.</returns>
        public async Task<string> RegistroUsuariosAsync(Usuario usuario)
        {
            string respuesta = "";

            try
            {
                // Llamar al servicio de autenticación para crear el _usuario
                FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(Setting.FirebaseApiKey));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(usuario.CorreoElectronico, usuario.Contrasenya);
                string token = auth.FirebaseToken;

                // Si se ha creado el _usuario correctamente
                if (token != null)
                {
                    // Obtener el ID único del _usuario recién registrado
                    this.IdUsuario = auth.User.LocalId;

                    respuesta = "OK";
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Manejar la excepción de autenticación de Firebase
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    // El correo electrónico ya está en uso
                    respuesta = "El correo electrónico ya está en uso ";
                }
                else
                {
                    // Otras posibles excepciones de autenticación de Firebase
                    respuesta = "El correo electrónico no tiene un formato válido";
                }
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones generales
                respuesta = "Error al registrar el _usuario" + ex.Message;
            }

            return respuesta; // Devolver la respuesta
        }
    


        /// <summary>
    /// Valida la autenticación del _usuario con el correo electrónico y la contraseña proporcionados.
    /// </summary>
    /// <param name="correo">Correo electrónico del _usuario.</param>
    /// <param name="contrasenya">Contraseña del _usuario.</param>
    /// <returns>Un mensaje de respuesta indicando el resultado de la autenticación.</returns>
        public async Task<string> ValidarUsuariosAsync(string correo, string contrasenya)
        {
            string respuesta = "";
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Setting.FirebaseApiKey));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(correo, contrasenya);
                var content = await auth.GetFreshAuthAsync();
                var serializedContent = JsonConvert.SerializeObject(content);
                Preferences.Set("FreshFirebaseToken", serializedContent);

                // Actualizar el campo IdUsuario del objeto Usuario con el Id del _usuario autenticado
                this.IdUsuario = auth.User.LocalId;
                respuesta = "Autenticación exitosa";
            }
            catch (FirebaseAuthException ex)
            {
                // Manejar la excepción de autenticación de Firebase
                respuesta = "Error de autenticación de Firebase: " + ex.Reason;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones más generales
                respuesta = "Error: " + ex.Message;
            }
            return respuesta;
        }




        /// <summary>
        /// Añade o actualiza un _usuario en la base de datos de Firebase.
        /// </summary>
        /// <param name="usuario">Usuario a añadir o actualizar.</param>
        /// <returns>Un mensaje de respuesta indicando el resultado de la operación.</returns>
        public async Task<string> AnyadirOActualizarUsuario(Usuario usuario)
        {
            try
            {
                if (this.IdUsuario != null)
                {
                    usuario.IdUsuario = this.IdUsuario;
                    // Crear un nodo en la base de datos de Firebase para almacenar la información del _usuario
                    var firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                    var usuarios = firebase.Child("Usuario");
                    await usuarios.Child(this.IdUsuario).PutAsync(usuario);
                    return "La operación se ha realizado correctamente.";
                }

                return "El _usuario no se ha podido registrar";
            }
            catch (FirebaseException ex)
            {
                // Manejar la excepción de Firebase
                return "Error de Firebase: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                return "Error: " + ex.Message;
            }
        }



        /// <summary>
        /// Obtiene el _usuario registrado con el ID de _usuario actual.
        /// </summary>
        /// <returns>El _usuario registrado con el ID de _usuario actual, o null si no se encuentra.</returns>
        public async Task<Usuario> ObtenerUsuarioRegistrado()
        {
            var firebase = new FirebaseClient(Setting.FireBaseDatabaseUrl);
            return await firebase.Child("Usuario").Child(IdUsuario).OnceSingleAsync<Usuario>();
        }




        /// <summary>
        /// Obtiene la lista de productos asociados a un _usuario con el ID especificado.
        /// </summary>
        /// <param name="id">ID del _usuario.</param>
        /// <returns>Una lista de productos del _usuario, o una lista vacía si no se encuentra ningún _producto o se produce un error.</returns>
        public async Task<List<Producto>> ObtenerListaProductos(string id)
        {
            var listaProductos = new List<Producto>();
            var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
            try
            {
                // Obtener una referencia al nodo del _usuario en la base de datos
                var usuarioNode = firebaseClient.Child("Usuario").Child(id);

                // Obtener la lista de productos del _usuario
                var productosSnapshot = await usuarioNode.Child("ListaProductos").OnceSingleAsync<List<Producto>>();

                // Agregar los productos directamente a la lista
                listaProductos.AddRange(productosSnapshot);
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra
                Console.WriteLine("Error al obtener productos: " + ex.Message);
            }

            return listaProductos;
        }



        /// <summary>
        /// Obtiene un _usuario que tiene un _producto con el ID especificado.
        /// </summary>
        /// <param name="idProducto">ID del _producto.</param>
        /// <returns>El _usuario que tiene el _producto con el ID especificado, o null si no se encuentra.</returns>
        public async Task<Usuario> ObtenerUsuarioPorIdProducto(string idProducto)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                // Obtiene una referencia a todos los usuarios en la base de datos
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                // Itera sobre todos los usuarios para encontrar el que tiene el _producto con el ID buscado
                foreach (var usuarioEntry in usuarios)
                {
                    var usuarioDic = usuarioEntry.Object; // Obtiene el diccionario del _usuario
                    var usuario = usuarioDic; // Obtiene el _usuario del diccionario

                    // Verifica si el _usuario tiene una lista de productos y si alguno de los productos tiene el ID buscado
                    if (usuario != null && usuario.ListaProductos != null && usuario.ListaProductos.Any(producto => producto.IdProducto == idProducto))
                    {
                        // Si se encuentra un _producto con el ID buscado, devuelve el _usuario
                        return usuario;
                    }
                }

                // Si no se encuentra ningún _usuario con el _producto buscado, devuelve null
                return null;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener el _usuario por ID de _producto: " + ex.Message);
                return null;
            }
        }



        /// <summary>
        /// Obtiene todos los IDs de los usuarios en la base de datos.
        /// </summary>
        /// <returns>Una lista de IDs de usuarios, o null si ocurre un error.</returns>
        public async Task<List<string>> ObtenerTodosLosIdsUsuarios()
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                var usuarios = await firebaseClient.Child("Usuario").OnceAsync<Usuario>();

                List<string> listaIdsUsuarios = new List<string>();

                foreach (var usuarioEntry in usuarios)
                {
                    listaIdsUsuarios.Add(usuarioEntry.Key);
                }

                return listaIdsUsuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener todos los IDs de los usuarios: " + ex.Message);
                return null;
            }
        }



        /// <summary>
        /// Obtiene un _usuario de la base de datos Firebase por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del _usuario a buscar.</param>
        /// <returns>El _usuario encontrado o null si no se encuentra.</returns>
        public async Task<Usuario> ObtenerUsuarioPorId(string idUsuario)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);
                // Obtener una referencia al nodo del _usuario por su ID en la base de datos
                var usuarioSnapshot = await firebaseClient.Child("Usuario").Child(idUsuario).OnceSingleAsync<Usuario>();

                // Si se encuentra un _usuario con el ID proporcionado, devuelve el _usuario
                return usuarioSnapshot;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir
                Console.WriteLine("Error al obtener el _usuario por ID: " + ex.Message);
                return null;
            }
        }



        /// <summary>
        /// Agrega un _producto al _usuario en la base de datos.
        /// </summary>
        /// <param name="producto">Producto a agregar.</param>
        /// <returns>Un mensaje indicando el resultado de la operación.</returns>
        public async Task<string> AgregarProductoAUsuario(Producto producto)
        {
            try
            {
                var firebaseClient = new FirebaseClient(Setting.FireBaseDatabaseUrl);

                // Obtener una referencia al nodo del _usuario en la base de datos
                var usuarioNode = firebaseClient.Child("Usuario").Child(IdUsuario);

                // Obtener la lista de productos del _usuario
                var listaProductos = await usuarioNode.Child("ListaProductos").OnceSingleAsync<List<Producto>>();

                // Si la lista de productos no existe, inicializarla como una nueva lista
                if (listaProductos == null)
                {
                    listaProductos = new List<Producto>();
                }

                // Agregar el nuevo _producto a la lista de productos del _usuario
                listaProductos.Add(producto);

                // Guardar la lista actualizada de productos en la base de datos
                await usuarioNode.Child("ListaProductos").PutAsync(listaProductos);

                return "Producto agregado exitosamente al usuario.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar _producto al usuario: " + ex.Message);
                return "Error al agregar producto al _usuario: " + ex.Message;
            }
        }
    }
}

