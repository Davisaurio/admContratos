using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class ReporteFallos
    {
              
        public int IdContratistasActas { get; set; }
        public int idPaquete { get; set; }
        public string NoProcedimiento { get; set; }
        public int Paquete { get; set; }
        public string NumPaquete  { get; set; }
        public string Modalidad { get; set; }
        public string DescObra { get; set; }
        public string Proyecto { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
        public int Orden { get; set; }
        public string Empresa { get; set; }
        public int Plazo { get; set; }
        public decimal Importe { get; set; }
        public DateTime FechadeContratacion { get; set; }
        public DateTime FechaCita { get; set; }
        public string HoraCita { get; set; }
        public string HoraTermino { get; set; }
        public string inv1 { get; set; }
        public string inv1cargo { get; set; }
        public string inv2 { get; set; }
        public string inv2cargo { get; set; }
        public string inv3 { get; set; }
        public string inv3cargo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public string contrato { get; set; }
        public string descObrafallo { get; set; }
        public decimal ImporteAnticipo { get; set; }
        public decimal Importefallo { get; set; }
        public string Oficio { get; set; }
        public decimal ImporteAutorizado { get; set; }
        public DateTime fechaautorizacion { get; set; }
        public string ContratistaGanador { get; set; }
        public string Asistente { get; set; }
        public int ejercicio { get; set; }
        public string MotivoDesechamiento { get; set; }
        public string DesechamientoFallo { get; set; }
        public string MotivoDesechamientoFallo { get; set; }
        public string  Recurso { get; set; }
        public string  Director { get; set; }
        public string  CargoDirecto { get; set; }
        public DateTime FechaContrato  { get; set; }
        public string  HoraContrato { get; set; }
        public DateTime FechaFallo  { get; set; }
        public string HoraFallo { get; set; }
        public decimal AutirizadoProyecto { get; set; }

    }
}