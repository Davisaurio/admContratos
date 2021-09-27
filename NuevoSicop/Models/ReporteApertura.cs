using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class ReporteApertura
    {
            public int Id { get; set; }
            public string Modalidad { get; set; }
            public string NoProcedimiento { get; set; }
            public string NumPaquete { get; set; }
            public string DescObra { get; set; }
            public DateTime Fechaapertura { get; set; }
            public string Horaapertura { get; set; }
            public string Lugar { get; set; }
            public string Director { get; set; }
            public string CargoDirector { get; set; }
            public int IdContratistasActas { get; set; }
            public int IdPaquete { get; set; }
            public int  Paquete { get; set; }
            public string Proyecto { get; set; }
            public string Localidad { get; set; }
            public string Municipio { get; set; }
            public int Orden { get; set; }
            public string Empresa { get; set; }
            public int  Plazo { get; set; }
            public decimal  Importe { get; set; }
            public string Inv1 { get; set; }
            public string Inv1Cargo { get; set; }
            public string Inv2 { get; set; }
            public string Inv2Cargo { get; set; }
            public string Oficio { get; set; }
            public decimal ImporteAutorizado { get; set; }
            public DateTime Fechaautorizacion { get; set; }
            public string Asistente { get; set; }
            public int  Ejercicio { get; set; }
            public string MotivoDesechamiento { get; set; }
            public string DesechamientoFallo { get; set; }
            public string MotivoDesechamientoFallo { get; set; }
            public string Recurso { get; set; }
            public DateTime FechaContrato { get; set; }
            public string HoraContrato { get; set; }
            public DateTime FechaFallo { get; set; }
            public string HoraFallo { get; set; }
            public decimal AutorizadoProyecto { get; set; }
            public int IdactaApertura { get; set; }
            public string Invitado3 { get; set; }
            public string CargoInv3 { get; set; }
            public string hrTerminaApertura  { get; set; }
    
}
}