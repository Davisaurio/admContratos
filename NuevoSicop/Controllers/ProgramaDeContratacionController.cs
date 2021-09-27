using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.WebPages;
using Microsoft.Owin.Security.Provider;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Root.Reports;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using WebGrease.Css;

namespace NuevoSicop.Controllers
{

    public class ProgramaDeContratacionController : Controller
    {

        private ContratosSevrEntities _bdCon = new ContratosSevrEntities();
        // GET: ProgramaDeContratacion
        sicop_tempEntities _bdsic = new sicop_tempEntities();


        public List<Programas> ObtenProgramas(int Year, int Prog)
        {
            var datos = new List<Programas>();

            if (Year > 0 && Prog == 0)
            {
                var pro =
                    from e in _bdsic.ESCU
                    join dop in _bdsic.DetalleOficiosProyecto
                        on new { ColA = e.id } equals new { ColA = dop.IdProyecto }
                    join dor in _bdsic.DetalleOficiosRecurso
                        on new { ColB = dop.IdDetalleOficiosRecurso } equals new { ColB = dor.Id }
                    join p in _bdsic.PGO
                        on new { ColE = (int)dor.IdRecurso } equals new { ColE = p.Clave }
                    where p.ANIO == Year
                    select new
                    {
                        e.id,
                        p.DESCRIPCION,
                        p.ANIO,
                        e.ClaveRecurso,
                        dop.IdDetalleOficiosRecurso,
                        e.CCT_TURNO,
                        dor.Oficio,
                        p.Nivel

                    };
                foreach (var item in pro.Distinct())
                {
                    datos.Add(new Programas()
                    {
                        Anio = (int)item.ANIO,
                        ClaveProyecto = item.CCT_TURNO,
                        Descripcion = item.DESCRIPCION.Length > 30 ? item.DESCRIPCION.Substring(0, 30) : item.DESCRIPCION,
                        IdProyecto = item.id,
                        IdRecurso = (int)item.ClaveRecurso,
                        Nivel = item.Nivel,
                        Oficio = item.Oficio

                    });

                }
            }

            if (Year > 0 && Prog > 0)
            {
                var pro =
                    from e in _bdsic.ESCU
                    join dop in _bdsic.DetalleOficiosProyecto
                        on new { ColA = e.id } equals new { ColA = dop.IdProyecto }
                    join dor in _bdsic.DetalleOficiosRecurso
                        on new { ColB = dop.IdDetalleOficiosRecurso } equals new { ColB = dor.Id }
                    join p in _bdsic.PGO
                        on new { ColE = (int)dor.IdRecurso } equals new { ColE = p.Clave }
                    where p.ANIO == Year && e.ClaveRecurso == Prog
                    select new
                    {
                        e.id,
                        p.DESCRIPCION,
                        p.ANIO,
                        e.ClaveRecurso,
                        dop.IdDetalleOficiosRecurso,
                        e.CCT_TURNO,
                        dor.Oficio,
                        p.Nivel

                    };
                foreach (var item in pro.Distinct())
                {
                    datos.Add(new Programas()
                    {
                        Anio = (int)item.ANIO,
                        ClaveProyecto = item.CCT_TURNO,
                        Descripcion = item.DESCRIPCION.Length > 30 ? item.DESCRIPCION.Substring(0, 30) : item.DESCRIPCION,
                        IdProyecto = item.id,
                        IdRecurso = (int)item.ClaveRecurso,
                        Nivel = item.Nivel,
                        Oficio = item.Oficio

                    });

                }
            }
            return datos;
        }

        public JsonResult CargarProyectos(int ent, int anio)
        {
            var resp = new Listas();
            var res = ObtenProgramas(anio, ent);

            resp.Proyectos =
                new SelectList(res.Select(x => new { id = x.IdProyecto, desc = x.ClaveProyecto.ToUpper() }).OrderBy(x => x.desc), "id",
                    "desc");
            return Json(resp);
        }


        public JsonResult CargarProgramas(int anio)
        {
            var resp = new Listas();
            var res = ObtenProgramas(anio, 0);
            resp.Progra = new SelectList(res.Select(x => new { id = x.IdRecurso, desc = x.Oficio + " - " + x.Nivel + " - " + x.Descripcion + "..." }).Distinct().OrderBy(x => x.desc), "id", "desc");
            return Json(resp);
        }


