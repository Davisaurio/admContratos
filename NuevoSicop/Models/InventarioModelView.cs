using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AngularCRUD.Models
{
    public class InventarioModelView
    {
        public InventarioModelView()
        {
            Exito = "";
            Error = "";
            FiltroFInicio = DateTime.Now.AddDays(-7);
            FiltroFFinal = DateTime.Now;
            ListaBienes = new List<Bien>();
            NuevoBien = new Bien();
            ListaDireciones = new SelectList(new List<string> { "Seleccione la Dirección" });
            ListaEnagenacion = new SelectList(new List<string> { "Seleccione tipo enagenación" });
            ListaEstados = new SelectList(new List<string> { "Seleccione El Estado del bien" });
            ListaPersonal = new SelectList(new List<string> { "Seleccione la asignación" });
            ListaUbicaciones = new SelectList(new List<string> { "Seleccione la Ubicación" });

        }

        //Mensajes 

        public string Exito { get; set; }
        public string Error { get; set; }

        //Filtros
        [DisplayName("PERSONAL")]
        public int FiltroPeronal { get; set; }
        [DisplayName("DIRECCIÓN")]
        public int FiltroDireccion { get; set; }
        [DisplayName("MARCA")]
        public string FiltroMarca { get; set; }
        [DisplayName("ENAGENACION")]
        public int FiltroEnajenacion { get; set; }
        [DisplayName("DESDE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FiltroFInicio { get; set; }




        [DisplayName("HASTA ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FiltroFFinal { get; set; }


        public string FiltroNombre { get; set; }
        //Listas 
        [DisplayName("ESTADOS")]

        public SelectList ListaEstados { get; set; }
        [DisplayName("UBICACIÓN")]
        public SelectList ListaUbicaciones { get; set; }
        [DisplayName("DIRECCIÓN")]
        public SelectList ListaDireciones { get; set; }
        [DisplayName("PERSONAL")]
        public SelectList ListaPersonal { get; set; }
        [DisplayName("ENAGENACIÓN")]
        public SelectList ListaEnagenacion { get; set; }
        //Propiedades 
        [DisplayName("BUSCA NOMBRE")]
        public string TraeNombres { get; set; }
        [DisplayName("BIENES")]
        public List<Bien> ListaBienes { get; set; }
        [DisplayName("BIEN")]
        public Bien NuevoBien { get; set; }
        [DisplayName("EDITAR  BIEN")]
        public int EditarBien { get; set; }
    }
    public class Bien
    {
        public Bien()
        {
            FechaFactura = DateTime.Now;
            FechaRegistro = DateTime.Now;
        }
        [DisplayName("ID")]
        [Required]
        public int Id { get; set; }
        [DisplayName("CLAVE")]
        [Required]
        public string Clave { get; set; }
        [DisplayName("NOMBRE")]
        [Required]
        public string Nombre { get; set; }
        [DisplayName("MARCA")]
        public string Marca { get; set; }
        [DisplayName("TIPO")]
        public string Tipo { get; set; }
        [DisplayName("SERIE")]
        public string Serie { get; set; }
        [DisplayName("MODELO")]
        public string Modelo { get; set; }
        [DisplayName("ESTADO")]
        [Required]
        public int IdEstado { get; set; }
        [DisplayName("ESTADO")]
        public string Estado { get; set; }
        [DisplayName("UBICACION")]
        [Required]
        public int IdUbicacion { get; set; }
        [DisplayName("UBICACION")]
        public string Ubicacion { get; set; }

        [DisplayName("DIRECCIÓN")]
        [Required]
        public int IdDireccion { get; set; }

        [DisplayName("DIRECCIÓN")]
        public string Direccion { get; set; }
        [DisplayName("OBSERVACIONES")]
        public string Observaciones { get; set; }
        [DisplayName("FECHA FACTURA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFactura { get; set; }
        [DisplayName("VALOR FACTURA")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Valorfactura { get; set; }
        [DisplayName("VALOR ACTUAL")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ValorActual { get; set; }
        [DisplayName("PERSONAL")]
        [Required]
        public int IdPresonal { get; set; }
        [DisplayName("PERSONAL")]
        [Required]
        public string Asignado { get; set; }
        [DisplayName("TIPO ENAGENACIÓN")]
        [Required]
        public int IdTipoEnagenacion { get; set; }
        [DisplayName("TIPO ENAGENACIÓN")]
        public string TipoEnagenacion { get; set; }
        [DisplayName("CARACTERÍSTICAS")]
        public string Caracteristicas { get; set; }
        [DisplayName("FECHA REGISTRO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaRegistro { get; set; }


    }


}
