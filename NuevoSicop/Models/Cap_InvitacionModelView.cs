using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Models
{
    public class Cap_InvitacionModelView
    {
        public Cap_InvitacionModelView()
        {
            ListaProyectos    = new List<ProyInv>();

            ListaInvitados    = new List<Licitante>();
            ListaCcp          = new List<CopiaPara>();
            ListaFirmas       = new SelectList(new List<string>() { "Seleccione el Firmante " });
            ListaEjercicios   = new SelectList(new List<string>() {"Seleccione el Ejercicio"});
            ListaPaquetes     = new SelectList(new List<string>() { "Seleccione el Paquete" });
            ListaExpAcreditada= new SelectList(new List<string>() { "Seleccione el Experiencia Acreditada" });
            ListaContratistas = new SelectList(new List<string>() { "Seleccione el Contratista" });
            ListaLeyendas     = new SelectList(new List<string>() { "Seleccione el Leyenda" });
            ListaJuntas       = new SelectList(new List<string>() { "Seleccione el Núm. de Junta" });
            IdEjercicio = 0;
            IdPaquete = 0;
            Error = "";
            Exito = "";
            VerInvitacion = false;
            VerJunta = false;
            VerAperura = false;
            VerFallo = false; 
        }

        public string Error { get; set; }
        public string Exito { get; set; }

        public SelectList ListaExpAcreditada { get; set; }
        public SelectList ListaFirmas { get; set; }
        public SelectList ListaContratistas { get; set; }
        public SelectList ListaEjercicios { get; set; }
        public SelectList ListaPaquetes { get; set; }
        public SelectList  ListaLeyendas { get; set; }

        public  SelectList ListaJuntas { get; set; }

        [DisplayName("EJERCICIO")]
        public int IdEjercicio { get; set; }
        [DisplayName("PAQUETE")]
        public int IdPaquete { get; set; }
        [DisplayName("Año")]

        public int Anio { get; set; }
        [DisplayName("PAQUETE")]
        public int Paquete { get; set; }
        [DisplayName("NO. PAQUETE")]
        public string NoPaquete { get; set; }
        [DisplayName("PROCEDIMIENTO")]
        public string Procedimiento { get; set; }
        [DisplayName("RECUPERACIÓN")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Recuperacion { get; set; }
        [DisplayName("PROYECTOS")]
        public List<ProyInv> ListaProyectos { get; set; }
        [DisplayName("INVERSIÓN")]
        public string  TipoInversion { get; set; }
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }
        [DisplayName("FUNDAMENTO")]
        public string FundamentoLegal { get; set; }
        [DisplayName("OFICIO PROGRAMA")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OficioPrograma { get; set; }
        [DisplayName("CONVOCATORIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Convocatoria { get; set; }
        [DisplayName("VENTA BASES")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy,   HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime LimiteVentaBases { get; set; }
        [DisplayName("FECHA VISITA")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy,   HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Visita { get; set; }
        [DisplayName("JUNTA ACLARACIONES")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy,   HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime JuntaAclaraciones { get; set; }
        [DisplayName("APERTURA ECONÓMICA")]

        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy,   HH:mm }", ApplyFormatInEditMode = true)]
        public DateTime Apertura  { get; set; }
        [DisplayName("FECHA FALLO")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy ,   HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fallo { get; set; }
        [DisplayName("INICIO ESTIMADO")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime InicioEstimada { get; set; }
        [DisplayName("FIRMA OFICIO")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy,   HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FirmaContrato { get; set; }
        [DisplayName("INVITADOS")]
        public List<Licitante> ListaInvitados { get; set; }
        [DisplayName("INVITADO")]
        public Licitante NuevoInvitado { get; set; }
        [DisplayName("DIRECTOR")]
        public int IdDirectorDeArea { get; set; }
        [DisplayName("FIRMA")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public int  FirmaOficio { get; set; }
        [DisplayName("REPRESENTANTE")]
        public int  IdRepresentante { get; set; }
        [DisplayName("LEYENDA")]
        public int IdLeyenda { get; set; }
        [DisplayName("Vo.Bo.")]
        public string  VoBo { get; set; }
        [DisplayName("EXP. ACREDIT.")]
        public int IdExperienciaAcreditada { get; set; }
        [DisplayName("INVITADO")]
        public int  IdEliminaInvitado { get; set; }
        [DisplayName("EXPEDIENTE C.C.P.")]
        public CopiaPara NuevaCopia { get; set; }
        public List<CopiaPara> ListaCcp { get; set; }

        [DisplayName("ELIMINA C.C.P.")]
        public int  IdEliminaCcp { get; set; }

        public bool VerInvitacion { get; set; }
        public bool VerJunta { get; set; }
        
        public bool VerAperura { get; set; }
        public bool VerFallo { get; set; }
    }
    public class Licitante
    {
       [DisplayName("NOMBRE")]
        public string Invitado { get; set; }
        [DisplayName("ID")]
        public int IdInvitado { get; set; }
        [DisplayName("OFICIO")]
        public string Oficio { get; set; }
        [DisplayName("FECHA OFICIO")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaOficio { get; set; }        
    }
    public class ProyInv 
     {
        [DisplayName("IDPROYECTO")]
        public int  IdProyecto { get; set; }
        [DisplayName("Clave")]
        public string  Clave { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string DescripcionObra { get; set; }
        [DisplayName("PLAZO DE EJECUCIÓN")]
        public int PlazoEjecucion { get; set; }
        [DisplayName("ANTICIPO")]
        public int Anticipo { get; set; }
        [DisplayName("IMPORTE AUTORIZADO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ImporteAut { get; set; }
        [DisplayName("CAPITAL CONTABLE")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal CapitalContable { get; set; }
        [DisplayName("RESIDENTE")]
        public string Residente { get; set; }
        [DisplayName("PRESUPUESTO BASE")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal PresupuestoBase { get; set; }
     }

    public class CopiaPara
        {
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("ORDEN")]
        public int Orden { get; set; }
        [DisplayName("NOMBRE")]
        public string  Nombre { get; set; }
        [DisplayName("PUESTO")]
        public string  Puesto { get; set; }
        }

    public class Reunion
    {
        public DateTime FechaReunion { get; set; }
        public DateTime HoraReunion { get; set; }
        public DateTime HoraTermino { get; set; }

    



    }

}