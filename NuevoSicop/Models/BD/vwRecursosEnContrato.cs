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
    
    public partial class vwRecursosEnContrato
    {
        public int ID { get; set; }
        public string Contrato { get; set; }
        public int idproyecto { get; set; }
        public string ClaveProyecto { get; set; }
        public int IdOficio { get; set; }
        public string Oficio { get; set; }
        public int IdRecurso { get; set; }
        public string recurso { get; set; }
        public string Alcance { get; set; }
        public Nullable<decimal> AutorizadoOficio { get; set; }
        public Nullable<decimal> AutorizadoProyecto { get; set; }
        public Nullable<System.DateTime> fechaoficio { get; set; }
        public Nullable<double> importeProyecto { get; set; }
    }
}
