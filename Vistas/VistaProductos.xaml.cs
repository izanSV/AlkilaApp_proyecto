using AlkilaApp.Animaciones;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using AlkilaApp.Vistas;

namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VistaProductos : ContentPage
    {
        #region Atributos

        /// <summary>
        /// Servicio de _usuario
        /// </summary>
        private ServicioUsuario _servicioUsuario;

        /// <summary>
        /// Servicio de _alquiler
        /// </summary>
        private ServicioAlquiler _servicioAlquiler;

        /// <summary>
        /// Servicio de _producto
        /// </summary>
        private ServicioProducto _servicioProducto = new ServicioProducto();

        /// <summary>
        /// Producto
        /// </summary>
        private Producto _producto = new Producto();

        /// <summary>
        /// Atributo para cambiar el color del frame seleccionado
        /// </summary>
        private Frame _frameSeleccionado;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase VistaProductos
        /// </summary>
        /// <param name="id">ID del _usuario</param>
        public VistaProductos(string id)
        {
            InitializeComponent();
            _servicioUsuario = new ServicioUsuario();
            _servicioAlquiler = new ServicioAlquiler();
            _servicioUsuario.IdUsuario = id;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Cargar la lista de productos
        /// </summary>
        private async void CargarListaProductos()
        {
            try
            {
                var listaProductos = await _servicioProducto.ObtenerProductosUsuariosNoLogeados(_servicioUsuario.IdUsuario);
                ProductosCollectionView.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                await MostrarError(ex.Message);
            }
        }

        /// <summary>
        /// Cargar los tipos de _producto
        /// </summary>
        private void CargarTiposProducto()
        {
            var tiposProducto = Enum.GetNames(typeof(TipoProducto)).ToList();
            TiposProductoCollectionView.ItemsSource = tiposProducto;
        }

        /// <summary>
        /// Verificar si el _producto pertenece al _usuario registrado
        /// </summary>
        /// <param name="producto">Producto</param>
        /// <returns>Booleano indicando si el _producto pertenece al _usuario registrado</returns>
        private bool ProductoPerteneceAlUsuarioRegistrado(Producto producto)
        {
            var listaProductosUsuarioRegistrado = (List<Producto>)ProductosCollectionView.ItemsSource;
            return listaProductosUsuarioRegistrado.Contains(producto);
        }

        /// <summary>
        /// Comprobar el estado de los alquileres
        /// </summary>
        private async void ComprobarEstado()
        {
            await EliminarAlquilerFinalizado();
            await UltimoDiaProductoAlquilado();
        }

        /// <summary>
        /// Verificar el último día de _alquiler de un _producto
        /// </summary>
        /// <returns>Booleano indicando si se cumplió la primera condición</returns>
        private async Task<bool> UltimoDiaProductoAlquilado()
        {
            try
            {
                List<Alquiler> listaProductosAlquilados = await _servicioAlquiler.GetAlquileresByUsuarioCompradorYVendedorId(_servicioUsuario.IdUsuario);
                bool primeraCondicionCumplida = false;

                foreach (Alquiler item in listaProductosAlquilados)
                {
                    primeraCondicionCumplida = await ProcesarAlquiler(item, primeraCondicionCumplida);
                }

                return primeraCondicionCumplida;
            }
            catch (Exception ex)
            {
                await MostrarError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Procesar el _alquiler de un _producto
        /// </summary>
        /// <param name="alquiler">Objeto Alquiler</param>
        /// <param name="primeraCondicionCumplida">Estado de la primera condición</param>
        /// <returns>Booleano indicando si se cumplió la primera condición</returns>
        private async Task<bool> ProcesarAlquiler(Alquiler alquiler, bool primeraCondicionCumplida)
        {
            if (alquiler.EstadoAlquiler == Estado.Pendiente && alquiler.IdUsuarioVendedor.Equals(_servicioUsuario.IdUsuario) && !primeraCondicionCumplida)
            {

                if (alquiler.NoMasRespuesta)
                {
                   
                  bool respuesta =  await DisplayAlert("Información", $"Comprueba el buzón de mensajes, tienes una propuesta para alquilar el siguiente producto:\n\n Nombre producto:{alquiler.NombreProductoAlquilado}", "ACEPTAR","NO VOLVER A PREGUNTAR");
                   
                    if (!respuesta) 
                    { 
                        alquiler.NoMasRespuesta = false;
                        await _servicioAlquiler.InsertarOAlquilarAlquiler(alquiler);
                    }

                }
              
                return true;

            }

            if (alquiler.EstadoAlquiler == Estado.Aceptado && alquiler.FechaFin <= DateTime.Now)
            {
                await ActualizarEstadoAlquiler(alquiler, Estado.Enviado);
            }

            Producto producto = await ObtenerProducto(alquiler.IdProducto);

            if (alquiler.EstadoAlquiler == Estado.Enviado && alquiler.IdUsuarioComprador.Equals(_servicioUsuario.IdUsuario))
            {
                await ProcesarAlquilerEnviado(alquiler, producto);
            }

            if (alquiler.EstadoAlquiler == Estado.Recibido && alquiler.IdUsuarioVendedor.Equals(_servicioUsuario.IdUsuario))
            {
                await ProcesarAlquilerRecibido(alquiler, producto);
                
            }

            return primeraCondicionCumplida;
        }


        /// <summary>
        /// Eliminar alquileres finalizados
        /// </summary>
        private async Task EliminarAlquilerFinalizado()
        {
            List<Alquiler> listaProductosAlquilados = await _servicioAlquiler.GetAlquileresByUsuarioCompradorYVendedorId(_servicioUsuario.IdUsuario);
            var alquileresFinalizados = listaProductosAlquilados.Where(alquiler => alquiler.EstadoAlquiler == Estado.Finalizado).ToList();

            foreach (var alquiler in alquileresFinalizados)
            {
                await _servicioAlquiler.EliminarAlquilerPorId(alquiler.IdAlquiler);
            }
        }

        /// <summary>
        /// Actualizar el estado de un _alquiler
        /// </summary>
        /// <param name="item">Objeto Alquiler</param>
        /// <param name="nuevoEstado">Nuevo estado del _alquiler</param>
        private async Task ActualizarEstadoAlquiler(Alquiler item, Estado nuevoEstado)
        {
            item.EstadoAlquiler = nuevoEstado;
            await _servicioAlquiler.InsertarOAlquilarAlquiler(item);
        }

        /// <summary>
        /// Procesar _alquiler enviado
        /// </summary>
        /// <param name="alquiler">Objeto Alquiler</param>
        /// <param name="producto">Objeto Producto</param>
        private async Task ProcesarAlquilerEnviado(Alquiler alquiler, Producto producto)
        {
            if (alquiler.EstadoAlquiler == Estado.Enviado)
            {
                await _servicioAlquiler.InsertarOAlquilarAlquiler(alquiler);

                // si el _usuario presiona sobre no preguntar mas, entonces se deshabilitara la entrada y no se mostrará mas
                if (!alquiler.NoMasRespuesta)
                {
                    // inicializamos a true para poder ver el alert
                    alquiler.NoMasRespuesta = true;
                    bool respuesta = await DisplayAlert("Información", $"¿Hola, recuerda que hoy tienes que devolver sus pertenencias al usuario \n Producto: {alquiler.NombreProductoAlquilado} ,\n Nombre de usuario: {alquiler.NombreUsuarioComprador}", "ACEPTAR","NO VOLVER A PREGUNTAR");
                    if (!respuesta)
                    {
                        alquiler.NoMasRespuesta = false;
                        await _servicioAlquiler.InsertarOAlquilarAlquiler(alquiler);
                    }
                }
                
                bool valoracion = await MostrarAlertaConConfirmacion("Para nosotros es muy importante la valoración de nuestros clientes, te gustaría dar una calificación al producto que has alquilado?", "ACEPTAR ☑", "CANCELAR ☒");

                if (valoracion)
                {
                    ValorarProducto(producto, alquiler.IdUsuarioVendedor);
                }

                await ActualizarEstadoAlquiler(alquiler, Estado.Recibido);
            }
        }

        /// <summary>
        /// Procesar _alquiler recibido
        /// </summary>
        /// <param name="item">Objeto Alquiler</param>
        /// <param name="producto">Objeto Producto</param>
        private async Task ProcesarAlquilerRecibido(Alquiler item, Producto producto)
        {
            bool aceptarProducto = await MostrarAlertaConConfirmacion($"¿Has recibido tu producto?\nNombre del producto alquilado:{producto.Nombre}\nPrecio por dia:{producto.Precio}\nCategoría del producto:{producto.Tipo}", "SI", "NO");

            if (aceptarProducto)
            {
                item.EstadoAlquiler = Estado.Finalizado;
                await _servicioAlquiler.InsertarOAlquilarAlquiler(item);

                if (producto != null)
                {
                    producto.EstaAlquilado = false;
                    await _servicioProducto.ActualizarProductoAUsuario(producto, _servicioUsuario.IdUsuario);
                }
            }
        }

        /// <summary>
        /// Obtener un _producto por ID
        /// </summary>
        /// <param name="productoId">ID del _producto</param>
        /// <returns>Objeto Producto</returns>
        private async Task<Producto> ObtenerProducto(string productoId)
        {
            return await _servicioProducto.ObtenerProductoPorId(productoId);
        }


        /// <summary>
        /// Mostrar alerta con mensaje y confirmación
        /// </summary>
        /// <param name="mensaje">Mensaje de la alerta</param>
        /// <param name="aceptar">Texto del botón de aceptar</param>
        /// <param name="cancelar">Texto del botón de cancelar</param>
        /// <returns>Booleano indicando la elección del _usuario</returns>
        private async Task<bool> MostrarAlertaConConfirmacion(string mensaje, string aceptar, string cancelar)
        {
            return await DisplayAlert("", mensaje, aceptar, cancelar);
        }

        /// <summary>
        /// Valorar un _producto
        /// </summary>
        /// <param name="producto">Objeto Producto</param>
        /// <param name="id">ID del _usuario</param>
        private async void ValorarProducto(Producto producto, string id)
        {
            string action = await DisplayActionSheet("¿Cual es la valoración para este producto?", "CANCELAR", "", "1", "2", "3", "4", "5");

            double valoracion = 0;

            switch (action)
            {
                case "1":
                    valoracion = 1;
                    break;
                case "2":
                    valoracion = 2;
                    break;
                case "3":
                    valoracion = 3;
                    break;
                case "4":
                    valoracion = 4;
                    break;
                case "5":
                    valoracion = 5;
                    break;
                case "Cancelar":
                    return;
            }

            producto.TotalUsuariosValorados++;
            producto.TotalValoracionProductos += valoracion;

            if (producto.TotalUsuariosValorados > 0)
            {
                producto.Valoracion = producto.TotalValoracionProductos / producto.TotalUsuariosValorados;
            }
            else
            {
                producto.Valoracion = 0;
            }

            await _servicioProducto.ActualizarProductoAUsuario(producto, id);
            CargarListaProductos();
        }

        /// <summary>
        /// Mostrar mensaje de error
        /// </summary>
        /// <param name="mensaje">Mensaje de error</param>
        private async Task MostrarError(string mensaje)
        {
            await DisplayAlert("Error", $"Se produjo un error: {mensaje}", "ACEPTAR");
        }

        #endregion

        #region Botones

        /// <summary>
        /// Manejar el evento de tap en una tarjeta de _producto
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void OnCardTapped(object sender, EventArgs e)
        {
            var frame = (Frame)sender;
            frame.IsEnabled = false;
            var producto = frame.BindingContext as Producto;

            if (producto != null && ProductoPerteneceAlUsuarioRegistrado(producto))
            {
                var detallesProductoPage = new DetallesProducto(_servicioUsuario.IdUsuario, producto, esProductoDelUsuario: true);
                await Navigation.PushAsync(detallesProductoPage);
                frame.IsEnabled = true;
            }
            else
            {
                var detallesProductoPage = new DetallesProducto(_servicioUsuario.IdUsuario, producto, esProductoDelUsuario: false);
                await Navigation.PushAsync(detallesProductoPage);
                frame.IsEnabled = true;
            }
        }

        /// <summary>
        /// Manejar el evento de tap en un tipo de _producto
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void OnCollectionViewItemTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                if (_frameSeleccionado != null)
                {
                    _frameSeleccionado.BackgroundColor = Color.FromRgb(240, 248, 255);
                    _frameSeleccionado.BorderColor = Color.FromRgb(290, 248, 255);
                }

                _frameSeleccionado = frame;
                _frameSeleccionado.BackgroundColor = Color.FromRgb(290, 248, 255);
                _frameSeleccionado.BorderColor = Color.FromRgb(240, 148, 155);

                string? item = frame.BindingContext as string;
                var tipoProducto = (TipoProducto)Enum.Parse(typeof(TipoProducto), item);
                var listaProductos = await _servicioProducto.ObtenerListaProductosPorTipo(tipoProducto, _servicioUsuario.IdUsuario);

                ProductosCollectionView.ItemsSource = listaProductos;
            }
        }

        /// <summary>
        /// Manejar el evento de clic en el botón de añadir _producto
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void AnyadirProductoClicked(object sender, EventArgs e)
        {
            using (new Botones(idAnyadirProductoClicked))
            {
                try
                {
                    IsEnabled = false;
                    await Botones.animacionImageButton(sender, e);
                    await Navigation.PushAsync(new AnyadirProductos(_servicioUsuario.IdUsuario));
                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Manejar el evento de clic en el botón de mis alquileres
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void MisAlquileresClicked(object sender, EventArgs e)
        {
            using (new Botones(idMisAlquileresClicked))
            {
                try
                {
                    IsEnabled = false;
                    await Botones.animacionImageButton(sender, e);
                    await Navigation.PushAsync(new ProductosAlquilados(_servicioUsuario.IdUsuario));
                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Manejar el evento de clic en el botón de mostrar datos del _usuario
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void MostrarDatosUsuarioClicked(object sender, EventArgs e)
        {
            using (new Botones(idMostrarDatosUsuarioClicked))
            {
                try
                {
                    IsEnabled=false;
                    await Botones.animacionImageButton(sender, e);
                    await Navigation.PushAsync(new EditarDatosUsuario(this.Navigation, _servicioUsuario.IdUsuario));
                    IsEnabled=true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Manejar el evento de clic en el botón de vista principal
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void VistaPrincipalClicked(object sender, EventArgs e)
        {
            using (new Botones(idVistaPrincipalClicked))
            {
                try
                {
                    IsEnabled=false;
                    await Botones.animacionImageButton(sender, e);
                    await Navigation.PushAsync(new VistaProductos(_servicioUsuario.IdUsuario));
                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Manejar el evento de clic en el botón de cerrar sesión
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void CerrarSesionClicked(object sender, EventArgs e)
        {
            idCerrarSesionClicked.IsEnabled = false;

            Dispatcher.Dispatch(async () =>
            {
                var leave = await DisplayAlert("", "¿Quieres salir de la sesión actual?", "ACEPTAR", "CANCELAR");

                if (leave)
                {
                    await Navigation.PushAsync(new Login());
                }
                else
                {
                    idCerrarSesionClicked.IsEnabled = true;
                }
            });
        }

        /// <summary>
        /// Manejar el evento de presionar el botón hacia atrás
        /// </summary>
        /// <returns>Booleano indicando si se manejó el evento</returns>
        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                var leave = await DisplayAlert("", "¿Quieres salir de la aplicación?", "ACEPTAR", "CANCELAR");

                if (leave)
                {
                    App.Current.Quit();
                }
            });

            return true;
        }

        #endregion

        /// <summary>
        /// Método ejecutado al inicializar la vista
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarTiposProducto();
            CargarListaProductos();
            ComprobarEstado();
        }
    }
}