        //    d.url = '@Url.Action("CargarimporteAut", "ProgramaDeContratacion")';
        //                    d.type = "POST";
        //                    d.data = JSON.stringify({ ent: ind, anio: year, proy:pr
        //});

        public JsonResult CargarimporteAut(int proy)
        {
            double resp = 0.0;
            if (_bdsic.ESCU.Any(x => x.id == proy))
            {
                resp = (double)_bdsic.ESCU.FirstOrDefault(x => x.id == proy).importe_autorizado.GetValueOrDefault(0);
            }


            return Json(resp);
        }

        public ProgramaContratacionModelView LlenarDatos(ProgramaContratacionModelView Pro)
        {

            var TipoInvesion = Pro.IdTipoInversion;
            Pro.ListaEjercicios = new SelectList(_bdsic.PGO.Select(x => new { Anio = x.ANIO }).Distinct().OrderBy(x => x.Anio), "Anio", "Anio");
            var pqmax = Pro.IdEjercicio > 0 ? _bdsic.PaquetesProcedimiento.Where(x => x.Anio == Pro.IdEjercicio).Max(x => x.Paquete) : 0;
            //var pqmax = Pro.IdEjercicio>0? _bdsic.PaquetesProcedimiento.Where(x => x.Anio == Pro.IdEjercicio).Max(x => x.Paquete):0;
            if (Pro.IdEjercicio >= 1)
            {
                Pro.Paquetes = new SelectList(_bdsic.PaquetesProcedimiento.Where(x => x.Anio == Pro.IdEjercicio && x.Paquete != null).Select(x => new { Paq = x.idPaquete, Desc = (x.Descripcion).ToString() }).OrderBy(x => x.Desc), "Paq", "Desc");


            }

            if (_bdsic.PaquetesProcedimiento.Any(x => x.idPaquete == Pro.IdPaquete && x.Anio == Pro.IdEjercicio))
            {


                var pq = _bdsic.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == Pro.IdPaquete && x.Anio == Pro.IdEjercicio);


                var Rec = ObtenProgramas(Pro.IdEjercicio, 0);




                Pro.Recursos = new SelectList(Rec.Select(x => new { id = x.IdRecurso, desc = x.Oficio + " - " + x.Nivel + " - " + x.Descripcion + "..." }).Distinct().OrderBy(x => x.desc), "id", "desc");


                //var Proy = Pro.IdRecurso>=1?_bdsic.ESCU.Join(_bdsic.)

                //if (Pro.IdRecurso >= 1) 
                //{

                //}


                var pcont = _bdsic.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Pro.IdPaquete);

                var legal = new Fundamento();          
                    legal.Estatal = new SelectList(_bdCon.Fundamentos.Where(x => x.TipoFundamento == "ESTATAL"), "Id","Fundamento");
                    legal.Federal = new SelectList(_bdCon.Fundamentos.Where(x => x.TipoFundamento == "FEDERAL"), "Id","Fundamento");            
                Pro.Paq = new PaquetePy();
                Pro.Paq.Anio = pq.Anio.GetValueOrDefault(0);
                Pro.Paq.Descripcion = pq.Descripcion;
                Pro.Paq.NoProcedimiento = pq.NoProcedimiento;
                Pro.Paq.NumPaquete = pq.Paquete.GetValueOrDefault(0);
                Pro.PaqNuevo.NumPaquete = pqmax.GetValueOrDefault(0) + 1;
                Pro.PaqNuevo.Anio = pq.Anio.GetValueOrDefault(0);
                Pro.Paq.Recuperacion = (decimal)pq.Recuperacion.GetValueOrDefault(0);

                Pro.Modalidad = new SelectList(_bdsic.Modalidad, "Clave_Modalidad", "NomModalidad");
                Pro.ListaServidores = new SelectList(_bdsic.DC.OrderBy(x => x.Nombre), "Clave", "Nombre");
                Pro.ListaLeyenda = new SelectList(_bdsic.Leyendas, "id", "Leyendas1");



