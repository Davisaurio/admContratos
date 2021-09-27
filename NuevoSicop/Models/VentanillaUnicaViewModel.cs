using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Models
{
    public class VentanillaUnicaViewModel
    {
        public  VentanillaUnicaViewModel()
        {
           this.ListaEstimaciones = new List<Estim>();
             this.NuevoMemo = new Memo();
            this.EstEstimacion= new SelectList(new List<string> {"Seleccione un Estado"});
            this.Personal = new SelectList(new List<string > {"Seleccione su firmante"});
            Estatus = 3;
            Error = "";
            Exito = "";
            FechaInicial = DateTime.Now.AddDays(-60); 
            FechaFinal = DateTime.Now;
        }

        public string  Exito  { get; set; }
        public string  Error { get; set; }
        public SelectList EstEstimacion { get; set; }
        public SelectList  Personal { get; set; }
        public List<Estim> ListaEstimaciones { get; set; }
       
        public Estim Estima { get; set; }
        public Memo NuevoMemo { get; set; }
        [DisplayName("ESTATUS")]
        public int Estatus  { get; set; }
        [DisplayName("INGRESADO ENTRE")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicial { get; set; }
        [DisplayName(" Y EL ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }
    }


   

    public class Memo
    {

        public Memo()
        {
            TextoMemo = "Presupuesto de obra final, bitácora con una copia, oficio de terminación de obra, Garantía de impermeabilización, pruebas de laboratorio, concentrado de estimaciones.";
            Para = 139;
            Atentamente = 120;

        }

        public int  Estima { get; set; }
        public string  Proyecto { get; set; }
        public DateTime FechaMemo { get; set; }
        [DisplayName("TEXTO")]
        public string TextoMemo { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("DEVOLUCIÓN")]

        public double Devolucion { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("RETENCIÓN")]
        public double Retencion { get; set; }
        [DisplayName("PARA:")]
        public int Para { get; set; }
        public int ConCopia { get; set; }
        [DisplayName("ATTE.")]
        public int Atentamente { get; set; }
        [DisplayName("NÚM:")]
        public string NumMemo { get; set; }





    }
    public class Estim
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("NÚM MEMO")]
        public string NoMemo { get; set; }

        [DisplayName("EST.")]
        public int NoEstima { get; set; }

        [DisplayName("PROYECTO")]
        public string  Proyecto { get; set; }

        [DisplayName("TIPO")]
        public string TipoEstimacion { get; set; }

        [DisplayName("CONTRATO")]
        public string Contrato { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("IMPORTE")]
        public double ImporteOriginal { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("ANTICIPO ")]
        public double AnticipoAmortizado { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("FECHA MEMO")]
        public string FechaCreacionMemo { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("DEVOLUCION")]
        public double Devolucion { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [DisplayName("PENALIZACIÓN")]
        public double Retencion { get; set; }
        [DisplayName("ESTATUS")]
        public string Estatus { get; set; }


    }
}

