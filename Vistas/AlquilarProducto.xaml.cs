using AlkilaApp.Modelos;
using System.ComponentModel;

namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlquilarProducto : ContentPage, INotifyPropertyChanged
    {
        #region Atributos
        /// <summary>
        /// Objeto Servicio Alquiler
        /// </summary>
        private ServicioAlquiler servicioAlquilar = new ServicioAlquiler();
        /// <summary>
        /// Objeto ServicioUsuario
        /// </summary>
        private ServicioUsuario servicioUsuario = new ServicioUsuario();
        /// <summary>
        /// Objeto producto
        /// </summary>
        private Producto producto = new Producto();
        /// <summary>
        /// Objeto alquiler
        /// </summary>
        private Alquiler alquiler = new Alquiler();
        /// <summary>
        /// Objeto Aplicar cambios 
        /// </summary>

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase AlquilarProducto.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <param name="usuario">Objeto Usuario que realiza la compra.</param>
        /// <param name="pro">Objeto Producto que se alquila.</param>
        public AlquilarProducto(string? id, Usuario usuario, Producto pro)
        {
            // Obtenemos los datos temporales del producto con el usuario comprador y vendedor
            InitializeComponent();

            // El botón de los términos y condiciones, al iniciar la app, quiero que este inactivo, al presionar cambiará el estado
            imgBtnGuardar.IsEnabled = false;
            checkBox.IsEnabled = false;
            btnLeerCondicion.IsEnabled = false;

            servicioUsuario.IdUsuario = id;
            alquiler.IdUsuarioComprador = id;
            alquiler.IdUsuarioVendedor = usuario.IdUsuario;
            alquiler.IdProducto = pro.IdProducto;
            alquiler.NombreProductoAlquilado = pro.Nombre;
            alquiler.FotoProductoAlquilado = pro.Foto;
            alquiler.NombreUsuarioComprador = usuario.Nombre;

            // Obtenemos los datos del producto obtenido por el usuario
            producto = pro;

            // Suscribirse al evento DateSelected para los DatePickers
            FechaInicio.DateSelected += FechaInicio_DateSelected;
            FechaFin.DateSelected += FechaFin_DateSelected;
        }

        #endregion

        /// <summary>
        /// Calcula la diferencia en días entre las fechas seleccionadas.
        /// </summary>
        /// <returns>Número de días de diferencia.</returns>
        public int CalcularFecha()
        {
            // Obtener las fechas seleccionadas
            DateTime fechaRecogida = FechaInicio.Date;
            DateTime fechaDevolucion = FechaFin.Date;

            // Calcular la diferencia en días
            TimeSpan diferencia = fechaDevolucion - fechaRecogida;
            int diasDiferencia = diferencia.Days;
            return diasDiferencia;
        }

        /// <summary>
        /// Maneja el evento de cambio de estado del CheckBox.
        /// </summary>
        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            imgBtnGuardar.IsEnabled = e.Value; // Habilita o deshabilita el botón según el estado del CheckBox
        }

        #region Botones

        /// <summary>
        /// Guarda los datos del alquiler.
        /// </summary>
        private async void GuardarDatosClicked(object sender, EventArgs e)
        {
            await animaacionButton(sender, e);

            // Almacenamos los datos del alquiler
            DateTime fechaActual = DateTime.Now.Date;
            DateTime fechaInicio = FechaInicio.Date;
            DateTime fechaFin = FechaFin.Date;
            alquiler.EstadoAlquiler = Estado.Pendiente;
            alquiler.NoMasRespuesta = true;
            alquiler.IdAlquiler = Guid.NewGuid().ToString();

            if (fechaInicio < fechaActual || fechaFin < fechaActual)
            {
                await DisplayAlert("Error", "No se puede seleccionar una fecha anterior al día actual", "ACEPTAR ");
                return;
            }

            if (fechaInicio > fechaFin)
            {
                await DisplayAlert("Error", "La fecha de devolución no puede ser anterior a la fecha de recogida", "ACEPTAR ");
                return;
            }

            int diasAlquiler = CalcularFecha();

            if (diasAlquiler <= 0)
            {
                await DisplayAlert("Error", "La duración del alquiler debe ser de al menos un día", "ACEPTAR");
                return;
            }
            else
            {
                alquiler.FechaInicio = FechaInicio.Date;
                alquiler.FechaFin = FechaFin.Date;

                await servicioAlquilar.InsertarOAlquilarAlquiler(alquiler);

                // Volver a la página donde están los alquileres
                await DisplayAlert("Información", "Recibirás un mensaje cuando el usuario haya visto tu propuesta", "ACEPTAR");

                await Navigation.PushAsync(new VistaProductos(servicioUsuario.IdUsuario));
            }
        }

        /// <summary>
        /// Maneja el evento de selección de fecha de inicio.
        /// </summary>
        private void FechaInicio_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Obtener la fecha actual
            DateTime fechaActual = DateTime.Now;

            // Obtener la fecha seleccionada
            DateTime fechaSeleccionada = FechaInicio.Date;

            // Comprobar si la fecha seleccionada es anterior a la fecha actual
            if (fechaSeleccionada < fechaActual)
            {
                // Mostrar una alerta
                DisplayAlert("Error", "La fecha de inicio no puede ser anterior a la fecha actual.", "ACEPTAR ");

                // Restablecer la fecha de inicio al valor de la fecha actual
                FechaInicio.Date = fechaActual;
            }

            // Llamar al método para realizar el cálculo
            BtnRealizarCalculo(sender, e);
        }

        /// <summary>
        /// Maneja el evento de selección de fecha de fin.
        /// </summary>
        private void FechaFin_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Obtener la fecha actual
            DateTime fechaActual = DateTime.Now;

            // Obtener la fecha seleccionada
            DateTime fechaSeleccionada = FechaFin.Date;

            // Comprobar si la fecha seleccionada es anterior a la fecha actual
            if (fechaSeleccionada < fechaActual)
            {
                DisplayAlert("Error", "La fecha de finalización no puede ser anterior a la fecha actual", "ACEPTAR ");

                // Restablecer la fecha de finalización
                FechaFin.Date = fechaActual;
            }

            // Llamar al método para realizar el cálculo
            BtnRealizarCalculo(sender, e);
        }

        /// <summary>
        /// Maneja el evento de cancelar la acción actual.
        /// </summary>
        private async void CancelarDatosClicked(object sender, EventArgs e)
        {
            await animaacionButton(sender, e);
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Realiza el cálculo del costo del alquiler.
        /// </summary>
        private async void BtnRealizarCalculo(object sender, EventArgs e)
        {
            btnLeerCondicion.IsEnabled = true;

            // Obtener la cantidad de días de alquiler
            int diasAlquiler = CalcularFecha();

            // Calcular el costo del alquiler
            double costoAlquiler = CalcularCostoAlquiler(diasAlquiler);

            // Mostrar el costo del alquiler en la etiqueta
            CostoAlquilerLabel.Text = $"{costoAlquiler} €";
            DiasTotalesLabel.Text = $"☀ Días totales: {diasAlquiler}";

            // Almacenamos el precio total
            alquiler.PrecioTotal = costoAlquiler;
        }

        /// <summary>
        /// Calcula el costo total del alquiler según los días.
        /// </summary>
        /// <param name="diasAlquiler">Cantidad de días de alquiler.</param>
        /// <returns>Costo total del alquiler.</returns>
        private double CalcularCostoAlquiler(int diasAlquiler)
        {
            // El cálculo es sobre el precio base
            double tarifaDiaria = producto.Precio; // Tarifa diaria fija en euros
            double costoTotal = 0.0;

            if (diasAlquiler <= 3)
            {
                // Si el alquiler es igual o menor a 3 días, se aplica la tarifa diaria estándar
                costoTotal = diasAlquiler * tarifaDiaria;
            }
            else
            {
                // Si el alquiler es mayor a 3 días, se aplica un descuento del 20% a partir del cuarto día
                int diasRestantesDespuesDeUnaSemana = diasAlquiler - 3;
                double costoPrimeraSemana = 3 * tarifaDiaria;
                double costoDespuesDeUnaSemana = diasRestantesDespuesDeUnaSemana * (tarifaDiaria * 0.8); // Aplicar descuento del 20%
                costoTotal = costoPrimeraSemana + costoDespuesDeUnaSemana;
            }

            // Redondear el costo total a dos decimales
            costoTotal = Math.Round(costoTotal, 2);

            return costoTotal;
        }

        /// <summary>
        /// Muestra los términos y condiciones y habilita el CheckBox si se aceptan.
        /// </summary>
        private async void BtnLeerCondiciones(object sender, EventArgs e)
        {
            bool aceptado = await DisplayAlert("Términos y Condiciones de Uso", "Bienvenido a [AlkilaApp.inc]...\n\nPor favor, lee estos términos y condiciones cuidadosamente antes de utilizar nuestra aplicación/servicio.\n\nAceptación de los Términos\n\nAl acceder o utilizar la aplicación/servicio de cualquier manera, aceptas estar sujeto a estos términos y condiciones. Si no estás de acuerdo con alguno de estos términos, no utilices la aplicación/servicio.\n\nUso del Servicio\n\nLa aplicación/servicio y su contenido son propiedad de [Nombre de la Empresa] y están protegidos por las leyes de derechos de autor correspondientes. Estás autorizado a utilizar la aplicación/servicio solo con fines personales y no comerciales.\n\nPrivacidad\n\nTu privacidad es importante para nosotros. Consulta nuestra Política de Privacidad para comprender cómo recopilamos, utilizamos y protegemos tu información personal.\n\nContenido del Usuario\n\nAl utilizar la aplicación/servicio, puedes proporcionar cierta información, como comentarios, opiniones, etc. Al hacerlo, otorgas a [ALKILA] una licencia no exclusiva, transferible, sublicenciable, libre de regalías para utilizar, reproducir, modificar, adaptar, publicar, traducir y distribuir dicho contenido en cualquier forma, medio o tecnología.\n\nResponsabilidades del Usuario\n\nEres responsable de mantener la confidencialidad de tu cuenta y contraseña, así como de restringir el acceso a tu dispositivo. Eres responsable de todas las actividades que ocurran bajo tu cuenta.\n\nModificaciones\n\nNos reservamos el derecho de modificar o revisar estos términos y condiciones en cualquier momento. Te notificaremos cualquier cambio mediante la publicación de los términos y condiciones actualizados en la aplicación/servicio. El uso continuado de la aplicación/servicio después de dichos cambios constituye tu aceptación de los términos y condiciones revisados.\n\nTerminación\n\nNos reservamos el derecho de suspender o dar por terminado tu acceso a la aplicación/servicio en cualquier momento y por cualquier motivo sin previo aviso.\n\nContacto\n\nSi tienes alguna pregunta sobre estos términos y condiciones, contáctanos en [ALK.RU@.OUI.GS].\n\nÚltima actualización: 20/04/2024", "ACEPTAR", "CANCELAR");

            if (aceptado)
            {
                // Si el usuario presionó "Aceptar", habilitar el CheckBox
                checkBox.IsEnabled = true;
            }
        }

        /// <summary>
        /// Simula la animación de presionar el botón.
        /// </summary>
        private async Task animaacionButton(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            // Desactivar la interacción
            button.InputTransparent = true;

            // Simular la animación de presionar el botón
            await button.ScaleTo(0.6, 40);
            await button.ScaleTo(1, 50);
            await button.ScaleTo(0.9, 60);
            await button.ScaleTo(1.1, 70);

            // Revertir la desactivación de la interacción
            button.InputTransparent = false;
        }

        #endregion
    }
}
