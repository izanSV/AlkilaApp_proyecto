using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using AlkilaApp.Servicios.Implementacion;

namespace AlkilaApp.Vistas
{
    public partial class ProductosAlquilados : ContentPage
    {
        #region Atributos

        /// <summary>
        /// Servicio de _usuario
        /// </summary>
        private ServicioUsuario _servicioUsuario = new ServicioUsuario();

        /// <summary>
        /// Servicio de _alquiler
        /// </summary>
        private ServicioAlquiler _servicioAlquilar = new ServicioAlquiler();

        /// <summary>
        /// Servicio de _producto
        /// </summary>
        private ServicioProducto _servicioProducto = new ServicioProducto();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase ProductosAlquilados
        /// </summary>
        /// <param name="id">ID del _usuario</param>
        public ProductosAlquilados(string id)
        {
            _servicioUsuario.IdUsuario = id;
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
                List<Alquiler> listaProductosAlquilados = await _servicioAlquilar.GetAlquileresByUsuarioCompradorYVendedorId(_servicioUsuario.IdUsuario);

                if (listaProductosAlquilados.Count > 0)
                {
                    ProductosAlquiladosCollectionView.ItemsSource = listaProductosAlquilados;

                    foreach (Alquiler item in listaProductosAlquilados)
                    {
                        if (item.EstadoAlquiler == Estado.Pendiente || item.IdUsuarioVendedor.Equals(_servicioUsuario.IdUsuario))
                        {
                            await ComprobarProductosAlquilados(item);
                        }
                    }

                    circuloCarga.IsVisible = false;
                    circuloCarga.IsRunning = false;
                    textoEspera.IsVisible = false; // Esto es un texto descriptivo mientras est� cargando el c�rculo
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
                Producto producto = await _servicioProducto.ObtenerProductoPorId(item.IdProducto);

                if (item.EstadoAlquiler.Equals(Estado.Pendiente) && item.IdUsuarioVendedor.Equals(_servicioUsuario.IdUsuario))
                {
                    var resultado = await DisplayAlert("�Alquilar producto?", "Quieres alquilar el producto " + item.NombreProductoAlquilado + " por " + item.PrecioTotal + "�", "ACEPTAR", "CANCELAR");

                    if (resultado)
                    {
                        item.EstadoAlquiler = Estado.Aceptado;
                        producto.EstaAlquilado = true;
                        await _servicioProducto.ActualizarProductoAUsuario(producto, _servicioUsuario.IdUsuario);
                        await _servicioAlquilar.InsertarOAlquilarAlquiler(item);

                    }
                    else
                    {
                        item.EstadoAlquiler = Estado.Cancelado;
                        await _servicioAlquilar.InsertarOAlquilarAlquiler(item);
                    }

                    ServicioWhatsApp servicioWhatsApp = new ServicioWhatsApp();
                    Usuario usuario = await _servicioUsuario.ObtenerUsuarioPorId(item.IdUsuarioVendedor);
                    Usuario otroUsuario = await _servicioUsuario.ObtenerUsuarioPorId(item.IdUsuarioComprador);


                    string respuesta = resultado ? "aceptado" : "denegado";

                    string detalleAlquiler = $"{usuario.Nombre} ha {respuesta} alquilar el producto {producto.Nombre}, precio: {item.PrecioTotal}�, hasta el d�a: {item.FechaFin} con el c�digo {item.IdAlquiler}";

                    await servicioWhatsApp.EnviarPlantilla(otroUsuario.NumeroTelefono,detalleAlquiler);
                    await CargarListaProductos();
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepci�n que ocurra sin detener la aplicaci�n
                Console.WriteLine($"Se produjo un error en ComprobarProductosAlquilados(): {ex.Message}");
            }
        }

        #endregion

        #region Botones

        /// <summary>
        /// Mostrar informaci�n sobre los estados de los productos alquilados
        /// </summary>
        /// <param name="sender">Objeto que env�a el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private async void InformacionEstadosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Informaci�n sobre el producto alquilado",
                "\nAceptado: " + "Deber�s de llevar el producto a la ubicaci�n establecida en el maps. \n\n" +
                "\nCancelado: " + "Se ha denegado la petici�n de alquilar un producto.\n\n" +
                "\nEnvio: " + "Es cuando el usuario que posee tu producto tiene que devolv�rtelo.\n\n" +
                "\nRecibido: " + "Cuando recibes el producto, tendr�s que confirmar a la plataforma que lo has recibido.\n\n",
                "ACEPTAR");
        }

        #endregion

        /// <summary>
        /// M�todo ejecutado al inicializar la vista
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Llamar al m�todo que carga la lista de productos alquilados
            await CargarListaProductos();
        }
    }
}
