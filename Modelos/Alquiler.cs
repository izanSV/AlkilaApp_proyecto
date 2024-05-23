using System;
using System.Collections.Generic;

namespace AlkilaApp.Modelos
{
    /// <summary>
    /// Clase que representa un _alquiler.
    /// </summary>
    public class Alquiler
    {
        private DateTime _FechaInicio; // Campo privado para almacenar la fecha de inicio del _alquiler.

        /// <summary>
        /// Fecha de inicio del _alquiler.
        /// </summary>
        public DateTime FechaInicio
        {
            get => _FechaInicio;
            set
            {
                _FechaInicio = value; // Se asigna el valor ingresado a la fecha de inicio.
            }
        }

        private DateTime _FechaFin; // Campo privado para almacenar la fecha de fin del _alquiler.

        /// <summary>
        /// Fecha de fin del _alquiler.
        /// </summary>
        public DateTime FechaFin
        {
            get => _FechaFin;
            set
            {
                _FechaFin = value; // Se asigna el valor ingresado a la fecha de fin.
            }
        }

        private string _IdProducto; // Campo privado para almacenar el ID del _producto alquilado.

        /// <summary>
        /// ID del _producto alquilado.
        /// </summary>
        public string IdProducto
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value; // Se asigna el valor ingresado al ID del _producto alquilado.
            }
        }

        private string _IdAlquiler; // Campo privado para almacenar el ID del _alquiler.

        /// <summary>
        /// ID del _alquiler.
        /// </summary>
        public string IdAlquiler
        {
            get => _IdAlquiler;
            set
            {
                _IdAlquiler = value; // Se asigna el valor ingresado al ID del _alquiler.
            }
        }

        private string _IdUsuarioComprador; // Campo privado para almacenar el ID del _usuario comprador.

        /// <summary>
        /// ID del _usuario comprador.
        /// </summary>
        public string IdUsuarioComprador
        {
            get => _IdUsuarioComprador;
            set
            {
                _IdUsuarioComprador = value; // Se asigna el valor ingresado al ID del _usuario comprador.
            }
        }

        private string _IdUsuarioVendedor; // Campo privado para almacenar el ID del _usuario vendedor.

        /// <summary>
        /// ID del _usuario vendedor.
        /// </summary>
        public string IdUsuarioVendedor
        {
            get => _IdUsuarioVendedor;
            set
            {
                _IdUsuarioVendedor = value; // Se asigna el valor ingresado al ID del _usuario vendedor.
            }
        }

        private string _NombreProductoAlquilado; // Campo privado para almacenar el nombre del _producto alquilado.

        /// <summary>
        /// Nombre del _producto alquilado.
        /// </summary>
        public string NombreProductoAlquilado
        {
            get => _NombreProductoAlquilado;
            set
            {
                _NombreProductoAlquilado = value; // Se asigna el valor ingresado al nombre del _producto alquilado.
            }
        }

        private string _FotoProductoAlquilado; // Campo privado para almacenar la foto del _producto alquilado.

        /// <summary>
        /// Foto del _producto alquilado.
        /// </summary>
        public string FotoProductoAlquilado
        {
            get => _FotoProductoAlquilado;
            set
            {
                _FotoProductoAlquilado = value; // Se asigna el valor ingresado a la foto del _producto alquilado.
            }
        }

        private double _PrecioTotal; // Campo privado para almacenar el precio total del _alquiler.

        /// <summary>
        /// Precio total del _alquiler.
        /// </summary>
        public double PrecioTotal
        {
            get => _PrecioTotal;
            set
            {
                _PrecioTotal = value; // Se asigna el valor ingresado al precio total del _alquiler.
            }
        }

        private string _NombreUsuarioComprador; // Campo privado para almacenar el nombre del _usuario comprador.

        /// <summary>
        /// Nombre del _usuario comprador.
        /// </summary>
        public string NombreUsuarioComprador
        {
            get => _NombreUsuarioComprador;
            set
            {
                _NombreUsuarioComprador = value; // Se asigna el valor ingresado al nombre del _usuario comprador.
            }
        }

        /// <summary>
        /// Estado del _alquiler.
        /// </summary>
        public Estado EstadoAlquiler { get; set; }


        /// <summary>
        /// Esta variable es para no tener que mostrar siempre los mensajes de alerta
        /// // cambiaran el estado y no se volveran a mostrar
        /// </summary>
        private bool _noMasRespuesta;
        public bool NoMasRespuesta
        {
            get => _noMasRespuesta;
            set
            {
                _noMasRespuesta = value;
            }
        }
    }


    /// <summary>
    /// Enumeración que representa el estado del _alquiler.
    /// </summary>
    public enum Estado
    {
        Pendiente = 0,
        Aceptado = 1,
        Cancelado = 2,
        Finalizado = 3,
        Enviado = 4,
        Recibido = 5
    }
}
