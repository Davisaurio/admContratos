using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class DictamenModelView
    {

        public  DictamenModelView()
        {
            ListaEjercicios  = new SelectList(new List<string>() { "Seleccione el Ejercicio"     });
            ListaPaquetes    = new SelectList(new List<string>() { "Seleccione el Paquete"       });
            ListaPersonal    = new SelectList(new List<string>() { "Seleccione el Representante" });
            ListaProyectos   = new List<ProyInv>();
            Proyectos        = new List<string>();
            Error = "";
            Exito = "";
        }

        public string Error { get; set; }
        public string Exito { get; set; }

        
        public SelectList ListaEjercicios { get; set; }
        public SelectList ListaPaquetes { get; set; }

        public SelectList ListaPersonal { get; set; }

        [DisplayName("PAQUETE")]
        public int IdPaquete { get; set; }
        [DisplayName("EJERCICIO")]
        public int IdEjercicio { get; set; }

        [DisplayName("PROYECTO")]
        public string IdProyecto { get; set; }

        [DisplayName("AÑO")]
        public int Anio { get; set; }
        [DisplayName("PAQUETE")]
        public int NumPaquete { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        public string DescPaquete { get; set; }
        [DisplayName("PROCEDIMIENTO")]
        public string Procedimiento { get; set; }
        [DisplayName("RECUPERCAIÓN")]
        public decimal Recuperacion { get; set; }
        public string TipoInversion { get; set; }
        [DisplayName("PROYECTOS")]
        public List<ProyInv> ListaProyectos { get; set; }

        public List<string> Proyectos { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("FECHA DE ELABORACIÓN")]
        public DateTime FechaElaboracion { get; set; }
        
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }

        public int Elaboro { get; set; }
        public int Reviso  { get; set; }
        public int Aprobo  { get; set; }
        public bool VerDictamen { get; set; }

       


    }


}