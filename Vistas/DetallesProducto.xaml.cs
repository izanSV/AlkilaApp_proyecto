using AlkilaApp.Animaciones;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using System.Text;

namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetallesProducto : ContentPage
    {
        #region Atributos

        private ServicioUbicacion _servicioUbicacion;
        private ServicioUsuario _servicioUsuario;
        private ServicioProducto _servicioProducto;
        private Usuario _infoUsuario;
        private Producto _producto;
        private EditarDatosUsuario _editarDatosUsuario;
        private AnyadirProductos _anyadirProductos;
        private Ubicacion _ubicacion;
        private ServicioAlquiler _servicioAlquiler;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase DetallesProducto.
        /// </summary>
        /// <param name="idUsuario">ID del _usuario actual.</param>
        /// <param name="producto">Producto a mostrar.</param>
        /// <param name="esProductoDelUsuario">Indica si el _producto pertenece al _usuario actual.</param>
        public DetallesProducto(string idUsuario, Producto producto, bool esProductoDelUsuario)
        {
            InitializeComponent();
            iconoEmpresa.IsVisible = false;

            _servicioProducto = new ServicioProducto();
            _servicioUbicacion = new ServicioUbicacion();
            _servicioUsuario = new ServicioUsuario();
            _servicioUsuario.IdUsuario = idUsuario;
            _editarDatosUsuario = new EditarDatosUsuario(this.Navigation, _servicioUsuario.IdUsuario);
            _anyadirProductos = new AnyadirProductos(_servicioUsuario.IdUsuario);
            _servicioAlquiler = new ServicioAlquiler();

            BindingContext = producto;

            AsignarDatosProducto(producto);
            HabilitarBotones(producto, esProductoDelUsuario);
            DetallesUsuario(producto.IdProducto);
            ObtenerFechaFinalAlquiler(producto);
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Asigna los datos del _producto a los elementos de la interfaz de _usuario.
        /// </summary>
        /// <param name="producto">Producto a mostrar.</param>
        private void AsignarDatosProducto(Producto producto)
        {
            NombreLabel.Text = producto.Nombre;
            PrecioLabel.Text = $"Precio: {producto.Precio}";
            DescripcionLabel.Text = producto.DescripcionProducto;
            FotoImage.Source = producto.Foto;
            TipoLabel.Text = Enum.GetName(typeof(TipoProducto), producto.Tipo);

            _producto = producto;
        }

        /// <summary>
        /// Obtiene la fecha de finalización del _alquiler del _producto.
        /// </summary>
        /// <param name="producto">Producto a consultar.</param>
        private async void ObtenerFechaFinalAlquiler(Producto producto)
        {
            Alquiler alquiler = await _servicioAlquiler.ObtenterAlquilerporProductoId(producto.IdProducto);

            if (alquiler != null && producto.IdProducto.Equals(alquiler.IdProducto) && producto.EstaAlquilado)
            {
                string fechaFinAlquiler = alquiler.FechaFin.ToString("yyyy-MM-dd");
                idDescAlquiler.Text = "Fecha de finalización del _producto:";
                idFechaFinAlquiler.Text = fechaFinAlquiler;
            }
        }

        /// <summary>
        /// Habilita o deshabilita los botones de la interfaz según el estado del _producto.
        /// </summary>
        /// <param name="producto">Producto a evaluar.</param>
        /// <param name="esProductoDelUsuario">Indica si el _producto pertenece al _usuario actual.</param>
        private void HabilitarBotones(Producto producto, bool esProductoDelUsuario)
        {
            AlquilarButton.IsVisible = esProductoDelUsuario && !producto.EstaAlquilado;
            EditarButton.IsVisible = !esProductoDelUsuario;
            EliminarButton.IsVisible = !esProductoDelUsuario;
        }

        /// <summary>
        /// Muestra los detalles del _usuario propietario del _producto.
        /// </summary>
        /// <param name="idProducto">ID del _producto.</param>
        public async void DetallesUsuario(string idProducto)
        {
            try
            {
                _infoUsuario = await _servicioUsuario.ObtenerUsuarioPorIdProducto(idProducto);
                UsuarioLabel.Text = _infoUsuario.Nombre;
                FotoLabel.Source = _infoUsuario.Foto;
                iconoEmpresa.IsVisible = _infoUsuario.EsEmpresa;

                _ubicacion = await _servicioUbicacion.ObtenerLocalizacionAsync(_infoUsuario.IdUsuario);

                DatosUbicacionUsuario.Text = _ubicacion != null
                    ? $"{_ubicacion.Calle}, {_ubicacion.Localidad}"
                    : "El _usuario no ha registrado una ubicación";
            }
            catch (Exception)
            {
                Console.WriteLine("Error al obtener el nombre del _usuario.");
            }
        }

        /// <summary>
        /// Navega a la página de _alquiler de productos.
        /// </summary>
        private async void AlquilarButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AlquilarProducto(_servicioUsuario.IdUsuario, _infoUsuario, _producto));
        }

        /// <summary>
        /// Muestra opciones de navegación y abre la ruta en el mapa.
        /// </summary>
        /// <param name="ubicacion">Ubicación a la que navegar.</param>
        public async void RutaNavegacionUbicacion(Ubicacion ubicacion)
        {
            var options = await DisplayActionSheet("Selecciona el modo de navegación", "Cancelar", null, "Conducción", "Caminando", "En bicicleta");

            switch (options)
            {
                case "Conducción":
                    await RutaNavegacionUbicacion(ubicacion, NavigationMode.Driving);
                    break;
                case "Caminando":
                    await RutaNavegacionUbicacion(ubicacion, NavigationMode.Walking);
                    break;
                case "En bicicleta":
                    await RutaNavegacionUbicacion(ubicacion, NavigationMode.Bicycling);
                    break;
            }
        }

        /// <summary>
        /// Abre la navegación en el mapa con el modo especificado.
        /// </summary>
        /// <param name="ubicacion">Ubicación a la que navegar.</param>
        /// <param name="mode">Modo de navegación.</param>
        private async Task RutaNavegacionUbicacion(Ubicacion ubicacion, NavigationMode mode)
        {
            var location = new Location(ubicacion.Latitud, ubicacion.Longitud);
            var options = new MapLaunchOptions
            {
                NavigationMode = mode
            };

            try
            {
                await Map.Default.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al abrir la navegación: {ex.Message}", "ACEPTAR");
            }
        }

        /// <summary>
        /// Obtiene el nombre del lugar basado en la latitud y longitud.
        /// </summary>
        /// <param name="latitud">Latitud del lugar.</param>
        /// <param name="longitud">Longitud del lugar.</param>
        /// <returns>Nombre del lugar.</returns>
        public async Task<string> ObtenerNombreLugar(double latitud, double longitud)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitud, longitud);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var sb = new StringBuilder();

                    if (!string.IsNullOrWhiteSpace(placemark.Thoroughfare))
                        sb.AppendLine(placemark.Thoroughfare);
                    if (!string.IsNullOrWhiteSpace(placemark.Locality))
                        sb.AppendLine(placemark.Locality);
                    if (!string.IsNullOrWhiteSpace(placemark.SubLocality))
                        sb.AppendLine(placemark.SubLocality);
                    if (!string.IsNullOrWhiteSpace(placemark.SubThoroughfare))
                        sb.AppendLine(placemark.SubThoroughfare);
                    if (!string.IsNullOrWhiteSpace(placemark.CountryName))
                        sb.AppendLine(placemark.CountryName);

                    return sb.ToString();
                }
                else
                {
                    return "Nombre de lugar no encontrado";
                }
            }
            catch (Exception ex)
            {
                return $"Error al obtener el nombre del lugar: {ex.Message}";
            }
        }

        /// <summary>
        /// Abre la aplicación de mapas al tocar la imagen del mapa.
        /// </summary>
        private void OnFrameTapped(object sender, EventArgs e)
        {
            if (_ubicacion != null)
            {
                RutaNavegacionUbicacion(_ubicacion);
            }
        }

        /// <summary>
        /// Elimina el _producto si no está alquilado.
        /// </summary>
        private async void EliminarButton_Clicked(object sender, EventArgs e)
        {
            if (!_producto.EstaAlquilado)
            {
                var result = await DisplayAlert("Confirmación", $"¿Deseas eliminar el siguiente producto?", "OK", "Cancel");

                if (result)
                {
                    await _servicioProducto.EliminarProductoPorId(_producto.IdProducto);

                    await DisplayAlert("Éxito", "El producto ha sido eliminado correctamente", "OK");

                    _editarDatosUsuario.CargarMiListaProductos();
                    await Navigation.PushAsync(_editarDatosUsuario);
                }
            }
            else
            {
                await DisplayAlert("Error", "No puedes eliminar un producto mientras esté alquilado.", "OK");
            }
        }

        /// <summary>
        /// Navega a la página de edición del producto.
        /// </summary>
        private async void EditarButton_Clicked(object sender, EventArgs e)
        {
            if (_producto.EstaAlquilado)
            {
                await DisplayAlert("Error", "No puedes editar un producto mientras esté alquilado.", "OK");
                return;
            }

            if (_producto.Precio < 0.00)
            {
                await DisplayAlert("Error", "Por favor, Selecciona un precio válido.", "ACEPTAR");
                return;
            }

            try
            {
                _anyadirProductos.CargarDatosProductos(_producto);
                await Navigation.PushAsync(_anyadirProductos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Envía un mensaje de WhatsApp al número especificado.
        /// </summary>
        /// <param name="phoneNumber">Número de teléfono a enviar el mensaje.</param>
        public async Task EnviarWhatsApp(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    throw new ArgumentNullException(nameof(phoneNumber), "El número de teléfono no puede estar vacío.");
                }

                string whatsappUri = "whatsapp://send?phone=" + phoneNumber;
                await Browser.OpenAsync(new Uri(whatsappUri), BrowserLaunchMode.External);
            }
            catch (ArgumentNullException)
            {
                await DisplayAlert("Error", "No se proporcionó un número de teléfono.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se produjo un error al abrir WhatsApp: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de WhatsApp.
        /// </summary>
        private async void WhatsappClicked(object sender, EventArgs e)
        {
            try
            {
                string telefono = _infoUsuario.NumeroTelefono;
                bool abrirContacto = await DisplayAlert("", $"¿Deseas abrir el contacto de WhatsApp de \"{_infoUsuario.Nombre}\"?", "Sí", "No");

                if (abrirContacto)
                {
                    await EnviarWhatsApp("34" + telefono);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se produjo un error al abrir WhatsApp: {ex.Message}", "OK");
            }
        }

        #endregion
    }
}
