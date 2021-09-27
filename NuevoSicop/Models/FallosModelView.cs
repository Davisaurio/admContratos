using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.Shared;

namespace NuevoSicop.Models
{
    public class FallosModelView
    {
        public FallosModelView()
        {

            ListaProyectosSelec=new SelectList(new List<string>() { "Seleccione el Proyecto" });
            ListaEjercicios   = new SelectList(new List<string>() { "Seleccione el Ejercicio" });
            ListaPaquetes     = new SelectList(new List<string>() { "Seleccione el Paquete" });
            ListaPersonal     = new SelectList(new List<string>() { "Seleccione el Representante" });
            ListaSupervisores = new SelectList(new List<string>() { "Seleccione el Supervisor" });
            ListaServExternos = new SelectList(new List<string>() { "Seleccione el Supervisor Externo" });
            ListaTipoServicio = new SelectList(new List<string>() { "Seleccione el Tipo servicio" });
            ListaContratistas = new SelectList(new List<string>() { "Seleccione el Contratista ganador" });
            ListaIVA          = new SelectList(new List<string>() { "0.16","0.15" });
            ListaContratos    = new List<Contrato>();
            ListaProyectos    = new List<ProyInv>();
            ListaInvitadosExternos = new List<InvitadoExterno>();
            Proyectos         = new List<string>();
            Error             = "";
            Exito             = "";            
            VerFallo          = false;
            CMIC              = false;
            Cumplimiento      = false;
            ViciosOcultos     = false;
            NuevoContrato     = new Contrato();
            NuevoGanador      = new Ganador();
            ListaSolventes = new List<Contratista>();
            ListaRechazado = new List<Contratista>();
            NuevoInvitado = new Rubricas();
        }


        public string Error { get; set; }
        public string Exito { get; set; }

        public SelectList ListaProyectosSelec { get; set; }
        public SelectList ListaEjercicios { get; set; }
        public SelectList ListaPaquetes { get; set; }
        public SelectList ListaPersonal { get; set; }
        public SelectList ListaSupervisores { get; set; }
        public SelectList ListaServExternos { get; set; }
        public SelectList ListaTipoServicio { get; set; }
        public SelectList  ListaContratistas { get; set; }
        public SelectList ListaIVA { get; set; }
        public List<Contratista> ListaSolventes { get; set; }
        public List<Contratista> ListaRechazado { get; set; }
        public List<InvitadoExterno>  ListaInvitadosExternos{ get; set; }
        public Contratista NuevoContratista { get; set; }
        public string NoContrato { get; set; }
        public List<string> Proyectos  { get; set; }

        [DisplayName("PAQUETE")]
        public int IdPaquete { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio { get; set; }
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
        [DisplayName("CONTRATISTA")]
        public int IdContratista { get; set; }

        public string IdProyecto { get; set; }
        public string TipoInversion { get; set; }
        [DisplayName("PROYECTOS")]
        public List<ProyInv> ListaProyectos { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha REUNIÓN")]
        public DateTime FechaReunion { get; set; }
        [DisplayName("HORA REUNIÓN")]
        public string HoraReunion { get; set; }
        [DisplayName("TERMINA REUNIÓN")]
        public string HoraFinReunion { get; set; }
        [DisplayName("LUGAR REUNIÓN")]
        public string LugarDeReunion { get; set; }
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }

        public bool  VerFallo { get; set; }
        [DisplayName("¿Pertenece a CMIC?")]
        public bool CMIC { get; set; }
        [DisplayName("¿Aplica garantía de Cumplimiento?")]
        public bool Cumplimiento { get; set; }
        [DisplayName("¿Aplica garantía de vicios ocultos?")]
        public bool ViciosOcultos { get; set; }
        public string Notas { get; set; }

        [DisplayName("Obra/Adquisición/Servicio")]
        public string OAS { get; set;}

        [DisplayName("PRESIDENTE DE LICITACIÓN")]
        public int  IdPresidente  { get; set; }
        [DisplayName("REVISÓ")]
        public int IdReviso { get; set; }
        [DisplayName("SERVIDOR EXTERNO")]
        public int IdservidorExterno { get; set; }
        [DisplayName("CONTRATO(S)")]
        public List<Contrato> ListaContratos { get; set; }
        public Contrato NuevoContrato { get; set; }
        public Ganador NuevoGanador { get; set; }
        public SelectList ListaParticipantes  { get; set; }
        public int IdParticipante { get; set; }
        public Rubricas NuevoInvitado { get; set; }
        public ActaRubrica NuevaRubrica { get; set; }
        public string NuevoAsistente { get; set; }
        public int IdAsistente { get; set; }
    }

    public class Contrato
    {
        public Contrato()
        {
            ListaGanadores = new List<Ganador>(); 
            FechaInicio = DateTime.Now;
            FechaTermino = DateTime.Now;

          

        }

        [DisplayName("GANADOR")]
        public List<Ganador> ListaGanadores { get; set; }
        [DisplayName("PROYECTO")]
        public int IdProyecto { get; set; }
        [DisplayName("PROYECTO")]
        public string Proyecto { get; set; }
        [DisplayName("NO. CONTRATO")]
        public string NoContrato { get; set; }
        [DisplayName("IMPORTE CON IVA")]
        public decimal ImporteConIVA { get; set; }
        [DisplayName("IVA")]
        public decimal IVA { get; set; }
        [DisplayName("ANTICIPO")]
        public int Anticipo { get; set; }
        [DisplayName("MONTO ANTICIPO")]
        public decimal MontoAnticipo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("FECHA INICIO")]
        public DateTime FechaInicio { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("FECHA TÉRMINO")]
        public DateTime FechaTermino { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string Descripcion { get; set; }

      

    }
    public class Ganador
    {
        [DisplayName("NÚM.")]
        public int Numero  { get; set; }
        [DisplayName("CLAVE")]
        public int Clave { get; set; }
        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
    }

    public class Contratista
    {
        public int Id { get; set; }
        [DisplayName("ESTADO")]
        public   int Estatus  { get; set; }
        [DisplayName("NO.")]
        public int  Num { get; set; }
        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
        [DisplayName("ASISTENTE")]
        public string Asistente { get; set; }
        [DisplayName("¿FUE DESECHADO? ")]
        public bool Desechado { get; set; }
        [DisplayName("OBSERVACIONES")]
        public string Observaciones { get; set; }
        [DisplayName("ORDEN")]
        public int Orden { get; set; }
    }



}