                Pro.ConvocaInvitacion = (DateTime)pq.fchConvocatoria.GetValueOrDefault(DateTime.Now);
                Pro.VentaBases = ObtenHora((DateTime)pq.fchLimiteInscripcion.GetValueOrDefault(DateTime.Now), pq.hrLimiteInscripcion);
                Pro.Visita = ObtenHora((DateTime)pq.fchVisitaSitio.GetValueOrDefault(DateTime.Now), pq.hrVisitaSitio);
                Pro.JuntaAclaraciones = ObtenHora((DateTime)pq.fchJuntaAclaracion.GetValueOrDefault(DateTime.Now), pq.hrJuntaAclaracion);
                Pro.Apertura = ObtenHora((DateTime)pq.fchAperturaTecnicaEconomica.GetValueOrDefault(DateTime.Now), pq.hrAperturaTecnicaEconomica);
                Pro.Fallo = ObtenHora((DateTime)pq.fchFallo.GetValueOrDefault(DateTime.Now), pq.hrFallo);
                Pro.Firma = ObtenHora((DateTime)pq.fchContrato.GetValueOrDefault(DateTime.Now), pq.hrContrato);
                Pro.FechaInicio = (DateTime)pq.FechaInicioEstimada.GetValueOrDefault(DateTime.Now);

                if (pcont != null)
                {
                    Pro.ProgramaContratacion = (DateTime)pcont.Fechaoficio.GetValueOrDefault(DateTime.Now);
                    // Pro.ProgramaContratacion = (DateTime)pcont.Fechaoficio == null ? DateTime.MinValue : pcont.Fechaoficio.GetValueOrDefault(DateTime.Now);
                    Pro.IdReviso = pcont.Reviso != null ? pcont.Reviso.GetValueOrDefault(0) : 0;
                    Pro.IdVoBo = pcont.VoBo != null ? pcont.VoBo.GetValueOrDefault(0) : 0;
                    Pro.IdAutorizo = pcont.Autorizo != null ? pcont.Autorizo.GetValueOrDefault(0) : 0;
                    Pro.IdTipoInversion = TipoInvesion == null ? pcont.TipoInversion : TipoInvesion;
                    Pro.IdMOdalidad = pcont.Modalidad.GetValueOrDefault(0);

                    Pro.FundamentoLegalDesc = pcont.Articulo;
                }
                Pro.TipoInversion = new SelectList(new List<string>() { "ESTATAL", "FEDERAL" });


                switch (Pro.IdTipoInversion)
                {
                    case "ESTATAL":
                        {
                            Pro.FundamentoLegal = legal.Estatal;
                        }
                        break;
                    case "FEDERAL":
                        {
                            Pro.FundamentoLegal = legal.Federal;
                        }
                        break;
                }

                if (_bdsic.DetallePaqueteProyecto.Any(x => x.IdPaquete == Pro.IdPaquete))
                {
                    var paquetes = _bdsic.DetallePaqueteProyecto.Where(x => x.IdPaquete == Pro.IdPaquete);
                    foreach (var Elemento in paquetes)
                    {
                        var py = _bdsic.ESCU.FirstOrDefault(x => x.id == Elemento.IdProyecto);
                        Pro.Paq.Proy.Add(new Proyecto()
                        {
                            Id = Elemento.Id,
                            Descripcion = py.NOMESC,
                            PlazoEjecucion = Elemento.PlazoEjecucion.GetValueOrDefault(0),
                            Anticipo = Elemento.Anticipo.GetValueOrDefault(0),
                            ImporteAutorizado = Elemento.ImporteAutorizado.GetValueOrDefault(0),
                            CapitalContable = Elemento.CapitalContable.GetValueOrDefault(0),

                        });
                    }
                }
            }







