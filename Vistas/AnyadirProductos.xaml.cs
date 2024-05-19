using AlkilaApp.Animaciones;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using AlkilaApp.Servicios.Implementacion;
using SkiaSharp;
using System.ComponentModel;

namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnyadirProductos : ContentPage, INotifyPropertyChanged
    {
        #region Atributos 

        public ServicioProducto servicioProducto;
        public Producto producto = new Producto();
        public ServicioUsuario servicioUsuario;
        public EditarDatosUsuario _editarDatosUsuario;
        public Helpers _helperFoto = new Helpers();

        // Variable para controlar que el usuario pueda volver a la página anterior en el caso de que quiera añadir un producto
        private bool _paginaDetalleProducto = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase AnyadirProductos.
        /// </summary>
        /// <param name="id">ID del usuario actual.</param>
        public AnyadirProductos(string id)
        {
            NavigationPage.SetHasBackButton(this, true);
            InitializeComponent();
            servicioProducto = new ServicioProducto();
            servicioUsuario = new ServicioUsuario { IdUsuario = id };
            idEditarClicked.IsEnabled = false;

            // Inicializar EditarDatosUsuario para actualizar la lista de productos
            _editarDatosUsuario = new EditarDatosUsuario(this.Navigation, id);

            // Agregar las opciones del enum TipoProducto al Picker
            foreach (TipoProducto tipo in Enum.GetValues(typeof(TipoProducto)))
            {
                TipoProductoPicker.Items.Add(tipo.ToString());
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carga los datos del producto a editar.
        /// </summary>
        /// <param name="prodEdit">Producto a editar.</param>
        public async void CargarDatosProductos(Producto prodEdit)
        {
            try
            {
                idGuardarClicked.IsEnabled = false;
                idEditarClicked.IsEnabled = true;
                _paginaDetalleProducto = false;

                if (prodEdit != null)
                {
                    boton_foto_perfil.Source = prodEdit.Foto;
                    NombreEntry.Text = prodEdit.Nombre;
                    DescripcionEditor.Text = prodEdit.DescripcionProducto;
                    PrecioEntry.Text = prodEdit.Precio.ToString();
                    TipoProductoPicker.SelectedItem = prodEdit.Tipo.ToString();

                    producto = prodEdit;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al cargar los datos del producto.", e);
            }
        }

        /// <summary>
        /// Guarda los datos del producto.
        /// </summary>
        /// <returns>Task representando la operación asincrónica.</returns>
        public async Task GuardarDatosProducto()
        {
            if (!CamposCompletos())
            {
                await DisplayAlert("Error", "Por favor, completa todos los campos.", "ACEPTAR");
                return;
            }

           if (boton_foto_perfil.Source.Equals(""))
            {
                await DisplayAlert("Error", "Por favor, Selecciona una imagen de la galería.", "ACEPTAR");
                return; 
            }


            if (double.TryParse(PrecioEntry.Text, out double precio) && TipoProductoPicker.SelectedItem != null)
            {
                producto.IdProducto = Guid.NewGuid().ToString();
                producto.Nombre = NombreEntry.Text;
                producto.DescripcionProducto = DescripcionEditor.Text;
                producto.EstaAlquilado = false;
                producto.Precio = precio;
                producto.Valoracion = 0;
                producto.TotalUsuariosValorados = 0;
                producto.TotalValoracionProductos = 0;
                producto.Tipo = Enum.Parse<TipoProducto>(TipoProductoPicker.SelectedItem.ToString());

                string respuesta = await servicioUsuario.AgregarProductoAUsuario(producto);

                if (producto != null)
                {
                    await DisplayAlert("Confirmación", respuesta, "ACEPTAR");
                    await _editarDatosUsuario.CargarMiListaProductos();
                    await Navigation.PushAsync(new EditarDatosUsuario(this.Navigation, servicioUsuario.IdUsuario));
                }
            }
            else
            {
                await DisplayAlert("Error", "El precio ingresado no es válido o no se ha seleccionado un tipo de producto.", "ACEPTAR");
            }
        }

        /// <summary>
        /// Verifica si todos los campos del formulario están completos.
        /// </summary>
        /// <returns>True si todos los campos están completos; de lo contrario, false.</returns>
        public bool CamposCompletos()
        {
            return !string.IsNullOrWhiteSpace(NombreEntry.Text) &&
                   !string.IsNullOrWhiteSpace(DescripcionEditor.Text) &&
                   TipoProductoPicker.SelectedItem != null &&
                   !string.IsNullOrWhiteSpace(PrecioEntry.Text);
        }

        /// <summary>
        /// Edita los datos del producto.
        /// </summary>
        public async void EditarDatosProducto()
        {
            if (double.TryParse(PrecioEntry.Text, out double precio) && TipoProductoPicker.SelectedItem != null)
            {
                producto.Nombre = NombreEntry.Text;
                producto.DescripcionProducto = DescripcionEditor.Text;
                producto.Precio = precio;
                producto.Tipo = Enum.Parse<TipoProducto>(TipoProductoPicker.SelectedItem.ToString());

                await servicioProducto.ActualizarProductoAUsuario(producto, servicioUsuario.IdUsuario);
                await _editarDatosUsuario.CargarMiListaProductos();
            }
            else
            {
                await DisplayAlert("Error", "El precio ingresado no es válido o no se ha seleccionado un tipo de producto.", "ACEPTAR");
            }
        }

        #endregion

        #region Botones

        /// <summary>
        /// Maneja el evento de clic en el botón para seleccionar una imagen.
        /// </summary>
        private async void LogoEmpresaClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Seleccione una imagen" });

                if (result != null)
                {
                    string fileName = Guid.NewGuid().ToString() + ".jpg";

                    using (var stream = await result.OpenReadAsync())
                    using (var bitmap = SKBitmap.Decode(stream))
                    {
                        using (var resizedBitmap = bitmap.Resize(new SKImageInfo(480, 640), SKFilterQuality.Medium))
                        {
                            using (var resizedImageStream = new MemoryStream())
                            {
                                resizedBitmap.Encode(resizedImageStream, SKEncodedImageFormat.Jpeg, 100);
                                var resizedImageData = resizedImageStream.ToArray();

                                var imageUrl = await _helperFoto.AddFoto(new MemoryStream(resizedImageData), fileName);

                                if (imageUrl != null)
                                {
                                    producto.Foto = imageUrl;
                                    boton_foto_perfil.Source = imageUrl;
                                }
                                else
                                {
                                    await DisplayAlert("Error", "No se pudo subir la imagen.", "ACEPTAR");
                                }
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                await DisplayAlert("Error", "La operación fue cancelada.", "ACEPTAR");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "ACEPTAR");
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de cancelar.
        /// </summary>
        private async void CancelarClicked(object sender, EventArgs e)
        {
            using (new Botones(idCancelarClicked))
            {
                try
                {
                    await Botones.animaacionImageButton(sender, e);
                    await Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de guardar.
        /// </summary>
        private async void GuardarClicked(object sender, EventArgs e)
        {
            using (new Botones(idGuardarClicked))
            {
                try
                {
                    await Botones.animaacionImageButton(sender, e);
                    await GuardarDatosProducto();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de editar.
        /// </summary>
        private async void EditarClicked(object sender, EventArgs e)
        {
            using (new Botones(idEditarClicked))
            {
                try
                {
                    await Botones.animaacionImageButton(sender, e);
                    EditarDatosProducto();
                    await Navigation.PushAsync(_editarDatosUsuario);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            await Botones.animaacionImageButton(sender, e);
        }

        #endregion

        #region Métodos Sobrescritos

        /// <summary>
        /// Controla la acción al presionar el botón de retroceso.
        /// </summary>
        /// <returns>True para indicar que se ha manejado el evento.</returns>
        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                var leave = await DisplayAlert("", "¿Deseas salir sin guardar los datos?", "ACEPTAR", "CANCELAR");

                if (leave)
                {
                    await Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));
                }
            });

            return true;
        }

        #endregion
    }
}
