using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Models
{
    public class CatalogoEscuelasViewModels
    {
        public CatalogoEscuelasViewModels()
        {

            this.ListaNivelesEsc = new SelectList(new List<string> { "SELECCIONE NIVEL" });
            this.ListaLocalidades = new SelectList(new List<string> { "SELECCIONE LA LOCALIDAD" });
            this.ListaMunicipios = new SelectList(new List<string> { "SELECCIONE EL MUNICIPO" });
            this.ListaEntidades = new SelectList(new List<string> { "SELECCIONE LA ENTIDAD" });
            this.ListaDescrsost = new SelectList(new List<string> { "SELECCIONE DESCSOFT" });
            this.ListaModalidad= new SelectList(new List<string> { "SELECCIONE LA MODALIDAD" });
            this.ListaSectores = new SelectList(new List<string> { "SELECCIONE EL SECTOR" });
            this.ListaZonas = new SelectList(new List<string> { "SELECCIONE LA ZONA" });

            Error = "";
            Exito = "";
            NuevaModalidad = "";
            NuevoNivel = "";
            NuevoDescSost = "";
        }

        [DisplayName("DESCSOST")]
        public string  NuevoDescSost { get; set; }
        [DisplayName("NIVEL")]
        public string  NuevoNivel { get; set; }
        [DisplayName("MODALIDAD")]
        public string  NuevaModalidad { get; set; }
        public string  ModalEditar { get; set; }
        [DisplayName("NOMBRE")]
        public string FiltroNombre { get; set; }
        [DisplayName("NIVEL")]
        public int  FiltroNivel { get; set; }
        [DisplayName("MODALIDAD")]
        public int  FiltroModalidad { get; set; }
        [DisplayName("MUNICIPIO")]
        public int  FiltroMunicipio { get; set; }
        public SelectList ListaZonas { get; set; }
        public SelectList ListaSectores { get; set; }
        public SelectList ListaNivelesEsc { get; set; }
        public SelectList ListaEntidades { get; set; }
        public SelectList ListaMunicipios { get; set; }
        public SelectList ListaLocalidades { get; set; }
        public SelectList ListaModalidad { get; set; }
        public SelectList ListaDescrsost { get; set; }
        public string Error { get; set; }
        public string Exito { get; set; }
        public List<Escuela> ListaEscuelas = new List<Escuela>();
        public Escuela NuevaEscuela = new Escuela();
        public int  EliminaEscuela { get; set; }
        public int EditarEscuela { get; set; }



    }

    public class Escuela
    {
        [DisplayName("ID")]
        public int  Id { get; set; }
        [DisplayName("MODALIDAD")]
        public int  IdModalidad { get; set; }
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }
        [DisplayName("NOMBRE")]
        public string  Nombre { get; set; }
        [DisplayName("CLAVE")]
        public string Clave { get; set; }
        public int  IdClave { get; set; }
        [DisplayName("NIVEL")]
        public string Nivel { get; set; }
        [DisplayName("NIVEL")]
        public int IdNivel { get; set; }
        [DisplayName("SECTOR")]
        public string Sector { get; set; }
        [DisplayName("ZONA ESCOLAR")]
        public string ZonaEscolar{ get; set;}
        [DisplayName("DIRECCIÓN")]
        public string Direccion { get; set; }
        [DisplayName("MUNICIPIO")]
        public string Municipio  { get; set; }
        [DisplayName("LOCALIDAD")]
        public string Localidad  { get; set; }
        [DisplayName("Localidad")]
        public int IdLocalidad { get; set; }
        [DisplayName("DESCSOST")]
        public string  Descrsost { get; set; }
        [DisplayName("DESC SOST")]
        public int IdDescrsost { get; set; }
        [DisplayName("ENTIDAD")]
        public int IdEntidad { get; set; }
        [DisplayName("MUNICIPIO")]
        public int IdMunicipio { get; set; }
        [DisplayName("CLAVE LOC")]
        public string  ClaveLocalidad { get; set; }
    }
}
public class Listas
{
    public Listas()
    {
        Genericos = new SelectList(new List<string>() { "SELECCIONE UNELEMENTO" });
        Localidades = new SelectList(new List<string>() { "SELECCIONE UNA LOCALIDAD" });
        Municipios = new SelectList(new List<string>() { "SELECCIONE UNA MUNICIPIO" });
        Proyectos  = new SelectList(new List<string>() { "SELECCIONE UN PROYECTO " });
        Progra = new SelectList(new List<string>() { "SELECCIONE UN PROGRAMA" });
        ClaveLocalidad = "";
    }
    public string  ClaveLocalidad { get; set; }
    public SelectList Municipios { get; set; }
    public SelectList Localidades { get; set; }
    public SelectList Proyectos { get; set; }
    public SelectList Progra { get; set; }
    public SelectList Genericos { get; set; }
}