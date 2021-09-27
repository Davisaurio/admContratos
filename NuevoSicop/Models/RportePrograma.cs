using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class RportePrograma
    {
        public DateTime FechaPrograma { get; set; }
        public string Licitacion { get; set; }
        public string NumPaquete { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInvitacion { get; set; }
        public DateTime FechaVentaBases { get; set; }
        public DateTime FechaVisita { get; set; }
        public DateTime FechaJunta { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaFallo { get; set; }
        public string HoraVentaBases { get; set; }
        public string HoraVisita { get; set; }
        public string HoraJunta { get; set; }
        public string HoraApertura { get; set; }
        public string HoraFallo { get; set; }

        public DateTime FirmaContrato { get; set; }
        public DateTime InicioEstimado { get; set; }
        public string Proyecto { get; set; }
        public int Periodo { get; set; }
        public decimal Anticipo { get; set; }
        public string Reviso { get; set; }
        public string CargoReviso { get; set; }
        public string Vobo { get; set; }
        public string CargoVobo { get; set; }
        public string Autorizo { get; set; }
        public string Cargoautorizo { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
        public string Modalidad { get; set; }









    }
}