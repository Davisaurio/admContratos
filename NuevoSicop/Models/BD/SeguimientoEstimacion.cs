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
    
    public partial class SeguimientoEstimacion
    {
        public int ID { get; set; }
        public Nullable<int> NoEstimacion { get; set; }
        public string CveTipoEstimacion { get; set; }
        public Nullable<decimal> ImporteOriginal { get; set; }
        public Nullable<decimal> AnticipoAmortizado { get; set; }
        public Nullable<decimal> AnticipoConsiderar { get; set; }
        public Nullable<int> PorcentajeAnti { get; set; }
        public string Observaciones { get; set; }
        public string NoContrato { get; set; }
        public Nullable<System.DateTime> FechaEntradaVentanilla { get; set; }
        public Nullable<decimal> ImporteRetenido { get; set; }
        public Nullable<decimal> Devolucion { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> FechaEnvioSupervision { get; set; }
        public Nullable<System.DateTime> FechaDevolucion { get; set; }
        public Nullable<System.DateTime> FechaAutorizaResidente { get; set; }
        public string Memo { get; set; }
        public Nullable<System.DateTime> FechaEnvioTecnica { get; set; }
        public Nullable<System.DateTime> FechaCreacionMemo { get; set; }
        public Nullable<System.DateTime> FechaEnvioCP { get; set; }
        public Nullable<System.DateTime> FechaApruebaCP { get; set; }
        public Nullable<decimal> ImporteSinIVA { get; set; }
        public Nullable<decimal> ImporteIVA { get; set; }
        public Nullable<int> PorcentajeIVA { get; set; }
        public Nullable<System.DateTime> FechaCapturaCP { get; set; }
        public string Bis { get; set; }
        public string NoProyecto { get; set; }
    }
}
