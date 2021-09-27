using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Facebook;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using NuevoSicop.Reportes;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace NuevoSicop.Controllers
{
    
    public class FallosController : Controller
    {

         private readonly sicop_tempEntities _sicop = new sicop_tempEntities();
// GET: Fallos
        public FallosModelView CargaDatos(FallosModelView fallo)
        {
            var ejercicios =_sicop.PaquetesProcedimiento.Select(x => new { anio = x.Anio }).Distinct().OrderBy(x => x.anio);

            fallo.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");
            if (fallo.IdEjercicio > 0)
            {
                var paquetes =
                    _sicop.PaquetesProcedimiento.Where(x => x.Anio == fallo.IdEjercicio)
                        .Select(x => new { NumPaquete = x.idPaquete, Paquete = x.Descripcion })
                        .Distinct()
                        .OrderBy(x => x.Paquete);
                fallo.ListaPaquetes = new SelectList(paquetes, "NumPaquete", "paquete");
            }
            if (fallo.IdPaquete > 0)
            {
                fallo.VerFallo = _sicop.VwFallos.Any(x => x.ejercicio == fallo.IdEjercicio && x.idPaquete == fallo.IdPaquete);

                var personal =
                    _sicop.DC.Select(x => new {Id = x.Clave, Nombre = x.Nombre + " - " + x.Cargo})
                        .OrderBy(x => x.Nombre)
                        .Distinct();
                fallo.ListaPersonal = new SelectList(personal.OrderBy(x => x.Nombre), "Id", "Nombre");
                var supervisores =
                    _sicop.SUPERVIS.Where(x => x.ROL == "RESIDENTE").Select(x => new { Id = x.CLAVE, Nombre = x.NOMBRE });
                fallo.ListaSupervisores = new SelectList(supervisores.OrderBy(x => x.Nombre), "Id", "Nombre");
                var externos = _sicop.InvitadosActas.Select(x => new { x.id, nombre = x.Invitado + " - " + x.Cargo });
                fallo.ListaServExternos = new SelectList(externos.OrderBy(x => x.nombre), "id", "nombre");
                var Proys = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == fallo.IdPaquete);
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

                var ContFallo = _sicop.VwFallos.FirstOrDefault(x => x.ejercicio == fallo.IdEjercicio && x.idPaquete == fallo.IdPaquete);
                fallo.VerFallo = ContFallo != null;
                //fallo.ListaProy = new SelectList(
                //    pr.Select(x => new {Id = x.IdDetallesPaquetesProyecto, x.Proyecto}), "Id", "Proyecto");
                //fallo.VerFallo =
                //    _sicop.VwAperturaTecnica.Any(
                //        x => x.ejercicio == fallo.IdEjercicio && x.idPaquete == fallo.IdPaquete);
                //var Empresas = _sicop.EMPRESAS.Select(x => new {Id = x.Clave, x.Nombre}).OrderBy(x => x.Nombre);
                //fallo.ListaEmpresas = new SelectList(Empresas.OrderBy(x => x.Nombre), "Id", "Nombre");



                if (_sicop.DetallePaqueteProyecto.Any(x => x.IdPaquete == fallo.IdPaquete))
                {
                   
                    var contratistasActas = _sicop.ContratistasActas.Where(x => x.IdPaquete == fallo.IdPaquete && x.Desechamiento == "No").Join(_sicop.EMPRESAS, tb1 => tb1.idContratista, tb2 => tb2.Clave, (tb1, tb2) => new { id = tb1.idContratista, nombre = tb2.Nombre, num= tb1.idContratista, asitente= tb1.Asistente, desechado = tb1.DesechamientoFallo, observa= tb1.MotivoDesechamientoFallo, orden=tb1.Orden });

                    fallo.ListaContratistas = new SelectList(contratistasActas, "id", "nombre");
                    foreach (var item in contratistasActas.OrderBy(x=>x.orden))
                    {
                        if (item.desechado == "No")
                        {
                            fallo.ListaSolventes.Add(new Contratista()
                            {
                                Id= item.id.GetValueOrDefault(0),
                                Num = item.num.GetValueOrDefault(0),
                                Nombre = item.nombre,
                                Asistente = item.asitente,
                                Observaciones = item.observa,
                                Orden= item.orden.GetValueOrDefault(0),


                            });
                        }
                        else
                        {
                            fallo.ListaRechazado.Add(new Contratista()
                            {
                                Id = item.id.GetValueOrDefault(0),
                                Num = item.num.GetValueOrDefault(0),
                                Nombre = item.nombre,
                                Asistente = item.asitente,
                                Observaciones= item.observa,

                            });
                        }
                    }
                    var detpaq = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == fallo.IdPaquete);
                    foreach (var item in detpaq)
                    {
                        var proyecto = _sicop.ESCU.FirstOrDefault(x => x.id == item.IdProyecto);
                        if (_sicop.FalloProyecto.Any(x => x.IdDetallePaqueteProyecto == item.Id))
                        {

                            var FalloPy = _sicop.FalloProyecto.FirstOrDefault(x => x.IdDetallePaqueteProyecto == item.Id);

                            fallo.ListaContratos.Add(new Contrato()
                            {
                                NoContrato = FalloPy.No_Contrato,
                                Proyecto = proyecto.CCT_TURNO,
                                ImporteConIVA = FalloPy.Importe.GetValueOrDefault(0),
                                IVA = FalloPy.IVA.GetValueOrDefault(0),
                                Anticipo = item.Anticipo.GetValueOrDefault(0),
                                MontoAnticipo = (decimal) FalloPy.ImporteAnticipo.GetValueOrDefault(0),
                                FechaInicio = FalloPy.FechaInicio.GetValueOrDefault(DateTime.Now),
                                FechaTermino = FalloPy.FechaTermino.GetValueOrDefault(DateTime.Now),
                                Descripcion = FalloPy.Desc_obra,
                                ListaGanadores = ObtenGanador(fallo.IdPaquete, FalloPy.Contratista.GetValueOrDefault(0)),
                            });

                        }
                    }


                }
                if (_sicop.ControlInvitadosActas.Any(x => x.IdPaquete == fallo.IdPaquete && x.TipoActa == 4))
                {
                    var invExternos =
                        _sicop.ControlInvitadosActas.Where(x => x.IdPaquete == fallo.IdPaquete && x.TipoActa == 4);
                    foreach (var item in invExternos)
                    {
                        var invitado = _sicop.InvitadosActas.FirstOrDefault(x => x.id == item.idInvitadoActa);
                        fallo.ListaInvitadosExternos.Add(new InvitadoExterno()
                        {
                            Id = invitado.id,
                            Cargo = invitado.Cargo,
                            Nombre = invitado.Invitado,
                        });
                    }
                }

                var pys =  Proys.Join(_sicop.ESCU, x => x.IdProyecto, y => y.id,(x, y) => new {Id = x.IdProyecto, Clave = y.CCT_TURNO});
                fallo.ListaProyectosSelec = new SelectList(pys,"Id", "Clave");
                foreach (var it in pys)
                {
                    fallo.Proyectos.Add(it.Clave);
                }
                

                foreach (var Pro in Proys)
                {
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == Pro.IdProyecto);
                    var resid = _sicop.SUPERVIS.FirstOrDefault(x => x.CLAVE == Pro.Residente).NOMBRE;
                    fallo.ListaProyectos.Add(new ProyInv()
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

                if (_sicop.PaquetesProcedimiento.Any(x => x.idPaquete == fallo.IdPaquete))
                {
                    var paquete = _sicop.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == fallo.IdPaquete);
                    fallo.Anio = paquete.Anio.GetValueOrDefault(0);
                    fallo.NumPaquete = paquete.Paquete.GetValueOrDefault(0);
                    fallo.DescPaquete = paquete.Descripcion;
                    fallo.Procedimiento = paquete.NoProcedimiento;
                    fallo.Recuperacion = paquete.Recuperacion.GetValueOrDefault(0);
                    fallo.HoraReunion = paquete.hrFallo;

                       
                }


                if (_sicop.Fallo.Any(x => x.IdPaquete == fallo.IdPaquete))
                {
                    
                    var datosFallo = _sicop.Fallo.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete);
                    var programa = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete);
                    var modalidad = _sicop.Modalidad.FirstOrDefault(x => x.Clave_Modalidad == programa.Modalidad).NomModalidad;
                    
                    var externo =
                        _sicop.ControlInvitadosActas.FirstOrDefault(
                            x => x.IdPaquete == fallo.IdPaquete && x.TipoActa == 3);
                    fallo.TipoInversion = programa.TipoInversion;
                    fallo.Modalidad = modalidad;
                    fallo.FechaReunion = datosFallo.FechaCita.GetValueOrDefault(DateTime.Now);
                    fallo.HoraReunion = datosFallo.HoraCita;
                    fallo.HoraFinReunion = datosFallo.HoraTermino;
                    fallo.ListaTipoServicio = new SelectList(new List<string>() { "Obra pública", "Adquisición del sector público", "Servicios relacionado con la obra pública", "Arrendamiento del sector público", "Tipo servicio del sector público" });
                    fallo.CMIC = datosFallo.PerteneceCMIC == "SI";
                    fallo.Cumplimiento = datosFallo.GarantiaCumplimiento == "Si";
                    fallo.ViciosOcultos = datosFallo.GarantiaViciosOcultos == "Si";
                    fallo.Notas = datosFallo.Notas;
                    fallo.OAS = datosFallo.Obra_Adquisicion_Servicio == "OBRA" ? "Obra Pública" : "";
                    fallo.IdPresidente =      !(datosFallo.Invitado1 == null)? (int)datosFallo.Invitado1:0;
                    fallo.IdReviso =          !(datosFallo.Invitado2 == null)? (int)datosFallo.Invitado2:0;
                    fallo.IdservidorExterno = !(datosFallo.Invitado3 == null)? (int)datosFallo.Invitado3:0; 


                }                
            }

            return fallo;
        }




        public List<Ganador> ObtenGanador(int IdPaquete, int contratista)
        {
            var datos = new List<Ganador>();
            if (contratista != 0)
            {

               
                var contratistas =
                    _sicop.ContratistasActas.FirstOrDefault(
                        x => x.IdPaquete == IdPaquete && x.idContratista == contratista);
                var nombrecontratista = _sicop.EMPRESAS.Any(x => x.Clave == contratista)
                    ? _sicop.EMPRESAS.FirstOrDefault(x => x.Clave == contratista).Nombre
                    : "";
                datos.Add(new Ganador()
                {
                    Numero = (int) contratistas.Orden.GetValueOrDefault(0),
                    Clave = contratista,
                    Nombre = nombrecontratista,
                });
            }
            else
            {
                datos.Add(new Ganador()
                {
                    Numero = 0,
                    Clave = 0,
                    Nombre = "",
                });

            }
            return datos;
        }


        [Authorize(Roles = "Administrador,P-Programa,P-Programa2,J-Contratos")]
        public ActionResult Index(FallosModelView fallo)
        {
            fallo = new FallosModelView();
            fallo = CargaDatos(fallo);

            return View(fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa,P-Programa2,J-Contratos")]
        [HttpPost]

        public ActionResult Ejercicio(FallosModelView fallo)
        {
           fallo = CargaDatos(fallo);

            return View("Index", fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa,P-Programa2,J-Contratos")]
        [HttpPost]

        public ActionResult Paquetes(FallosModelView fallo)
        {
            fallo = CargaDatos(fallo);

            return View("Index", fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult IniciaActa(FallosModelView fallo)
        {
            var Exito = "";
            var Error = "";

            try
            {
                if (_sicop.Fallo.Any(x => x.IdPaquete == fallo.IdPaquete))
                {
                    var dato = _sicop.Fallo.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete);
                    if (!string.IsNullOrEmpty(fallo.HoraFinReunion))
                    {
                        dato.HoraTermino = fallo.HoraFinReunion;
                        _sicop.SaveChanges();

                        Exito = "Se guardo la hora de termino de la reunión correctamente";
                    }
                    else
                    {
                        Error = "Falta seleccionar una hora de término";
                    }
                    

                }
                else
                {
                    var dato = _sicop.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == fallo.IdPaquete);

                    if (!string.IsNullOrEmpty(fallo.HoraFinReunion))
                    {
                        _sicop.Fallo.Add(new Fallo()
                        {
                            IdPaquete = fallo.IdPaquete,
                            FechadeContratacion = dato.fchContrato,
                            FechaCita = dato.fchFallo,
                            HoraCita = dato.hrFallo,
                            HoraTermino = fallo.HoraFinReunion,

                        });
                        _sicop.SaveChanges();
                        Exito = "Se guardo la hora de termino correctamente";
                    }
                    else
                    {
                        Error = "Falta seleccionar una hora de término";
                    }                   
                }

           }
            catch (Exception e)
            {
                
                Error = "Hubo algun error al tratar de guardar la hora de reunión " +e;
            }
            OrdenaContratistas(fallo.IdPaquete);
            fallo = CargaDatos(fallo);
            fallo.Exito = Exito;
            fallo.Error = Error;
            return View("Index", fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult GuardarFallo(FallosModelView fallo)
        {
            var Exito = "";
            var Error = "";
            try
            {
                if (_sicop.Fallo.Any(x => x.IdPaquete == fallo.IdPaquete))
                {
                    var falloPaquete = _sicop.Fallo.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete);
                    falloPaquete.Obra_Adquisicion_Servicio = fallo.OAS == "Obra Pública" ? "OBRA" : fallo.OAS;
                    falloPaquete.PerteneceCMIC = fallo.CMIC ? "SI" : "No";
                    falloPaquete.GarantiaCumplimiento = fallo.Cumplimiento ? "Si" : "No";
                    falloPaquete.GarantiaViciosOcultos = fallo.ViciosOcultos ? "Si" : "No";
                    falloPaquete.Notas = string.IsNullOrEmpty(fallo.Notas) ? null : fallo.Notas;
                    _sicop.SaveChanges();
                    Exito = "Se actualizaron correctamente los cámbios";
                }
                else
                {

                    Error = "Primero debe guardar la hora de termino del fallo";
                }
            }
            catch (Exception e)
            {
                Error = "No fue posible guardar los datos del fallo: " +e;

            }           
            fallo = CargaDatos(fallo);
            fallo.Exito = Exito;
            fallo.Error = Error;
            return View("Index", fallo);
        }


        [Authorize(Roles = "Administrador,P-Programa,P-Programa2")]
        [HttpPost]

        public ActionResult GuardaFirmas(FallosModelView fallo)
        {
            var Exito = "";
            var Error = "";

            try
            {
                if (_sicop.Fallo.Any(x => x.IdPaquete == fallo.IdPaquete))
                {
                    var dato = _sicop.Fallo.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete);
                    dato.Invitado1 = fallo.IdPresidente!=null ? (int )fallo.IdPresidente:dato.Invitado1 ;
                    dato.Invitado2 = fallo.IdReviso!= null ? (int ) fallo.IdReviso: dato.Invitado2;
                    dato.Invitado3 = fallo.IdservidorExterno!=null?(int)fallo.IdservidorExterno : dato.Invitado3;
                    _sicop.SaveChanges();
                    Exito = "Se guardaron los firmantes correctmente";
                }
                else
                {
                    Error = "Se requeire agregar la hora de termino de la cita del fallo";
                }                
            }
            catch (Exception e)
            {
                Error = "No fue posible guardar alguna de las firmas: "+e;
            }            
            fallo = CargaDatos(fallo);
            fallo.Exito = Exito;
            fallo.Error = Error;
            return View("Index", fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult NuevoServExterno(FallosModelView fallo)
        {
            var error = "";
            var exito = "";            
            if (!string.IsNullOrEmpty(fallo.NuevaRubrica.LicDep) &&
                !string.IsNullOrEmpty(fallo.NuevaRubrica.Representante))
            {
                _sicop.InvitadosActas.Add(new InvitadosActas()
                {
                    Invitado = fallo.NuevaRubrica.LicDep.ToUpper(),
                    Cargo = fallo.NuevaRubrica.Representante.ToUpper(),
                });
                _sicop.SaveChanges();
                exito = "Se guardo la el servidor externo correctamente";
                var idservidorexterno = _sicop.InvitadosActas.FirstOrDefault(x => x.Invitado == fallo.NuevaRubrica.LicDep &&
                             x.Cargo == fallo.NuevaRubrica.Representante).id;
                if (_sicop.Fallo.Any(x => x.IdPaquete == fallo.IdPaquete))
                {
                    var ex =_sicop.Fallo.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete );
                    ex.Invitado3 = idservidorexterno;
                    _sicop.SaveChanges();
                    exito = "Se guardaron la  firmas correctamente";
                }
                else
                {                  
                    error  = "No se ha dado de alta el la fecha termino de fallo";
                }
            }
            else
            {
                error = "Se encuentran vacio alguno de los campos ";
            }
            fallo = CargaDatos(fallo);
            fallo.Error = error;
            fallo.Exito = exito;
            return View("Index", fallo);

        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaContrato(FallosModelView fallo)
        {
            var error = "";
            var exito = "";
            
            if (_sicop.DetallePaqueteProyecto.Any(
                x => x.IdPaquete == fallo.IdPaquete && x.IdProyecto == fallo.NuevoContrato.IdProyecto))
            {
                if (!string.IsNullOrEmpty(fallo.NuevoContrato.NoContrato))
                {

                
                var idPaq = _sicop.DetallePaqueteProyecto.FirstOrDefault(
                    x => x.IdPaquete == fallo.IdPaquete && x.IdProyecto == fallo.NuevoContrato.IdProyecto);
                if (_sicop.FalloProyecto.Any(x => x.IdDetallePaqueteProyecto == idPaq.Id))
                {

                    var dato = _sicop.FalloProyecto.FirstOrDefault(x => x.IdDetallePaqueteProyecto == idPaq.Id);
                    dato.FechaInicio = fallo.NuevoContrato.FechaInicio;
                    dato.FechaTermino = fallo.NuevoContrato.FechaTermino;
                    dato.Importe = fallo.NuevoContrato.ImporteConIVA;
                    dato.ImporteAnticipo = fallo.NuevoContrato.MontoAnticipo;
                    dato.No_Contrato = fallo.NuevoContrato.NoContrato;
                    dato.IVA = (double)fallo.NuevoContrato.IVA==0.16 ? 16:15;
                    dato.Desc_obra = fallo.NuevoContrato.Descripcion;
                    _sicop.SaveChanges();
                        exito = "Se modificó exitosamente el nuevo contrato";
                    }
                else
                {
                    _sicop.FalloProyecto.Add(new FalloProyecto()
                    {
                       IdDetallePaqueteProyecto = idPaq.Id,
                        FechaInicio = fallo.NuevoContrato.FechaInicio,
                        FechaTermino = fallo.NuevoContrato.FechaTermino,
                        Importe =  fallo.NuevoContrato.ImporteConIVA,
                        ImporteAnticipo = fallo.NuevoContrato.MontoAnticipo,
                        No_Contrato = fallo.NuevoContrato.NoContrato,
                        IVA = (double)fallo.NuevoContrato.IVA == 0.16 ? 16 : 15,
                        Desc_obra = fallo.NuevoContrato.Descripcion,                        
                    });
                    _sicop.SaveChanges();
                    exito = "Se agregó exitosamente el nuevo contrato";
                }
                }
                else
                {
                    error = "Necesita una clave de contrato para este proceso";
                }                
            }
            else
            {

                error = "No se encontró el proyecto";
            }
            fallo = CargaDatos(fallo);
            fallo.Error = error;
            fallo.Exito = exito;
            return View("Index", fallo);

        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult GuardaGanador(FallosModelView fallo)
        {
            var Exito = "";
            var Error = "";
            if (_sicop.DetallePaqueteProyecto.Any(
                    x => x.IdPaquete == fallo.IdPaquete && x.ESCU.CCT_TURNO == fallo.IdProyecto))
             {
                var paqproy =
                    _sicop.DetallePaqueteProyecto.FirstOrDefault(
                        x => x.IdPaquete == fallo.IdPaquete && x.ESCU.CCT_TURNO == fallo.IdProyecto);

                if (_sicop.FalloProyecto.Any(x => x.IdDetallePaqueteProyecto == paqproy.Id))
                {
                    var dato = _sicop.FalloProyecto.FirstOrDefault(x => x.IdDetallePaqueteProyecto == paqproy.Id);
                    try
                    {                        
                        var repre =   _sicop.REP_LEGA.Any(x => x.CLAVE == fallo.IdContratista)
                                    ? _sicop.REP_LEGA.FirstOrDefault(x => x.CLAVE == fallo.IdContratista).CONSEC
                                    : 0;
                        var Orden =_sicop.ContratistasActas.Any(x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.IdContratista)
                                    ? _sicop.ContratistasActas.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.IdContratista).Orden
                                    : 0;
                        if (_sicop.Detalle_Contratista.Any(x => x.Contrato == dato.No_Contrato))
                        {
                            var detalle = _sicop.Detalle_Contratista.FirstOrDefault(x => x.Contrato == dato.No_Contrato);                            
                            detalle.Clave = fallo.IdContratista;
                            detalle.IdEmpresaRepLegal = repre;
                            detalle.Numero = Orden;
                        }
                        else
                        {
                            _sicop.Detalle_Contratista.Add(new Detalle_Contratista()
                            {   Contrato =  dato.No_Contrato,
                                Numero = Orden,
                                Clave =  fallo.IdContratista,
                                IdEmpresaRepLegal = repre,
                                 Proyecto = paqproy.IdProyecto
                            });
                        }
                    
                        dato.Contratista = fallo.IdContratista;
                        _sicop.SaveChanges();
                        Exito = "Se Asigno correctamente el ganador";
                    }
                    catch (Exception e)
                    {
                        Error = "Error al tratar de asignar ganador:"+e;
                    }               
                }
                else
                {
                    Error = "Error al buscar el fallo ";
                }
            }
            else
            {
                Error = "Error al tratar de buscar el proyecto";
            }            
            fallo = CargaDatos(fallo);
            fallo.Exito = Exito;
            fallo.Error = Error;
            return View("Index", fallo);
        }

        public bool OrdenaContratistas(int IdPaquete)
        { var resu= true; 
            try
            {
                var vista = _sicop.VwFallos.Where(x => x.idPaquete == IdPaquete && x.DesechamientoFallo == "No" && x.Importe >0).OrderBy(x=>x.Importe);
                var datos = _sicop.ContratistasActas.Where(x => x.IdPaquete == IdPaquete && x.DesechamientoFallo == "No");
                var cont = 1;
                foreach (var item in vista)
                {
                    if (datos.Any(x => x.idContratistasActas == item.idContratistasActas))
                    {
                        datos.FirstOrDefault(x => x.idContratistasActas == item.idContratistasActas).Orden = cont;
                        cont++;
                    }
                    
                }
                _sicop.SaveChanges();
            }
            catch
            {
                resu = false;
            }
        

            return resu;
        }
    

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult DesecharContratista(FallosModelView fallo)
        {
            var Exito = "";
            var Error = "";
            if (_sicop.ContratistasActas.Any(
                    x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.NuevoContratista.Id))
            {
                var dato = _sicop.ContratistasActas.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.NuevoContratista.Id);
                dato.MotivoDesechamientoFallo = fallo.NuevoContratista.Observaciones.ToUpper();
                dato.DesechamientoFallo = "Si";
                dato.Orden = null;
                _sicop.SaveChanges();

                Exito = "Se desecho el contratista correctamente";
                OrdenaContratistas(fallo.IdPaquete);
            }
            else
            {
                Error = "No se encontro el dato del contratista";
            }            
            fallo = CargaDatos(fallo);
            fallo.Exito = Exito;
            fallo.Error = Error;
            return View("Index", fallo);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult NoDesecharContratista(FallosModelView fallo)
        {
            var Exito = "";
            var Error = "";
            if (_sicop.ContratistasActas.Any(
                x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.NuevoContratista.Id))
            {
                var dato =
                    _sicop.ContratistasActas.FirstOrDefault(
                        x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.NuevoContratista.Id);
                dato.DesechamientoFallo = "No";
                _sicop.SaveChanges();
                Exito = "Se quitó el contratista de desechadas correctamente";
                OrdenaContratistas(fallo.IdPaquete);
            }
            else
            {
                Error = "No se pudo localizar el dato ";
            }

            fallo = CargaDatos(fallo);
            fallo.Exito = Exito;
            fallo.Error = Error;
            return View("Index", fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregarExterno(FallosModelView fallo)
        {
            var error = "";
            var exito = "";
            if (_sicop.InvitadosActas.Any(x => x.id == fallo.IdservidorExterno))
            {
                try
                {
                    _sicop.ControlInvitadosActas.Add(new ControlInvitadosActas()
                    {
                        idInvitadoActa = fallo.IdservidorExterno,
                        TipoActa = 4,
                        NoActa = 1,
                        IdPaquete = fallo.IdPaquete,
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
            fallo = CargaDatos(fallo);
            fallo.Error = error;
            fallo.Exito = exito;
            return View("Index", fallo);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaExterno(FallosModelView fallo)
        {
            var error = "";
            var exito = "";
            if (_sicop.ControlInvitadosActas.Any(x => x.IdPaquete == fallo.IdPaquete && x.TipoActa == 4 && x.idInvitadoActa == fallo.IdservidorExterno))
            {
                try
                {
                    var dato = _sicop.ControlInvitadosActas.FirstOrDefault(
                        x =>
                            x.IdPaquete == fallo.IdPaquete && x.TipoActa == 4 &&
                            x.idInvitadoActa == fallo.IdservidorExterno);
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

            fallo = CargaDatos(fallo);
            fallo.Error = error;
            fallo.Exito = exito;
            return View("Index", fallo);

        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EditarAsistente(FallosModelView fallo)
        {
            var error = "";
            var exito = "";
            try
            {           
                if (_sicop.ContratistasActas.Any(x=>x.IdPaquete ==fallo.IdPaquete && x.idContratista==fallo.IdAsistente &&  !string.IsNullOrEmpty(fallo.NuevoAsistente)) )
                {
                    var dato =_sicop.ContratistasActas.FirstOrDefault(x => x.IdPaquete == fallo.IdPaquete && x.idContratista == fallo.IdAsistente);
                    dato.Asistente = fallo.NuevoAsistente.ToUpper();
                    _sicop.SaveChanges();
                    exito = "Se guardó Correctamente el nombre del asistente";
                }
                else
                {
                    error = "No de encontraron los datos para  guardar el asistente";
                }
            }
            catch (Exception ex)
            {
                error = "No fue posible guardar el registro del asistente error: "+ex;
            }      
            fallo = CargaDatos(fallo);
            fallo.Error = error;
            fallo.Exito = exito;
            return View("Index", fallo);

        }
    }
}