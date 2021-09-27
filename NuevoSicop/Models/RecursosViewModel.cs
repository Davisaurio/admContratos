using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Root.Reports;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

namespace NuevoSicop.Models
{
    public class RecursosViewModel
    {
        public RecursosViewModel()
        {

            this.ListaEjerciciosFiscales = new SelectList(new List<string>{"SELECCIONE EJERCICIO FISCAL"});
            this.ListaNiveles = new SelectList(new List<string> { "SELECCIONE NIVEL" });
            this.ListaRecursos = new SelectList(new List<string> { "SELECCIONE RECURSO" });
            ProgEliminar = 0;
            ProgEditar = 0;
            NuevoRecurso = new Programa();
            FiltroNivel = 0;
            FiltroEjercicio = 0;
            FiltroRecurso = 0;
            Programas = new List<Programa>();
            Error = "";
            Exito = "";
            ModalEditar = "";
        }
        [DisplayName("EJERCICIO")]
        public int  FiltroEjercicio { get; set; }
        [DisplayName("NIVEL")]
        public int  FiltroNivel { get; set; }
        [DisplayName("RECURSO")]
        public int FiltroRecurso  { get; set; }
        public OficioRecurso NuevOficio { get; set; }
        public SelectList ListaEjerciciosFiscales { get; set; }

        public int  ElimarOfi { get; set; }
       
        public SelectList ListaNiveles { get; set; }
        public SelectList ListaRecursos { get; set; }
        public Programa NuevoRecurso { get; set; }
        public List<Programa> Programas { get; set; }
        public string  Error { get; set; }
        public string Exito { get; set; }
        public int ProgEliminar { get; set; }
        public int ProgEditar { get; set; }
        public string ModalEditar { get; set; }
        [DisplayName("NUEVO RECURSO")]
        public string  AgregaRecurso { get; set; }
    }

    public class Programa
    {
        [Required(ErrorMessage = "Se reequiere seleccional un ejercicio fiscal.")]
        [DisplayName("EJERCICIO FISCAL")]
       
        public int IdEjercicio { get; set; }
        [DisplayName("EJERCICIO FISCAL")]
        public string Ejercicio { get; set; }
        [Required(ErrorMessage = "Se requeire una descripcion de programa")]
        [DisplayName("PROGRAMA")]
        public string NombrePrograma { get; set; }
        [DisplayName("RECURSO INTERNO ")]
        public bool RecursoInterno { get; set; }
        [Required(ErrorMessage = "Se requeire seleccionar un recurso")]
        [DisplayName("RECURSO")]
        public  int IdRecurso { get; set; }
        [DisplayName("FUENTE FINACIERA")]
        [Required(ErrorMessage = "La Clave de Fuente Financiera Es requerida")]
        public string ClaveFF { get; set; }
        [DisplayName("MUNICIPAL")]
        [Required(ErrorMessage = "Debe de ingresar un Inversión Municipal.")]
        [Range(0,100, ErrorMessage = "El valor {0} debe ser numérico.")]
        [RegularExpression("^\\d+$", ErrorMessage = "La Inversión Municipal debe contener sólo números.")]
        public double InversionMunicipal { get; set; }
        [DisplayName("ESTATAL")]
        [Required(ErrorMessage = "Debe de ingresar un Inversión Estatal.")]
        [Range(0, 100, ErrorMessage = "El valor {0} debe ser numérico.")]
        [RegularExpression("^\\d+$", ErrorMessage = "La Inversión Estatal debe contener sólo números.")]
        public double InversionEstatal { get; set; }
        [DisplayName("FEDERAL")]
        [Required(ErrorMessage = "Debe de ingresar un Inversión Federal.")]
        [Range(0, 100, ErrorMessage = "El valor {0} debe ser numérico.")]
        [RegularExpression("^\\d+$", ErrorMessage = "La Inversión Federal debe contener sólo números.")]
        public double InversionFederal { get; set; }
        [Required(ErrorMessage = "Se requeire seleccionar un nivel")]
        [DisplayName("NIVEL")]
        public int IdNivel { get; set; }
        [DisplayName("PROCEDENCIA")]
        public string Procedencia { get; set; }
        [DisplayName("OFICIOS")]
        public List<OficioRecurso> Oficios { get; set; }
        [DisplayName("ESTATUS")]
        public bool Estatus { get; set; }
        [DisplayName("CLAVE")]
        public string  Clave { get; set; }
        [DisplayName("NIVEL")]
        public string  Nivel { get; set; }
        [DisplayName("RECURSO")]
        public string  DescRecurso { get; set; }

    }

    public class OficioRecurso
    {
        public int  IdOficio  { get; set; }
        public int IdPrograma { get; set; }
        [DisplayName("EJERCICIO")]
        public int  IdEjercicio { get; set; }
        [DisplayName("NOMBRE DEL RECURSO")]
        public string  NombreRecurso { get; set; }
        [DisplayName("EMISORA")]
        public string  Emisora { get; set; }
        [DisplayName("OFICIO: ")]
        [Required(ErrorMessage = "Ingrese numero de oficio")]      
        public string  NumeroOficio { get; set; }
        [DisplayName("FECHA:")]
        [Required(ErrorMessage = "Seleccione una fecha de Oficio")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [RegularExpression("(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\\d\\d", ErrorMessage = "Fecha Invalida")]
        public DateTime FechaOficio { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("IMPORTE:")]
        [Required(ErrorMessage = "Es requerido el Importe Autorizado")]
        public double  ImporteAutorizado { get; set; }
    }


}