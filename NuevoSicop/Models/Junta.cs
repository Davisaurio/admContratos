using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class Junta
    {
        public int idcontratistaActas { get; set; }
        public int  NoActa { get; set; }
        public int IdPaquete { get; set; }
        public string Modalidad { get; set; }
        public string NoProcedimiento { get; set; }
        public int Paquete { get; set; }
        public string NumPaquete { get; set; }
        public string DescTrabajos { get; set; }
        public DateTime FechaJunta { get; set; }
        public string HoraJunta { get; set; }
        public string DescripcionObra { get; set; }
        public string Empresa { get; set; }
        public string RepreDependencia { get; set; }
        public string RepreCargo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string HoraTermino { get; set; }
        public string Director { get; set; }
        public string CargoDirector { get; set; }
        public string Invitado1 { get; set; }
        public string CargoInv1 { get; set; }
        public string Nombre { get; set; }
        public string Asistente { get; set; }
        public int No_Preguntas { get; set; }
        public int Ejercicio { get; set; }
        public string Inv3 { get; set; }
        public string CargoInv3 { get; set; }
        
    }
}