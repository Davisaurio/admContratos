using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    
    public class DatosProgramacionController : Controller
    {
        private sicop_tempEntities _bdsicop = new sicop_tempEntities();


       

        [Authorize(Roles = "Administrador,P-Datos")]
        public BaseProgramaciónModelView CargaDatos(BaseProgramaciónModelView datos)
        {
            var anios= _bdsicop.VwContratosProg.Select(x =>new { x.ANIO}).Distinct().OrderBy(x => x.ANIO);
            datos.ListaEjercicios = new SelectList(anios,"ANIO","ANIO");
            var an = datos.Ejercicio > 0 ? datos.Ejercicio : DateTime.Now.Year;  
           
            var nuevabase = _bdsicop.VwContratosProg.Where(x => x.ANIO == an).Distinct().OrderBy(x => x.Proyecto);
            datos.ListaPaquetes = new List<Paquete>();
            foreach (var item in nuevabase)
            {
                datos.ListaPaquetes.Add(new Paquete()
                {
                    NoPaquete = (int)item.Paquete.GetValueOrDefault(0),
                    Proyecto = item.Proyecto,
                    Programa = item.DESCRIPCION,
                    IniciaPeriodo = (DateTime)item.Inicia.GetValueOrDefault(DateTime.MinValue),
                    TerminaPeriodo = (DateTime)item.termina.GetValueOrDefault(DateTime.MinValue),
                    Importe = (decimal)item.inversionAutorizada.GetValueOrDefault(0),
                    Anticipo = (decimal)(item.inversionAutorizada.GetValueOrDefault(0) * (decimal)item.Anticipo.GetValueOrDefault(1) / 100),
                    Nivel = item.nivel,
                    Plazo = (int)item.plazo_dias_nat.GetValueOrDefault(0)
                });
            }
            return datos;
        }


        // GET: DatosProgramacion
        public ActionResult Index()
        {
            //var a =  _bdsicop
            BaseProgramaciónModelView programacion = new BaseProgramaciónModelView();


            programacion = CargaDatos(programacion);


            return View("index", programacion);
        }

        [Authorize(Roles = "Administrador,P-Datos")]
        [HttpPost]
        public ActionResult Filtros(BaseProgramaciónModelView programacion)
        {


            programacion = CargaDatos(programacion);


            return View("index", programacion);
        }


        [Authorize(Roles = "Administrador,P-Datos")]
        [HttpPost]
        public ActionResult Excell(BaseProgramaciónModelView datos)
        {

            
            var anios = _bdsicop.VwContratosProg.Select(x => new { x.ANIO }).Distinct().OrderBy(x => x.ANIO);
            datos.ListaEjercicios = new SelectList(anios, "ANIO", "ANIO");
            var an = datos.Ejercicio > 0 ? datos.Ejercicio : DateTime.Now.Year;

            var nuevabase = _bdsicop.VwContratosProg.Where(x => x.ANIO == an).Distinct().OrderBy(x => x.Paquete).ThenBy(x => x.Proyecto); ;

            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Paquetes");

                ws.Cells["A2"].LoadFromCollection(nuevabase, true);
                ws.Cells["A1:AQ1"].Merge = true;

                ws.Cells["A1"].Value = "ITIFE";
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ;
                ws.Cells["A1"].Style.Font.Size = 20;

                ws.Cells["A1:AQ2"].Style.Font.Bold = true;
              //  ws.Cells["A3:G" + (base + 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                ws.Column(1).Style.Font.Bold = true;
                ws.Column(1).Width = 8.29;
                ws.Column(2).Width = 12;
                ws.Column(2).Style.Font.Size = 11;
                ws.Column(3).Width = 8;
                ws.Column(3).Style.Font.Size = 11;
                ws.Column(4).Width = 59;
                ws.Column(4).Style.Font.Size = 9;
                ws.Column(4).Style.WrapText=true ;
                ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                ws.Column(4).Style.VerticalAlignment= ExcelVerticalAlignment.Distributed;
                ws.Column(5).Style.Font.Size = 11;
                ws.Column(5).Width = 10;
                ws.Column(6).Width = 14;
                ws.Column(7).Width = 12;
                ws.Column(8).Width = 14;


                ws.Column(9).Width = 14;
                ws.Column(10).Width = 14;
                ws.Column(11).Width = 14;
                ws.Column(12).Width = 14;

                ws.Column(13).Width = 59;
                ws.Column(13).Style.Font.Size = 9;
                ws.Column(13).Style.WrapText = true;
                ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                ws.Column(13).Style.VerticalAlignment = ExcelVerticalAlignment.Distributed;
                ws.Column(14).Width = 59;
                ws.Column(14).Style.Font.Size = 9;
                ws.Column(14).Style.WrapText = true;
                ws.Column(14).Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                ws.Column(14).Style.VerticalAlignment = ExcelVerticalAlignment.Distributed;
                ws.Column(15).Width = 19;
                ws.Column(15).Style.Numberformat.Format = "$ #,##0.00";
                ws.Column(16).Width = 19;
                ws.Column(16).Style.Numberformat.Format = "$ #,##0.00";
                ws.Column(17).Width = 14;
                ws.Column(18).Width = 14;
                ws.Column(18).Width = 14;
                ws.Column(19).Width = 14;
                ws.Column(19).Style.Numberformat.Format = "$ #,##0.00";
                ws.Column(20).Width = 14;
                ws.Column(20).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Column(21).Width = 18;
                ws.Column(21).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Column(22).Width = 14;
                ws.Column(22).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Column(23).Width = 14;
                ws.Column(23).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(24).Width = 14;
                ws.Column(24).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(25).Width = 14;
                ws.Column(26).Width = 14;
                ws.Column(26).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(27).Width = 14;
                ws.Column(27).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                ws.Column(28).Width = 14;
                ws.Column(28).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(29).Width = 14;
                ws.Column(29).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                ws.Column(30).Width = 14;
                ws.Column(30).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(31).Width = 14;
                ws.Column(31).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                ws.Column(32).Width = 14;
                ws.Column(32).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(33).Width = 14;
                ws.Column(33).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                ws.Column(34).Width = 14;
                ws.Column(34).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(35).Width = 14;
                ws.Column(35).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                ws.Column(36).Width = 14;
                ws.Column(37).Width = 14;
                ws.Column(37).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(38).Width = 14;
                ws.Column(38).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(39).Width = 14;

                ws.Column(40).Style.Numberformat.Format = "dd/mm/yyyy";
                ws.Column(40).Width = 14;
                
                ws.Column(41).Width = 50;
                ws.Column(42).Style.Numberformat.Format = "$ #,##0.00";
                ws.Column(42).Width = 14;
                ws.Column(43).Style.Numberformat.Format = "$ #,##0.00";
                ws.Column(43).Width = 20;

                //ws.Column().Style.Numberformat.Format = " ###0.00";


                // ACCEDER AL ARCHIVO DE CUENTAS 

                var fileBytes = pck.GetAsByteArray();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition",
                    "attachment;filename=BD_Paquetes-" +DateTime.Now.ToShortDateString()+  ".xlsx");
                // REEMPLAZAR EL NOMBRE DE ARCHIVO QUE DESEA

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                var sw = new StringWriter();
                Response.BinaryWrite(fileBytes);
                Response.End();
            }

            return RedirectToAction("Index");
        }



        [Authorize(Roles = "Administrador,P-Datos")]
        [HttpPost]
        public ActionResult Excell2(BaseProgramaciónModelView datos)
        {


            var anios = _bdsicop.VwContratosProg.Select(x => new { x.ANIO }).Distinct().OrderBy(x => x.ANIO) ;
            datos.ListaEjercicios = new SelectList(anios, "ANIO", "ANIO");
            var an = datos.Ejercicio > 0 ? datos.Ejercicio : DateTime.Now.Year;

            var nuevabase =  _bdsicop.vwPaqInvitados.Where(x => x.Anio == an).Distinct().OrderBy(x => x.Paquete);



            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Paquetes");

                ws.Cells["A2"].LoadFromCollection(nuevabase, true);
                ws.Cells["A1:G1"].Merge = true;

                ws.Cells["A1"].Value = "ITIFE";
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ;
                ws.Cells["A1"].Style.Font.Size = 20;

                ws.Cells["A1:G2"].Style.Font.Bold = true;
                //  ws.Cells["A3:G" + (base + 2)].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //ws.Column(1).Style.Font.Bold = true;
                //ws.Column(1).Width = 8.29;
                //ws.Column(2).Width = 12;
                //ws.Column(2).Style.Font.Size = 11;
                //ws.Column(3).Width = 8;
                //ws.Column(3).Style.Font.Size = 11;
                //ws.Column(4).Width = 59;
                //ws.Column(4).Style.Font.Size = 9;
                //ws.Column(4).Style.WrapText = true;
                //ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                //ws.Column(4).Style.VerticalAlignment = ExcelVerticalAlignment.Distributed;
                //ws.Column(5).Style.Font.Size = 11;
                ws.Column(5).Width = 90;
                ws.Column(6).Width = 16;
                //ws.Column(7).Width = 12;
                //ws.Column(8).Width = 14;


                //ws.Column(9).Width = 14;
                //ws.Column(10).Width = 14;
                //ws.Column(11).Width = 14;
                //ws.Column(12).Width = 14;

                //ws.Column(13).Width = 59;
                //ws.Column(13).Style.Font.Size = 9;
                //ws.Column(13).Style.WrapText = true;
                //ws.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                //ws.Column(13).Style.VerticalAlignment = ExcelVerticalAlignment.Distributed;
                //ws.Column(14).Width = 59;
                //ws.Column(14).Style.Font.Size = 9;
                //ws.Column(14).Style.WrapText = true;
                //ws.Column(14).Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                //ws.Column(14).Style.VerticalAlignment = ExcelVerticalAlignment.Distributed;
                //ws.Column(15).Width = 19;
                //ws.Column(15).Style.Numberformat.Format = "$ #,##0.00";
                //ws.Column(16).Width = 19;
                //ws.Column(16).Style.Numberformat.Format = "$ #,##0.00";
                //ws.Column(17).Width = 14;
                //ws.Column(18).Width = 14;
                //ws.Column(18).Width = 14;
                //ws.Column(19).Width = 14;
                //ws.Column(19).Style.Numberformat.Format = "$ #,##0.00";
                //ws.Column(20).Width = 14;
                //ws.Column(20).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Column(21).Width = 18;
                //ws.Column(21).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Column(22).Width = 14;
                //ws.Column(22).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //ws.Column(23).Width = 14;
                //ws.Column(23).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(24).Width = 14;
                //ws.Column(24).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(25).Width = 14;
                //ws.Column(26).Width = 14;
                //ws.Column(26).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(27).Width = 14;
                //ws.Column(27).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                //ws.Column(28).Width = 14;
                //ws.Column(28).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(29).Width = 14;
                //ws.Column(29).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                //ws.Column(30).Width = 14;
                //ws.Column(30).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(31).Width = 14;
                //ws.Column(31).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                //ws.Column(32).Width = 14;
                //ws.Column(32).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(33).Width = 14;
                //ws.Column(33).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                //ws.Column(34).Width = 14;
                //ws.Column(34).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(35).Width = 14;
                //ws.Column(35).Style.Numberformat.Format = "[$-es-MX]hh:mm:ss AM/PM";
                //ws.Column(36).Width = 14;
                //ws.Column(37).Width = 14;
                //ws.Column(37).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(38).Width = 14;
                //ws.Column(38).Style.Numberformat.Format = "dd/mm/yyyy";
                //ws.Column(39).Width = 14;
                ////ws.Column().Style.Numberformat.Format = " ###0.00";


                // ACCEDER AL ARCHIVO DE CUENTAS 

                var fileBytes = pck.GetAsByteArray();
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition",
                    "attachment;filename=BD_Concursos-" + DateTime.Now.ToShortDateString() + ".xlsx");
                // REEMPLAZAR EL NOMBRE DE ARCHIVO QUE DESEA

                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";
                var sw = new StringWriter();
                Response.BinaryWrite(fileBytes);
                Response.End();
            }

            return RedirectToAction("Index");
        }
    }
}