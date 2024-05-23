using System;

namespace AlkilaApp.Modelos
{
    /// <summary>
    /// Esta clase es necesaria para poder establecer a los usuarios una localización para saber donde se úbican.
    /// </summary>
    public class Ubicacion
    {
        // Atributos

        /// <summary>
        /// Localidad de la ubicación.
        /// </summary>
        private string localidad;

        /// <summary>
        /// Latitud de la ubicación.
        /// </summary>
        private double latitud;

        /// <summary>
        /// Longitud de la ubicación.
        /// </summary>
        private double longitud;

        /// <summary>
        /// Calle de la ubicación.
        /// </summary>
        private string calle;

        /// <summary>
        /// Identificador del _usuario asociado a la ubicación.
        /// </summary>
        private string idUsuario;

        // Propiedades

        /// <summary>
        /// Obtiene o establece la localidad de la ubicación.
        /// </summary>
        public string Localidad
        {
            get { return localidad; }
            set { localidad = value; }
        }

        /// <summary>
        /// Obtiene o establece la latitud de la ubicación.
        /// </summary>
        public double Latitud
        {
            get { return latitud; }
            set { latitud = value; }
        }

        /// <summary>
        /// Obtiene o establece la longitud de la ubicación.
        /// </summary>
        public double Longitud
        {
            get { return longitud; }
            set { longitud = value; }
        }

        /// <summary>
        /// Obtiene o establece la calle de la ubicación.
        /// </summary>
        public string Calle
        {
            get { return calle; }
            set { calle = value; }
        }

        /// <summary>
        /// Obtiene o establece el identificador del _usuario asociado a la ubicación.
        /// </summary>
        public string IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; }
        }
    }
}
