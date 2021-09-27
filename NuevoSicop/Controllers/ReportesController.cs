using CrystalDecisions.CrystalReports.Engine;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using Root.Reports;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Controllers
{
    public class ReportesController : Controller
    {
        sicopEntities _sicop = new sicopEntities();
        ContolEntities _BDControl= new  ContolEntities();

        public ReportesModelView CargaDatos(ReportesModelView Paq)
        {
            Paq.Ejercicios = new SelectList( _sicop.VwInvitacion1.OrderBy(x=>x.ejercicio).Select(x=> new {anio =  x.ejercicio }).Distinct(),"anio","anio");

            if (Paq.IdEjercicio > 0)
            {
                Paq.Paquetes = new SelectList(_sicop.VwInvitacion1.Where(x => x.ejercicio == Paq.IdEjercicio).Select(x=>new {NumPaquete= x.NumPaquete,Paquete= x.paquete  }).Distinct().OrderBy(x => x.NumPaquete), "NumPaquete", "paquete");

            }            
            return Paq;
        }
        
        // GET: Reportes
        public ActionResult Index()
        {
            var Paq = new ReportesModelView();
            Paq = CargaDatos(Paq); 
            return View("index",Paq);
        }
        [HttpPost]
        public ActionResult Ejercicio(ReportesModelView Paq)
        {
            Paq = CargaDatos(Paq);

            return View("index",Paq );
        }

        public static Stream ToStream(string direccion, ImageFormat format)
        {
            var img = Image.FromFile(direccion);

            var ms = new MemoryStream();
            img.Save(ms, format);
            // Si tu vas a leer desde un stream, necesitas resetear la posicion al inicio.
            ms.Position = 0;
            return ms;
        }


        public List<Ren> TraeRenglones(int IdBloque)
        {
            var blq = _BDControl.Columnas.Where(x => x.IdBloque == IdBloque).Select(x=>x.Id);
            var Rens = new List<Ren>();
            foreach (var IdColumna in blq)
            {
               
                if (_BDControl.Renglon.Any(x => x.IdColumna == IdColumna))
                {
                    var datRens = _BDControl.Renglon.Where(x => x.IdColumna == IdColumna);

                    foreach (var DRen in datRens)
                    {
                        Rens.Add(new Ren
                        {
                            Id = DRen.Id,
                            Nuevo = DRen.Nuevo,
                            Alineacion = DRen.Alineacion,
                            Alto = DRen.Alto,
                            Negritas = (Boolean)DRen.Negritas,
                            Imagen = DRen.Imagen,
                            Texto = DRen.Texto,
                            Numero = (int)DRen.Numero,
                            Parametro = (int)DRen.Parametro,
                            IdFuente = (int)DRen.IdFuente,
                            NuevoR = DRen.NuevoRen,
                            Parametros = DRen.Parametros,
                            IdCol = IdColumna,
                            Ancho = DRen.Ancho,                                          
                        });

                    }


                }

            } 
            
            

            return Rens; 
        }


        public List<Col> TraeColumas(int idBloque)
        {
            var cols = new List<Col>();
            if (_BDControl.Columnas.Any(x => x.IdBloque== idBloque))
            {
                var cls = _BDControl.Columnas.Where(x => x.IdBloque == idBloque);
                foreach(var cl in cls )
                cols.Add(new Col {
                    Id= cl.Id,
                    Ancho = cl.Ancho, 
                    Multilinea = cl.MultiLinea,
                    Nombre = cl.Nombre,
                    //Renglones = new List<Ren>();
            });
            }
            return cols;
        }

        public ReportesModelView CargaReporte(ReportesModelView Rep, string Nom)
        { 
         
           
            if (_BDControl.Reportes.Any(x => x.Nombre == Nom))
            {
                var DatReporte = _BDControl.Reportes.FirstOrDefault(x => x.Nombre == Nom);
                //Rep.Report = new Repo();
                Rep.Report.Ancho = DatReporte.Ancho;
                Rep.Report.Alto = DatReporte.Alto;
                Rep.Report.Id = DatReporte.Id;
                Rep.Report.Nombre = DatReporte.Nombre;
                Rep.Report.Descripcion = DatReporte.Descripcion;

                if (_BDControl.Fuentes.Any(x => x.IdReporte == DatReporte.Id))
                {
                    var Letras = _BDControl.Fuentes.Where(x => x.IdReporte == DatReporte.Id);
                    Rep.Report.Fuentes.Clear();
                    foreach (var fuen in Letras)
                    {
                        Rep.Report.Fuentes.Add(new Fuente
                        {
                            Id = fuen.Id,
                            Tipo= fuen.Tipo,
                            Alto = fuen.Alto,
                            Nombre= fuen.Nombre,
                        });
                    }


                }
                             
                if (_BDControl.Bloques.Any(x => x.IdReporte == DatReporte.Id))
                {
                    var DatBloques = _BDControl.Bloques.Where(x => x.IdReporte == DatReporte.Id);
                    Rep.Report.Bloques.Clear();
                    foreach (var DatBloque in DatBloques)
                    {
                        Rep.Report.Bloques.Add(new Bloque
                        {
                            Id= DatBloque.Id,
                            PosIzq =DatBloque.PosicionIzquierda,
                            PosArriba= DatBloque.PosicionArriba,
                            AnchoBorde = DatBloque.AnchoBorde,
                            RGBColor = DatBloque.Color,
                            Nombre = DatBloque.Nombre,
                            Fijo = (Boolean)DatBloque.Fijo.GetValueOrDefault(false),   
                            Cols = TraeColumas(DatBloque.Id),   
                            Renglones = TraeRenglones(DatBloque.Id)                     
                        });
                    }
                }
            }
            return Rep;
        }

        public string ObtenDato(System.Collections.IEnumerable items, string parametro, int iteraccion)
        {
            var texto = "";
            var prueba = new SelectList(items , parametro, parametro);
            var Contador = 1;  
            foreach (var prue in prueba.ToList())
            {
                if (Contador == iteraccion)
                {
                    texto = prue.Text;
                }
                Contador++;
            }
            
          
            return texto;
        }

       


        public ReportesModelView CreaReporte(ReportesModelView Paq, System.Collections.IEnumerable datos)
        {
            Paq.Report = new Repo();
            Paq.Report.MiReporte = new ReportePDF();

            var DefiniH = new FontDef(Paq.Report.MiReporte, FontDef.StandardFont.Helvetica);
            var DefiniT = new FontDef(Paq.Report.MiReporte, FontDef.StandardFont.TimesRoman);
            var DefiniC = new FontDef(Paq.Report.MiReporte, FontDef.StandardFont.Courier);
            var Iteraccion = 0;
            foreach (var dato in datos)
            {   Iteraccion++;
                Paq = CargaReporte(Paq, "Invitación");
                Paq.Report.MiReporte.NewPage(Paq.Report.Alto, Paq.Report.Ancho);
                foreach (var fue in Paq.Report.Fuentes)
                {
                    if (fue.Tipo == "Helvetica")
                    {
                        fue.Prop = new FontPropMM(DefiniH, fue.Alto);
                    }
                    if (fue.Tipo == "TimesRoman")
                    {
                        fue.Prop = new FontPropMM(DefiniT, fue.Alto);
                    }
                    if (fue.Tipo == "Courier")
                    {
                        fue.Prop = new FontPropMM(DefiniC, fue.Alto);
                    }
                }
                foreach (var bl in Paq.Report.Bloques)
                {
                    if (bl.Fijo)
                    {
                       Paq.Report.MiReporte.NewPage(Paq.Report.Alto, Paq.Report.Ancho);
                    }
                    
                    bl.Layout = new ListLayoutManager(Paq.Report.MiReporte);

                    using (bl.Layout)
                    {

                        PenProp lineabde = new PenPropMM(Paq.Report.MiReporte, bl.AnchoBorde, ColorTranslator.FromHtml(bl.RGBColor));
                        bl.Layout.tlmColumnDef_Default.penProp_BorderH = lineabde;
                        bl.Layout.tlmCellDef_Default.penProp_Line = lineabde;
                        foreach (var col in bl.Cols)
                        {
                            col.TlmCol = new TlmColumnMM(bl.Layout, col.Ancho);
                            if (col.Multilinea)
                            {
                                bl.Layout.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                            }


                        }
                        bl.Layout.container_CreateMM(Paq.Report.MiReporte.page_Cur, bl.PosIzq, bl.PosArriba);
                        var CambioLinea = true;

                        //bl.Layout.NewRow();

                        foreach (var rw in bl.Renglones.OrderBy(x => x.Id))
                        {
                            //foreach (var rw in col.Renglones)
                            //{
                            switch (rw.Alineacion)
                            {
                                case 1:
                                    bl.Layout.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                                    break;
                                case 2:
                                    bl.Layout.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                                    break;
                                case 3:
                                    bl.Layout.tlmCellDef_Default.rAlignH = RepObj.rAlignRight;
                                    break;
                            }
                            if (rw.NuevoR || CambioLinea)
                            {
                                bl.Layout.NewRow();
                                CambioLinea = false;
                            }
                            if (rw.Nuevo)
                            {
                                try { bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.NewLine(); }
                                catch
                                { }
                            }


                            if (!string.IsNullOrEmpty(rw.Imagen))
                            {
                                if (rw.Imagen.Length > 1)
                                {
                                    Stream imagen = ToStream(Path.Combine(HttpContext.Server.MapPath(""), rw.Imagen), ImageFormat.Jpeg);
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.Add(new RepString(Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop, ""));
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.NewLine();
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.NewLineMM(rw.Alto);
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.Add(new RepImageMM(imagen, rw.Ancho, rw.Alto));
                                }                               

                            }

                            if (!string.IsNullOrEmpty(rw.Texto))
                            {

                                if (!string.IsNullOrEmpty(rw.Parametros))
                                {
                                    var para = rw.Parametros.Split(',');
                                    var cont = 1;
                                    foreach (var it in para)
                                    {
                                        var tex = ObtenDato(datos, it, Iteraccion);
                                        rw.Texto = rw.Texto.Replace("[" + cont + "]", tex);
                                        cont++;

                                    }

                                }


                                if (rw.Negritas)
                                {
                                    Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop.bBold = true;
                                }

                                bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.Add(new RepString(Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop, rw.Texto + " "));
                                if (rw.Negritas)
                                {
                                    Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop.bBold = false;
                                }
                            }



                        }

                    }
                }
            }
            return Paq;
        }


        public ReportesModelView CreaReporteHTML(ReportesModelView Paq, System.Collections.IEnumerable datos)
        {
            
            var Factor = 1.5;
            Paq.Report = new Repo();
            Paq.Report.MiReporte = new ReportePDF();

            var DefiniH = new FontDef(Paq.Report.MiReporte, FontDef.StandardFont.Helvetica);
            var DefiniT = new FontDef(Paq.Report.MiReporte, FontDef.StandardFont.TimesRoman);
            var DefiniC = new FontDef(Paq.Report.MiReporte, FontDef.StandardFont.Courier);
            var Iteraccion = 0;
            foreach (var dato in datos)
            {
                Iteraccion++;
                Paq = CargaReporte(Paq, "Invitación");
                Paq.Report.MiReporte.NewPage(Paq.Report.Alto, Paq.Report.Ancho);
                Paq.Htmltext += "<div  style ='width:"+ Paq.Report.Ancho*Factor + "; height:"+Paq.Report.Alto*Factor + "; border:1px solid black; display:Block; margin:5px;' >";
                
                foreach (var fue in Paq.Report.Fuentes)
                {
                    if (fue.Tipo == "Helvetica")
                    {
                        fue.Prop = new FontPropMM(DefiniH, fue.Alto);
                    }
                    if (fue.Tipo == "TimesRoman")
                    {
                        fue.Prop = new FontPropMM(DefiniT, fue.Alto);
                    }
                    if (fue.Tipo == "Courier")
                    {
                        fue.Prop = new FontPropMM(DefiniC, fue.Alto);
                    }
                }
                foreach (var bl in Paq.Report.Bloques)
                {
                   



                    if (bl.Fijo)
                    {
                         Paq.Report.MiReporte.NewPage(Paq.Report.Alto, Paq.Report.Ancho);
                        Paq.Htmltext += "</div><div  style ='width:" + Paq.Report.Ancho * Factor + "; height:" + Paq.Report.Alto * Factor + "; color:White; border:1px solid blue; margin:5px;border:1px solid black; display:Block;' > ";
                    }

                    bl.Layout = new ListLayoutManager(Paq.Report.MiReporte);

                    using (bl.Layout)
                    {

                        PenProp lineabde = new PenPropMM(Paq.Report.MiReporte, bl.AnchoBorde, ColorTranslator.FromHtml(bl.RGBColor));
                        bl.Layout.tlmColumnDef_Default.penProp_BorderH = lineabde;
                        bl.Layout.tlmCellDef_Default.penProp_Line = lineabde;
                        Paq.Htmltext += "<div style=  left:" + bl.PosIzq * Factor*-1 + "; top:" + bl.PosArriba*Factor*-1 + "; border:1px solid black; display:Block; '> ";
                        foreach (var col in bl.Cols)
                        {
                            col.TlmCol = new TlmColumnMM(bl.Layout, col.Ancho);
                            if (col.Multilinea)
                            {
                                bl.Layout.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                            }
                            Paq.Htmltext += "<div style = 'width:" + col.Ancho * Factor + " '> ";

                        }
                        bl.Layout.container_CreateMM(Paq.Report.MiReporte.page_Cur, bl.PosIzq, bl.PosArriba);

                        

                        var CambioLinea = true;

                        //bl.Layout.NewRow();

                        foreach (var rw in bl.Renglones.OrderBy(x => x.Id))
                        {
                            //foreach (var rw in col.Renglones)
                            //{
                            switch (rw.Alineacion)
                            {
                                case 1:
                                    bl.Layout.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                                    break;
                                case 2:
                                    bl.Layout.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                                    break;
                                case 3:
                                    bl.Layout.tlmCellDef_Default.rAlignH = RepObj.rAlignRight;
                                    break;
                            }
                            if (rw.NuevoR || CambioLinea)
                            {
                                bl.Layout.NewRow();
                                CambioLinea = false;
                            }
                            if (rw.Nuevo)
                            {
                                try { bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.NewLine(); }
                                catch
                                { }
                            }


                            if (!string.IsNullOrEmpty(rw.Imagen))
                            {
                                if (rw.Imagen.Length > 1)
                                {
                                    Stream imagen = ToStream(Path.Combine(HttpContext.Server.MapPath(""), rw.Imagen), ImageFormat.Jpeg);
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.Add(new RepString(Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop, ""));
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.NewLine();
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.NewLineMM(rw.Alto);
                                    bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.Add(new RepImageMM(imagen, rw.Ancho, rw.Alto));
                                    Paq.Htmltext += "<img src='" + rw.Imagen + "' style='width: " + rw.Ancho * Factor + "'>";
                                }
                            }

                            if (!string.IsNullOrEmpty(rw.Texto))
                            {

                                if (!string.IsNullOrEmpty(rw.Parametros))
                                {
                                    var para = rw.Parametros.Split(',');
                                    var cont = 1;
                                    foreach (var it in para)
                                    {
                                        var tex = ObtenDato(datos, it, Iteraccion);
                                        rw.Texto = rw.Texto.Replace("[" + cont + "]", tex);
                                        cont++;

                                    }

                                }

                                Paq.Htmltext += "<p>";
                                if (rw.Negritas)
                                {
                                    Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop.bBold = true;
                                    Paq.Htmltext += "<strong>";
                                }

                                bl.Cols.FirstOrDefault(x => x.Id == rw.IdCol).TlmCol.Add(new RepString(Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop, rw.Texto + " "));
                                Paq.Htmltext +=  rw.Texto ;
                                                                 

                                if (rw.Negritas)
                                {
                                    Paq.Report.Fuentes.FirstOrDefault(x => x.Id == rw.IdFuente).Prop.bBold = false;
                                    Paq.Htmltext += "</strong>";
                                }
                                Paq.Htmltext += "</p>";
                            }

                        }
                        foreach (var col in bl.Cols)
                        {
                            Paq.Htmltext += "</div>";
                        }

                        Paq.Htmltext += "</div> ";
                    }

                    if (bl.Fijo)
                    {
                      Paq.Htmltext += "</div>";
                    }
                    
                }
                //Paq.Htmltext += "</div>";
            }




            return Paq;              
        }




        [HttpPost]
        public ActionResult Paquetes(ReportesModelView Paq)
        {
            

         //  var datos = _sicop.VwInvitacion1.Where(x => x.ejercicio == Paq.IdEjercicio && x.NumPaquete == Paq.IdPaquete);
            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "Invitacion_Federal.rpt"));
            //rd.SetDataSource(_sicop.VwInvitacion1.Where(x => x.ejercicio == Paq.IdEjercicio && x.NumPaquete == Paq.IdPaquete).ToList());
            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();
            //try
            //{
            //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //    stream.Seek(0, SeekOrigin.Begin);
            //    return File(stream, "application/pdf", "Invitaciones.pdf");
            //}
            //catch
            //{
            //    throw;
            //}



            //Paq = CreaReporteHTML(Paq, datos);

            //RT.ResponsePDF(Paq.Report.MiReporte, System.Web.HttpContext.Current.Response);
            return View("Index", Paq); 
            //return RedirectToAction("Index");
        }
    }
}