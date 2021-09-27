using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Models
{
    public class JuntaModeView
    {
        public JuntaModeView()
        {
            Exito = "";
            Error = "";
            ListaEjercicios    = new SelectList(new List<string>() { "Seleccione el Ejercicio" });
            ListaPaquetes      = new SelectList(new List<string>() { "Seleccione el Paquete" });
            ListaJuntas        = new SelectList(new List<string>() { "Seleccione La Junta" });
            ListaPersonal      = new SelectList(new List<string>() { "¿Quien conduce el evento?" });
            ListaResidentes    = new SelectList(new List<string>() { "¿Quien es el Residentes?" });
            ListaServExternos  = new SelectList(new List<string>() {"Servidor Publico Externo"}  );
            ListaEmpresas      = new SelectList(new List<string>() { "Selecione una empresa" });
            ListaNotas         = new SelectList(new List<string>() { "Selecione una Nota" });
            ListaProyectos     = new List<ProyInv>();
            ListaAclaraciones  = new List<Aclaracion>();
            ListaAsistentes    = new List<Asistente>();          
            NuevoServExterno   = new ServExterno();
            NuevoAsistente     = new Asistente();
            NuevAclaracion     = new Aclaracion();
            IdEjercicio = 0;
            IdPaquete = 0;
            NoActa = 0;
            VerJunta = false; 
            FechaReunion = DateTime.Now;
            FechaTerminoReunion = DateTime.Now;
        }
        public string Exito { get; set; }
        public string Error { get; set; }
        public SelectList ListaEjercicios { get; set; }
        public SelectList ListaPaquetes { get; set; }
        public SelectList ListaJuntas { get; set; }
        public SelectList ListaResidentes { get; set; }
        public SelectList ListaPersonal { get; set; }
        public SelectList ListaServExternos { get; set; }
        public SelectList  ListaEmpresas { get; set; }


        public SelectList ListaNotas { get; set; }
        [DisplayName("NOTA")]
        public int IdNota { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio { get; set; }
        [DisplayName("PAQUETE")]
        public int IdPaquete { get; set; }
        [DisplayName("NÚM. JUNTA")]
        public int NoActa { get; set; }
        [DisplayName("AÑO")]
        public int Anio { get; set; }
        [DisplayName("PAQUETE")]
        public int NumPaquete { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string NumPaqueteDesc { get; set; }
        [DisplayName("PROCEDIMIENTO")]
        public string NoProcedimiento { get; set; }
        [DisplayName("TIPO INVERSION")]
        public string TipoInversion { get; set; }
        [DisplayName("RECUPERCAIÓN")]
        public decimal Recuperacion { get; set; }
        [DisplayName("FECHA REUNIÓN")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaReunion { get; set; }
        [DisplayName("HORA REUNIÓN")]        
        public string  HoraReunion { get; set; }
        [DisplayName("FECHA TÉRMINO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaTerminoReunion { get; set; }
        [DisplayName("HORA TÉRMINO")]
        public string HoraTerminoReunion { get; set; }
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }
        [DisplayName("PROYECTOS")]
        public List<ProyInv> ListaProyectos { get; set; }
        [DisplayName("ACLARACIONES")]
        public List<Aclaracion> ListaAclaraciones { get; set; }
        [DisplayName("ASISTENTES")]
        public List<Asistente> ListaAsistentes { get; set; }
        [DisplayName("SERVIDOR EXTERNO")]
        public ServExterno NuevoServExterno { get; set; }

        [DisplayName("ASISTENTE")]
        public Asistente NuevoAsistente { get; set; }
        [DisplayName("ASISTENTE")]
        public int IdEliminaAsistente { get; set; }

        [DisplayName("ACLARACIÓN")]
        public Aclaracion NuevAclaracion { get; set; }
        [DisplayName("ACLARACIÓN")]
        public Aclaracion EliminaAclaracion { get; set; }
        [DisplayName("CONDUCE EVENTO")]
        public int IdConduceEvento { get; set; }
        [DisplayName("SERVIDOR PUBLICO 1")]
        public int IdSvrPublico1 { get; set; }
        [DisplayName("SERVIDOR PUBLICO 2")]
        public int IdSvrPublico2 { get; set; }
        [DisplayName("SERVIDOR PUBLICO EXTERNO")]
        public int IdSvrPublicoExterno { get; set; }
        [DisplayName("BUSCAR")]
        public string BuscaEmpresa { get; set; }
        [DisplayName("PREGUNTA")]
        public string Pregunta { get; set; }

        [DisplayName("Respuesta")]
        public string Respuesta { get; set; }

        [DisplayName("EXTERNO")]
        public string NuevoExterno { get; set; }
        [DisplayName("CARGO")]
        public string NuevoExternoCargo { get; set; }
        [DisplayName("BUSCAR NOTA")]
        public string BuscaNota  { get; set; }     
        
          
        public bool VerJunta { get; set; }
        public class Aclaracion
        {
            public int Id { get; set; }
            [DisplayName("DESCRIPCIÓN")]
            [MaxLength(500, ErrorMessage = "Maximo de caracteres 500...")]
            [DataType(DataType.MultilineText)]
            public string Descripcion { get; set; }
        }
        public class Asistente
        {
            [DisplayName("CLAVE")]
            public int Clave  { get; set; }
            [DisplayName("ID")]
            public int IdContratista { get; set; }
            [DisplayName("CONTRATISTA ")]
            public string Contratista { get; set; }
            [DisplayName("ASISTE")]
            public string Asiste { get; set; }
            [DisplayName("No.PREG.")]
            public int NumPreguntas { get; set; }
        }
        public class ServExterno
        {
            [DisplayName("ID")]
            public int Id { get; set; }
            [DisplayName("NOMBRE")]
            public string Nombre { get; set; }
            [DisplayName("CARGO")]
            public string Cargo { get; set; }
        }
    }
}