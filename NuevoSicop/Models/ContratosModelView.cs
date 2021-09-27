using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class ContratosModelView
    {
         public   ContratosModelView()
        {
            ListaEjercicios = new SelectList(new List<string> { "-Seleccione un Ejercicio-" });
            ListaContratos  = new SelectList(new List<string>  { "-Seleccione un Contrato-" });
            BuscarContrato = "";
            Error = "";
            Exito = "";
            PyContrato = new List<ProyectoContrato>();
        }
        public SelectList  ListaEjercicios { get; set; }
        public SelectList  ListaContratos { get; set; }
        [DisplayName("BUSCAR")]
        public string  BuscarContrato { get; set; }
        public string  Error { get; set; }
        public string  Exito { get; set; }
        [DisplayName("Ejercicio:")]
        public  int IdEjercicio { get; set; }
        [DisplayName("Contrato:")]
        public string IdContrato { get; set; }
        [DisplayName("Recurso:")]

        public List<ProyectoContrato>  PyContrato { get; set; }

        public int Idproyecto { get; set; }
        public string Proyecto { get; set; }

        public string Recurso { get; set; }
        [DisplayName("Inversión:")]
        public string TipoInversion { get; set; }
        [DisplayName("Modalidad:")]
        public string modalidad { get; set; }
        [DisplayName("Contrato:")]
        public string Contrato { get; set; }
        [DisplayName("Tipo Persona:")]
        public string TipoPersona { get; set; }
        [DisplayName("Inversión:")]
        public string Inversion { get; set; }
        [DisplayName("No. Procedimiento:")]
        public string Procedimiento { get; set; }
        //[DisplayName("Modalidad")]
        //public string Modalidad { get; set; }
        [DisplayName("Monto:")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Monto { get; set; }
        [DisplayName("IVA:")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal Iva { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd DE MMMMM DE yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Inicia:")]
        public DateTime Inicia { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd DE MMMMMM DE yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Termina:")]
        public DateTime Termina { get; set; }

        [DisplayName("Porcentaje Anticipo:")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal AnticipoxCien { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("Monto Anticipo:")]
        public decimal Anticipo { get; set; }

        [DisplayName("Plazo Días Naturales:")]
        public int Plazo { get; set; }

        [DisplayName("Contratista:")]
        public string Contratista { get; set; }

        [DisplayName("Descripción:")]
        public string Descripcion { get; set; }

        


    }

    public class ProyectoContrato
    {
        public int Idproyecto { get; set; }
        public string Proyecto { get; set; }

        public string Recurso { get; set; }
        [DisplayName("Inversión:")]
        public string TipoInversion { get; set; }
        [DisplayName("Modalidad:")]
        public string modalidad { get; set; }
        [DisplayName("Contrato:")]
        public string Contrato { get; set; }
        [DisplayName("Tipo Persona:")]
        public string TipoPersona { get; set; }
        [DisplayName("Inversión:")]
        public string Inversion { get; set; }
        [DisplayName("No. Procedimiento:")]
        public string Procedimiento { get; set; }
        //[DisplayName("Modalidad")]
        //public string Modalidad { get; set; }
        [DisplayName("Monto:")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Monto { get; set; }
        [DisplayName("IVA:")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal Iva { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd DE MMMMM DE yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Inicia:")]
        public DateTime Inicia { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd DE MMMMMM DE yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Termina:")]
        public DateTime Termina { get; set; }

        [DisplayName("Porcentaje Anticipo:")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal AnticipoxCien { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("Monto Anticipo:")]
        public decimal Anticipo { get; set; }

        [DisplayName("Plazo Días Naturales:")]
        public int Plazo { get; set; }

        [DisplayName("Contratista:")]
        public string Contratista { get; set; }

        [DisplayName("Descripción:")]
        public string Descripcion { get; set; }


    }
    
}