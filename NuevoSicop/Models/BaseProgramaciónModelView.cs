using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class BaseProgramaciónModelView
    {
        public BaseProgramaciónModelView()
        {
            Exito = "";
            Error = "";
            Ejercicio =0;
            ListaEjercicios = new SelectList(new List<string>() {"Seleccione un Ejercicio"});
            ListaNivel      = new SelectList(new List<string>() { "Seleccione un Nivel" });
        }
        public string Exito { get; set; }
        public string Error { get; set; }
        public SelectList ListaEjercicios { get; set; }
        public int Ejercicio { get; set; }
        public SelectList  ListaNivel { get; set; }
        public string Nivel { get; set; }
        public List<Paquete> ListaPaquetes { get; set; }
        public Paquete Pq { get; set; }
    }

    public class Paquete
    {
        [DisplayName("Paquete")]
        public int?   NoPaquete { get; set; }
        [DisplayName("Proyecto")]
        public string  Proyecto { get; set; }
        [DisplayName("Programa")]
        public string  Programa { get; set; }
        [DisplayName("Inicia")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime  IniciaPeriodo { get; set; }
        [DisplayName("Termina")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TerminaPeriodo { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("Importe")]
        public decimal Importe { get; set; }
        [DisplayName("Anticipo")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  Anticipo { get; set; }
        [DisplayName("Nivel")]
        public string Nivel { get; set; }
        [DisplayName("Plazo")]
        public int Plazo { get; set; }
    }

}