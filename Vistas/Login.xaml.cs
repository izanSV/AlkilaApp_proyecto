using AlkilaApp.Animaciones;
using AlkilaApp.Servicios.Implementacion;

namespace AlkilaApp
{
    public partial class Login : ContentPage
    {
        #region Atributos

        /// <summary>
        /// Servicio de usuario
        /// </summary>
        private ServicioUsuario _Servicio;

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        private string _correoElectronico;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        private string _contrasenya;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de la clase Login
        /// </summary>
        public Login()
        {
            InitializeComponent();
            _Servicio = new ServicioUsuario();
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        public string CorreoElectronico
        {
            get => _correoElectronico;
            set => _correoElectronico = value;
        }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public string Contrasenya
        {
            get => _contrasenya;
            set => _contrasenya = value;
        }

        /// <summary>
        /// Método para almacenar los datos de entrada del usuario
        /// </summary>
        public async void EntradaDatos()
        {
            CorreoElectronico = CorreoElectronicoEntry.Text;
            Contrasenya = ContrasenyaEntry.Text;
        }

        /// <summary>
        /// Verifica que los campos de correo electrónico y contraseña no estén vacíos
        /// </summary>
        /// <returns>Verdadero si ambos campos tienen datos, falso en caso contrario</returns>
        private bool CamposNoVacios()
        {
            EntradaDatos();

            if (string.IsNullOrWhiteSpace(CorreoElectronico) || string.IsNullOrWhiteSpace(Contrasenya))
            {
                DisplayAlert("Error", "No puede haber campos vacíos", "ACEPTAR");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Método para validar el usuario
        /// </summary>
        public async Task ValidarUsuario()
        {
            try
            {
                string respuesta = "";
                if (CamposNoVacios())
                {
                    respuesta = await _Servicio.ValidarUsuariosAsync(CorreoElectronico, Contrasenya);

                    string id = _Servicio.IdUsuario;

                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        await DisplayAlert("Confirmación ツ", "El usuario se ha logeado correctamente", "ACEPTAR");
                        await Navigation.PushAsync(new VistaProductos(id));
                    }
                    else
                    {
                        await DisplayAlert("Error", "Usuario o contraseña incorrecto", "ACEPTAR");
                    }
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "No hemos podido validar el usuario", "ACEPTAR");
            }
        }

        #endregion

        #region Botones 

        /// <summary>
        /// Evento al hacer clic en el botón de login
        /// </summary>
        private async void LoginClicked(object sender, EventArgs e)
        {
            using (new Botones(idLoginClicked))
            {
                try
                {
                    await Botones.animaacionButton(sender, e);
                    await ValidarUsuario();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Evento al hacer clic en el botón de registro
        /// </summary>
        private async void RegistroClicked(object sender, EventArgs e)
        {
            using (new Botones(idRegistroClicked))
            {
                try
                {
                    await Botones.animaacionButton(sender, e);
                    await Navigation.PushAsync(new Registro());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Sobrescribe el comportamiento del botón de retroceso del dispositivo
        /// </summary>
        /// <returns>Siempre retorna true para sobrescribir el comportamiento predeterminado</returns>
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
    }
}