            return Pro;
        }

        public DateTime ObtenHora(DateTime fecha, string stext)
        {
            var fechaN = new DateTime();
            fechaN = DateTime.Now;
            if (!string.IsNullOrEmpty(stext))
            {
                DateTime tiempo;
                try
                {
                    tiempo = Convert.ToDateTime(stext);
                }
                catch (Exception)
                {

                    tiempo = DateTime.MinValue;
                }
                fechaN = fecha.Date + tiempo.TimeOfDay;
            }
            return fechaN;
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        public ActionResult Index()
        {
            var pro = new ProgramaContratacionModelView();
            pro.IdEjercicio = 2019;
            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Ejercicio(ProgramaContratacionModelView pro)
        {

            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Paquete(ProgramaContratacionModelView pro)
        {
            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult TipoInvesion(ProgramaContratacionModelView pro)
        {
            if (_bdsic.ProgramaDeContratacion.Any(x => x.IdPaquete == pro.IdPaquete))
            {
                var pcont = _bdsic.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == pro.IdPaquete);
                pcont.TipoInversion = pro.IdTipoInversion;
                pcont.Articulo = "";

                _bdsic.SaveChanges();
            }
            else
            {

                _bdsic.ProgramaDeContratacion.Add(new ProgramaDeContratacion()
                {
                    IdPaquete = pro.IdPaquete,
                    TipoInversion = pro.IdTipoInversion,
                    Articulo = ""
                });
                _bdsic.SaveChanges();
            }
            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Busqueda(ProgramaContratacionModelView pro)
        {
            var pcmv = pro.BuscarProyecto;
            if (_bdsic.ESCU.Any(x => x.CCT_TURNO == pro.BuscarProyecto))
            {
                var py = _bdsic.ESCU.FirstOrDefault(x => x.CCT_TURNO == pcmv).id;
                var pq = _bdsic.DetallePaqueteProyecto.FirstOrDefault(x => x.IdProyecto == py);
                var ejercicio = _bdsic.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == pq.IdPaquete);

                pro.IdEjercicio = (int)ejercicio.Anio;
                pro.IdPaquete = pq.IdPaquete;
            }
            pro = LlenarDatos(pro);

            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaPaquete(ProgramaContratacionModelView pro)
        {
            try
            {
                if (pro.PaqNuevo.Anio > 0 && !string.IsNullOrEmpty(pro.PaqNuevo.NoProcedimiento) &&
                    pro.PaqNuevo.NumPaquete > 0)
                {
                    _bdsic.PaquetesProcedimiento.Add(new PaquetesProcedimiento()
                    {
                        Anio = pro.PaqNuevo.Anio,
                        Paquete = pro.PaqNuevo.NumPaquete,
                        Descripcion = "PAQUETE NO. " + pro.PaqNuevo.NumPaquete.ToString("000"),
                        NoProcedimiento = pro.PaqNuevo.NoProcedimiento,
                        Recuperacion = pro.PaqNuevo.Recuperacion 

                    });
                    _bdsic.SaveChanges();
                    pro.IdEjercicio = pro.PaqNuevo.Anio;
                    pro.IdPaquete = _bdsic.PaquetesProcedimiento.FirstOrDefault(x => x.Anio == pro.PaqNuevo.Anio && x.Paquete == pro.PaqNuevo.NumPaquete).idPaquete;
                    pro = LlenarDatos(pro);
                    pro.Exito = "Se Guardo el Paquete Correctamente";
                }
                else
                {

                    pro.IdEjercicio = pro.PaqNuevo.Anio;
                    pro = LlenarDatos(pro);
                    pro.Exito = "Faltan datos favor de revisar";
                }




            }
            catch
            {
                pro.IdEjercicio = pro.PaqNuevo.Anio;
                pro = LlenarDatos(pro);
                pro.Exito = "Error al Guardar los datos favor de revisar";
            }

            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaPaquete(ProgramaContratacionModelView pro)
        {
            if (_bdsic.PaquetesProcedimiento.Any(x => x.idPaquete == pro.IdPaquete))
            {
                var paq = _bdsic.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == pro.IdPaquete);
                try
                {
                    _bdsic.PaquetesProcedimiento.Remove(paq);
                    _bdsic.SaveChanges();
                    pro = LlenarDatos(pro);
                    pro.Exito = "Se elimino correctamente el paquete seleccionado";
                }
                catch
                {
                    pro = LlenarDatos(pro);
                    pro.Exito = "Se encontró un problema al traetar de eliminar el Paquete";
                }
            }
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Recursos(ProgramaContratacionModelView pro)
        {
            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult CargaProyectos(ProgramaContratacionModelView pro)
        {
            pro = LlenarDatos(pro);
            return View("index", pro);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult GuardarPrograma(ProgramaContratacionModelView pro)
        {

            if (_bdsic.PaquetesProcedimiento.Any(x => x.idPaquete == pro.IdPaquete) && _bdsic.ProgramaDeContratacion.Any(x => x.IdPaquete == pro.IdPaquete))
            {
                var datos = _bdsic.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == pro.IdPaquete);
                var pcon = _bdsic.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == pro.IdPaquete);

                pcon.Fechaoficio = pro.ProgramaContratacion;
                datos.fchConvocatoria = pro.ConvocaInvitacion;
                datos.fchLimiteInscripcion = pro.VentaBases;
                datos.fchVisitaSitio = pro.Visita;
                datos.fchJuntaAclaracion = pro.JuntaAclaraciones;
                datos.fchAperturaTecnicaEconomica = pro.Apertura;
                datos.fchFallo = pro.Fallo;
                datos.fchContrato = pro.Firma;
                datos.FechaInicioEstimada = pro.FechaInicio;
                datos.hrLimiteInscripcion = pro.VentaBases.ToString("HH:mm");
                datos.hrVisitaSitio = pro.Visita.ToString("HH:mm");
                datos.hrAperturaTecnicaEconomica = pro.Apertura.ToString("HH:mm");
                datos.hrFallo = pro.Fallo.ToString("HH:mm");
                datos.hrContrato = pro.Firma.ToString("HH:mm");
                datos.hrJuntaAclaracion = pro.JuntaAclaraciones.ToString("HH:mm");
                _bdsic.SaveChanges();

            }






            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult GuardarPrograma2(ProgramaContratacionModelView pro)
        {
            if (pro.IdReviso > 0 && pro.IdAutorizo > 0 && pro.IdVoBo > 0)
            {

                if (_bdsic.ProgramaDeContratacion.Any(x => x.IdPaquete == pro.IdPaquete))
                {
                    var pcont = _bdsic.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == pro.IdPaquete);


                    pcont.Reviso = pro.IdReviso;
                    pcont.Autorizo = pro.IdAutorizo;
                    pcont.VoBo = pro.IdVoBo;

                    _bdsic.SaveChanges();

                }
                pro = LlenarDatos(pro);
                pro.Exito = "Se Guardaron las firmas correctamente";
                return View("index", pro);
            }
            else
            {
                pro = LlenarDatos(pro);
                pro.Error = "Falto seleccionar una o mas firmas, favor de revisar ";
                return View("index", pro);


            }


        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Modalidad(ProgramaContratacionModelView pro)
        {
            if (_bdsic.ProgramaDeContratacion.Any(x => x.IdPaquete == pro.IdPaquete))
            {
                var pcont = _bdsic.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == pro.IdPaquete);
                pcont.Modalidad = pro.IdMOdalidad;
                _bdsic.SaveChanges();
            }



            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult FundamentoLegal(ProgramaContratacionModelView pro)
        {
            if (_bdsic.ProgramaDeContratacion.Any(x => x.IdPaquete == pro.IdPaquete))
            {
                var pcont = _bdsic.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == pro.IdPaquete);
                var fund = _bdCon.Fundamentos.FirstOrDefault(x => x.Id == pro.IdFundamento);
                pcont.Articulo = fund.Fundamento;
                pcont.cveArticulo = 2;
                _bdsic.SaveChanges();
            }
            else
            {


            }

            pro = LlenarDatos(pro);
            return View("index", pro);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaProyecto(ProgramaContratacionModelView pro)
        {
            var Ejer = pro.IdEjercicio;
            var paq = pro.IdPaquete;
            if (pro.AsignaProyecto.PlazoEjecucion <= 0 || pro.AsignaProyecto.Anticipo < 0 ||
                pro.AsignaProyecto.ImporteAutorizado <= 0)
            {
                pro.IdPaquete = paq;
                pro.IdEjercicio = Ejer;
                pro = LlenarDatos(pro);
                pro.Error = "No Fue posible agreggar proyecto, algunos de sus datos estan vacios o no tienen valor valido.";
                return View("Index", pro);
            }
            else
            {
                try
                {
                    _bdsic.DetallePaqueteProyecto.Add(new DetallePaqueteProyecto()
                    {
                        IdPaquete = pro.IdPaquete,
                        IdProyecto = pro.AsignaProyecto.IdProyecto,
                        PlazoEjecucion = pro.AsignaProyecto.PlazoEjecucion,
                        Anticipo = (int)pro.AsignaProyecto.Anticipo,
                        ImporteAutorizado = pro.AsignaProyecto.ImporteAutorizado,
                        CapitalContable = pro.AsignaProyecto.CapitalContable,
                        Residente = 600
                    });
                    _bdsic.SaveChanges();
                    pro.IdPaquete = paq;
                    pro.IdEjercicio = Ejer;
                    pro = LlenarDatos(pro);
                    pro.Exito = "Se agrego el proyecto Correctamente";
                    return View("Index", pro);
                }
                catch (Exception)
                {
                    pro.Error = "No fue posible Agregar el proyecto favor de verificar sus datos";
                    return RedirectToAction("Index");
                }
            }


        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaProyecto(ProgramaContratacionModelView pro)
        {
            var Ejer = pro.IdEjercicio;
            var paq = pro.IdPaquete;

            if (_bdsic.DetallePaqueteProyecto.Any(x => x.Id == pro.IdEliminaProyecto))
            {
                var datoElim = _bdsic.DetallePaqueteProyecto.FirstOrDefault(x => x.Id == pro.IdEliminaProyecto);
                _bdsic.DetallePaqueteProyecto.Remove(datoElim);
                _bdsic.SaveChanges();
                pro.IdPaquete = paq;
                pro.IdEjercicio = Ejer;
                pro = LlenarDatos(pro);
                pro.Exito = "Se eliminó correctamente el proyecto";
                return View("index", pro);
            }
            else
            {
                pro.IdPaquete = paq;
                pro.IdEjercicio = Ejer;
                pro = LlenarDatos(pro);
                pro.Exito = "No es posible eliminar El proyecto";
                return View("index", pro);

            }

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


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult ProgramaContratacion(ProgramaContratacionModelView pro)
        {

            pro = LlenarDatos(pro);



            var ProgramaContratacion = new ReportePDF();
            ProgramaContratacion.NewPageVertical();
            var fuente = new FontDef(ProgramaContratacion, FontDef.StandardFont.Helvetica);
            FontProp fp1 = new FontPropMM(fuente, 1.75);
            FontProp fp2 = new FontPropMM(fuente, 2.5);


            var logoitife = ToStream(Path.Combine(HttpContext.Server.MapPath("../img/"), "LOGOITIFE1.jpg"), ImageFormat.Jpeg);
            ListLayoutManager Datos1 = null;
            using (Datos1 = new ListLayoutManager(ProgramaContratacion))
            {
                PenProp lineaBorde = new PenPropMM(ProgramaContratacion, 0.001, Color.Transparent);
                Datos1.tlmColumnDef_Default.penProp_BorderH = lineaBorde;
                Datos1.tlmCellDef_Default.penProp_Line = lineaBorde;
                TlmColumn col_partidas = new TlmColumnMM(Datos1, 190);
                col_partidas.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                //col_partidas.tlmCellDef_Default.rAlignV = RepObj.p;
                col_partidas.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;

                Datos1.container_CreateMM(ProgramaContratacion.page_Cur, 10, 10);
                Datos1.NewRow();
                col_partidas.Add(new RepString(fp1, ""));
                col_partidas.NewLine();
                col_partidas.NewLineMM(35);
                col_partidas.Add(new RepImageMM(logoitife, 50, 30));
                Datos1.NewRow();
                col_partidas.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                Datos1.NewRow();
                fp2.bBold = true;
                col_partidas.Add(new RepString(fp2, "PROGRAMA DE FECHAS DE LICITACIÓN "));
                fp2.bBold = false;
                col_partidas.tlmCellDef_Default.rAlignH = RepObj.rAlignRight;
                Datos1.NewRow();
                col_partidas.Add(new RepString(fp1, "VILLAHERMOSA, TAB., A " + pro.ProgramaContratacion.ToLongDateString().ToUpper()));
                col_partidas.NewLine();
                col_partidas.Add(new RepString(fp1, ""));
                Datos1.NewRow();
                col_partidas.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                Datos1.NewRow();

                col_partidas.Add(new RepString(fp1, ""));
                col_partidas.NewLine();

                col_partidas.Add(new RepString(fp2,
                    "LICITACIÓN NO. " + pro.Paq.NoProcedimiento + "                            PAQUETE NO." +
                    pro.Paq.NumPaquete.ToString("00#")));
                foreach (var item in pro.Paq.Proy)
                {
                    Datos1.NewRow();
                    col_partidas.Add(new RepString(fp1, ""));
                    col_partidas.NewLine();
                    col_partidas.Add(new RepString(fp2, item.Descripcion));
                }
            }
            ListLayoutManager Datos2 = null;
            using (Datos2 = new ListLayoutManager(ProgramaContratacion))
            {
                PenProp lineaBorde = new PenPropMM(ProgramaContratacion, 0.001, Color.Black);
                Datos2.tlmColumnDef_Default.penProp_BorderH = lineaBorde;
                Datos2.tlmCellDef_Default.penProp_Line = lineaBorde;
                TlmColumn col1 = new TlmColumnMM(Datos2, 60);
                col1.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col1.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn col2 = new TlmColumnMM(Datos2, 30);
                col2.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col2.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn col3 = new TlmColumnMM(Datos2, 45);
                col3.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col3.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn col4 = new TlmColumnMM(Datos2, 45);
                col4.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col4.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                Datos2.container_CreateMM(ProgramaContratacion.page_Cur, 10, Datos1.rCurY_MM + 20);
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "EVENTO"));
                col2.Add(new RepString(fp1, "DÍA"));
                col3.Add(new RepString(fp1, "FECHA"));
                col4.Add(new RepString(fp1, "HORA"));
                fp1.bBold = false;
                col1.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                col2.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                col3.tlmCellDef_Default.rAlignH = RepObj.rAlignRight;
                col4.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;

                Datos2.NewRow();
                var f1 = pro.ProgramaContratacion.ToLongDateString().ToUpper().Split(',');

                col1.Add(new RepString(fp1, "OFICIO DE INVITACIÓN"));
                col2.Add(new RepString(fp1, f1[0]));
                col3.Add(new RepString(fp1, f1[1]));
                col4.Add(new RepString(fp1, ""));
                Datos2.NewRow();
                var f2 = pro.VentaBases.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "LIMITE VENTA DE BASES "));
                col2.Add(new RepString(fp1, f2[0]));
                col3.Add(new RepString(fp1, f2[1]));
                col4.Add(new RepString(fp1, pro.VentaBases.TimeOfDay.ToString()));
                Datos2.NewRow();
                var f3 = pro.Visita.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "VISITA AL LUGAR DE LA OBRA "));
                col2.Add(new RepString(fp1, f3[0]));
                col3.Add(new RepString(fp1, f3[1]));
                col4.Add(new RepString(fp1, pro.Visita.TimeOfDay.ToString()));
                Datos2.NewRow();
                var f4 = pro.JuntaAclaraciones.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "JUNTA ACLARATORIA "));
                col2.Add(new RepString(fp1, f4[0]));
                col3.Add(new RepString(fp1, f4[1]));
                col4.Add(new RepString(fp1, pro.JuntaAclaraciones.TimeOfDay.ToString()));
                Datos2.NewRow();
                var f5 = pro.Apertura.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "APERTURA DE PROPOSICIONES"));
                col2.Add(new RepString(fp1, f5[0]));
                col3.Add(new RepString(fp1, f5[1]));
                col4.Add(new RepString(fp1, pro.Apertura.TimeOfDay.ToString()));
                Datos2.NewRow();
                var f6 = pro.Fallo.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "ADJUDICAIÓN Y FALLO"));
                col2.Add(new RepString(fp1, f6[0]));
                col3.Add(new RepString(fp1, f6[1]));
                col4.Add(new RepString(fp1, pro.Fallo.TimeOfDay.ToString()));
                Datos2.NewRow();
                var f7 = pro.Firma.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "FIRMA DE COTRATO Y PRES. GARANTIAS."));
                col2.Add(new RepString(fp1, f7[0]));
                col3.Add(new RepString(fp1, f7[1]));
                col4.Add(new RepString(fp1, pro.Firma.TimeOfDay.ToString()));
                Datos2.NewRow();
                var f8 = pro.FechaInicio.ToLongDateString().ToUpper().Split(',');
                col1.Add(new RepString(fp1, "INICIO PROBABLE DE LAS OBRAS"));
                col2.Add(new RepString(fp1, f8[0]));
                col3.Add(new RepString(fp1, f8[1]));
                col4.Add(new RepString(fp1, ""));
            }


            ListLayoutManager Datos3 = null;
            using (Datos3 = new ListLayoutManager(ProgramaContratacion))
            {
                PenProp lineaBorde = new PenPropMM(ProgramaContratacion, 0.001, Color.Transparent);
                Datos3.tlmColumnDef_Default.penProp_BorderH = lineaBorde;
                Datos3.tlmCellDef_Default.penProp_Line = lineaBorde;
                TlmColumn col1 = new TlmColumnMM(Datos3, 190);
                col1.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col1.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                Datos3.container_CreateMM(ProgramaContratacion.page_Cur, 10, Datos1.rCurY_MM + Datos2.rCurY_MM + 20);
                Datos3.NewRow();

                foreach (var item in pro.Paq.Proy)
                {
                    Datos3.NewRow();
                    col1.Add(new RepString(fp1,
                        "DURACION DE LA OBRA " + item.Descripcion.Split('-')[0] + ", " + item.PlazoEjecucion +
                        " DÍAS NATURALES, ANTICIPO DE LA OBRA: " + item.Anticipo + "%"));
                    Datos3.NewRow();
                    col1.Add(new RepString(fp1, " "));

                }


            }



            ListLayoutManager Datos4 = null;
            using (Datos4 = new ListLayoutManager(ProgramaContratacion))
            {
                PenProp lineaBorde = new PenPropMM(ProgramaContratacion, 0.001, Color.Transparent);
                Datos4.tlmColumnDef_Default.penProp_BorderH = lineaBorde;
                Datos4.tlmCellDef_Default.penProp_Line = lineaBorde;
                TlmColumn col1 = new TlmColumnMM(Datos4, 63);
                col1.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col1.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn col2 = new TlmColumnMM(Datos4, 63);
                col2.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col2.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn col3 = new TlmColumnMM(Datos4, 64);
                col3.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col3.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                Datos4.container_CreateMM(ProgramaContratacion.page_Cur, 10, 200 + 20);
                Datos4.NewRow();



                col1.Add(new RepString(fp1, "REVISÓ"));
                col2.Add(new RepString(fp1, "Vo. Bo.:"));
                col3.Add(new RepString(fp1, "AUTORIZÓ"));
                Datos3.NewRow();
                col1.NewLine();
                col2.NewLine();
                col3.NewLine();
                col1.Add(new RepString(fp1, ""));
                col2.Add(new RepString(fp1, ""));
                col3.Add(new RepString(fp1, ""));
                col1.NewLineMM(20);
                col2.NewLineMM(20);
                col3.NewLineMM(20);
                Datos3.NewRow();
                var Rev = _bdsic.DC.FirstOrDefault(x => x.Clave == pro.IdReviso);
                var vobo = _bdsic.DC.FirstOrDefault(x => x.Clave == pro.IdVoBo);
                var aut = _bdsic.DC.FirstOrDefault(x => x.Clave == pro.IdAutorizo);
                if (Rev != null)
                {
                    col1.Add(new RepString(fp1, Rev.Nombre));
                }
                if (vobo != null)
                {
                    col2.Add(new RepString(fp1, vobo.Nombre));
                }
                if (aut != null)
                {
                    col3.Add(new RepString(fp1, aut.Nombre));
                }

                col1.NewLine();
                col2.NewLine();
                col3.NewLine();
                if (Rev != null)
                {
                    col1.Add(new RepString(fp1, Rev.Cargo));
                }
                if (vobo != null)
                {
                    col2.Add(new RepString(fp1, vobo.Cargo));
                }
                if (aut != null)
                {
                    col3.Add(new RepString(fp1, aut.Cargo));
                }






            }


            RT.ResponsePDF(ProgramaContratacion, System.Web.HttpContext.Current.Response);
            return RedirectToAction("Index");
        }

    }
}