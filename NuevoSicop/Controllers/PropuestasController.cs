using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class PropuestasController : Controller
    {
        private readonly sicop_tempEntities _sicop = new sicop_tempEntities();
        // GET: Propuestas   
        public PropuestaModelView CargaDatos(PropuestaModelView propuesta)
        {
            var ejercicios =
                _sicop.PaquetesProcedimiento.Select(x => new {anio = x.Anio}).Distinct().OrderBy(x => x.anio);

            propuesta.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");

            if (propuesta.IdEjercicio > 0)
            {
                var paquetes =
                    _sicop.PaquetesProcedimiento.Where(x => x.Anio == propuesta.IdEjercicio)
                        .Select(x => new {NumPaquete = x.idPaquete, Paquete = x.Descripcion})
                        .Distinct()
                        .OrderBy(x => x.Paquete);
                propuesta.ListaPaquetes = new SelectList(paquetes, "NumPaquete", "paquete");
            }
            if (propuesta.IdPaquete > 0)
            {
                var personal =
                    _sicop.DC.Select(x => new {Id = x.Clave, Nombre = x.Nombre + " - " + x.Cargo})
                        .OrderBy(x => x.Nombre)
                        .Distinct();
                propuesta.ListaPersonal = new SelectList(personal.OrderBy(x => x.Nombre), "Id", "Nombre");
                var supervisores =
                    _sicop.SUPERVIS.Where(x => x.ROL == "RESIDENTE").Select(x => new {Id = x.CLAVE, Nombre = x.NOMBRE});
                propuesta.ListaSupervisores = new SelectList(supervisores.OrderBy(x => x.Nombre), "Id", "Nombre");
                var externos = _sicop.InvitadosActas.Select(x => new {x.id, nombre = x.Invitado + " - " + x.Cargo});
                propuesta.ListaServExternos = new SelectList(externos.OrderBy(x => x.nombre), "id", "nombre");
                var Proys = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == propuesta.IdPaquete);
                List<PropuestaProyecto> pr = new List<PropuestaProyecto>();
                foreach (var item in Proys)
                {
                    pr.Add(new PropuestaProyecto()
                    {
                        IdDetallesPaquetesProyecto = item.Id,
                        Proyecto = _sicop.ESCU.FirstOrDefault(x => x.id == item.IdProyecto).CCT_TURNO,
                    }
                        );
                }
                propuesta.ListaProy = new SelectList(
                    pr.Select(x => new {Id = x.IdDetallesPaquetesProyecto, x.Proyecto}), "Id", "Proyecto");
                propuesta.VerAperura =
                    _sicop.VwAperturaTecnica.Any(
                        x => x.ejercicio == propuesta.IdEjercicio && x.idPaquete == propuesta.IdPaquete);
                var Empresas = _sicop.EMPRESAS.Select(x => new {Id = x.Clave, x.Nombre}).OrderBy(x => x.Nombre);
                propuesta.ListaEmpresas = new SelectList(Empresas.OrderBy(x => x.Nombre), "Id", "Nombre");
                foreach (var Pro in Proys)
                {
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == Pro.IdProyecto);
                    var resid = _sicop.SUPERVIS.FirstOrDefault(x => x.CLAVE == Pro.Residente).NOMBRE;
                    propuesta.ListaProyectos.Add(new ProyInv()
                    {
                        IdProyecto = py.id,
                        Clave = py.CCT_TURNO,
                        DescripcionObra = py.NOMESC,
                        PlazoEjecucion = Pro.PlazoEjecucion.GetValueOrDefault(0),
                        Anticipo = Pro.Anticipo.GetValueOrDefault(0),
                        ImporteAut = Pro.ImporteAutorizado.GetValueOrDefault(0),
                        CapitalContable = Pro.CapitalContable.GetValueOrDefault(0),
                        PresupuestoBase = Pro.ImportePresupuestoBase.GetValueOrDefault(0),
                        Residente = resid,
                    });
                }

                if (_sicop.ControlInvitadosActas.Any(x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3))
                {
                    var invExternos =
                        _sicop.ControlInvitadosActas.Where(x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3);
                    foreach (var item in invExternos)
                    {                     
                        if (_sicop.InvitadosActas.Any(x => x.id == item.idInvitadoActa))
                        { var invitado = _sicop.InvitadosActas.FirstOrDefault(x => x.id == item.idInvitadoActa);
                             propuesta.ListaInvitadosExternos.Add(new InvitadoExterno()
                            {
                                Id = invitado.id,
                                Cargo = invitado.Cargo,
                                Nombre = invitado.Invitado,
                            });

                        }

                       
                    }
                }
                var ContActas = _sicop.ContratistasActas.Where(x => x.IdPaquete == propuesta.IdPaquete);

                foreach (var item in ContActas)
                {
                    var empresa = _sicop.EMPRESAS.FirstOrDefault(x => x.Clave == item.idContratista);
                    var pro = new List<PropuestaProyecto>();
                    if (_sicop.DetallePaqueteProyecto.Any(x => x.IdPaquete == propuesta.IdPaquete))
                    {
                        var dpp = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == propuesta.IdPaquete).ToList();
                        foreach (var it in dpp)
                        {
                            var proyecto = _sicop.ESCU.FirstOrDefault(x => x.id == it.IdProyecto);
                            if (
                                _sicop.DetalleImporteProyectoLicitante.Any(
                                    x => x.IdDetallePaqueteProyecto == it.Id && x.IdContratista == item.idContratista))
                            {
                                var prop =
                                    _sicop.DetalleImporteProyectoLicitante.FirstOrDefault(
                                        x =>
                                            x.IdDetallePaqueteProyecto == it.Id && x.IdContratista == item.idContratista);
                                pro.Add(new PropuestaProyecto()
                                {
                                    Proyecto = proyecto.CCT_TURNO,
                                    Importe = (decimal) prop.Importe,
                                    Plazo = (int) prop.Plazo,
                                });
                            }
                        }
                        propuesta.ListaLicitantes.Add(new LicitantesApertura()
                        {
                            No = item.idContratistasActas,
                            Licitante = empresa.Nombre,
                            PresentaProp = item.AsistioPresentacionApertura,
                            Asistente = item.Asistente,
                            AdquirioBases = item.Bases,
                            FueDesechado = item.Desechamiento,
                            Propuestas = pro,
                            Observaciones = item.MotivoDesechamiento,
                            IdLicitante = item.idContratista.GetValueOrDefault(0),

                        });
                    }

                }



                var paquete = _sicop.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == propuesta.IdPaquete);
                var programa = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == propuesta.IdPaquete);
                var modalidad =
                    _sicop.Modalidad.FirstOrDefault(x => x.Clave_Modalidad == programa.Modalidad).NomModalidad;
                var externo =
                    _sicop.ControlInvitadosActas.FirstOrDefault(
                        x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3);
                propuesta.TipoInversion = programa.TipoInversion;

                propuesta.Anio = paquete.Anio.GetValueOrDefault(0);
                propuesta.NumPaquete = paquete.Paquete.GetValueOrDefault(0);
                propuesta.DescPaquete = paquete.Descripcion;
                propuesta.Procedimiento = paquete.NoProcedimiento;
                propuesta.Recuperacion = paquete.Recuperacion.GetValueOrDefault(0);

                propuesta.Modalidad = modalidad;
                propuesta.FechaReunion = paquete.fchAperturaTecnicaEconomica.GetValueOrDefault(DateTime.Now);
                propuesta.HoraReunion = paquete.hrAperturaTecnicaEconomica;
                if (_sicop.ActaPresentacionAperturaProposiciones.Any(x => x.IdPaquete == propuesta.IdPaquete))
                {

                 //   var idacta = _sicop.ActaPresentacionAperturaProposiciones.Any(x => x.IdPaquete == propuesta.IdPaquete)
                 //? _sicop.ActaPresentacionAperturaProposiciones.Where(x => x.IdPaquete == propuesta.IdPaquete)
                 //    .Max(x => x.Id)
                 //: 0;
                    var acta = _sicop.ActaPresentacionAperturaProposiciones.FirstOrDefault(
                        x => x.IdPaquete == propuesta.IdPaquete);


                    propuesta.IdRepresentante = acta.Representante_Depen.GetValueOrDefault(0);
                    propuesta.IdServidorPublico1 = acta.Invitado1 == null ? 0 : int.Parse(acta.Invitado1);
                    propuesta.IdServidorPublico2 = acta.Invitado2 == null ? 0 : int.Parse(acta.Invitado2);
                    propuesta.IdServidorPublicoExterno = externo?.idInvitadoActa.GetValueOrDefault(0) ?? 0;
                    propuesta.HoraFinReunion = acta.HoraTermino;
                    propuesta.LugarDeReunion = acta.Lugar;

                    if (_sicop.NOTASACTAAPERTURA.Any(x => x.IDACTAAPERTURA == acta.Id))
                    {
                        var Notas = _sicop.NOTASACTAAPERTURA.Where(x => x.IDACTAAPERTURA == acta.Id);
                        foreach (var itm in Notas)
                        {
                            propuesta.ListaNotas.Add(new Notas()
                            {
                                Id = itm.IDNOTAAPERTURA,
                                Descripcion = itm.DESCRIPCION

                            });
                        }

                    }
                    if (_sicop.DetalleRubricas.Any(x => x.IdActaAperturaPresentacion == acta.Id))
                    {
                        var ListaRubrica = _sicop.DetalleRubricas.Where(x => x.IdActaAperturaPresentacion == acta.Id);
                        foreach (var item in ListaRubrica)
                        {
                            propuesta.ListaRubricas.Add(new ActaRubrica()
                            {
                                IdRubrica = item.Id,
                                LicDep = item.Cargo,
                                Representante = item.Nombre,
                            });

                        }


                    }





                }


            }


            return propuesta;
        }


        [Authorize(Roles = "Administrador,P-Programa")]

        public ActionResult Index(PropuestaModelView propuesta)
        {
            propuesta = new PropuestaModelView();
            propuesta = CargaDatos(propuesta);

            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Ejercicio(PropuestaModelView propuesta)
        {
            propuesta = CargaDatos(propuesta);
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Paquetes(PropuestaModelView propuesta)
        {
            propuesta = CargaDatos(propuesta);
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult IniciaActa(PropuestaModelView propuesta)
        {
            var Error = "";
            var Exito = "";

            if (!String.IsNullOrEmpty(propuesta.LugarDeReunion))
            {
                if (_sicop.ActaPresentacionAperturaProposiciones.Any(x => x.IdPaquete == propuesta.IdPaquete))
                {
                    var dato =
                        _sicop.ActaPresentacionAperturaProposiciones.FirstOrDefault(
                            x => x.IdPaquete == propuesta.IdPaquete);
                    dato.Lugar = propuesta.LugarDeReunion;
                    dato.HoraTermino = propuesta.HoraFinReunion;
                    _sicop.SaveChanges();
                    Exito = "Se actualizo correctamente lel lugar y hora de reunión";
                }
                else
                {
                    _sicop.ActaPresentacionAperturaProposiciones.Add(new ActaPresentacionAperturaProposiciones()
                    {
                        IdPaquete = propuesta.IdPaquete,
                        Lugar = propuesta.LugarDeReunion,
                        HoraTermino = propuesta.HoraReunion,
                    });
                    _sicop.SaveChanges();
                    Exito = "Se Agregó correctamente l ahora y lugar de reunión";

                }
            }
            else
            {
                Error = "Se requeire que coloque donde de llevo a cabo la junta";
            }
            propuesta = CargaDatos(propuesta);
            propuesta.Error = Error;
            propuesta.Exito = Exito;
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarPropuesta(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";

            if (propuesta.NuevaPropuesta.IdDetallesPaquetesProyecto > 0 &&
                propuesta.NuevaPropuesta.IdDetallesPaquetesProyecto > 0 && propuesta.NuevaPropuesta.IdEmpresa > 0)
            {
                try
                {
                    if (
                        _sicop.DetalleImporteProyectoLicitante.Any(
                            x =>
                                x.IdContratista == propuesta.NuevaPropuesta.IdEmpresa &&
                                x.IdDetallePaqueteProyecto == propuesta.NuevaPropuesta.IdDetallesPaquetesProyecto))
                    {
                        var dato =
                            _sicop.DetalleImporteProyectoLicitante.FirstOrDefault(
                                x =>
                                    x.IdContratista == propuesta.NuevaPropuesta.IdEmpresa &&
                                    x.IdDetallePaqueteProyecto == propuesta.NuevaPropuesta.IdDetallesPaquetesProyecto);
                        dato.Importe = propuesta.NuevaPropuesta.Importe;
                        dato.Plazo = propuesta.NuevaPropuesta.Plazo;
                        _sicop.SaveChanges();
                        exito = "Se actualizo la propuesta correctamente";
                        propuesta.NuevaPropuesta = new PropuestaProyecto();
                    }
                    else
                    {
                        _sicop.DetalleImporteProyectoLicitante.Add(new DetalleImporteProyectoLicitante()
                        {
                            IdContratista = propuesta.NuevaPropuesta.IdEmpresa,
                            IdDetallePaqueteProyecto = propuesta.NuevaPropuesta.IdDetallesPaquetesProyecto,
                            Importe = propuesta.NuevaPropuesta.Importe,
                            Plazo = propuesta.NuevaPropuesta.Plazo,
                        });
                        _sicop.SaveChanges();
                        exito = "Se inserto la propuesta correctamente";
                        propuesta.NuevaPropuesta = new PropuestaProyecto();
                    }
                }
                catch
                {
                    error = "No se ha pododo agregar la propuesta debido a un erro ";
                }
            }
            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult GuardaFirmas(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";


            if (_sicop.ActaPresentacionAperturaProposiciones.Any(x => x.IdPaquete == propuesta.IdPaquete))
            {
                var dato =
                    _sicop.ActaPresentacionAperturaProposiciones.FirstOrDefault(x => x.IdPaquete == propuesta.IdPaquete);

                dato.Representante_Depen = propuesta.IdRepresentante > 0
                    ? propuesta.IdRepresentante
                    : dato.Representante_Depen;
                dato.Invitado1 = propuesta.IdServidorPublico1 > 0
                    ? propuesta.IdServidorPublico1.ToString()
                    : dato.Invitado1;
                dato.Invitado2 = propuesta.IdServidorPublico2 > 0
                    ? propuesta.IdServidorPublico2.ToString()
                    : dato.Invitado2;
                _sicop.SaveChanges();
                exito = "Se guardaron las las firmas correctamente";
                if (propuesta.IdServidorPublicoExterno > 0)
                {
                    if (_sicop.ControlInvitadosActas.Any(
                        x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3))
                    {
                        var ex =
                            _sicop.ControlInvitadosActas.FirstOrDefault(
                                x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3);
                        ex.idInvitadoActa = propuesta.IdServidorPublicoExterno;
                        _sicop.SaveChanges();
                        exito = "Se guardaron las firmas correctamente";
                    }
                    else
                    {
                        _sicop.ControlInvitadosActas.Add(new ControlInvitadosActas()
                        {
                            TipoActa = 3,
                            IdPaquete = propuesta.IdPaquete,
                            idInvitadoActa = propuesta.IdServidorPublicoExterno,
                        });
                        _sicop.SaveChanges();
                        exito = "Se guardaron las firmas correctamente";
                    }
                }
            }
            else
            {
                error = "No se encontró el Acta en la base de datos";
            }

            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult NuevaNota(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";

            if (!String.IsNullOrEmpty(propuesta.NuevaNota.Descripcion))
            {
                int idacta = 0;
                if (_sicop.ActaPresentacionAperturaProposiciones.Any(x => x.IdPaquete == propuesta.IdPaquete))
                {
                    idacta = _sicop.ActaPresentacionAperturaProposiciones.FirstOrDefault(
                        x => x.IdPaquete == propuesta.IdPaquete).Id;


                    _sicop.NOTASACTAAPERTURA.Add(new NOTASACTAAPERTURA()
                    {
                        DESCRIPCION = propuesta.NuevaNota.Descripcion.ToUpper(),
                        IDACTAAPERTURA = idacta,
                    });
                    _sicop.SaveChanges();
                    exito = "Se guardo la nota correctamente";
                }
                else
                {
                    error = "No fue posible encontrar la acta de apertura";
                }
            }
            else
            {
                error = "No se encontro la nota para guardar";

            }



            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);

        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaNota(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";


            if (_sicop.NOTASACTAAPERTURA.Any(x => x.IDNOTAAPERTURA == propuesta.NuevaNota.Id))
            {
                var dato = _sicop.NOTASACTAAPERTURA.FirstOrDefault(x => x.IDNOTAAPERTURA == propuesta.NuevaNota.Id);
                _sicop.NOTASACTAAPERTURA.Remove(dato);
                _sicop.SaveChanges();
                exito = "Se eliminó Correctamente la Nota de apertura";


            }
            else
            {
                error = "No se encontró la nota de apertura";
            }


            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EditarNota(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";


            if (_sicop.NOTASACTAAPERTURA.Any(x => x.IDNOTAAPERTURA == propuesta.NuevaNota.Id))
            {
                var dato = _sicop.NOTASACTAAPERTURA.FirstOrDefault(x => x.IDNOTAAPERTURA == propuesta.NuevaNota.Id);
                dato.DESCRIPCION = propuesta.NuevaNota.Descripcion.ToUpper();
                _sicop.SaveChanges();
                exito = "Se actualizó correctamente la Nota de apertura";
            }
            else
            {
                error = "No se encontró la nota de apertura";
            }


            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EditarAsistente(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";


            if (_sicop.ContratistasActas.Any(x => x.IdPaquete == propuesta.IdPaquete && x.idContratistasActas == propuesta.NuevoAsistente ))
            {
                var dato =
                    _sicop.ContratistasActas.FirstOrDefault(
                        x => x.IdPaquete == propuesta.IdPaquete && x.idContratistasActas == propuesta.NuevoAsistente);
                dato.Asistente = propuesta.AsistenteEditar.ToUpper();
                _sicop.SaveChanges();
                exito = "Se actualizó correctamente la el asistente de apertura";
            }
            else
            {
                error = "No se encontró el asistente";
            }


            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaLicitante(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";
            if (propuesta.NuevoLicitante.IdLicitante > 0 && !string.IsNullOrEmpty(propuesta.NuevoLicitante.PresentaProp))
            {

                if (_sicop.ContratistasActas.Any(
                    x =>
                        x.IdPaquete == propuesta.IdPaquete &&
                        x.idContratista == propuesta.NuevoLicitante.IdLicitante))
                {
                    var dato =
                        _sicop.ContratistasActas.FirstOrDefault(
                            x =>
                                x.IdPaquete == propuesta.IdPaquete &&
                                x.idContratista == propuesta.NuevoLicitante.IdLicitante);
                    dato.Asistente = propuesta.NuevoLicitante.PresentaProp.ToUpper();
                    dato.Bases = propuesta.NuevoLicitante.CheckBases == true ? "Si" : "No";
                    dato.AsistioPresentacionApertura = propuesta.NuevoLicitante.CheckPropuesta == true ? "Si" : "No";
                    dato.Desechamiento = propuesta.NuevoLicitante.CheckDesechado == true ? "Si" : "No";
                    dato.DesechamientoFallo = "No";
                    dato.MotivoDesechamiento = "";
                    _sicop.SaveChanges();
                    exito = "Se actualizo correctamente el Licitante";
                }
                else
                {
                    _sicop.ContratistasActas.Add(new ContratistasActas()
                    {
                        IdPaquete = propuesta.IdPaquete,
                        idContratista = propuesta.NuevoLicitante.IdLicitante,
                        Asistente = propuesta.NuevoLicitante.PresentaProp.ToUpper(),
                        Bases = propuesta.NuevoLicitante.CheckBases == true ? "Si" : "No",
                        AsistioPresentacionApertura = propuesta.NuevoLicitante.CheckPropuesta == true ? "Si" : "No",
                        Desechamiento = propuesta.NuevoLicitante.CheckDesechado == true ? "Si" : "No",
                        AsistioVisitaSitio = "Si",
                        DesechamientoFallo = "No",
                        MotivoDesechamiento = "",
                        No_Preguntas = 0,
                        Importe = 0,
                        Plazo = 0,



                    });
                    _sicop.SaveChanges();
                    exito = "Se Agrego el Contratista Correctamente";

                }
                ;

            }
            else
            {
                error = "Seleccione un contratista y agregue el asistente";
            }
            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaLicitante(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";
            if (
                _sicop.ContratistasActas.Any(
                    x => x.idContratistasActas == propuesta.NuevoLicitante.No && x.IdPaquete == propuesta.IdPaquete))
            {
                var dato =
                    _sicop.ContratistasActas.FirstOrDefault(
                        x => x.idContratistasActas == propuesta.NuevoLicitante.No && x.IdPaquete == propuesta.IdPaquete);
                dato.MotivoDesechamiento = propuesta.NuevoLicitante.Observaciones;
                _sicop.ContratistasActas.Remove(dato);
                _sicop.SaveChanges();
                exito = "Se eliminó con exito el licitante";
            }
            else
            {
                error = "No se encontraron los datos para eliminar el Licitante";
            }


            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult RechazaPropuesta(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";

            if (!string.IsNullOrEmpty(propuesta.NuevoLicitante.Observaciones))
            {

                //var acta=  _sicop.ActaPresentacionAperturaProposiciones.FirstOrDefault(x => x.IdPaquete == propuesta.IdPaquete).Id;

                if (
                    _sicop.ContratistasActas.Any(
                        x => x.idContratistasActas == propuesta.NuevoLicitante.No && x.IdPaquete == propuesta.IdPaquete))
                {
                    var dato =
                        _sicop.ContratistasActas.FirstOrDefault(
                            x =>
                                x.idContratistasActas == propuesta.NuevoLicitante.No &&
                                x.IdPaquete == propuesta.IdPaquete);
                    dato.Desechamiento = propuesta.NuevoLicitante.CheckDesechado == true ? "Si" : "No";
                    dato.MotivoDesechamiento = propuesta.NuevoLicitante.Observaciones.ToUpper();
                    _sicop.SaveChanges();
                    exito = "Se guardó correctamente las observaciones";
                }
                else
                {
                    error = "No Se encontró el licitante";
                }

            }
            else
            {
                error = "No tiene lleno el motivo de ";
            }

            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarPresupuestoBase(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";

            try
            {
                if (_sicop.DetallePaqueteProyecto.Any(
                    x => x.IdPaquete == propuesta.IdPaquete && x.IdProyecto == propuesta.ClaveProyecto) &&
                    propuesta.PresupuestoBase > 0)

                {
                    var dato =
                        _sicop.DetallePaqueteProyecto.FirstOrDefault(
                            x => x.IdPaquete == propuesta.IdPaquete && x.IdProyecto == propuesta.ClaveProyecto);
                    dato.ImportePresupuestoBase = propuesta.PresupuestoBase;

                    _sicop.SaveChanges();
                    exito = "Se cambió el presupuesto base correctamente";
                }
                else
                {
                    error = "No fue posible cambiar el presupuesto base";
                }
            }
            catch (Exception)
            {

                error = "Error al tratar de cambiar el presupuesto base";
            }
            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult NuevaRubrica(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";
            try
            {
                //var idacta = _sicop.ActaPresentacionAperturaProposiciones.Any(x => x.IdPaquete == propuesta.IdPaquete)
                //    ? _sicop.ActaPresentacionAperturaProposiciones.Where(x => x.IdPaquete == propuesta.IdPaquete)
                //        .Max(x => x.Id)
                //    : 0;

                var idacta =
                    _sicop.ActaPresentacionAperturaProposiciones.FirstOrDefault(x => x.IdPaquete == propuesta.IdPaquete).Id;

                if (idacta > 0)
                {
                    _sicop.DetalleRubricas.Add(new DetalleRubricas()
                    {
                        Nombre = propuesta.NuevaRubrica.Representante.ToUpper(),
                        Cargo = propuesta.NuevaRubrica.LicDep.ToUpper(),
                        IdActaAperturaPresentacion = idacta,
                    });
                    _sicop.SaveChanges();
                    exito = "Se guardó correctamente la rubrica";
                }
                else
                {
                    error = "No se encontro un acta de apertura";
                }

            }
            catch
            {
                error = "No fue posible agregar la rubrica por causas desconocidas";
            }





            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);

        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaRubrica(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";
            if (_sicop.DetalleRubricas.Any(x => x.Id == propuesta.NuevaRubrica.IdRubrica))
            {
                var dato = _sicop.DetalleRubricas.FirstOrDefault(x => x.Id == propuesta.NuevaRubrica.IdRubrica);
                _sicop.DetalleRubricas.Remove(dato);
                _sicop.SaveChanges();
                exito = "Se eliminó correctamente la rubrica";
            }
            else
            {
                error = "No se encontro la rúbrica";
            }
            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);

        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult NuevoServExterno(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";


            if (!string.IsNullOrEmpty(propuesta.NuevaRubrica.LicDep) &&
                !string.IsNullOrEmpty(propuesta.NuevaRubrica.Representante))
            {

                _sicop.InvitadosActas.Add(new InvitadosActas()
                {
                    Invitado = propuesta.NuevaRubrica.LicDep.ToUpper(),
                    Cargo = propuesta.NuevaRubrica.Representante.ToUpper(),

                });
                _sicop.SaveChanges();
                exito = "Se guardo la el servidor externo correctamente";

                var idservidorexterno =
                    _sicop.InvitadosActas.FirstOrDefault(x => x.Invitado == propuesta.NuevaRubrica.LicDep &&
                                                              x.Cargo == propuesta.NuevaRubrica.Representante).id;

                if (_sicop.ControlInvitadosActas.Any(
                    x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3))
                {
                    var ex =
                        _sicop.ControlInvitadosActas.FirstOrDefault(
                            x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3);
                    ex.idInvitadoActa = idservidorexterno;
                    _sicop.SaveChanges();
                    exito = "Se guardaron la  firmas correctamente";
                }
                else
                {
                    _sicop.ControlInvitadosActas.Add(new ControlInvitadosActas()
                    {
                        TipoActa = 3,
                        IdPaquete = propuesta.IdPaquete,
                        idInvitadoActa = idservidorexterno,
                    });
                    _sicop.SaveChanges();
                    exito = "Se guardaron la firma correctamente";
                }

            }
            else
            {
                error = "Se encuentran vacio alguno de los campos ";
            }

            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);

        }



        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarExterno(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";
            if (_sicop.InvitadosActas.Any(x => x.id == propuesta.IdServidorPublicoExterno))
            {
                try
                {
                    _sicop.ControlInvitadosActas.Add(new ControlInvitadosActas()
                    {
                        idInvitadoActa = propuesta.IdServidorPublicoExterno,
                        TipoActa = 3,
                        NoActa = 1,
                        IdPaquete = propuesta.IdPaquete,                        
                    });
                    _sicop.SaveChanges();
                    exito = "Se guardó el registro servidor externo correctamente";
                }
                catch (Exception ex)
                {
                    error = "no fue posible guardar el registro error:" + ex;
                }                
            }
            else
            {
                error = "No se encontró el servidor a agregar";
            }            
            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaExterno(PropuestaModelView propuesta)
        {
            var error = "";
            var exito = "";
            if (_sicop.ControlInvitadosActas.Any(x => x.IdPaquete == propuesta.IdPaquete && x.TipoActa== 3  && x.idInvitadoActa == propuesta.IdServidorPublicoExterno))
            {
                try
                {
                    var dato =  _sicop.ControlInvitadosActas.FirstOrDefault(
                        x =>
                            x.IdPaquete == propuesta.IdPaquete && x.TipoActa == 3 &&
                            x.idInvitadoActa == propuesta.IdServidorPublicoExterno);
                    _sicop.ControlInvitadosActas.Remove(dato);

                    _sicop.SaveChanges();
                    exito = "Se Eliminó el registro servidor externo correctamente";
                }
                catch (Exception ex)
                {
                    error = "No fue posible Eliminar el registro error:" + ex;
                }
            }
            else
            {
                error = "No se encontró el servidor el servidor externo";
            }

            propuesta = CargaDatos(propuesta);
            propuesta.Error = error;
            propuesta.Exito = exito;
            return View("Index", propuesta);

        }


    }
}