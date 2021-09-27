using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class ProgramaContratacionModelView
    {
        public ProgramaContratacionModelView()
        {
            ListaEjercicios = new SelectList(new List<string> {"Seleccione Ejercicicio "});
            Paquetes = new SelectList(new List<string> {"Seleccione Paquete"});
            TipoInversion = new SelectList(new List<string> {"Seleccione El tipo de inversión"});
            FundamentoLegal = new SelectList(new List<string> { "Seleccione El fundamento legal" });

            Modalidad = new SelectList(new List<string> {"Seleccione La Modalidad"});
            ListaLeyenda = new SelectList(new List<string> {"Seleccione un aleyenda "});
            ListaServidores= new SelectList(new List<string> {"Seleccione un servidor"});
            Recursos = new SelectList(new List<string> { "Seleccione un Recurso" });
            ListaProyectos= new SelectList(new List<string> { "Seleccione Un Proyecto" });

            Exito = "";
            Error = "";            
            VentaBases = DateTime.Now.Date +new  TimeSpan(16, 00, 0);
            ProgramaContratacion = DateTime.Now;
            ConvocaInvitacion = DateTime.Now;
            Visita = DateTime.Now.Date + new TimeSpan(09, 00, 0);
            JuntaAclaraciones=DateTime.Now.Date + new TimeSpan(09, 00, 0);
            Apertura= DateTime.Now.Date + new TimeSpan(09, 00, 0);
            Fallo= DateTime.Now.Date + new TimeSpan(09, 00, 0);
            Firma= DateTime.Now.Date + new TimeSpan(09, 00, 0);
            FechaInicio=DateTime.Now;
            Paq= new PaquetePy();
            PaqNuevo = new PaquetePy();


        }

        [DisplayName("Recurso")]
        public SelectList Recursos { get; set; }
        [DisplayName("Recurso")]
        public int IdRecurso  { get; set; }
        [DisplayName("Proyecto")]
        public SelectList ListaProyectos { get; set; }

        [DisplayName("Año")]
        public SelectList ListaEjercicios { get; set; }
        [DisplayName("Año")]
        public int IdEjercicio  { get; set; }
        [DisplayName("Paquete")]
        public SelectList Paquetes { get; set; }
        [DisplayName("Paquete")]
        public int IdPaquete { get; set; }
        [DisplayName("Inversión")]
        public SelectList  TipoInversion { get; set; }
        [DisplayName("Inversión")]
        public string IdTipoInversion { get; set; }

        [DisplayName("Modalidad")]

        public SelectList Modalidad { get; set; }
        [DisplayName("Modalidad")]
        public int IdMOdalidad { get; set; }
        [DisplayName("Busqueda")]
        public string BuscarProyecto { get; set; }
        [DisplayName("Paquete")]
        public PaquetePy Paq { get; set; }


        [DisplayName("Paquete Nuevo")]
        public PaquetePy PaqNuevo { get; set; }

        [DisplayName("Fundament")]
        public SelectList FundamentoLegal { get; set; }
        [DisplayName("Fundamento")]
        public int IdFundamento  { get; set; }
        [DisplayName("Fundamento")]
        public string FundamentoLegalDesc { get; set; }

        [DisplayName("Programa de Contratación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ProgramaContratacion { get; set; }
        [DisplayName("Convocatoria/Invitación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ConvocaInvitacion { get; set; }
        [DisplayName("Limite de venta de bases")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime VentaBases { get; set; }
        [DisplayName("Visita")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Visita { get; set; }
        [DisplayName("Junta de Aclaraciones")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime JuntaAclaraciones { get; set; }
        [DisplayName("Apertura")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime  Apertura { get; set; }
        [DisplayName("Fallo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fallo { get; set; }
        [DisplayName("Firma de Contrato")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Firma { get; set; }
        [DisplayName("Inicio estimado")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }
        [DisplayName("Servidores")]
        public SelectList ListaServidores { get; set; }
        [DisplayName("Revisó")]
        public int IdReviso { get; set; }
        [DisplayName("Vo. Bo.")]
        public int IdVoBo { get; set; }
        [DisplayName("Autorizó")]
        public int  IdAutorizo { get; set; }
        [DisplayName("Leyenda para el reporte")]
        public  SelectList ListaLeyenda{ get; set; }
        [DisplayName("Leyenda para el reporte")]
        public int  IdLeyenda { get; set; }
        [DisplayName("Proyecto")]
        public Proyecto AsignaProyecto { get; set; }
        public string  Error { get; set; }
        public string  Exito { get; set; }
        public int IdEliminaProyecto { get; set; }

    }

    public class PaquetePy
    {    public PaquetePy ()
         {
            Proy=new List<Proyecto>();
          }

        [DisplayName("Año")]
        public int  Anio { get; set; }
        [DisplayName("No. Paquete")]
        public int NumPaquete { get; set; }
        [DisplayName("No Procedimiento")]
        public string  NoProcedimiento { get; set; }
        [DisplayName("Recuperación")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  Recuperacion  { get; set; }
        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }
        [DisplayName("Detalles de proyecto")]
        public List<Proyecto> Proy { get; set; }

    }

    public class Proyecto
    {

        public int  Id { get; set; }
        [DisplayName("IdProyecto")]
        [Required]

        public int  IdProyecto { get; set; }

        [DisplayName("Descripción de la obra")]
        public string  Descripcion { get; set; }
        [Required]
        [Range(1,500)]
        [DisplayName("Plazo de ejecución")]
        public int  PlazoEjecucion { get; set; }
        [DisplayName("Anticipo %")]
        [Required]
        [Range(1, 100)]
        public decimal  Anticipo { get; set; }
        [Required]

        [DisplayName("Importe Autorizado")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  ImporteAutorizado { get; set; }
        [Required]

        [DisplayName("% Capital Contable")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  CapitalContable { get; set; }

       

      
    }

    public class Fundamento
    {
        public  Fundamento() 
        {
           Estatal = new SelectList(new List<string>() {"-Seleccione la base legal"});
           Federal = new SelectList(new List<string>() { "-Seleccione la base legal" });
        }

        public SelectList Estatal { get; set; }
        public SelectList Federal { get; set; }
    }

    public class Programas        
    {
        public int IdProyecto  { get; set; }
        public string Oficio { get; set; }
        public string Descripcion { get; set; }
        public int Anio { get; set; }
        public string  ClaveProyecto { get; set; }
        public int  IdRecurso { get; set; }
        public string  Nivel { get; set; }
       


    }


}