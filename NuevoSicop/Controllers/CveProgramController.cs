using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class CveProgramController : Controller
    {
        // GET: CveProgram
        sicop_tempEntities _sicop = new sicop_tempEntities();


        public ClaveProgramaticaModelView CargaDatos(ClaveProgramaticaModelView Proy)
        {
            var ejercicios = _sicop.PaquetesProcedimiento.Select(x => new { anio = x.Anio }).Distinct().OrderBy(x => x.anio);
            Proy.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");
          
            Proy.ListaProyectos= new SelectList(_sicop.ESCU.Where(x => x.Anio == Proy.IdEjercicio).Select(x=>new { x.CCT_TURNO, x.id }).OrderBy(x=>x.CCT_TURNO),"id","cct_turno");

            if (Proy.IdProyecto > 0 && _sicop.ESCU.Any(x => x.id == Proy.IdProyecto))
            {
                Proy.Caratula = false;
               var datos = _sicop.ESCU.FirstOrDefault(x => x.id == Proy.IdProyecto);
                Proy.ClaveProyecto = datos.CCT_TURNO;
                Proy.Descripcion = datos.NOMESC;
                Proy.Importe = (decimal) datos.importe_autorizado;
                Proy.Caratula = _sicop.VwReporteCaratula.Any(x => x.proyecto == Proy.ClaveProyecto);
                Proy.ClaveProgramatica = _sicop.ESCU.FirstOrDefault(x => x.id == Proy.IdProyecto).Estatus != null ? _sicop.ESCU.FirstOrDefault(x => x.id == Proy.IdProyecto).Estatus : "Sin clave";
            }

            
            
            return Proy;
        }


        public ActionResult Index()
        {
            ClaveProgramaticaModelView Proy = new ClaveProgramaticaModelView();

            Proy = CargaDatos(Proy);

            return View("Index", Proy );
        }

        public ActionResult Ejercicio(ClaveProgramaticaModelView proy)
        {
          proy = CargaDatos(proy);
          
               
          return View("Index", proy );
        }

        public ActionResult Ejercicio2(ClaveProgramaticaModelView proy)
        {
            proy = CargaDatos(proy);


            return View("Index", proy);
        }

        public ActionResult ActualizaClave(ClaveProgramaticaModelView proy)
        {
            var Error = "";
            var Exito = "";
            if (_sicop.ESCU.Any(x => x.id == proy.IdProyecto) && proy.ClaveProgramatica != "Sin clave")
            {
                try
                {
                    var proyecto = _sicop.ESCU.FirstOrDefault(x => x.id == proy.IdProyecto);
                    proyecto.Estatus = proy.ClaveProgramatica;
                    _sicop.SaveChanges();
                    Exito = "Se agregó correctamente la clave programática";
                }
                catch
                {
                    Error = "Ocurrió Un error Por lo que no se pudo guardar el la Clave Programática";
                }
            }
            else
            {
                Error = "No se encuentra el proyecto o Falta agregar una clave programática";
            }
            proy = CargaDatos(proy);
            proy.Error = Error;
            proy.Exito = Exito;

            return View("Index", proy);
        }


        public List<RepCaratula> LlenaCaratula(int ejercicio,int idproyecto)
        {
            var Rep = new List<RepCaratula>();

            var Proyecto = _sicop.ESCU.FirstOrDefault(x => x.id == idproyecto).CCT_TURNO;
            var valores = _sicop.VwReporteCaratula.Where(x => x.ejercicio == ejercicio && x.proyecto == Proyecto);
            foreach (var item in valores)
            {
                Rep.Add(new RepCaratula()
                {

                    NumPaquete = item.paqueteDesc,
                    PaqueteDesc = item.paqueteDesc,
                    Partida = item.clavepartida,
                    FuenteFianciera = item.ClaveFF,
                    Procedencia = item.Procedencia,
                    ClaveProcedencia = item.Clave,
                    Proyecto = item.proyecto,
                    ClaveProgramatica = item.claveProgramatica,
                    Contrato = item.no_contrato,
                    ImporteContratado = (decimal)item.monto_contrato.GetValueOrDefault(0),
                    Partidadesc = string.IsNullOrEmpty(item.Partidadesc) ? "" : item.Partidadesc,
                    TipoInversion = item.TipoInversion,
                    Programa = item.Programa,
                    Modalidad = item.modalidad,
                    NoProcedimiento = item.NoProcedimiento,
                    FechaContrato = item.FechaContrato.GetValueOrDefault(DateTime.MinValue),
                    DescripcionTrabajo = item.DescripcionTrabajo,
                    DescripcionTrabajo2 = item.DescripcionTrabajo2,
                    Plazo = item.plazo.GetValueOrDefault(0),
                    FechaInicio = item.fechainicio.GetValueOrDefault(DateTime.MinValue),
                    FechaTermino = item.FechaTermino.GetValueOrDefault(DateTime.MinValue),
                    MontoAutorizado = (decimal)item.MontoAutorizado,
                    Anticipo = (decimal)item.anticipo,
                    Oficio = item.Oficio,
                    Fecha = item.Fecha.GetValueOrDefault(DateTime.MinValue),
                    CCT = item.CCT,
                    Contratista = item.Contratista,
                    Representante = string.IsNullOrEmpty(item.Representante) ? "" : item.Representante,
                    Empresa = item.Empresa,
                    DomicilioEmpresa = item.domicilioempresa,
                    Localidad = item.empLocalidad,
                    CPEmpresa = item.CPEmpresa,
                    Correo = item.Correo,
                    RFCEmpresa = item.RFCEmpresa,
                    Imss = item.Imss,
                    Infonavit = item.Infonavit,
                    Padron = item.Padron,
                    Depen = item.dep,
                    ClaveFinanzas = item.CadenaFianzas == null ? "" : item.CadenaFianzas,
                });
            }

            return Rep;
        }


        public ActionResult Caratula (ClaveProgramaticaModelView proy)
        {

            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaCaratula(proy.IdEjercicio, proy.IdProyecto);
            if (Datos.Count() > 0)
            {
                
                paq = Datos.FirstOrDefault().NumPaquete;

                
                 rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "Caratula1.rpt")); 
                rd.SetDataSource(Datos);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Carátula PAQ" + paq.ToString() + ".pdf");
                }
                catch
                {
                    throw;
                }

            }
            else
            {
                proy.Error = "No se encontró sufucuente información para generar este reporte.";
                return RedirectToAction("Index");
            }
        }




    }
}