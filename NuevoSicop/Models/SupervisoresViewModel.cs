using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
   


    public class SupervisoresViewModel
    {
       public SupervisoresViewModel()
       {
           Exito = "";
           Error ="";
            this.ListasRoles= new SelectList(new List<String>() {"Seleccione el Roll"});
           this.ListaSupervisores = new List<PersonalSup>();
       }
        [DisplayName("ROLL")]
        public SelectList ListasRoles { get; set; }
        public string Exito { get; set; }
        public string Error { get; set; }

        [DisplayName("FILTRAR ROLL")]
        public int FiltroIdRoll { get; set; }


        [DisplayName("NUEVO")]
        public PersonalSup NuevoSupervisor { get; set; }
        [DisplayName("ELIMNA")]
        public int  EliminaSup { get; set; }
        [DisplayName("PERSONAL")]
        public List<PersonalSup> ListaSupervisores { get; set; }

    }


    public class PersonalSup
    {

        [DisplayName("CLAVE")]
        public int  Id { get; set; }

        [DisplayName("NOMBRE")]

        public string  Nombre { get; set; }
        [DisplayName("ROLL")]
        public string  Roll { get; set; }


        [DisplayName("ROLL")]


        public int  IdRoll { get; set; }
        [DisplayName("CEDULA")]
        public string Cedula { get; set; }
        [DisplayName("DIRECCIÓN")]
        public string Direccion { get; set; }
        [DisplayName("TELÉFONO")]
        public string Telefono { get; set; }
        [DisplayName("ESTADO")]
        public String Estado { get; set; }




    }


}