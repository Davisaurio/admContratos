using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml.Sparkline;

namespace NuevoSicop.Models
{
    public class RegistroAdquisicion
    {
        public RegistroAdquisicion()

        {
            ListaEjercicios = new SelectList(new List<string> { "-Seleccione un Ejercicio-" });
            ListaProgramas = new SelectList(new List<string> { "-Seleccione un Programa-" });
            ListaContratos = new SelectList(new List<string> { "-Seleccione un Contrato-" });
            ListaProyectos = new SelectList(new List<string> { "-Seleccione un Proyecto-" });
            ListaRecursos = new SelectList(new List<string> {"-Seleccione un Recurso-"});
            ListaModalidad = new SelectList(new List<string> {"-Seleccione una modalidad -"});
            ListaPersonal = new SelectList(new List<string> { "-Seleccione una Prsonal -" });
            ListaProveedor = new SelectList(new List<string> { "-Seleccione una Proveedor -" });
            ListaTiposContrato = new SelectList(new List<string> { "-Seleccione una Tipo Contrato -" });
            ListaPartidaPresupuestal = new SelectList(new List<string> {"-seleccione la partida presupuestal-"});
            Proyectos = new List<Proy>();
            //ListaFundamentoLegal = new SelectList(new List<string> { "-Seleccione un fundamento Legal-" });
            FechaInicio = DateTime.Now;
            FechaTermino = DateTime.Now;
            FechaFirma= DateTime.Now;
            FechaProcedimiento= DateTime.Now;
            NumeroProcedimiento = "REQUISICIÓN";
            this.Iva =(decimal) 0.16;
            this.IdDirector = 130;
            
            Error = "";
            Exito = ""; 
        }

        public string EliminaLote { get; set; }
        public string  QuitaProyecto { get; set; }
        public string  Error  { get; set; }
        public string  Exito { get; set; }
        [DisplayName("BUSCAR")]
        public string  BuscarContrato { get; set; }
        [DisplayName("EJERCICIO")]
        public int  IdEjercicio { get; set; }
        [DisplayName("PROGRAMA")]
        public int  IdPrograma    { get; set; }
        [DisplayName("EJERCICIOS")]
        public SelectList ListaEjercicios  { get; set; }

