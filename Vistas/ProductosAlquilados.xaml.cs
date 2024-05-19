using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using AlkilaApp.Servicios.Implementacion;

namespace AlkilaApp.Vistas
{
    public partial class ProductosAlquilados : ContentPage
    {
        #region Atributos

        /// <summary>
        /// Servicio de usuario
        /// </summary>
        private ServicioUsuario servicioUsuario = new ServicioUsuario();

        /// <summary>
        /// Servicio de alquiler
        /// </summary>
        private ServicioAlquiler servicioAlquilar = new ServicioAlquiler();

        /// <summary>
        /// Servicio de producto
        /// </summary>
        private ServicioProducto servicioProducto = new ServicioProducto();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase ProductosAlquilados
        /// </summary>
        /// <param name="id">ID del usuario</param>
        public ProductosAlquilados(string id)
        {
            servicioUsuario.IdUsuario = id;
            InitializeComponent();

            textoNoHayProductos.IsVisible = false;
            imgNoProductos.IsVisible = false;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Cargar la lista de productos alquilados
        /// </summary>
        private async Task CargarListaProductos()
        {
            circuloCarga.IsRunning = true;

            try
            {
                List<Alquiler> listaProductosAlquilados = await servicioAlquilar.GetAlquileresByUsuarioCompradorYVendedorId(servicioUsuario.IdUsuario);

                if (listaProductosAlquilados.Count > 0)
                {
                    ProductosAlquiladosCollectionView.ItemsSource = listaProductosAlquilados;

                    foreach (Alquiler item in listaProductosAlquilados)
                    {
                        if (item.EstadoAlquiler == Estado.Pendiente || item.IdUsuarioVendedor.Equals(servicioUsuario.IdUsuario))
                        {
                            await ComprobarProductosAlquilados(item);
                        }
                    }

                    circuloCarga.IsVisible = false;
                    circuloCarga.IsRunning = false;
                    textoEspera.IsVisible = false; // Esto es un texto descriptivo mientras está cargando el círculo
                }
                else
                {
                    textoNoHayProductos.IsVisible = true;
                    imgNoProductos.IsVisible = true;
                    circuloCarga.IsVisible = false;
                    textoEspera.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar productos: " + ex.Message);
            }
        }

        /// <summary>
        /// Comprobar productos alquilados
        /// </summary>
        /// <param name="item">Objeto Alquiler</param>
        public async Task ComprobarProductosAlquilados(Alquiler item)
        {
            try
            {
                Producto producto = await servicioProducto.ObtenerProductoPorId(item.IdProducto);

                if (item.EstadoAlquiler.Equals(Estado.Pendiente) && item.IdUsuarioVendedor.Equals(servicioUsuario.IdUsuario))
                {
                    var resultado = await DisplayAlert("¿Alquilar producto?", "", "ACEPTAR", "CANCELAR");

                    if (resultado)
                    {
                        item.EstadoAlquiler = Estado.Aceptado;
                        producto.EstaAlquilado = true;
                        await servicioProducto.ActualizarProductoAUsuario(producto, servicioUsuario.IdUsuario);
                        await servicioAlquilar.InsertarOAlquilarAlquiler(item);
                    }
                    else
                    {
                        item.EstadoAlquiler = Estado.Cancelado;
                        await servicioAlquilar.InsertarOAlquilarAlquiler(item);
                    }

                    ServicioWhatsApp servicioWhatsApp = new ServicioWhatsApp();
                    Usuario usuario = await servicioUsuario.ObtenerUsuarioPorId(item.IdUsuarioVendedor);
                    Usuario otroUsuario = await servicioUsuario.ObtenerUsuarioPorId(item.IdUsuarioComprador);


                    string respuesta = resultado ? "aceptado" : "denegado";

                    string detalleAlquiler = $"{usuario.Nombre} ha {respuesta} alquilar el producto {producto.Nombre}, precio: {item.PrecioTotal}€, hasta el día: {item.FechaFin} con el código {item.IdAlquiler}";

                    string imagen = Setting.FotoWhatsApp;

                    await servicioWhatsApp.EnviarPlantilla(otroUsuario.NumeroTelefono,detalleAlquiler);
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra sin detener la aplicación
                Console.WriteLine($"Se produjo un error en ComprobarProductosAlquilados(): {ex.Message}");
            }
        }

        #endregion

        #region Botones

        /// <summary>
        /// Mostrar información sobre los estados de los productos alquilados
        /// </summary>
        /// <param name="sender">Objeto que envía el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void InformacionEstadosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Información sobre el producto alquilado",
                "\nAceptado: " + "Deberás de llevar el producto a la ubicación establecida en el maps. \n\n" +
                "\nCancelado: " + "Se ha denegado la petición de alquilar un producto.\n\n" +
                "\nEnvio: " + "Es cuando el usuario que posee tu producto tiene que devolvértelo.\n\n" +
                "\nRecibido: " + "Cuando recibes el producto, tendrás que confirmar a la plataforma que lo has recibido.\n\n",
                "ACEPTAR");
        }

        #endregion

        /// <summary>
        /// Método ejecutado al inicializar la vista
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Llamar al método que carga la lista de productos alquilados
            await CargarListaProductos();
        }
    }
}
