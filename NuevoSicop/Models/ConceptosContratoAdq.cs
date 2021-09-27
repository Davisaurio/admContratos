using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class ConceptosContratoAdq
    {
        public string  Proyecto { get; set; }
        public string  Partida  { get; set; }
        public string PartidaPresupuestal { get; set; }
        public string ClaveArt { get; set; }
        public string  Descripcion { get; set; }
        public int  Cantidad { get; set; }
        public decimal  PrecioUnidad { get; set; }
        public decimal  Importe { get; set; }
        public decimal IVA { get; set; }
        public decimal  ImporteTotal { get; set; }

    }
}