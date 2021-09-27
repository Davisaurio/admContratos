using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class ReporteContratoAdq
    {
        public int IdPaquete { get; set; }
        public string Dependencia { get; set; }
        public string TipoContrato { get; set; }
        public string Programa { get; set; }
        public int Ejercicio { get; set; }
        public string Proyecto { get; set; }
        public string Partida { get; set; }
        public string Procedimiento { get; set; }
        public DateTime FechaProcedimiento { get; set; }
        public string Articulo { get; set; }
        public string Contrato { get; set; }
        public DateTime FechaContrato { get; set; }
        public string DescripcionObra { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
        public int PlazoEjecucion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public decimal ImporteContratado { get; set; }
        public decimal ImporteAutorizado { get; set; }
        public decimal AnticipoXcien { get; set; }
        public string AnticipotipoPago { get; set; }
        public string DependeciaAnticipo { get; set; }
        public string UbicacionPagoAnticipo { get; set; }
        public decimal ImporteAnticipo { get; set; }
        public string OficioAutorizaionInv { get; set; }
        public DateTime FechaOficio { get; set; }
        public string Contratista { get; set; }
        public string Domicilio { get; set; }
        public string Correo { get; set; }
        public string CP { get; set; }
        public string RFC { get; set; }
        public string RegistroIMSS { get; set; }
        public string RegistroInfonavit { get; set; }
        public string RegistroPadron { get; set; }
        public string Representante { get; set; }
        public string RFCRepresentante { get; set; }
        public string CargoRepresentante { get; set; }
        public decimal Iva { get; set; }
        public string Director { get; set; }
        public string CargoDirector { get; set; }
        public string DireccionDependencia { get; set; }
        public string RFCDependencia { get; set; }
        public string Recurso { get; set; }
        public string Testigo1 { get; set; }
        public string CargoTestigo1 { get; set; }
        public string Testigo2 { get; set; }
        public string CargoTestigo2 { get; set; }
        public string Testigo3 { get; set; }
        public string CargoTestigo3 { get; set; }
        public string ConAnticipo { get; set; }
        public string FacturaRFCEstimacion { get; set; }
        public string FacturaA { get; set; }
        public string FacturaDireccion { get; set; }
        public string FacturaCiudad { get; set; }
        public string FacturaCP { get; set; }
        public string IFE { get; set; }
        public string TipoPersona { get; set; }
        public string NoEscritura { get; set; }
        public string Volumen { get; set; }
        public DateTime FechaEscritura { get; set; }
        public string NotarioNum { get; set; }
        public string NotariaTitular { get; set; }
        public string NotariaCiudad { get; set; }
        public string RPPCLugar { get; set; }
        public DateTime RPPCFecha { get; set; }
        public string RPPCFolio { get; set; }
        public string TipoInversion { get; set; }
        public string RepEscritura { get; set; }
        public string RepVolumen { get; set; }
        public DateTime RepFechaEscritura { get; set; }
        public string RepNumNotario { get; set; }
        public string RepNotarioTitular { get; set; }
        public string RepCiudadNotario { get; set; }
        public string RepRppcCiudad { get; set; }
        public DateTime RepRppcFecha { get; set; }
        public string RepRppcFolio { get; set; }
        public string RepLibroRPPC { get; set; }
        public string AplicaCMIC { get; set; }
        public string EmpFME { get; set; }
        public string NoInscripRPPCRep { get; set; }
        public string FoliosRPPCRep { get; set; }
        public string LibroRppcRep { get; set; }
        public string FMERep { get; set; }
        public DateTime FechaFallo { get; set; }
        public decimal AutorizadoProyecto { get; set; }
        public string  EntregaBienes { get; set; }
        public string  SupervisaContrato { get; set; }
        public bool FianzaCumplimiento { get; set; }
       


    }
}