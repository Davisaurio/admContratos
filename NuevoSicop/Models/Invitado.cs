using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuevoSicop.Models
{
    public class ReporteInvitado
    {
        public int Id { get; set; }

        public int idPaquete { get; set; }
        public string Oficio { get; set; }
        public int Clave { get; set; }
        public string Nombre { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaOficio { get; set; }
        public string TipoPersona { get; set; }
        public string Padron { get; set; }
        public string REPLEGAL { get; set; }
        public string PUESTO { get; set; }
        public string modalidad { get; set; }
        public string Articulo { get; set; }
        public string NoProcedimiento { get; set; }
        public string paquete { get; set; }
        public DateTime fchVisitaSitio { get; set; }
        public string hrVisitaSitio { get; set; }
        public DateTime fchJuntaAclaracion { get; set; }
        public string hrJuntaAclaracion { get; set; }
        public DateTime fchAperturaTecnicaEconomica { get; set; }
        public string hrAperturaTecnicaEconomica { get; set; }
        public DateTime FechaInicioEstimada { get; set; }
        public int  Anticipo { get; set; }
        public string descobra { get; set; }
        public string localidad { get; set; }
        public string municipio { get; set; }
        public int  PlazoEjecucion { get; set; }
        public string Director { get; set; }
        public string Direccion { get; set; }
        public string Cargo { get; set; }
        public string firmanombre { get; set; }
        public string firmacargo { get; set; }
        public string direccioncargo { get; set; }
        public string txtVoBo { get; set; }
        public int ejercicio { get; set; }
        public int NumPaquete { get; set; }
        public DateTime fchLimiteInscripcion { get; set; }
        public string hrLimiteInscripcion { get; set; }
        public decimal  CapitalContable { get; set; }
        public string ExpAcred { get; set; }
        public decimal Recuperacion { get; set; }
        public string  Proyecto { get; set; }



        //public int IdPaquete { get; set; }
        //public string Oficio { get; set; }
        //public int Clave { get; set; }
        //public string Nombre { get; set; }
        //public string Domicilio { get; set; }
        //public string Telefono { get; set; }
        //public DateTime FechaOficio { get; set; }
        //public string TipoPersona { get; set; }
        //public string RepresentanteLegal { get; set; }
        //public string Puesto { get; set; }
        //public string Modalidad { get; set; }
        //public string Articulo { get; set; }
        //public string NoProcedimiento { get; set; }
        //public string Paquete { get; set; }
        //public DateTime FechaVisitaSitio { get; set; }
        //public string HoraVisitaSitio { get; set; }
        //public DateTime FechaJuntaAclaraciones { get; set; }
        //public string HoraJuntaAclaraciones { get; set; }
        //public DateTime FechaAperturaTecnica { get; set; }
        //public string HoraAperturaTecnica { get; set; }
        //public DateTime InicioEstimada { get; set; }
        //public int Anticipo { get; set; }
        //public string DescripcionObra { get; set; }
        //public string Localidad { get; set; }
        //public string Municipio { get; set; }
        //public int PlazoEjecucion { get; set; }
        //public string Director { get; set; }
        //public string Direccion { get; set; }
        //public string FirmaNombre { get; set; }
        //public string FirmaCargo { get; set; }
        //public string DireccionCargo { get; set; }
        //public string TxtVobo { get; set; }
        //public int Ejercicio { get; set; }
        //public int NumPaquete { get; set; }
    }
}