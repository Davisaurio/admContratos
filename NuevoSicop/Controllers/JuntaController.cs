using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace NuevoSicop.Controllers
{
    public class JuntaController : Controller
    {
        // GET: Junta

        private sicop_tempEntities _sicop = new sicop_tempEntities();

        public JuntaModeView CargaDatos(JuntaModeView jta)
        {
            var eje = jta.IdEjercicio;
            var paq = jta.IdPaquete;

            var ejercicios =
                _sicop.PaquetesProcedimiento.Select(x => new {anio = x.Anio}).Distinct().OrderBy(x => x.anio);

            jta.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");

            if (jta.IdEjercicio > 0)
            {
                var paquetes =
                    _sicop.PaquetesProcedimiento.Where(x => x.Anio == jta.IdEjercicio)
                        .Select(x => new {NumPaquete = x.idPaquete, Paquete = x.Descripcion})
                        .Distinct()
                        .OrderBy(x => x.Paquete);
                jta.ListaPaquetes = new SelectList(paquetes, "NumPaquete", "paquete");
            }
            if (jta.IdPaquete > 0)
            {
                var juntas =
                    _sicop.ActaJuntaAclaracion.Where(x => x.IdPaquete == jta.IdPaquete)
                        .Select(x => new {x.Id, Acta = x.NoActa})
                        .OrderBy(x => x.Acta);
                jta.ListaJuntas = new SelectList(juntas, "Id", "Acta");
                var personal =
                    _sicop.DC.Select(x => new {Id = x.Clave, Nombre = x.Nombre + " - " + x.Cargo})
                        .OrderBy(x => x.Nombre)
                        .Distinct();
                jta.ListaPersonal = new SelectList(personal.OrderBy(x => x.Nombre), "Id", "Nombre");
                var Residentes =
                    _sicop.SUPERVIS.Where(x=>x.ROL=="RESIDENTE").Select(x => new { Id = x.CLAVE, Nombre = x.NOMBRE })
                        .OrderBy(x => x.Nombre)
                        .Distinct();
                jta.ListaResidentes = new SelectList(Residentes.OrderBy(x => x.Nombre), "Id", "Nombre");
                

                var externos = _sicop.InvitadosActas.Select(x => new {x.id, nombre = x.Invitado + " - " + x.Cargo}).OrderBy(x=>x.nombre);
                jta.ListaServExternos = new SelectList(externos, "id", "nombre");
                var empresas = _sicop.EMPRESAS.Select(x => new {Id = x.Clave, Nombre = x.Nombre}).OrderBy(x => x.Nombre);
                jta.ListaEmpresas = new SelectList(empresas.Take(20), "Id", "Nombre");
                var Proys = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == jta.IdPaquete);

                foreach (var Pro in Proys)
                {
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == Pro.IdProyecto);
                    var resid = _sicop.SUPERVIS.FirstOrDefault(x => x.CLAVE == Pro.Residente).NOMBRE;
                    jta.ListaProyectos.Add(new ProyInv()
                    {
                        DescripcionObra = py.NOMESC,
                        PlazoEjecucion = Pro.PlazoEjecucion.GetValueOrDefault(0),
                        Anticipo = Pro.Anticipo.GetValueOrDefault(0),
                        ImporteAut = Pro.ImporteAutorizado.GetValueOrDefault(0),
                        CapitalContable = Pro.CapitalContable.GetValueOrDefault(0),
                        Residente = resid,
                    });

                }

                var paquete = _sicop.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == jta.IdPaquete);
                var Programa = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == jta.IdPaquete);

                var Modalidad = Programa != null
                    ? _sicop.Modalidad.FirstOrDefault(x => x.Clave_Modalidad == Programa.Modalidad).NomModalidad
                    : "";
                jta.Anio = paquete.Anio.GetValueOrDefault(0);

                var Contr = _sicop.ContratistasActas.Where(x => x.IdPaquete == jta.IdPaquete);

                foreach (var item in Contr)
                {
                    var Empresa = _sicop.EMPRESAS.FirstOrDefault(x => x.Clave == item.idContratista).Nombre;
                    jta.ListaAsistentes.Add(new JuntaModeView.Asistente()
                    {
                        Clave = item.idContratistasActas,
                        IdContratista = item.idContratista.GetValueOrDefault(0),
                        Contratista = Empresa,
                        Asiste = item.Asistente,
                        NumPreguntas = item.No_Preguntas.GetValueOrDefault(0),
                    });
                }
                jta.NumPaquete = paquete.Paquete.GetValueOrDefault(0);
                jta.NumPaqueteDesc = paquete.Descripcion;
                jta.NoProcedimiento = paquete.NoProcedimiento;
                jta.Recuperacion = paquete.Recuperacion.GetValueOrDefault(0);
                jta.TipoInversion = Programa.TipoInversion;
                jta.Modalidad = Modalidad;
                jta.FechaReunion = paquete.fchJuntaAclaracion.GetValueOrDefault(DateTime.Now);
                if (_sicop.ActaJuntaAclaracion.Any(x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa))
                {
                    var junta =
                        _sicop.ActaJuntaAclaracion.FirstOrDefault(
                            x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa);
                    jta.FechaTerminoReunion = junta.FechaTermino.GetValueOrDefault(DateTime.Now);
                    jta.HoraTerminoReunion = junta.FechaTermino.GetValueOrDefault(DateTime.Now).ToString("HH:mm");
                    jta.IdConduceEvento = junta.Representante_Depen.GetValueOrDefault(0);
                    jta.IdSvrPublico1 = junta.Invitado1 == null ? 0 : int.Parse(junta.Invitado1);
                    jta.IdSvrPublico2 = junta.Invitado2 == null ? 0 : int.Parse(junta.Invitado2);
                    if (
                        _sicop.ControlInvitadosActas.Any(
                            x => x.IdPaquete == jta.IdPaquete && x.NoActa == junta.NoActa && x.TipoActa == 3))
                    {
                        var externo =
                            _sicop.ControlInvitadosActas.FirstOrDefault(
                                x =>
                                    x.IdPaquete == jta.IdPaquete && x.NoActa == junta.NoActa &&
                                    x.TipoActa == 3);
                        jta.IdSvrPublicoExterno = (int) externo.idInvitadoActa;
                    }
                    else
                    {
                        jta.IdSvrPublicoExterno = 0;
                    }
                    var Aclara = _sicop.DetalleAclaraciones.Where(x => x.IdActaJuntaAclaracion == jta.NoActa);
                    foreach (var item in Aclara)
                    {
                        jta.ListaAclaraciones.Add(new JuntaModeView.Aclaracion()
                        {
                            Id = item.Id,
                            Descripcion = item.Aclaracion,
                        });
                    }
                    var Contjunta =
                        _sicop.VwJuntaAclaraciones.FirstOrDefault(
                            x => x.Ejercicio == jta.IdEjercicio && x.idPaquete == jta.IdPaquete);
                    jta.VerJunta = Contjunta != null;
                }
                var a = jta.ListaAclaraciones.Count();
            }

            jta.IdPaquete = paq;
            jta.IdEjercicio = eje;
            return jta;
        }

        [Authorize(Roles = "Administrador,P-Programa")]

        public ActionResult Index()
        {
            var jta = new JuntaModeView();
            jta = CargaDatos(jta);

            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Ejercicio(JuntaModeView jta)
        {
            jta = CargaDatos(jta);
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Paquetes(JuntaModeView jta)
        {
            jta = CargaDatos(jta);
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Actas(JuntaModeView jta)
        {
            jta = CargaDatos(jta);
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarAclaratoria(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            if (jta.IdEjercicio > 0 && jta.IdPaquete > 0 && jta.NoActa > 0)
            {
                if (_sicop.ActaJuntaAclaracion.Any(x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa))
                {
                    var acta =
                        _sicop.ActaJuntaAclaracion.FirstOrDefault(
                            x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa);
                    _sicop.DetalleAclaraciones.Add(new DetalleAclaraciones()
                    {
                        IdActaJuntaAclaracion = jta.NoActa,
                        IdCatalogo = null,
                        NoActa = acta.NoActa,
                        Aclaracion = jta.NuevAclaracion.Descripcion = jta.NuevAclaracion.Descripcion.ToUpper(),

                    });
                    _sicop.SaveChanges();
                    Exito = "Se guardó con exito la nota aclaratoria";
                }
                else
                {
                    Error = "No se encontró el acta";
                }
            }
            else
            {
                Error = "No es posible guardar la aclaración verifique bien sus datos";
            }
            jta = CargaDatos(jta);
            jta.Error = Error;
            jta.Exito = Exito;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaAclaracion(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";

            if (_sicop.ActaJuntaAclaracion.Any(x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa))
            {
                var acta =
                    _sicop.ActaJuntaAclaracion.FirstOrDefault(x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa);
                if (
                    _sicop.DetalleAclaraciones.Any(
                        x => x.IdActaJuntaAclaracion == acta.Id && x.Id == jta.EliminaAclaracion.Id))
                {
                    var Dato =
                        _sicop.DetalleAclaraciones.FirstOrDefault(
                            x => x.IdActaJuntaAclaracion == acta.Id && x.Id == jta.EliminaAclaracion.Id);
                    _sicop.DetalleAclaraciones.Remove(Dato);
                    _sicop.SaveChanges();
                    Exito = "Se eliminó Correctamente la aclaración";
                }
                else
                {
                    Error = "No nos fue posible eliminar la aclaratoria";
                }
            }
            else
            {
                Error = "El Dato no fue posible localizarlo.";
            }
            jta = CargaDatos(jta);
            jta.Error = Error;
            jta.Exito = Exito;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EditarAclaracion(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            if (_sicop.DetalleAclaraciones.Any(x => x.Id == jta.EliminaAclaracion.Id))
            {
                var dato = _sicop.DetalleAclaraciones.FirstOrDefault(x => x.Id == jta.EliminaAclaracion.Id);
                jta.NuevAclaracion.Id = dato.Id;
                jta.NuevAclaracion.Descripcion = dato.Aclaracion;
                Exito = "Se cargo el dato para su edición";
            }
            else
            {
                Error = "No se encuentró la aclaración";
            }
            jta = CargaDatos(jta);
            jta.Error = Error;
            jta.Exito = Exito;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarNuevaActa(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";

            try
            {
                var acta = _sicop.ActaJuntaAclaracion.Where(x => x.IdPaquete == jta.IdPaquete);

                var numacta = _sicop.ActaJuntaAclaracion.Any(x => x.IdPaquete == jta.IdPaquete)
                    ? acta.Max(x => x.NoActa).GetValueOrDefault(0) + 1
                    : 1;
                var horainicio = jta.FechaReunion.ToString("HH:mm");
                var horatermino = jta.FechaTerminoReunion.ToString("HH:mm");
                _sicop.ActaJuntaAclaracion.Add(new ActaJuntaAclaracion()
                {
                    IdPaquete = jta.IdPaquete,
                    FechaInicio = jta.FechaReunion,
                    NoActa = numacta,
                    HoraInicio = horainicio,
                    FechaTermino = jta.FechaTerminoReunion,
                    HoraTermino = horatermino,
                    Representante_Depen = 119,

                });
                _sicop.SaveChanges();
                Exito = "Se guardó Correctamente el acta de junta";
            }
            catch
            {
                Error = "No fue Posible guardar el acta";
            }

            jta = CargaDatos(jta);
            jta.Error = Error;
            jta.Exito = Exito;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaActa(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            try
            {
                var Acta =
                    _sicop.ActaJuntaAclaracion.FirstOrDefault(x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa);
                _sicop.ActaJuntaAclaracion.Remove(Acta);
                _sicop.SaveChanges();
                var actas2 = _sicop.ActaJuntaAclaracion.Where(x => x.IdPaquete == jta.IdPaquete);
                var contador = 1;
                foreach (var item  in actas2)
                {
                    item.NoActa = contador;
                    contador++;
                }
                _sicop.SaveChanges();
                jta.NoActa = 0;
                Exito = "Se eliminó correctamente el acta";
            }
            catch
            {
                Error = "No fue posible eliminar el acta";
            }
            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }

        public JsonResult LlenaEmpresas(string proy)
        {
            var resp = new Listas();
            var res =
                _sicop.EMPRESAS.Where(x => x.Nombre.Contains(proy)).Select(x => new {Id = x.Clave, Nombre = x.Nombre});
            resp.Progra = new SelectList(res.Distinct().OrderBy(x => x.Nombre), "Id", "Nombre");
            return Json(resp);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarAsistente(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            if (string.IsNullOrEmpty(jta.NuevoAsistente.Asiste))
            {
                Error = "El debe llenarse el Nombre del asistente";
            }
            else
            {
                if (
                    _sicop.ContratistasActas.Any(
                        x => x.IdPaquete == jta.IdPaquete && x.idContratista == jta.NuevoAsistente.IdContratista))
                {
                    Error = "Ya se agregó a esta empresa";
                }
                else
                {
                    var orden = _sicop.ContratistasActas.Any(x => x.IdPaquete == jta.IdPaquete)
                        ? _sicop.ContratistasActas.Where(x => x.IdPaquete == jta.IdPaquete)
                            .Max(x => x.Orden)
                            .GetValueOrDefault(0) + 1
                        : 1;
                    _sicop.ContratistasActas.Add(new ContratistasActas()
                    {
                        IdPaquete = jta.IdPaquete,
                        idContratista = jta.NuevoAsistente.IdContratista,
                        Asistente = jta.NuevoAsistente.Asiste.ToUpper(),
                        Orden = orden,
                        AsistioVisitaSitio = "Si",
                        Bases = "Si",
                        Desechamiento = "No",
                        AsistioPresentacionApertura = "Si",
                        DesechamientoFallo = "No",
                        No_Preguntas = 0,
                        FirmaFallo = true,
                        FirmaApertura = true
                    });
                    _sicop.SaveChanges();
                    Exito = "Se guardo exitosamente el asistente";
                }
            }
            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaAsistente(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            try
            {

                var dato =
                    _sicop.ContratistasActas.FirstOrDefault(
                        x => x.IdPaquete == jta.IdPaquete && x.idContratistasActas == jta.IdEliminaAsistente);

               
                    var nombre = dato.Asistente;
                    _sicop.ContratistasActas.Remove(dato);
                    _sicop.SaveChanges();
                    Exito = "Se eliminó correctamente el asistente de la junta " + nombre;
               
            }
            catch
            {
                Error = "No fue posible eliminar el asistente de la junta ";
            }
            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EditaAsistente(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            try
            {

                var dato =
                    _sicop.ContratistasActas.FirstOrDefault(
                        x => x.IdPaquete == jta.IdPaquete && x.idContratistasActas == jta.IdEliminaAsistente);               
                    var nombre = dato.Asistente;
                    dato.Asistente = jta.NuevoAsistente.Asiste;
                    _sicop.SaveChanges();
                    Exito = "Se actualizo el nombre del asistente: " + nombre+"Por "+jta.NuevoAsistente.Asiste;
               
            }
            catch
            {
                Error = "No fue posible Actualizar el asistente";
            }
            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaPregunta(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";

            if (String.IsNullOrEmpty(jta.Pregunta))
            {
                Error = "No de recibió la pregunta";
            }
            else
            {
                if (_sicop.ContratistasActas.Any(x => x.idContratistasActas == jta.IdEliminaAsistente))
                {
                    var contrat =
                        _sicop.ContratistasActas.FirstOrDefault(x => x.idContratistasActas == jta.IdEliminaAsistente);

                    var empresa = contrat.idContratista.GetValueOrDefault(0);

                    var numpreg = _sicop.PreguntasActaVisita.Any(x => x.IdPaquete == jta.IdPaquete)
                        ? _sicop.PreguntasActaVisita.Where(x => x.IdPaquete == jta.IdPaquete)
                            .Max(x => x.no_pregunta)
                            .Value + 1
                        : 1;
                    var ident = _sicop.PreguntasActaVisita.Max(x => x.idPregunta) + 1;

                    try
                    {
                        var dato = new PreguntasActaVisita();
                        dato.IdPaquete = jta.IdPaquete;
                        dato.no_contratista = empresa;
                        dato.no_pregunta = numpreg;
                        dato.pregunta = jta.Pregunta;
                        dato.respuesta = jta.Respuesta;
                        dato.idPregunta = ident;
                        _sicop.PreguntasActaVisita.Add(dato);
                        _sicop.SaveChanges();
                        Exito = "Se guardó la pregunta Correctamente";
                    }
                    catch (Exception e)
                    {

                        Error = "No fueposible guardar el dato:";
                    }
                    //_sicop.ContratistasActas.FirstOrDefault(x => x.idContratistasActas == jta.IdEliminaAsistente).No_Preguntas= contrat.No_Preguntas+1;
                    //_sicop.SaveChanges();
                }
                else
                {
                    Error = "No se encontro el contratista";
                }
            }

            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarFirmas(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";
            try
            {
                if (_sicop.ActaJuntaAclaracion.Any(x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa))
                {
                    var dato =
                        _sicop.ActaJuntaAclaracion.FirstOrDefault(
                            x => x.IdPaquete == jta.IdPaquete && x.Id == jta.NoActa);
                    dato.Representante_Depen = jta.IdConduceEvento > 0 ? jta.IdConduceEvento : dato.Representante_Depen;
                    dato.Invitado1 = jta.IdSvrPublico1 > 0 ? jta.IdSvrPublico1.ToString() : dato.Invitado1;
                    dato.Invitado2 = jta.IdSvrPublico2 > 0 ? jta.IdSvrPublico2.ToString() : dato.Invitado2;
                    if (
                        _sicop.ControlInvitadosActas.Any(
                            x => x.IdPaquete == jta.IdPaquete && x.NoActa == dato.NoActa && x.TipoActa == 3))
                    {
                        var actualiza =
                            _sicop.ControlInvitadosActas.FirstOrDefault(
                                x => x.IdPaquete == jta.IdPaquete && x.NoActa == dato.NoActa && x.TipoActa == 3);
                        actualiza.idInvitadoActa = jta.IdSvrPublicoExterno;
                    }
                    else
                    {
                        _sicop.ControlInvitadosActas.Add(new ControlInvitadosActas()
                        {
                            IdPaquete = jta.IdPaquete,
                            NoActa = dato.NoActa,
                            TipoActa = 3,
                            idInvitadoActa = jta.IdSvrPublicoExterno,
                        });
                    }
                    _sicop.SaveChanges();
                    Exito = "Se guardaron los cámbios correctamente";
                }
                else
                {
                    Error = "No se pudo consultar la aclaración";
                }
            }
            catch (Exception e)
            {
                Error = "No fue posible guardar los cambios";
            }
            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaExterno(JuntaModeView jta)
        {
            var Error = "";
            var Exito = "";            
            try
            {
                if (!string.IsNullOrEmpty(jta.NuevoExterno) && !string.IsNullOrEmpty(jta.NuevoExternoCargo))
                {
                    _sicop.InvitadosActas.Add(new InvitadosActas()
                    {
                        Invitado = jta.NuevoExterno.ToUpper(),
                        Cargo = jta.NuevoExternoCargo.ToUpper(),
                    });
                    _sicop.SaveChanges();
                    Exito = "Se grego el Invitado Correctamente";
                }
                else
                {
                    Error = "No fue posible insertar los datos";
                }                
            }
            catch
            {
                Error = "No fue posible agregar el invitado debido a algun error desconocido";
            }
            jta = CargaDatos(jta);
            jta.Exito = Exito;
            jta.Error = Error;
            return View("Index", jta);
        }

        public JsonResult LlenaNotas(string Busca)
        {
            var resp = new Listas();
            if (Busca == "ITIFE")
            {

                var res = _sicop.CatalogoAclaracion3.Select(x => new {Nota = x.Aclaracion});
                resp.Progra = new SelectList(res, "Nota", "Nota");
            }
            else
            {

             
                var res =
                    _sicop.DetalleAclaraciones.Select(x => new {Nota = x.Aclaracion})
                        .Where(x => x.Nota.Contains(Busca))
                        .Take(20);
                resp.Progra = new SelectList(res.Distinct().OrderBy(x => x.Nota), "Nota", "Nota");
            }
            return Json(resp);
        }


    }
}