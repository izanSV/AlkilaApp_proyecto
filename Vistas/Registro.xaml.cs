using AlkilaApp.Animaciones;
using AlkilaApp.Modelos;
using AlkilaApp.Servicios;
using AlkilaApp.Vistas;
using Firebase.Auth;


namespace AlkilaApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registro : ContentPage
    {
        #region Atributos
        private ServicioUsuario _Servicio = new ServicioUsuario();
        private Usuario? _nuevoUsuario;
        private string? _Contrasenya;
        private string? _RepContrasenya;
        private string? _Nombre;
        private string? _Apellido;
        private string? _telefono;
        private DateTime _FechaNacimiento;
        private string? _Foto;

        #endregion


        #region Constructor
        public Registro()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        #endregion


        #region Metodos

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string? Nombre
        {
            get => _Nombre;
            set => _Nombre = value;
        }

        /// <summary>
        /// Teléfono del usuario.
        /// </summary>
        public string? Telefono
        {
            get => _telefono;
            set => _telefono = value;
        }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string? Apellido
        {
            get => _Apellido;
            set => _Apellido = value;
        }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        public string? Contrasenya
        {
            get => _Contrasenya;
            set => _Contrasenya = value;
        }

        /// <summary>
        /// Repetición de la contraseña del usuario.
        /// </summary>
        public string? RepContrasenya
        {
            get => _RepContrasenya;
            set => _RepContrasenya = value;
        }

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        public DateTime FechaNacimiento
        {
            get => _FechaNacimiento;
            set => _FechaNacimiento = value;
        }

        /// <summary>
        /// URL de la foto de perfil del usuario.
        /// </summary>
        public string? Foto
        {
            get => _Foto;
            set => _Foto = value;
        }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Indica si el usuario es un empresario.
        /// </summary>
        private bool UsuarioEmpresario { get; set; }




        /// <summary>
        /// Realiza el registro del usuario en la aplicación.
        /// </summary>
        public async Task RegistroUsuario()
        {
            try
            {
                UsuarioEmpresario = false;

                // Si el usuario introduce un código correcto (NIF), entenderemos que es una empresa, por el contrario es usuario normal
                bool esEmpresario = VentanaModalEmpresa.EsEmpresarioElUsuario();

                UsuarioEmpresario = esEmpresario;

                // Recogemos los datos escritos por el usuario
                EntradaDatos();

                // Realiza todas las comprobaciones necesarias antes de registrar al usuario
                bool condicionesCumplidas = ComprobarCondiciones();

                if (condicionesCumplidas)
                {
                    // Si todas las condiciones son satisfactorias, procede con el registro del usuario

                    _nuevoUsuario = new Usuario
                    {
                        Nombre = Nombre,
                        Apellido = Apellido,
                        CorreoElectronico = CorreoElectronico,
                        Contrasenya = Contrasenya,
                        FechaNacimiento = FechaNacimiento,
                        Foto = Setting.FotoPredeterminada,
                        EsEmpresa = UsuarioEmpresario,
                        NumeroTelefono = Telefono,
                        ListaProductos = new List<Producto>()
                    };

                        // Realiza el registro del usuario
                        string respuestaServicio = await _Servicio.RegistroUsuariosAsync(_nuevoUsuario);
                        
                    
                    // Si no se ha registrado correctamente, enviara un alert con un error, aprovechamos la validación de firebase del correo electrónico
                    if (!respuestaServicio.Equals("OK"))
                    {
                        await DisplayAlert("Error", respuestaServicio, "ACEPTAR");
                        return;
                    }
                   

                        // Añade o actualiza el usuario en el servicio
                        string respuesta = await _Servicio.AnyadirOActualizarUsuario(_nuevoUsuario);


                    if (_nuevoUsuario.IdUsuario != null)
                    {
                        // Aquí mostrara como que el usuario se ha registrado correctamente por que ha obtendio el ID
                        await DisplayAlert("", respuesta, "ACEPTAR");
                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "ACEPTAR");
            }
        }


        /// <summary>
        /// Verifica la entrada del usuario, en el caso de que sea null, lanza un mensaje.
        /// </summary>
        public void EntradaDatos()
            {
                if (NombreEntry != null)
                    Nombre = NombreEntry.Text;
                else
                    Nombre = string.Empty;

                if (ApellidoEntry != null)
                    Apellido = ApellidoEntry.Text;
                else
                    Apellido = string.Empty;

                if (CorreoElectronicoEntry != null)
                    CorreoElectronico = CorreoElectronicoEntry.Text;
                else
                    CorreoElectronico = string.Empty;

                if (ContrasenyaEntry != null)
                    Contrasenya = ContrasenyaEntry.Text;
                else
                    Contrasenya = string.Empty;

                if (RepContrasenyaEntry != null)
                    RepContrasenya = RepContrasenyaEntry.Text;
                else
                    RepContrasenya = string.Empty;

                FechaNacimiento = DateEntry.Date; // No puede ser nulo, ya que es un calendario

                if (TelefonoEntry != null)
                    Telefono = TelefonoEntry.Text;
                else
                    Telefono = string.Empty;

                // Verifica si alguno de los campos obligatorios está vacío
                if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) ||
                    string.IsNullOrWhiteSpace(CorreoElectronico) || string.IsNullOrWhiteSpace(Contrasenya) ||
                    string.IsNullOrWhiteSpace(RepContrasenya) || string.IsNullOrWhiteSpace(Telefono));
            }



        /// <summary>
        /// Valida que los camos no estén vacios, ademas de la contraseña es la misma que la de repetición, 
        /// Valida la condición de la contraseña
        /// </summary>
        public bool ComprobarCondiciones()
        {
         
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido) || string.IsNullOrWhiteSpace(CorreoElectronico) ||
                string.IsNullOrWhiteSpace(Contrasenya) || string.IsNullOrWhiteSpace(RepContrasenya) || string.IsNullOrWhiteSpace(Telefono))
            {
                // Muestra un mensaje de error si algún campo está vacío
              
                DisplayAlert("Error", "Por favor, no puedes dejar campos vacíos", "ACEPTAR");
              
                return false;
            }

            // la longitud del telefono tiene que ser de 9
            if (Telefono.Length != 9)
            {
                DisplayAlert("Error", "Comprobar longitud del numero de telefono", "ACEPTAR");
                return false;
            }
            // comprobamos el formato de la contraseña y del campo repetir contraseña

            if (!ValidarLongitudContrasenya() || !ValidarCoincidenciaContrasenyas())
            {
                return false;
            }else
            {
                // si se cumple la validación ha sido aceptada
                return true;
            }


        }


       
        /// <summary>
        /// Metodo para validar la contraseña del usuario el usuario
        /// </summary>

        public bool ValidarLongitudContrasenya()
        {
            if (Contrasenya.Length < 6)
            {
                 DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "ACEPTAR");
                return false;
            }
            return true;
        }




        /// <summary>
        /// Metodo para validar que es la misma contraseña
        /// </summary>

        public bool ValidarCoincidenciaContrasenyas()
        {
            if (Contrasenya != RepContrasenya)
            {
               DisplayAlert("Error", "Las contraseñas no coinciden", "ACEPTAR"); 
                return false;
            }
            return true;
        }

        #endregion


        #region Botones

        /// <summary>
        /// Maneja el evento de clic en el botón de registro de usuario.
        /// </summary>
        /// <param name="sender">Objeto que desencadenó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private async void RegistroUsuarioClicked(object sender, EventArgs e)
        {
            // Realiza una animación en el botón de registro
            using (new Botones(idRegistroUsuario))
            {
                try
                {
                    // Realiza la animación del botón
                    await Botones.animaacionButton(sender, e);
                    // Llama al método de registro de usuario
                    await RegistroUsuario();
                }
                catch (Exception ex)
                {
                    // Maneja cualquier excepción que pueda ocurrir durante el registro
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Maneja el evento de clic en el botón para empresas.
        /// </summary>
        /// <param name="sender">Objeto que desencadenó el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private async void EmpresaClicked(object sender, EventArgs e)
        {
            // Realiza una animación en el botón de empresa
            using (new Botones(idEmpresaClicked))
            {
                try
                {
                    // Realiza la animación del botón
                    await Botones.animaacionButton(sender, e);
                    // Abre una ventana modal para empresas
                    var miVentanaModal = new VentanaModalEmpresa();
                    await Navigation.PushModalAsync(miVentanaModal);
                }
                catch (Exception ex)
                {
                    // Maneja cualquier excepción que pueda ocurrir al abrir la ventana modal
                    Console.WriteLine(ex);
                }
            }
        }

        #endregion


    }
}
