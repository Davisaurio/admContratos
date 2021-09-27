using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using NuevoSicop.Controllers; 

namespace NuevoSicop.Controllers
{

    public class DictamenController : Controller
    {

       private readonly sicop_tempEntities _sicop = new sicop_tempEntities();
        // GET: Dictamen

        public ActionResult Index()
        {
            var datos  = new DictamenModelView();
            datos = CargaDatos(datos);
            return View(datos);
        }
        public DictamenModelView CargaDatos(DictamenModelView dat)
        {
            var ejercicios = _sicop.PaquetesProcedimiento.Select(x => new { anio = x.Anio }).Distinct().OrderBy(x => x.anio);
        
            dat.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");
            if (dat.IdEjercicio > 0)
            {
                var paquetes =
                    _sicop.PaquetesProcedimiento.Where(x => x.Anio == dat.IdEjercicio)
                        .Select(x => new {NumPaquete = x.idPaquete, Paquete = x.Descripcion})
                        .Distinct()
                        .OrderBy(x => x.Paquete);
                dat.ListaPaquetes = new SelectList(paquetes, "NumPaquete", "paquete");
            }
            if (dat.IdPaquete > 0)
            {
                dat.VerDictamen = _sicop.VwFallos.Any(x => x.ejercicio == dat.IdEjercicio && x.idPaquete == dat.IdPaquete);
                var personal =
                   _sicop.DC.Select(x => new { Id = x.Clave, Nombre = x.Nombre + " - " + x.Cargo })
                       .OrderBy(x => x.Nombre)
                       .Distinct();
                dat.ListaPersonal = new SelectList(personal.OrderBy(x => x.Nombre), "Id", "Nombre");
                var Proys = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == dat.IdPaquete);
                foreach (var Pro in Proys)
                {
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == Pro.IdProyecto);
                    var resid = _sicop.SUPERVIS.FirstOrDefault(x => x.CLAVE == Pro.Residente).NOMBRE;
                    dat.ListaProyectos.Add(new ProyInv()
                    {
                        Clave = py.CCT_TURNO,
                        DescripcionObra = py.NOMESC,
                        PlazoEjecucion = Pro.PlazoEjecucion.GetValueOrDefault(0),
                        Anticipo = Pro.Anticipo.GetValueOrDefault(0),
                        ImporteAut = Pro.ImporteAutorizado.GetValueOrDefault(0),
                        CapitalContable = Pro.CapitalContable.GetValueOrDefault(0),
                        Residente = resid,

                    });


                }

