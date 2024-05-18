using System;
using System.Collections.Generic;

namespace AlkilaApp.Modelos
{
    /// <summary>
    /// Clase que representa un alquiler.
    /// </summary>
    public class Alquiler
    {
        private DateTime _FechaInicio; // Campo privado para almacenar la fecha de inicio del alquiler.

        /// <summary>
        /// Fecha de inicio del alquiler.
        /// </summary>
        public DateTime FechaInicio
        {
            get => _FechaInicio;
            set
            {
                _FechaInicio = value; // Se asigna el valor ingresado a la fecha de inicio.
            }
        }

        private DateTime _FechaFin; // Campo privado para almacenar la fecha de fin del alquiler.

        /// <summary>
        /// Fecha de fin del alquiler.
        /// </summary>
        public DateTime FechaFin
        {
            get => _FechaFin;
            set
            {
                _FechaFin = value; // Se asigna el valor ingresado a la fecha de fin.
            }
        }

        private string _IdProducto; // Campo privado para almacenar el ID del producto alquilado.

        /// <summary>
        /// ID del producto alquilado.
        /// </summary>
        public string IdProducto
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value; // Se asigna el valor ingresado al ID del producto alquilado.
            }
        }

        private string _IdAlquiler; // Campo privado para almacenar el ID del alquiler.

        /// <summary>
        /// ID del alquiler.
        /// </summary>
        public string IdAlquiler
        {
            get => _IdAlquiler;
            set
            {
                _IdAlquiler = value; // Se asigna el valor ingresado al ID del alquiler.
            }
        }

        private string _IdUsuarioComprador; // Campo privado para almacenar el ID del usuario comprador.

        /// <summary>
        /// ID del usuario comprador.
        /// </summary>
        public string IdUsuarioComprador
        {
            get => _IdUsuarioComprador;
            set
            {
                _IdUsuarioComprador = value; // Se asigna el valor ingresado al ID del usuario comprador.
            }
        }

        private string _IdUsuarioVendedor; // Campo privado para almacenar el ID del usuario vendedor.

        /// <summary>
        /// ID del usuario vendedor.
        /// </summary>
        public string IdUsuarioVendedor
        {
            get => _IdUsuarioVendedor;
            set
            {
                _IdUsuarioVendedor = value; // Se asigna el valor ingresado al ID del usuario vendedor.
            }
        }

        private string _NombreProductoAlquilado; // Campo privado para almacenar el nombre del producto alquilado.

        /// <summary>
        /// Nombre del producto alquilado.
        /// </summary>
        public string NombreProductoAlquilado
        {
            get => _NombreProductoAlquilado;
            set
            {
                _NombreProductoAlquilado = value; // Se asigna el valor ingresado al nombre del producto alquilado.
            }
        }

        private string _FotoProductoAlquilado; // Campo privado para almacenar la foto del producto alquilado.

        /// <summary>
        /// Foto del producto alquilado.
        /// </summary>
        public string FotoProductoAlquilado
        {
            get => _FotoProductoAlquilado;
            set
            {
                _FotoProductoAlquilado = value; // Se asigna el valor ingresado a la foto del producto alquilado.
            }
        }

        private double _PrecioTotal; // Campo privado para almacenar el precio total del alquiler.

        /// <summary>
        /// Precio total del alquiler.
        /// </summary>
        public double PrecioTotal
        {
            get => _PrecioTotal;
            set
            {
                _PrecioTotal = value; // Se asigna el valor ingresado al precio total del alquiler.
            }
        }

        private string _NombreUsuarioComprador; // Campo privado para almacenar el nombre del usuario comprador.

        /// <summary>
        /// Nombre del usuario comprador.
        /// </summary>
        public string NombreUsuarioComprador
        {
            get => _NombreUsuarioComprador;
            set
            {
                _NombreUsuarioComprador = value; // Se asigna el valor ingresado al nombre del usuario comprador.
            }
        }

        /// <summary>
        /// Estado del alquiler.
        /// </summary>
        public Estado EstadoAlquiler { get; set; }
    }

    /// <summary>
    /// Enumeración que representa el estado del alquiler.
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
