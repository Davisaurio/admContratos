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
    
    public partial class VwFallos
    {
        public int idContratistasActas { get; set; }
        public int idPaquete { get; set; }
        public string NoProcedimiento { get; set; }
        public Nullable<int> Paquete { get; set; }
        public string Modalidad { get; set; }
        public string DescObra { get; set; }
        public string Proyecto { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
        public Nullable<int> Orden { get; set; }
        public string Empresa { get; set; }
        public Nullable<int> Plazo { get; set; }
        public Nullable<decimal> Importe { get; set; }
        public Nullable<System.DateTime> FechadeContratacion { get; set; }
        public Nullable<System.DateTime> FechaCita { get; set; }
        public string HoraCita { get; set; }
        public string HoraTermino { get; set; }
        public string inv1 { get; set; }
        public string inv1cargo { get; set; }
        public string inv2 { get; set; }
        public string inv2cargo { get; set; }
        public string inv3 { get; set; }
        public string inv3cargo { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public Nullable<System.DateTime> FechaTermino { get; set; }
        public string contrato { get; set; }
        public string descObrafallo { get; set; }
        public Nullable<decimal> ImporteAnticipo { get; set; }
        public Nullable<decimal> Importefallo { get; set; }
        public string Oficio { get; set; }
        public Nullable<decimal> ImporteAutorizado { get; set; }
        public Nullable<System.DateTime> fechaautorizacion { get; set; }
        public string ContratistaGanador { get; set; }
        public string Asistente { get; set; }
        public Nullable<int> ejercicio { get; set; }
        public string MotivoDesechamiento { get; set; }
        public string DesechamientoFallo { get; set; }
        public string MotivoDesechamientoFallo { get; set; }
        public string recurso { get; set; }
        public string director { get; set; }
        public string cargodirector { get; set; }
        public Nullable<System.DateTime> fechaContrato { get; set; }
        public string horaContrato { get; set; }
        public string NumPaquete { get; set; }
        public Nullable<System.DateTime> FechaFallo { get; set; }
        public string HoraFallo { get; set; }
        public Nullable<decimal> AutorizadoProyecto { get; set; }
    }
}
