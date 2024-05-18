using System;

namespace AlkilaApp.Modelos
{
    /// <summary>
    /// Clase que representa un producto en el sistema.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        private string? _IdProducto;
        public string IdProducto
        {
            get => _IdProducto;
            set
            {
                _IdProducto = value;
            }
        }

        /// <summary>
        /// Nombre del producto.
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
        /// Descripción del producto.
        /// </summary>
        private string? _DescripcionProducto;
        public string DescripcionProducto
        {
            get => _DescripcionProducto;
            set
            {
                _DescripcionProducto = value;
            }
        }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        private double _Precio;
        public double Precio
        {
            get => _Precio;
            set
            {
                _Precio = value;
            }
        }

        /// <summary>
        /// URL de la foto del producto.
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
        /// Indica si el producto está actualmente alquilado.
        /// </summary>
        private bool _estaAlquilado;
        public bool EstaAlquilado
        {
            get => _estaAlquilado;
            set
            {
                _estaAlquilado = value;
            }
        }

        /// <summary>
        /// Tipo de producto.
        /// </summary>
        public TipoProducto Tipo { get; set; }

        /// <summary>
        /// Valoración del producto.
        /// </summary>
        private double? _Valoracion;
        public double? Valoracion
        {
            get => _Valoracion;
            set
            {
                _Valoracion = value;
            }
        }

        /// <summary>
        /// Total de valoraciones para hacer la media.
        /// </summary>
        private double? _TotalValoracionProductos;
        public double? TotalValoracionProductos
        {
            get => _TotalValoracionProductos;
            set
            {
                _TotalValoracionProductos = value;
            }
        }

        /// <summary>
        /// Total de usuarios que han valorado el producto.
        /// </summary>
        private int? _TotalUsuariosValorados;
        public int? TotalUsuariosValorados
        {
            get => _TotalUsuariosValorados;
            set
            {
                _TotalUsuariosValorados = value;
            }
        }
    }
}

/// <summary>
/// Enumeración que representa  las categorias de los tipos de productos disponibles.
/// </summary>
public enum TipoProducto
{
    /// <summary>
    /// Producto de entretenimiento.
    /// </summary>
    Entretenimiento = 0,

    /// <summary>
    /// Producto de moda.
    /// </summary>
    Moda = 1,

    /// <summary>
    /// Producto deportivo.
    /// </summary>
    Deporte = 2,

    /// <summary>
    /// Herramientas.
    /// </summary>
    Herramientas = 3,

    /// <summary>
    /// Eventos.
    /// </summary>
    Eventos = 4

}
