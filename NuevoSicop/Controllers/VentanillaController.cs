using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{

    
    public class VentanillaController : Controller
    {
        private sicop_tempEntities _bdsicop = new sicop_tempEntities();
        // GET: Ventanilla
        [Authorize(Roles = "Administrador,T-ESTIMA")]
        [HttpPost]
        public  VentanillaUnicaViewModel CargarVentanilla( VentanillaUnicaViewModel vent)
        {
            var datos = _bdsicop.SeguimientoEstimacion.Where(x => x.Status == vent.Estatus && x.FechaEntradaVentanilla >vent.FechaInicial && x.FechaEntradaVentanilla<= vent.FechaFinal);
            var estados = _bdsicop.CatalogoSeguimientoEstimacion;
            
             
            foreach (var item in datos)
            {
                vent.ListaEstimaciones.Add(new Estim()
                {
                    Id = item.ID,
                    NoMemo = item.Memo,
                    NoEstima = item.NoEstimacion.GetValueOrDefault(0),
                    Contrato = item.NoContrato,
                    Proyecto = item.NoProyecto,
                    TipoEstimacion = item.CveTipoEstimacion,
                    ImporteOriginal = (double)item.ImporteOriginal,
                    AnticipoAmortizado = (double)item.AnticipoAmortizado,
                    FechaCreacionMemo = item.FechaCreacionMemo == null ? "Sin Fecha" : item.FechaCreacionMemo.Value.ToShortDateString(),
                    Estatus = estados.FirstOrDefault(x=>x.IDStatus ==  item.Status).Descripcion,
                    Devolucion = (double) item.Devolucion.GetValueOrDefault(0),
                    Retencion = (double) item.ImporteRetenido.GetValueOrDefault(0),

                });
            }
            vent.EstEstimacion= new SelectList(estados.Select(x=>new {Id= x.IDStatus,x.Descripcion}), "Id","Descripcion" );
            vent.Personal = new SelectList(_bdsicop.DC.Where(x=>x.Cargo.Contains("Director")),"Clave","Nombre");
            var A = _bdsicop.DetalleMemo.OrderByDescending(x => x.ID).FirstOrDefault().MEMO.Substring(0, 3);
            int  Aint = 1;

            if (int.TryParse(A, out Aint))
            {
                Aint++;
                vent.NuevoMemo.NumMemo= Aint+"/C.E./"+DateTime.Now.Year;

            }
            
            return (vent);
        }
        [Authorize(Roles = "Administrador,T-ESTIMA")]
        //[HttpPost]
        public ActionResult Index()
        {
            var vent = new VentanillaUnicaViewModel();
            vent = CargarVentanilla(vent);
            return View("Index",vent);
        }
        [Authorize(Roles = "Administrador,T-ESTIMA")]
        [HttpPost]
        public ActionResult Filtros(  VentanillaUnicaViewModel vent)
        {
            vent = CargarVentanilla(vent);
            return View("Index", vent);

        }

        [Authorize(Roles = "Administrador,T-ESTIMA")]
        [HttpPost]
        public ActionResult AgregarMemo(VentanillaUnicaViewModel vent)
        {

            if (
                _bdsicop.SeguimientoEstimacion.Any(
                    x => x.NoProyecto == vent.NuevoMemo.Proyecto && x.NoEstimacion == vent.NuevoMemo.Estima))
            {
               
                if (_bdsicop.DetalleMemo.Any(x => x.MEMO == vent.NuevoMemo.NumMemo))
                {
                    var memorandum = _bdsicop.DetalleMemo.FirstOrDefault(x => x.MEMO == vent.NuevoMemo.NumMemo);
                    memorandum.Para = vent.NuevoMemo.Para;
                    memorandum.FechaCreacionMemo = DateTime.Now;
                    memorandum.Anexos = vent.NuevoMemo.TextoMemo;
                    memorandum.Atentamente = vent.NuevoMemo.Atentamente;
                    memorandum.MEMO = vent.NuevoMemo.NumMemo;
                  
                    _bdsicop.DetalleMemo.Add(memorandum);
                    _bdsicop.SaveChanges();
                    vent.Exito += "Se modificó correctamente el Memo";
                }

                else
                {
                    var memo = new DetalleMemo()
                    {
                        Para = vent.NuevoMemo.Para,
                        FechaCreacionMemo = DateTime.Now,
                        Anexos = vent.NuevoMemo.TextoMemo,
                        Atentamente = vent.NuevoMemo.Atentamente,
                        MEMO = vent.NuevoMemo.NumMemo,
                         
                    };

                    _bdsicop.DetalleMemo.Add(memo);
                    _bdsicop.SaveChanges();
                    vent.Exito += "Se generó el Memo correctamente";
                }

                var estimacion = _bdsicop.SeguimientoEstimacion.FirstOrDefault(x => x.NoProyecto == vent.NuevoMemo.Proyecto && x.NoEstimacion == vent.NuevoMemo.Estima);
                estimacion.Status = 7;
                estimacion.FechaCreacionMemo = DateTime.Now;
                estimacion.Memo = vent.NuevoMemo.NumMemo;
                estimacion.ImporteRetenido = (decimal) vent.NuevoMemo.Retencion;
                estimacion.Devolucion = (decimal) vent.NuevoMemo.Devolucion;
                vent.Exito += ", se actualizo el estatus del seguimieto";
                
                _bdsicop.SaveChanges();
                vent.Exito += "Se generó el memo correctamente";
            }
            else
            {
                vent.Error = "No fue posible Generar el MEMO ya que no se encontro el proyecto ";
            }

            vent = CargarVentanilla(vent);

            return View("Index", vent);



        }

    }
}