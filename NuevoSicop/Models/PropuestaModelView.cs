using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Models
{
    public class PropuestaModelView
    {

        public PropuestaModelView()
        {
            Exito = "";
            Error = "";
            ListaEjercicios = new SelectList(new List<string>() { "Seleccione el Ejercicio" });
            ListaPaquetes = new SelectList(new List<string>() { "Seleccione el Paquete" });
            ListaPersonal = new SelectList(new List<string>() { "¿Quien conduce el evento?" });
            ListaSupervisores = new SelectList(new List<string>() { "¿Supervisor?" });
            ListaServExternos = new SelectList(new List<string>() { "Servidor Publico Externo" });
            ListaEmpresas = new SelectList(new List<string>() { "Seleccionar Empresa" });
            ListaProy = new SelectList(new List<string>() { "Seleccionar Proyectos" });

            ListaProyectos = new List<ProyInv>();
            ListaLicitantes = new List<LicitantesApertura>();
            ListaFirmaran = new List<Firmaran>();
            ListaRubricas = new List<ActaRubrica>();
            ListaInvitadosExternos = new List<InvitadoExterno>();
            ListaNotas = new List<Notas>();
            VerAperura = false;
            FechaReunion = DateTime.Now;
            FechadeFallo = DateTime.Now;
        }

        public decimal PresupuestoBase { get; set; }
        public int ClaveProyecto { get; set; }

        public string Error { get; set; }
        public string Exito { get; set; }

        public SelectList ListaEjercicios { get; set; }
        public SelectList ListaPaquetes { get; set; }
        public SelectList ListaProy { get; set; }
        public SelectList ListaPersonal { get; set; }
        public SelectList ListaSupervisores { get; set; }
        public SelectList ListaServExternos { get; set; }
        public SelectList ListaEmpresas { get; set; }

        [DisplayName("PAQUETE")]
        public int IdPaquete { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio { get; set; }
        [DisplayName("ACTA")]
        public int IdActa { get; set; }
        [DisplayName("AÑO")]
        public int Anio { get; set; }
        [DisplayName("PAQUETE")]
        public int NumPaquete { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string DescPaquete { get; set; }
        [DisplayName("PROCEDIMIENTO")]
        public string Procedimiento { get; set; }
        [DisplayName("RECUPERCAIÓN")]
        public decimal Recuperacion { get; set; }
        public string  TipoInversion { get; set; }
        [DisplayName("PROYECTOS")]
        public List<ProyInv> ListaProyectos { get; set; }

        public List<ActaRubrica> ListaRubricas { get; set; }

        public List<InvitadoExterno> ListaInvitadosExternos { get; set; }

        public InvitadoExterno NuevoInvitado { get; set; }

        public ActaRubrica NuevaRubrica { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("FECHA REUNIÓN")]
        public DateTime FechaReunion { get; set; }
        [DisplayName("HORA REUNIÓN")]
        public string HoraReunion { get; set; }
        [DisplayName("TERMINA REUNIÓN")]

        public string HoraFinReunion { get; set; }
        [DisplayName("LUGAR REUNIÓN")]
        public string LugarDeReunion { get; set; }
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }
        [DisplayName("ASISTENTES")]
        public List<LicitantesApertura> ListaLicitantes { get; set; }

        public LicitantesApertura NuevoLicitante { get; set; }
        public DateTime FechadeFallo { get; set; }
        public string HoraFallo { get; set; }
        [DisplayName("CONDUCE EVENTO")]
        public int IdRepresentante { get; set; }
        [DisplayName("SERVIDOR PUBLICO 1")]
        public int IdServidorPublico1 { get; set; }
        [DisplayName("SERVIDOR PUBLICO 2")]
        public int IdServidorPublico2 { get; set; }
        [DisplayName("SERVIDOR EXTERNO")]
        public int IdServidorPublicoExterno { get; set; }
        public List<Firmaran> ListaFirmaran { get; set; }
        public List<Notas>  ListaNotas { get; set; }
        [DisplayName("NUEVA NOTA")]
        public Notas NuevaNota { get; set; }
        [DisplayName("NUEVO ASISTENTE")]
        public int   NuevoAsistente { get; set; }

        public string AsistenteEditar  { get; set; }
        public bool VerAperura { get; set; }
        public PropuestaProyecto NuevaPropuesta { get; set; }


    }

    public class LicitantesApertura
    {
        public LicitantesApertura()
        {
            Propuestas= new List<PropuestaProyecto>();
        }
        [DisplayName("NO.")]
        public int No{ get; set; }
        [DisplayName("LICITANTE")]
        public string Licitante { get; set; }
        [DisplayName("LICITANTE")]
        public int IdLicitante { get; set; }
        [DisplayName("¿PRESENTA PROPUESTA?")]
        public string PresentaProp { get; set; }
        
        [DisplayName("ASISTENTE")]
        public string Asistente { get; set; }
        [DisplayName("ADQUIRIÓ BASES")]
        public string AdquirioBases { get; set; }
        [DisplayName("FUE DESECHADO")]
        public string FueDesechado { get; set; }
        [DisplayName("OBSERVACIONES")]
        public string Observaciones { get; set; }
        [DisplayName("PROPUESTAS")]
        public List<PropuestaProyecto> Propuestas { get; set; }

        public bool CheckPropuesta { get; set; }
        public bool CheckBases { get; set; }
        public bool CheckDesechado { get; set; }
    }

    public class PropuestaProyecto
    {
        [DisplayName("ID")]
        public int IdDetallesPaquetesProyecto{ get; set; }
        [DisplayName("PROYECTO")]
        public string Proyecto { get; set; }
        [DisplayName("IMPORTE")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Importe { get; set; }
        [DisplayName("PLAZO")]
        public int Plazo { get; set; }
        [DisplayName("Empresa")]
        public int IdEmpresa { get; set; }
    }

    public class Firmaran
    {
        [DisplayName("NO")]
        public int Id { get; set; }
        [DisplayName("EMPRESA")]
        public string Empresa { get; set; }
        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
        [DisplayName("FIRMA")]
        public bool Firma { get; set; }


    }

    public class Notas
    {
        [DisplayName("ID")]
        public int Id  { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string Descripcion { get; set; }

    }
    public class ActaRubrica
    {
        public int IdRubrica { get; set; }
        [DisplayName("LICITANTE/DEPENDENCIA")]
        public string LicDep { get; set; }

        [DisplayName("REPRESETANTE")]
       
        public string Representante { get; set; }
    }

    public class InvitadoExterno
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("CARGO")]
        public  string Cargo { get; set; }
        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
    }

}