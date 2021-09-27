using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class DocumentosModelView
    {
        public DocumentosModelView()
        {
            Exito = "";
            Error = "";
            ListaEjercicio = new SelectList(new List<string> { "- Seleccione el Ejerccio -" });
            ListaPaquetes = new SelectList(new List<string> { "- Seleccione el Paquete -" });
            ListaActas = new SelectList(new List<string> { "- Seleccione El Acta -" });
            ListaProyectos = new SelectList(new List<string> { "-Seleccione el PRoyecto -" });
            Proyectos = new List<string>();
        }
        public string Exito { get; set; }
        public string Error { get; set; }
        public  List<string> Proyectos  { get; set; }
        public SelectList ListaEjercicio { get; set; }
        public SelectList ListaPaquetes  { get; set; }
        public SelectList  ListaActas { get; set; }
        public SelectList ListaProyectos { get; set; }
        [DisplayName("PROYECTO")]
        public string  IdProyectoIndice { get; set; }
        [DisplayName("CLAVE PROG.")]
        public string  ClaveProgramatica { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio  { get; set; }
        [DisplayName("PAQUETE")]
        public int IdPaquete { get; set; }
        [DisplayName("PROYECTO")]
        public string IdProyecto { get; set; }
        [DisplayName("ACTA")]
        public int NoActa { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string Descripcion { get; set; }
        [DisplayName("INVERSIÓN")]
        public string TipoInversion { get; set; }
        [DisplayName("MODALIDAD")]
        public string  modalidad { get; set; }
        [DisplayName("CONTRATO")]
        public string  Contrato { get; set; }
        [DisplayName("CONTRATISTA")]
        public string  Contratista { get; set; }
        [DisplayName("MONTO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Monto { get; set; }
        public bool VerPrograma   { get; set; }
        public bool VerInvitacion { get; set; }
        public bool VerJunta { get; set; }
        public bool VerAperura { get; set; }
        public bool VerFallo { get; set; }
        public bool VerContrato { get; set; }        
    }     
}

