﻿using AlkilaApp.Modelos;
using AlkilaApp.Servicios.Implementacion;
using System.ComponentModel;

namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditarDatosUsuario : ContentPage, INotifyPropertyChanged
    {
        #region Atributos

        /// <summary>
        /// Objeto para la navegación
        /// </summary>
        private INavigation _Navigation;

        /// <summary>
        /// Servicio de _usuario
        /// </summary>
        private ServicioUsuario _servicioUsuario;

        /// <summary>
        /// Objeto _usuario
        /// </summary>
        private Usuario _usuario = new Usuario();

        /// <summary>
        /// Objeto ubicación
        /// </summary>
        private Ubicacion _ubicacion = new Ubicacion();

        /// <summary>
        /// Servicio de ubicación
        /// </summary>
        private ServicioUbicacion _servicioUbicacion = new ServicioUbicacion();

        /// <summary>
        /// Helper para manejar fotos
        /// </summary>
        private Helpers _helperFoto = new Helpers();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase EditarDatosUsuario
        /// </summary>
        /// <param name="navigation">Objeto de navegación</param>
        /// <param name="idUsuario">ID del _usuario</param>
        public EditarDatosUsuario(INavigation navigation, string idUsuario)
        {
            InitializeComponent();
            _Navigation = navigation;
            _servicioUsuario = new ServicioUsuario();
            _servicioUsuario.IdUsuario = idUsuario;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Carga los datos del _usuario
        /// </summary>
        public async void CargarDatosUsuario()
        {
            try
            {
                _usuario = await _servicioUsuario.ObtenerUsuarioRegistrado();
                _ubicacion = await _servicioUbicacion.ObtenerLocalizacionAsync(_servicioUsuario.IdUsuario);

                // Asignar los datos del _usuario a los campos
                NombreEntry.Text = _usuario.Nombre;
                ApellidoEntry.Text = _usuario.Apellido;
                ContrasenyaEntry.Text = _usuario.Contrasenya;
                CorreoElectronicoEntry.Text = _usuario.CorreoElectronico;
                DateEntry.Date = _usuario.FechaNacimiento;
                boton_foto_perfil.Source = _usuario.Foto;
                TelefonoEntry.Text = _usuario.NumeroTelefono;

                if (_ubicacion != null)
                {
                    // Cargamos los datos de la ubicación
                    CalleEntry.Text = _ubicacion.Calle;
                    LocalidadEntry.Text = _ubicacion.Localidad;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Actualiza la lista de productos
        /// </summary>
        public async Task ActualizarListaProductos()
        {
            try
            {
                var listaProductosActualizada = await _servicioUsuario.ObtenerListaProductos(_servicioUsuario.IdUsuario);
                ProductosCollectionView.ItemsSource = listaProductosActualizada;

                NoProductosLabel.IsVisible = listaProductosActualizada == null || listaProductosActualizada.Count == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }

        /// <summary>
        /// Carga inicialmente la lista de productos
        /// </summary>
        public async Task CargarMiListaProductos()
        {
            try
            {
                await ActualizarListaProductos();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }

        #endregion

        #region Ubicación

        /// <summary>
        /// Maneja el evento de clic en el botón de ubicación actual
        /// </summary>
        public async void BtnUbicacionActual(object sender, EventArgs e)
        {
            try
            {

                btnUbiReal.IsEnabled = false;

                _ubicacion = await RegistrarUbicacionEnTiempoReal();
                if (_ubicacion != null)
                {
                    await DisplayAlert("Ubicación registrada", (_ubicacion.Calle + " ," + _ubicacion.Localidad), "ACEPTAR");
                    await _servicioUbicacion.InsertarOActualizarUbicacion(_ubicacion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                throw;
            }

            btnUbiReal.IsEnabled = true;
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de comprobar ubicación
        /// </summary>
        public async void BtnComprobarUbicacion(object sender, EventArgs e)
        {
            try
            {
                btnComprUbica.IsEnabled = false;

                string localidad = LocalidadEntry.Text;
                string calle = CalleEntry.Text;

                _ubicacion = await RegistrarUbicacion(localidad, calle);
                Console.WriteLine(_ubicacion);
            }
            catch (Exception)
            {
                throw;
            }
            btnComprUbica.IsEnabled = true;

        }

        /// <summary>
        /// Registra la ubicación en función de la localidad y la calle
        /// </summary>
        public async Task<Ubicacion> RegistrarUbicacion(string localidad, string calle)
        {
            try
            {
                var coordenadas = await ObtenerCoordenadas(localidad, calle);

                if (coordenadas != null)
                {
                    double latitud = coordenadas.Latitude;
                    double longitud = coordenadas.Longitude;
                    string lugar = await ObtenerNombreLugar(latitud, longitud);

                    string[] partes = lugar.Split(',');
                    calle = partes[0];
                    localidad = partes[1];

                    await DisplayAlert("Ubicación registrada", lugar, "ACEPTAR");

                    return new Ubicacion
                    {
                        Latitud = latitud,
                        Longitud = longitud,
                        Calle = calle,
                        Localidad = localidad,
                    };
                }
                else
                {
                    await DisplayAlert("Error", "La ubicación no es correcta o no está activada", "ACEPTAR");
                    return null;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "No se ha podido encontrar la ubicación", "ACEPTAR");
                return null;
            }
        }

        /// <summary>
        /// Obtiene el nombre del lugar en función de la latitud y longitud
        /// </summary>
        public async Task<string> ObtenerNombreLugar(double latitud, double longitud)
        {
            try
            {
                string cadena = "";

                var placemarks = await Geocoding.GetPlacemarksAsync(latitud, longitud);
                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    cadena = placemark.Thoroughfare + ",";
                    cadena += placemark.SubThoroughfare + "\n";
                    cadena += placemark.Locality + ",";
                    cadena += placemark.SubLocality + ",";
                    cadena += placemark.CountryName;

                    return cadena;
                }
                else
                {
                    return "Nombre de lugar no encontrado";
                }
            }
            catch (Exception ex)
            {
                return "Error al obtener el nombre del lugar: " + ex.Message;
            }
        }

        /// <summary>
        /// Obtiene las coordenadas en función de la localidad y la calle
        /// </summary>
        public static async Task<Location?> ObtenerCoordenadas(string localidad, string calle)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync($"{calle}, {localidad}");
                return locations?.FirstOrDefault();
            }
            catch (FeatureNotSupportedException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Registra la ubicación en tiempo real
        /// </summary>
        public async Task<Ubicacion> RegistrarUbicacionEnTiempoReal()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permiso denegado", "No se puede acceder a la ubicación porque el permiso fue denegado.", "ACEPTAR");
                        return null;
                    }
                }

                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    double latitud = location.Latitude;
                    double longitud = location.Longitude;
                    string lugar = await ObtenerNombreLugar(latitud, longitud);

                    string[] partes = lugar.Split(' ');
                    string calle = partes[0];
                    string localidad = partes[1];

                    // rellenamos los campos una vez obtenidos
                    CalleEntry.Text = calle;
                    LocalidadEntry.Text = localidad;

                    return new Ubicacion
                    {
                        Latitud = latitud,
                        Longitud = longitud,
                        Calle = calle,
                        Localidad = localidad,
                    };

                    
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo obtener la ubicación actual.", "ACEPTAR");
                    return null;
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "La geolocalización no es soportada en este dispositivo.", "ACEPTAR");
                return null;
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "El permiso para acceder a la ubicación fue denegado.", "ACEPTAR");
                return null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener la ubicación: {ex.Message}", "ACEPTAR");
                return null;
            }
        }

        /// <summary>
        /// Abre Google Maps con la ubicación especificada
        /// </summary>
        public async Task AbrirGoogleMaps(string localidad, string calle, double longitud, double latitud)
        {
            var placemark = new Placemark
            {
                CountryName = "Spain",
                Thoroughfare = calle,
                Locality = localidad
            };

            var coordenadas = await ObtenerCoordenadas(localidad, calle);

            if (coordenadas != null)
            {
                latitud = coordenadas.Latitude;
                longitud = coordenadas.Longitude;

                await DisplayAlert("Error", $"Google Maps: {latitud},{longitud}", "ACEPTAR");

                string lugar = await ObtenerNombreLugar(latitud, longitud);
                await DisplayAlert("Error", lugar, "ACEPTAR");
            }

            try
            {
                await placemark.OpenMapsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener la ubicación en Google Maps: {ex.Message}", "ACEPTAR");
            }
        }

        #endregion

        #region Botones

        /// <summary>
        /// Habilita la edición de los campos de entrada
        /// </summary>
        public async void OnEditarCamposClicked(object sender, EventArgs e)
        {
            try
            {
                await animacionButton(sender, e);

                NombreEntry.IsEnabled = true;
                ApellidoEntry.IsEnabled = true;
                DateEntry.IsEnabled = true;
                TelefonoEntry.IsEnabled = true;
                boton_foto_perfil.IsEnabled = true;
                CalleEntry.IsEnabled = true;
                LocalidadEntry.IsEnabled = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cancela la edición de los datos del _usuario
        /// </summary>
        public async void CancelarDatosClicked(object sender, EventArgs e)
        {
            try
            {
                await animacionButton(sender, e);
                await _Navigation.PushAsync(new VistaProductos(_servicioUsuario.IdUsuario));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Guarda los datos del _usuario
        /// </summary>
        public async void GuardarDatosClicked(object sender, EventArgs e)
        {
            try
            {
                // aplicamos la animación
               await animacionButton(sender, e);
                // Verificar si los campos están vacíos
                if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                    string.IsNullOrWhiteSpace(ApellidoEntry.Text) ||
                    string.IsNullOrWhiteSpace(TelefonoEntry.Text))
                {
                    // Mostrar un mensaje de advertencia
                    await DisplayAlert("Campos vacíos", "Por favor, complete todos los campos antes de guardar.", "ACEPTAR");
                    return; // Salir del método sin guardar los datos
                }

                _usuario.Nombre = NombreEntry.Text;
                _usuario.Apellido = ApellidoEntry.Text;
                _usuario.FechaNacimiento = DateEntry.Date;
                _usuario.NumeroTelefono = TelefonoEntry.Text;

                //validación numero de telefono
                if (_usuario.NumeroTelefono.Length != 9 || _usuario.NumeroTelefono.Contains("-"))
                {
                    await DisplayAlert("Campos vacíos", "Por favor, Inserte un numero de telefono válido.", "ACEPTAR");
                    return; // Salir del método sin guardar los datos
                }

                string respuesta = await _servicioUsuario.AnyadirOActualizarUsuario(_usuario);

                if (_ubicacion != null)
                {
                    _ubicacion.IdUsuario = _servicioUsuario.IdUsuario;
                    await _servicioUbicacion.InsertarOActualizarUbicacion(_ubicacion);
                }

                await _Navigation.PushAsync(new VistaProductos(_servicioUsuario.IdUsuario));
            }
            catch (Exception ex)
            {
                // Manejar la excepción aquí, ya sea mostrando un mensaje de error o realizando alguna otra acción
                await DisplayAlert("Error", $"Ocurrió un error al guardar los datos: {ex.Message}", "ACEPTAR");
            }
        }


        /// <summary>
        /// Navega a la página de detalles del _producto
        /// </summary>
        private async void OnCardTapped(object sender, EventArgs e)
        {
            try
            {
                var frame = (Frame)sender;
                var producto = frame.BindingContext as Producto;

                var detallesProductoPage = new DetallesProducto(_servicioUsuario.IdUsuario, producto, esProductoDelUsuario: false);
                await Navigation.PushAsync(detallesProductoPage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el logo de la empresa para seleccionar una imagen
        /// </summary>
        private async void LogoEmpresa_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleccione una imagen"
                });

                if (result != null)
                {
                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    var stream = await result.OpenReadAsync();

                    var imageUrl = await _helperFoto.AddFoto(stream, fileName);
                    if (imageUrl != null)
                    {
                        _usuario.Foto = imageUrl;
                        boton_foto_perfil.Source = imageUrl;
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo subir la imagen.", "OK");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                await DisplayAlert("Error", "La operación fue cancelada.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Ocurrió un error inesperado: " + ex.Message, "OK");
            }
        }

        /// <summary>
        /// Navega a la página principal cuando se presiona el botón de retroceso
        /// </summary>
        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                await Navigation.PushAsync(new VistaProductos(_servicioUsuario.IdUsuario));
            });

            return true;
        }

        #endregion

        /// <summary>
        /// Maneja el evento de aparición de la página
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarDatosUsuario();
            CargarMiListaProductos();
        }

        /// <summary>
        /// Aplica una animación al botón cuando se presiona
        /// </summary>
        private async Task animacionButton(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;
            button.InputTransparent = true;

            await button.ScaleTo(0.6, 40);
            await button.ScaleTo(1, 50);
            await button.ScaleTo(0.9, 60);
            await button.ScaleTo(1.1, 70);

            button.InputTransparent = false;
        }
    }
}
