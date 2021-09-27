using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class EmpleadosModelView
    {
        public EmpleadosModelView()
        {
            Exito = " ";
            Error = " ";

            ListaDepartamentos = new SelectList(new List<String>() { " Sin Departamentos " });
            ListaDirecciones = new SelectList(new List<String>() { " Sin Direcciones " });
            ListaJefes = new SelectList(new List<String>() { " Sin personal " });

            FiltroDepartamento = 0;
            FiltroDireccion = 0;
            FiltroNombre = " ";
            NombDepto = "";
            AbreviaDepto = "";

        }
        [DisplayName("DIRECCIONES")]
        public SelectList ListaDirecciones { get; set; }
        [DisplayName("DEPARTAMENTOS")]
        public SelectList ListaDepartamentos { get; set; }

        [DisplayName("JEFES")]
        public SelectList ListaJefes { get; set; }
        public string Exito { get; set; }
        public string Error { get; set; }

        [DisplayName("FILTRAR ROLL")]
        public int FiltroDireccion { get; set; }
        public int FiltroDepartamento { get; set; }
        [DisplayName("NOMBRE")]
        public string FiltroNombre { get; set; }

        [DisplayName("NUEVO")]
        public Empleado NuevoEmpleado { get; set; }
        [DisplayName("ELIMNA")]
        public int EliminaEmpleado { get; set; }
        [DisplayName("EDITAR ")]
        public int IdEditarEmpleado { get; set; }
        [DisplayName("PERSONAL")]
        public List<Empleado> ListaEmpleados { get; set; }

        [DisplayName("DEPARTAMENTO")]
        public string NombDepto { get; set; }
        [DisplayName("ABREVIATURA")]
        public string AbreviaDepto { get; set; }


    }
    public class Empleado
    {
        [DisplayName("CLAVE")]
        public int Id { get; set; }
        [DisplayName("TRATO")]
        [Required]
        public string Trato { get; set; }
        [DisplayName("NOMBRE")]
        [Required]
        public string Nombre { get; set; }
        [DisplayName("APELLIDO PATERNO")]
        [Required]
        public string ApellidoP { get; set; }
        [DisplayName("APELLIDO MATERNO")]
        [Required]
        public string ApellidoM { get; set; }
        [DisplayName("CARGO ")]
        public string Cargo { get; set; }
        [DisplayName("TELÉFONO ")]
        public string Telefono { get; set; }
        [DisplayName("CORREO")]
        [Required]
        public string Correo { get; set; }
        [DisplayName("GENERO")]
        [Required]
        public Boolean Genero { get; set; }
        [DisplayName("GENERO")]
        public string GeneroTexto { get; set; }
        [DisplayName("DOMICILIO")]
        public string Domicilio { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("DIRECCIÓN")]
        [Required]
        public int IdDireccion { get; set; }
        [DisplayName("DIRECCIÓN")]
        public string Direccion { get; set; }
        [DisplayName("DEPARTAMENTO")]
        [Required]
        public int IdDepto { get; set; }
        [DisplayName("DEPARTAMENTO")]
        public string Depto { get; set; }
        [DisplayName("ESTATUS")]
        public string Estatus { get; set; }
        [DisplayName("FOTO")]
        public string Foto { get; set; }

        [DisplayName("FECHA NAC.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }
        [DisplayName("JEFE INMEDIATO")]
        [Required]
        public int IdJefe { get; set; }
    }



}
