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
    
    public partial class ContratistasActas
    {
        public int idContratistasActas { get; set; }
        public Nullable<int> idContratista { get; set; }
        public string Asistente { get; set; }
        public Nullable<int> No_Preguntas { get; set; }
        public Nullable<decimal> Importe { get; set; }
        public string Motivo { get; set; }
        public string AsistioVisitaSitio { get; set; }
        public string Bases { get; set; }
        public string Desechamiento { get; set; }
        public string AsistioPresentacionApertura { get; set; }
        public string MotivoDesechamiento { get; set; }
        public Nullable<int> Plazo { get; set; }
        public string DesechamientoFallo { get; set; }
        public string MotivoDesechamientoFallo { get; set; }
        public Nullable<bool> FirmaFallo { get; set; }
        public Nullable<bool> FirmaApertura { get; set; }
        public Nullable<int> Orden { get; set; }
        public Nullable<int> IdPaquete { get; set; }
    }
}
