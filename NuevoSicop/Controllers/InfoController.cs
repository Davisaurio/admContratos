using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using NuevoSicop.Controllers;

namespace NuevoSicop.Controllers
{

   
    public class InfoController : Controller
    {
        private sicop_tempEntities _sicop = new sicop_tempEntities();

        // GET: Info
        public ActionResult Index()
        {
            return View();
        }


        //[HttpGet('DatosEmp/{clave}')]

        public JsonResult Get(string clave)
        {
            List<RepCaratula> Respuesta = new List<RepCaratula>();

            Respuesta = LlenaCaratula(2021, 4000);

            return Json(Respuesta);
        }

        public List<RepCaratula> LlenaCaratula(int ejercicio, int idPaquete)
        {
            var Rep = new List<RepCaratula>();
            var valores = _sicop.VwReporteCaratula.Where(x => x.ejercicio == ejercicio && x.idPaquete == idPaquete);
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


    }
}