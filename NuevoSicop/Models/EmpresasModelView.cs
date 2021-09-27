using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Models
{
    public class EmpresasModelView
    {
        public EmpresasModelView()
        {
            Error = "";
            Exito = "";
            this.TipoEmpresa = new SelectList(new List<string>() { "No se han consultado los tipos de empresas" });
            this.TipoInscripcion = new SelectList(new List<string>() { "No se han consultado los tipos de inscripción" });
            this.TipoPersona = new SelectList(new List<string>() { "No se han consultado los tipo persona" });

            IdEditarEmpresa = 0;

            this.ListaFirmas = new SelectList(new List<string> { "NO se hanconsultado las firmas" });
            this.ListaEstados = new SelectList(new List<string>() { "No se han consultado los estados" });
            this.ListaMunicipios = new SelectList(new List<string>() { "No se han consultado los municipios" });

            this.ListaEmpresas = new List<Empresavw>();
            this.FiltroBuscar = "";
            FiltroTipoEmpresa = 0;
            FiltroTipoServicio = 0;
            FiltroTamanio = 0;
            IdEditarEmpresa = 0;
            this.NuevaEmpresa = new EmpresaCompleta();
            NuevoRepresentante = new Representante();
        }

        public Representante NuevoRepresentante { get; set; }


        public int IdEditarEmpresa { get; set; }
        public Empresavw VerEmpresa { get; set; }
        public string Error { get; set; }
        public string Exito { get; set; }

        public List<Empresavw> ListaEmpresas { get; set; }
        public SelectList TipoEmpresa { get; set; }
        public SelectList TipoPersona { get; set; }
        public SelectList TipoServicio { get; set; }
        public SelectList TipoInscripcion { get; set; }
        public SelectList TipoExperienciaAcreditada { get; set; }
        public SelectList Tamanio { get; set; }
        public SelectList ListaEstados { get; set; }
        public SelectList ListaMunicipios { get; set; }
        public SelectList ListaFirmas { get; set; }

        public int Elimina { get; set; }
        public Empresavw Emp;

        [DisplayName("NOMBRE")]
        public string FiltroBuscar { get; set; }
        [DisplayName("FILTRAR POR ")]
        public int FiltroTipoEmpresa { get; set; }
        [DisplayName("SERVICIO")]
        public int FiltroTipoServicio { get; set; }
        [DisplayName("TAMAÑO")]
        public int FiltroTamanio { get; set; }
        [DisplayName("PERSONA")]
        public int FiltroPersona { get; set; }

        public EmpresaCompleta NuevaEmpresa { get; set; }


    }



    public class Empresavw
    {

        public Empresavw()
        {
            IniciaVigencia = new DateTime();
            TerminaVigencia = new DateTime();
            IniciaVigencia = DateTime.Now;
            TerminaVigencia = DateTime.Now;
            Id = 0;
            Nombre = "";
            RFC = "";
            Tipo = "";
            TipoServicio = "";
            Tamanio = "";
            ClaveRegistro = "";
            CapitalContable = 0;
        }
        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("CLAVE")]
        public string ClaveRegistro { get; set; }

        [DisplayName("CAPTURA")]
        public double AvanceCaptura { get; set; }
        [DisplayName("PENDIENTE")]
        public double Pendiente { get; set; }
        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("TIPO")]
        public string Tipo { get; set; }
        [DisplayName("SERVICIO")]
        public string TipoServicio { get; set; }
        [DisplayName("TAMAÑO")]
        public string Tamanio { get; set; }
        [DisplayName("INICIO VIGENCIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime IniciaVigencia { get; set; }
        [DisplayName("TERMINO VIGENCIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TerminaVigencia { get; set; }
        [DisplayName("CAPITAL CONTABLE")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public decimal CapitalContable { get; set; }
    }



    public class EmpresaCompleta
    {
        public EmpresaCompleta()
        {
            ActaFechaConstancia = DateTime.Now;
            IdTipoInscripcion = 0;
            IdTipoServicio = 0;
            IdTipoPersona = 0;
            IdTamañoEmp = 0;
            IdExperienciaAcreditada = 0;
            IniciaVigencia = DateTime.Now;
            DateTime Terminavig = new DateTime();
            DateTime.TryParse((DateTime.Now.Year + 1).ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString(), out Terminavig);
            TerminaVigencia = Terminavig;
            FechaActaNacimiento = DateTime.Now;
            FechaRegistro = DateTime.Now;
            FechaRPPC = DateTime.Now;
            ClaveRegistro = "SAOP-ITIFE-";

        }

        [DisplayName("ID")]
        public int Id { get; set; }
        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
        [DisplayName("CAPITAL CONTABLE")]
        public decimal CapitalContable { get; set; }
        [DisplayName("RFC")]
        public string Rfc { get; set; }
        [DisplayName("CURP")]
        public string Curp { get; set; }
        [DisplayName("CLAVE REGISTRO")]
        public string ClaveRegistro { get; set; }
        [DisplayName("IMSS")]
        public string Imss { get; set; }
        [DisplayName("INFONAVIT")]
        public string Infonavit { get; set; }
        [DisplayName("FOLIO MERCANTIL ")]
        public string FolioMercantilElectronico { get; set; }
        [DisplayName("CMIC")]
        public string CMIC { get; set; }
        [DisplayName("SIEM")]
        public string SIEM { get; set; }
        [DisplayName("DOMICILIO")]
        public string Domicilio { get; set; }
        [DisplayName("NÚM. EXTERIOR")]
        public string NoExterior { get; set; }
        [DisplayName("NÚM. INTERIOR")]
        public string NumInterior { get; set; }
        [DisplayName("COLONIA")]
        public string Colonia { get; set; }
        [DisplayName("ESTADO")]
        public int IdEstado { get; set; }
        [DisplayName("MUNICIPIO")]
        public int IdMunicipio { get; set; }
        [DisplayName("TELÉFONO")]
        public string Telefono { get; set; }
        [DisplayName("FAX")]
        public string Fax { get; set; }
        [DisplayName("CORREO")]
        [EmailAddress]
        public string Correo { get; set; }
        [DisplayName("CAPITAL SOCIAL")]
        public decimal CapitalSocial { get; set; }
        [DisplayName("NÚM. ESCRITURA")]
        public string ActaNumEscritura { get; set; }
        [DisplayName("VOLUMEN ")]
        public string VolumenEscritura { get; set; }
        [DisplayName("FECHA CONSTANCIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ActaFechaConstancia { get; set; }
        [DisplayName("NÚM. NOTARIA")]
        public string NumNotaria { get; set; }

        [DisplayName("NOMBRE TITULAR")]
        public string TitularNotaria { get; set; }

        [DisplayName("NOMBRE ADSCRITO")]
        public string AdscritoNotaria { get; set; }

        [DisplayName("NOMBRE SUSTITUTO")]
        public string SustitutoNotaria { get; set; }

        [DisplayName("CIUDAD")]
        public string CiudadNotaria { get; set; }
        [DisplayName("MODALIDAD")]
        public string Modalidad { get; set; }
        [DisplayName("DIRECCIÓN")]
        public string DireccionNotaria { get; set; }
        [DisplayName("NÚM. RPPC")]
        public string NumRPPC { get; set; }
        [DisplayName("VOLUMEN")]
        public string VolRPPC { get; set; }
        [DisplayName("FECHA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaRPPC { get; set; }
        [DisplayName("CIUDAD")]
        public string CiudadRPPC { get; set; }
        [DisplayName("FOLIOS")]
        public string Folios { get; set; }
        [DisplayName("LIBRO")]
        public string LibroNum { get; set; }
        [DisplayName("ACTA NACIMIENTO")]
        public string NumActaNacimiento { get; set; }
        [DisplayName("FECHA ACTA ")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaActaNacimiento { get; set; }
        [DisplayName("LUGAR")]
        public string LugarExpedicion { get; set; }
        [DisplayName("LIBRO")]
        public string LibroActanacimiento { get; set; }
        [DisplayName("TAMAÑO EMPRESA")]
        public int IdTamañoEmp { get; set; }
        [DisplayName("TIPO INSCRIPCIÓN")]
        public int IdTipoInscripcion { get; set; }
        [DisplayName("TIPO PERSONA")]
        public int IdTipoPersona { get; set; }
        [DisplayName("TIPO SERVICIO")]
        public int IdTipoServicio { get; set; }
        [DisplayName("INICIA VIGENCIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime IniciaVigencia { get; set; }
        [DisplayName("TERMINA VIGENCIA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TerminaVigencia { get; set; }
        [DisplayName("REGISTRO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaRegistro { get; set; }
        [DisplayName("REGISTRÓ")]
        public int IdRegistro { get; set; }
        [DisplayName("REVISÓ")]
        public int IdReviso { get; set; }
        [DisplayName("VO.BO.")]
        public int IdVoBo { get; set; }
        [DisplayName("AUTORIZO")]
        public int IdAutorizo { get; set; }
        [DisplayName("LOCALIDAD")]
        public string Localidad { get; set; }
        [DisplayName("EXP. ACREDITADA")]
        public int IdExperienciaAcreditada { get; set; }


    }

    public class Representante
    {
        public Representante()
        {
            FechaEscritura = DateTime.Now;
            FechaRPPC = DateTime.Now;
            FechaNacimiento = DateTime.Now;
        }


        [DisplayName("NOMBRE")]
        public string Nombre { get; set; }
        [DisplayName("PUESTO")]
        public string Puesto { get; set; }
        [DisplayName("FOLIO ELECTORAL")]
        public string FolioElectoral { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("FOLIO MERCANTIL")]
        public string FolioMercantil { get; set; }
        [DisplayName("DIRECCIÓN")]
        public string Direccion { get; set; }
        [DisplayName("COLONIA")]
        public string Colonia { get; set; }
        [DisplayName("CÓDIGO POSTAL")]
        public string CP { get; set; }
        [DisplayName("CIUDAD")]
        public string Ciudad { get; set; }
        [DisplayName("TELÉFONO")]
        public string Telefono { get; set; }
        [DisplayName("CORREO")]
        public string Correo { get; set; }
        [DisplayName("NÚM. ESCRITURA")]
        public string NumEscritura { get; set; }

        [DisplayName("Vol. ESCRITURA")]
        public string VolEscritura { get; set; }
        [DisplayName("FECHA ESCRITURA")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaEscritura { get; set; }
        [DisplayName("NÚM. NOTARIO")]
        public string NumNotario { get; set; }
        [DisplayName("NOMBRE NOTARIO")]
        public string NombreNotario { get; set; }
        [DisplayName("LUGAR NOTARIO")]
        public string LugarNotario { get; set; }
        [DisplayName("NÚM RPPC")]
        public string NumRPPC { get; set; }
        [DisplayName("VOLUMEN RPPC")]
        public string VolRPPC { get; set; }
        [DisplayName("LUGAR RPPC")]
        public string LugarRPPC { get; set; }
        [DisplayName("FECHA RPPC")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaRPPC { get; set; }
        [DisplayName("FECHA NACIMIENTO")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }
        [DisplayName("LUGAR EXPEDICION ACTA")]
        public string LugarExpedicionActa { get; set; }
        [DisplayName("NOMBRE ADSCRITO")]
        public string NombreAdscrito { get; set; }

        [DisplayName("NOMBRE SUSTITUTO")]
        public string NombreSustituto { get; set; }
        [DisplayName("")]
        public string IdEmpresa { get; set; }

    }

}