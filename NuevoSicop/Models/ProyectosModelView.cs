using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class ProyectosModelView
    {
        public ProyectosModelView()
        {
            Error = "";
            Exito = "";
            ListaProyectos = new List<ProyectoInd>();
            NuevoProyecto = new ProyectoInd();
            ListaEscuelas = new SelectList(new List<string> { "Seleccione Escuela" });
            ListaEjercicios = new SelectList(new List<string> { "Selccione el Ejerccio" });
            ListaProgramas = new SelectList(new List<string> { "Seleccione el Programa" });
            ListaEscuelas = new SelectList(new List<string> { "Seleccione La Escuela" });
            FiltroPrograma = 0;
            FiltroClaveProyecto = "";
            FiltroPrograma = 0;


        }

        public string Error { get; set; }
        public string Exito { get; set; }
        [DisplayName("BUSCAR PROGRAMA")]
        public int FiltroPrograma { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio { get; set; }
        [DisplayName("CLAVE PROYECTO ")]
        public string FiltroClaveProyecto { get; set; }
        public string BuscaEscuela { get; set; }
        public SelectList ListaEscuelas { get; set; }
        public SelectList ListaProgramas { get; set; }
        public SelectList ListaEjercicios { get; set; }
        public ProyectoInd NuevoProyecto { get; set; }
        public List<ProyectoInd> ListaProyectos { get; set; }
    }

    public class ProyectoInd
    {
        public ProyectoInd()
        {
            FechaInicio = DateTime.Now;
            FechaTermino = DateTime.Now;
            FechaAviso = DateTime.Now;
        }

        public int Id { get; set; }
        [DisplayName("CLAVE PROYECTO")]
        public string Clave { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string Descripcion { get; set; }
        [DisplayName("IMPORTE AUTORIZADO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ImporteAutorizado { get; set; }
        [DisplayName("ANTICIPO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Anticipo { get; set; }
        [DisplayName("ESCUELA")]
        public int Escuela { get; set; }
        [DisplayName("ESCUELA")]
        public string NombreEscuela { get; set; }
        [DisplayName("CCT")]
        public string ClaveEscuela { get; set; }
        [DisplayName("INICIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }
        [DisplayName("TERMINA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaTermino { get; set; }
        [DisplayName("AVISO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaAviso { get; set; }
        [DisplayName("PROGRAMA")]
        public int ProgramaRecursos { get; set; }
        [DisplayName("PROGRAMA")]
        public string ProgramaRecursosT { get; set; }
    }
}