                if (_sicop.PaquetesProcedimiento.Any(x => x.idPaquete == dat.IdPaquete))
                {
                    var paquete = _sicop.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == dat.IdPaquete);
                    var programa = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == dat.IdPaquete);
                    var modalidad = _sicop.Modalidad.FirstOrDefault(x => x.Clave_Modalidad == programa.Modalidad).NomModalidad;
                    dat.Anio = paquete.Anio.GetValueOrDefault(0);
                    dat.NumPaquete = paquete.Paquete.GetValueOrDefault(0);
                    dat.DescPaquete = paquete.Descripcion;
                    dat.Procedimiento = paquete.NoProcedimiento;
                    dat.Recuperacion = paquete.Recuperacion.GetValueOrDefault(0);
                    dat.Modalidad = modalidad;
                }

                if (_sicop.Dictamen.Any(x => x.IdPaquete == dat.IdPaquete))
                {
                    var firmas = _sicop.Dictamen.FirstOrDefault(x => x.IdPaquete == dat.IdPaquete);
                    dat.Aprobo  = firmas.Aprobo.GetValueOrDefault(0);
                    dat.Elaboro = firmas.Elaboro.GetValueOrDefault(0);
                    dat.Reviso  = firmas.Reviso.GetValueOrDefault(0);
                }

                var proy = _sicop.VwFallos.Where(x => x.ejercicio == dat.IdEjercicio && x.idPaquete == dat.IdPaquete).Select(x => new { x.Proyecto }).Distinct();
                //dat.ListaProyectos = new SelectList(proy, "Proyecto", "Proyecto");

                foreach (var it in proy)
                {
                    dat.Proyectos.Add(it.Proyecto);
                }
            }            
            return (dat);
        }


        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult Ejercicio(DictamenModelView dat)
        {
            dat = CargaDatos(dat);

            return View("Index", dat);
        }


        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult Paquetes(DictamenModelView dat)
        {
            dat = CargaDatos(dat);

            return View("Index", dat);
        }

        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult IniciaActa(DictamenModelView dat)
        {
            dat = CargaDatos(dat);
            


            return View("Index", dat);
        }


        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult GuardaFirmas(DictamenModelView dat)
        {
            var Error = "";
            var Exito = "";
            try
            {
                if (_sicop.Dictamen.Any(x => x.IdPaquete == dat.IdPaquete))
                {
                    var datos = _sicop.Dictamen.FirstOrDefault(x => x.IdPaquete == dat.IdPaquete);
                    datos.Aprobo = dat.Aprobo;
                    datos.Elaboro = dat.Elaboro;
                    datos.Reviso = dat.Reviso;
                    _sicop.SaveChanges();
                    Exito = "Se actualizaron correctamente las Firmas";
                }
                else
                {
                    _sicop.Dictamen.Add(new Dictamen()
                    {
                        Aprobo = dat.Aprobo,
                        Elaboro = dat.Elaboro,
                        Reviso = dat.Reviso,
                        IdPaquete = dat.IdPaquete,
                    });
                    _sicop.SaveChanges();
                    Exito = "Se guardaron correctamente las firmas";
                }                                
            }
            catch (Exception)
            {

                Error = "Problemas al momento de guardar la firma"; 
            }
            
                
            dat = CargaDatos(dat);
            dat.Error = Error;
            dat.Exito = Exito; 
            return View("Index", dat);
        }

        public List<ReporteFallos> LlenaDictamen(int ejercicio, int idPaquete, string Proyecto)
        {
            var Rep = new List<ReporteFallos>();
            var falloactual = _sicop.VwFallos.Where(x => x.ejercicio == ejercicio && x.idPaquete == idPaquete && x.Proyecto == Proyecto);
            foreach (var item in falloactual)
            {
                Rep.Add(new ReporteFallos()
                {
                    idPaquete = item.idPaquete,
                    Paquete = item.Paquete.GetValueOrDefault(0),
                    descObrafallo = item.descObrafallo,
                    NoProcedimiento = item.NoProcedimiento,
                    Orden = item.Orden.GetValueOrDefault(0),
                    Empresa = item.Empresa,
                    Asistente = item.Asistente,
                    Modalidad = item.Modalidad,
                    fechaautorizacion = item.fechaautorizacion.GetValueOrDefault(DateTime.MinValue),
                    FechaCita = item.FechaCita.GetValueOrDefault(DateTime.MinValue),
                    FechaInicio = item.FechaInicio.GetValueOrDefault(DateTime.MinValue),
                    FechaTermino = item.FechaTermino.GetValueOrDefault(DateTime.MinValue),
                    HoraCita = item.HoraCita,
                    HoraTermino = item.HoraTermino,
                    DescObra = item.DescObra,
                    Localidad = item.Localidad,
                    Municipio = item.Municipio,
                    Proyecto = item.Proyecto,
                    Plazo = item.Plazo.GetValueOrDefault(0),
                    IdContratistasActas = item.idContratistasActas,
                    inv1 = item.inv1,
                    inv1cargo = item.inv1cargo,
                    inv2 = item.inv2,
                    inv2cargo = item.inv2cargo,
                    inv3 = "",
                    inv3cargo = "",
                    contrato = item.contrato,
                    Oficio = item.Oficio,
                    Importefallo = item.Importefallo.GetValueOrDefault(0),
                    ImporteAutorizado = item.ImporteAutorizado.GetValueOrDefault(0),
                    Importe = item.Importe.GetValueOrDefault(0),
                    ImporteAnticipo = item.ImporteAnticipo.GetValueOrDefault(0),
                    ejercicio = item.ejercicio.GetValueOrDefault(0),
                    DesechamientoFallo = item.DesechamientoFallo,
                    MotivoDesechamientoFallo = item.MotivoDesechamientoFallo,
                    ContratistaGanador = item.ContratistaGanador,
                    FechadeContratacion = item.FechadeContratacion.GetValueOrDefault(DateTime.MinValue),
                    Director = item.director,
                    CargoDirecto = item.cargodirector,
                    FechaContrato = item.FechadeContratacion.GetValueOrDefault(DateTime.MinValue),
                    HoraContrato = item.horaContrato,
                    Recurso = item.recurso,
                    NumPaquete = item.NumPaquete,
                    FechaFallo = item.FechaFallo.GetValueOrDefault(DateTime.MinValue),
                    HoraFallo = item.HoraFallo,
                    AutirizadoProyecto = item.AutorizadoProyecto.GetValueOrDefault(0)

                });
            }




            return Rep;
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Dictamenes(DictamenModelView dat)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
           
           
            var Datos = LlenaDictamen(dat.IdEjercicio, dat.IdPaquete, dat.IdProyecto).OrderBy(x => x.Orden);
            if (Datos.Count() > 0)
            {
                var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == dat.IdPaquete);
                paq = Datos.FirstOrDefault().NumPaquete;

                var invitados = new List<Rubricas>();
                var invitadosExt = _sicop.ControlInvitadosActas.Where(x => x.IdPaquete == dat.IdPaquete && x.TipoActa == 4);
                foreach (var item in invitadosExt)
                {
                    if (_sicop.InvitadosActas.Any(x => x.id == item.idInvitadoActa))
                    {
                        var firma = _sicop.InvitadosActas.FirstOrDefault(x => x.id == item.idInvitadoActa);
                        invitados.Add(new Rubricas()
                        {
                            Id = firma.id,
                            Nombre = firma.Invitado,
                            Cargo = firma.Cargo
                        });
                    }
                }                
                if (true )//(primer.TipoInversion == "FEDERAL")
                { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "Dictamen1.rpt")); }
                else
                { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "FalloEstatal.rpt")); }
                rd.SetDataSource(Datos);
                rd.Subreports["InvitadosExternos"].SetDataSource(invitados);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Dictamen " + paq + dat.IdProyecto + ".pdf");

                }
                catch
                {
                    throw;
                }

            }
            else
            {
                dat.Error = "No se encontró sufucuente información para generar este reporte.";
                return RedirectToAction("Index");
            }

            //dat = CargaDatos(dat);
            //return View("index", Datos);
        }
    }
}