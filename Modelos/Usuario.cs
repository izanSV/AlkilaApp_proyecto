namespace AlkilaApp.Modelos
{
    /// <summary>
    /// Clase que representa un usuario en el sistema.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        private string? _IdUsuario;
        public string IdUsuario
        {
            get => _IdUsuario;
            set
            {
                _IdUsuario = value;
            }
        }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        private string? _Nombre;
        public string Nombre
        {
            get => _Nombre;
            set
            {
                _Nombre = value;
            }
        }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        private string? _Apellido;
        public string Apellido
        {
            get => _Apellido;
            set
            {
                _Apellido = value;
            }
        }

        /// <summary>
        /// Número de teléfono del usuario.
        /// </summary>
        private string _NumeroTelefono;
        public string NumeroTelefono
        {
            get => _NumeroTelefono;
            set => _NumeroTelefono = value;
        }

        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        private string? _CorreoElectronico;
        public string CorreoElectronico
        {
            get => _CorreoElectronico;
            set
            {
                _CorreoElectronico = value;
            }
        }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        private string _Contrasenya;
        public string Contrasenya
        {
            get => _Contrasenya;
            set
            {
                _Contrasenya = value;
            }
        }

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        private DateTime _FechaNacimiento;
        public DateTime FechaNacimiento
        {
            get => _FechaNacimiento;
            set
            {
                _FechaNacimiento = value;
            }
        }

        /// <summary>
        /// URL de la foto del usuario.
        /// </summary>
        private string? _Foto;
        public string Foto
        {
            get => _Foto;
            set
            {
                _Foto = value;
            }
        }

        /// <summary>
        /// Indica si el usuario es una empresa.
        /// </summary>
        private bool _EsEmpresa;
        public bool EsEmpresa
        {
            get => _EsEmpresa;
            set
            {
                _EsEmpresa = value;
            }
        }

        /// <summary>
        /// Lista de productos asociados al usuario.
        /// </summary>
        private List<Producto>? _ListaProductos;
        public List<Producto> ListaProductos
        {
            get => _ListaProductos;
            set
            {
                _ListaProductos = value;
            }
        }
    }
}
