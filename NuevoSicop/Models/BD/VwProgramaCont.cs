//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuevoSicop.Models.BD
{
    using System;
    using System.Collections.Generic;
    
    public partial class VwProgramaCont
    {
        public int id { get; set; }
        public Nullable<System.DateTime> Fechaoficio { get; set; }
        public string NoProcedimiento { get; set; }
        public string numpaquete { get; set; }
        public Nullable<System.DateTime> fechaivitacion { get; set; }
        public Nullable<System.DateTime> fechaventabases { get; set; }
        public Nullable<System.DateTime> FechaVisita { get; set; }
        public Nullable<System.DateTime> FechaJunta { get; set; }
        public Nullable<System.DateTime> FechaApertura { get; set; }
        public Nullable<System.DateTime> FechaFallo { get; set; }
        public string horaventabases { get; set; }
        public string Horavisita { get; set; }
        public string HoraJunta { get; set; }
        public string HoraApertura { get; set; }
        public string HoraFallo { get; set; }
        public Nullable<System.DateTime> FechaContrato { get; set; }
        public Nullable<System.DateTime> FechaInicioEstimada { get; set; }
        public string Reviso { get; set; }
        public string AargoRev { get; set; }
        public string Vobo { get; set; }
        public string CargoVobo { get; set; }
        public string Autorizo { get; set; }
        public string CargoAut { get; set; }
        public Nullable<int> PlazodeEjecucion { get; set; }
        public Nullable<int> Anticipo { get; set; }
        public Nullable<decimal> ImporteAutorizado { get; set; }
        public string descTrabajos { get; set; }
        public string localidad { get; set; }
        public string municipio { get; set; }
        public string NOMESC { get; set; }
        public string Recurso { get; set; }
        public Nullable<int> ejercicio { get; set; }
        public int idPaquete { get; set; }
        public string proyecto { get; set; }
    }
}