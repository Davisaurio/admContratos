using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class ClaveProgramaticaModelView
    {
        public ClaveProgramaticaModelView ()
        {
            Exito = "";
            Error = "";

           ListaEjercicios  = new SelectList(new List<string> { "SELECCIONE EL EJERCICIO" });
            ListaProyectos = new SelectList(new List<string> { "SELECCIONE EL EJERCICIO" });
            Caratula = false;
        }


        public string Exito { get; set; }
        public string Error { get; set; }
        public SelectList ListaEjercicios { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio  { get; set; }
        public SelectList ListaProyectos { get; set; }
        [DisplayName("PROYECTO")]
        public int   IdProyecto   { get; set; }

        [DisplayName("CLAVE PROGRAMÁTICA")]
        public string ClaveProgramatica { get; set; }
        [DisplayName("PROYECTO")]
        public string ClaveProyecto { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string  Descripcion { get; set; }
        [DisplayName("IMPORTE AUTORIZADO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  Importe { get; set; }
        [DisplayName("CONTRATO")]
        public string  Contrato { get; set; }

        public bool Caratula { get; set; }


    }

   
}