        [DisplayName("CONTRATOS")]
        public SelectList ListaContratos { get; set; }
        [DisplayName("CONTRATO")]
        public string IdContrato  { get; set; }
        [DisplayName("PROGRAMAS")]
        public SelectList ListaProgramas { get; set; }
        [DisplayName("PARTIDAS")]
        public SelectList ListaPartidaPresupuestal { get; set; }
        [DisplayName("PROYECTOS")]
        public SelectList ListaProyectos { get; set; }
        [DisplayName("PROYECTO")]
        public int IdProyecto { get; set; }
        [DisplayName("TIPO CONTRATO")]
        public string TipoContrato { get; set; }
        [DisplayName("TIPOS CONTRATOS")]
        public SelectList ListaTiposContrato { get; set; }
        [DisplayName("NÚM. CONTRATO")]
        public string NoContrato { get; set; }
        [DisplayName("PROCEDIMIENTO")]
        public string NumeroProcedimiento { get; set; }
        [DisplayName("PROCEDIMIENTO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaProcedimiento  { get; set; }
        [DisplayName("MODALIDAD")]
        public string  IdModalidad { get; set; }
        [DisplayName("MODALIDAD")]
        public SelectList ListaModalidad { get; set; }
        [DisplayName("PROVEEDOR")]
        public int IdProveedor { get; set; }
        [DisplayName("PROVEEDOR")]
        public SelectList ListaProveedor { get; set; }
        [DisplayName("NUEVO PROYECTO ")]
        public int NuevoProyecto { get; set; }
        [DisplayName("DESCRIPCION TRABAJOS")]
        [Required(ErrorMessage = "Se reequiere una descripción de trabajos.")]
        public string DescripcionTrabajo  { get; set; }
        [DisplayName("FUNDAMENTO LEGAL")]       
        public SelectList ListaFundamentoLegal { get; set; }
        [DisplayName("FUNDAMENTO LEGAL")]
        public string FundamentoLegal { get; set; }
        [DisplayName("APLICA FIANZA")]
        public bool Fianza { get; set; }
        [DisplayName("GARANTIA CUMPLIMINETO")]
        public bool Garantia  { get; set; }
        [DisplayName("APLICA ANTICIPO")]
        public bool AplicaAnticipo { get; set; }
        [DisplayName("PROCENTAJE ANTICIPO")]
        [Range(0, 100, ErrorMessage = "El valor {0} debe ser numérico. entre 0 y 100")]
        [RegularExpression("^\\d+$", ErrorMessage = "el anticipo entre 0 -100  debe contener sólo números.")]
        public decimal PorcentajeAnticipo { get; set; }
        [DisplayName("IMPORTE CONTRATADO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Se reequiere agregar un importe")]
        public decimal  ImporteContratado { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal Iva { get; set; }
        [DisplayName("MONTO ADJ.")]
        [Required(ErrorMessage = "Agregue el Monto adjudicado")]
        public decimal MontoTotal { get; set; }
        [DisplayName("RECURSO")]
        [Required(ErrorMessage = "debe seleccionar un recurso")]
        public int Recurso { get; set; }
        [DisplayName("RECURSOS")]
        public SelectList ListaRecursos { get; set; }
        [DisplayName("FECHA INICIO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio  { get; set; }
        [DisplayName("FECHA TÉRMINO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaTermino { get; set; }
        [DisplayName("PLAZO")]
        public int Plazo { get; set; }
        [DisplayName("FECHA FIRMA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Se reequiere la fecha de firma.")]
        public DateTime  FechaFirma{ get; set; }
        [DisplayName("DIRECTOR")]
        [Required(ErrorMessage = "Se reequiere seleccional seleccionar al director.")]
        public int IdDirector  { get; set; }
        [DisplayName("PERSONAL")]
        public SelectList ListaPersonal { get; set; }
        [DisplayName("OBSERVACIONES")]
        public string Observaciones { get; set; }
        public List<Proy> Proyectos  { get; set; }

        public Proy NuevoProy { get; set; }  
        public ConceptoAdj NuevoConcepto { get; set; }

    }

    public class Proy
    {
       
        public int IdProyectosEncontrato { get; set; }
        public  string  CveContrato { get; set; }
        public string CveProyecto { get; set; }
        [DisplayName("PROYECTO")]
        public int IdProyecto { get; set; }
        [DisplayName("LOTE")]
        public bool  Lote { get; set;}
        [DisplayName("NOMBRE LOTE")]
        public string NombreLote { get; set; }
        public List<ConceptoAdj> Conceptos { get; set; }
        [DisplayName("IMPORTE TOTAL")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  ImporteTotal { get; set; }
    }

    public class ConceptoAdj
    {
        public int IdConcepto { get; set; } 
        public string IdClave { get; set; }
        [DisplayName("ID PROYECTO EN CONTRATO")]
        public int IdProyectoContrato  { get; set; }
        [DisplayName("LOTE")]
        public int Partida { get; set; }
        [DisplayName("CLAVE ARTÍCULO")]
        public string ClaveArticulo { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string Descripcion { get; set; }
        [DisplayName("CANTIDAD")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal Cantidad { get; set; }
        [DisplayName("PRECIO UNITARIO")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal  PrecioUnitario { get; set; }
        [DisplayName("IMPORTE")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal Importe { get; set; }
        [DisplayName("IMPORTE IVA")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ImporteIva { get; set; }
        [DisplayName("IMPORTE TOTAL")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal ImporteTotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public decimal Iva {get; set;}
        [DisplayName("PARTIDA PRESUPUESTAL")]
        [DisplayFormat(DataFormatString = "{0:##.#}", ApplyFormatInEditMode = true)]
        public int IdPartidaPresupuestal { get; set;}
    }
}