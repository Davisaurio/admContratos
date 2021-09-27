using Root.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class ReportesModelView
    {
        public ReportesModelView()
        {
            Ejercicios = new SelectList(new List<string> {"Seleccione el Ejerccio"});
            Paquetes = new SelectList(new List<string> { "Seleccione el Pequete" });
            Error = "";
            Exito = "";
            IdEjercicio = 0;
            IdPaquete = 0;

        }

        public string  Error { get; set; }
        public string  Exito { get; set; }


        public  string  Htmltext { get; set; }

        [DisplayName("Ejercicios")]
        public SelectList Ejercicios { get; set; }

        [DisplayName("Paquetes")]
        public SelectList Paquetes { get; set; }
        [DisplayName("Ejercicio")]
        public int IdEjercicio { get; set; }
        [DisplayName("Paquete")]
        public int IdPaquete { get; set; }

        public Repo Report { get; set; }


    }

    public class Repo
    {
        public Repo()
            { 
              Bloques= new List<Bloque>();
              Fuentes = new List<Fuente>();
            }

        public ReportePDF MiReporte { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Ancho { get; set; }
        public double Alto { get; set; }
        public string Descripcion { get; set; }

        public List<Bloque> Bloques { get; set; }

        public List<Fuente> Fuentes { get; set; }
    }


    public class Bloque
    {
        public Bloque()
        {
            Cols = new List<Col>();
            Renglones = new List<Ren>();
        }
        public int Id { get; set; }
        public double PosIzq { get; set; }
        public double PosArriba { get; set; }
        public double AnchoBorde { get; set; }
        public string RGBColor { get; set; }
        public string Nombre { get; set; }
        public Boolean Fijo { get; set; }
        public List<Col> Cols { get; set; }
        public List<Ren> Renglones { get; set; }
        public ListLayoutManager Layout { get; set; }
    }

    public class Fuente
    {

        public int Id { get; set; }
        public string Tipo { get; set; }
        public double Alto { get; set; }
        public string Nombre { get; set; }
        public FontProp Prop { get; set; }
    }

    public class Col
    {
        public Col()
        {
          
        }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Boolean Multilinea { get; set; }
        public double Ancho { get; set; }
        public TlmColumnMM TlmCol  { get; set; }

    }

    public class Ren
    {
        public int Id { get; set; }
        public  Boolean Nuevo { get; set; }
        public int Alineacion { get; set; }
        public double Alto { get; set; }
        public Boolean Negritas { get; set;}
        public string Imagen { get; set; }
        public string Texto { get; set; }
        public int Numero { get; set; }
        public int Parametro { get; set; }
        public int IdFuente { get; set; }
        public Boolean NuevoR { get; set; }
        public string Parametros{ get; set;}
        public int  IdCol { get; set; }
        public double Ancho { get; set; }


    }
    

}