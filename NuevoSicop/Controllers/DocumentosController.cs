using CrystalDecisions.CrystalReports.Engine;

using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Controllers
{
    public class DocumentosController : Controller
    {

        sicop_tempEntities _sicop = new sicop_tempEntities();
        // GET: Dcumentos
        
        public DocumentosModelView CargaDatos(DocumentosModelView Paq)
        {

            var ejercicios = _sicop.PaquetesProcedimiento.Select(x => new { anio = x.Anio }).Distinct().OrderBy(x => x.anio);
            Paq.ListaEjercicio = new SelectList(ejercicios, "anio", "anio");
            if (Paq.IdEjercicio > 0)
            {
                var paquetes = _sicop.PaquetesProcedimiento.Where(x => x.Anio == Paq.IdEjercicio).Select(x => new { NumPaquete = x.idPaquete, Paquete = x.Descripcion }).Distinct().OrderBy(x => x.Paquete);
                Paq.ListaPaquetes = new SelectList(paquetes, "NumPaquete", "paquete");

            }

            if (Paq.IdPaquete > 0)
            {

                //  var paquete = _sicop.VwJuntaAclaraciones.FirstOrDefault(x => x.idPaquete == Paq.IdPaquete);

                Paq.ListaActas = new SelectList(_sicop.  ActaJuntaAclaracion.Where(x => x.IdPaquete == Paq.IdPaquete).Distinct(), "Id", "NoActa");            
                var contInv = _sicop.VwProgramaCont.FirstOrDefault(x => x.ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete);
                var ContFallo = _sicop.VwFallos.FirstOrDefault(x => x.ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete);
                var Contjunta =_sicop.VwJuntaAclaraciones.FirstOrDefault(x => x.Ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete);
                var cont = _sicop.VwReporteContrato.FirstOrDefault(x => x.ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete);
                var Aper =
                    _sicop.VwAperturaTecnica.FirstOrDefault(
                        x => x.ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete); 

                Paq.VerContrato = cont !=null;
                Paq.VerInvitacion = contInv !=null ;
                Paq.VerJunta = Contjunta != null;
                Paq.VerPrograma  = true;
                Paq.VerAperura = Aper != null;
                Paq.VerFallo =ContFallo!=null;

                var proy = _sicop.VwFallos.Where(x => x.ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete).Select(x =>new { x.Proyecto }).Distinct();
                Paq.ListaProyectos = new SelectList(proy, "Proyecto", "Proyecto");
                foreach (var it in proy)
                {
                    Paq.Proyectos.Add(it.Proyecto);
                }

                if (!string.IsNullOrEmpty(Paq.IdProyectoIndice))
                {
                    if (_sicop.ESCU.Any(x => x.CCT_TURNO == Paq.IdProyectoIndice))
                    {
                        Paq.ClaveProgramatica =_sicop.ESCU.FirstOrDefault(x => x.CCT_TURNO == Paq.IdProyectoIndice).Estatus != null? _sicop.ESCU.FirstOrDefault(x => x.CCT_TURNO == Paq.IdProyectoIndice).Estatus:"Sin clave" ;
                    }
                }

                if ( cont!= null)
                {
                    Paq.Descripcion = cont.DescripcionTrabajo;
                    Paq.Contratista = cont.Empresa;
                    Paq.modalidad = cont.modalidad;
                    Paq.Monto = (decimal)cont.ImporteContrato;
                    Paq.TipoInversion = cont.TipoInversion;
                    Paq.Contrato = cont.no_contrato;
                    
                }
                else
                {
                    if (ContFallo != null)
                    {
                        Paq.Descripcion = ContFallo.descObrafallo;
                        Paq.Contratista = ContFallo.ContratistaGanador;
                        Paq.modalidad = ContFallo.Modalidad;
                        Paq.Monto = (decimal)ContFallo.Importefallo;
                        Paq.TipoInversion = ContFallo.recurso;
                        Paq.Contrato = "Sin Asignar";                         
                    }
                    else
                    {
                        
                        if (Contjunta != null)
                        {
                            Paq.Descripcion = Contjunta.DescTrabajos;
                            Paq.Contratista = "Sin asignar";
                            Paq.modalidad = Contjunta.Modalidad;
                            Paq.Monto = 0;
                            Paq.TipoInversion = "";
                            Paq.Contrato = "No asignado";
                        }
                        else
                        { if (contInv != null)
                            {
                                Paq.Descripcion = contInv.descTrabajos;
                                Paq.Contratista = "Sin Asignar";
                                Paq.modalidad = contInv.NOMESC;
                                Paq.Monto = (decimal)contInv.ImporteAutorizado;
                                Paq.TipoInversion = "";
                                Paq.Contrato = "No asignado";
                            }
                        }
                    }
                }
            }

            return Paq;
        }
        [Authorize(Roles = "Administrador,P-Programa,P-Programa2,J-Contratos")]
       
        public ActionResult Index()
        {
            var Paq = new DocumentosModelView();
                Paq = CargaDatos(Paq);
            return View("Index", Paq);
        }
        [Authorize(Roles = "Administrador,P-Programa,P-Programa2,J-Contratos")]
        [HttpPost]
        
        public ActionResult Ejercicio(DocumentosModelView Paq)
        {
            Paq = CargaDatos(Paq);
            return View("Index", Paq);
        }

        [Authorize(Roles = "Administrador,P-Programa,P-Programa2,J-Contratos")]
        [HttpPost]
        
        public ActionResult Paquetes(DocumentosModelView Paq)
        {
            Paq = CargaDatos(Paq);
            return View("Index", Paq);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
       
        public ActionResult Invitacion(DocumentosModelView Paq)
        {
            var copias = _sicop.Detalle_CcpDesignacion.Where(x => x.contrato == Paq.IdPaquete.ToString());
            var cp = "";
            foreach (var C in copias)
            {
                cp += "," + C.nombre_ccp + " - " + C.pueto_ccp;
            }
            var datos = LlenaInvitacion(Paq.IdEjercicio, Paq.IdPaquete);//_sicop.VwInvitacion1.Where(x => x.ejercicio == Paq.IdEjercicio && x.idPaquete == Paq.IdPaquete);
            ReportDocument rd = new ReportDocument();
            var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);

            if (primer.TipoInversion == "FEDERAL")
            { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "InvitacionFederal.rpt")); }
            else
            { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "InvitacionEsta.rpt")); }
            rd.SetDataSource(datos);
            rd.SetParameterValue("@CCP", cp);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Invitaciones.pdf");
            }
            catch
            {
                throw;
            }
        }
        [Authorize(Roles = "Administrador,P-Programa,J-Contratos")]
        [HttpPost]
        public ActionResult Paquetes2(DocumentosModelView Paq)
        {
            Paq = CargaDatos(Paq);
            return View("Index", Paq);
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult CargaActa(DocumentosModelView Paq)
        {



            Paq = CargaDatos(Paq);
            return View("Index", Paq);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult NoActa(DocumentosModelView Paq)
        {

            var NumPaquete = "";



            ReportDocument rd = new ReportDocument();
            if (Paq.IdPaquete > 0)
            {
                

                var Aclaraciones = LlenaJunta(Paq.IdEjercicio, Paq.IdPaquete, Paq.NoActa);

                var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);
                NumPaquete = Aclaraciones.FirstOrDefault().Paquete.ToString();

                //var Junta = _sicop.ActaJuntaAclaracion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);
                var Aclara = _sicop.DetalleAclaraciones.Where(x => x.IdActaJuntaAclaracion == Paq.NoActa);
                var Aclaraciontxt = "";
                var Cont = 1;
                foreach (var item in Aclara)
                {
                    Aclaraciontxt += Cont + ". " + item.Aclaracion + "|";
                    Cont++;
                }

                if (primer.TipoInversion == "FEDERAL")
                { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "ActaJuntaFederal.rpt")); }
                else
                { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "ActaJuntaEstatal.rpt")); }
                rd.SetDataSource(Aclaraciones);
                rd.SetParameterValue("ACLARACIONES", Aclaraciontxt);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

            }
            try
            {

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "JuntaAclaraciones - " + NumPaquete + ".pdf");
            }
            catch
            {
                throw;
            }

        }

        [Authorize(Roles = "Administrador,P-Programa2,P-Programa")]
        [HttpPost]
        public ActionResult SeleccionaProyecto(DocumentosModelView Paq)
        {
            Paq = CargaDatos(Paq);

            return View("Index", Paq);
        }

        [Authorize(Roles = "Administrador,P-Programa2")]
        [HttpPost]
        public ActionResult ActualizaClaveProgramatica(DocumentosModelView Paq)
        {
            var Error = "";
            var Exito = "";
            if (_sicop.ESCU.Any(x => x.CCT_TURNO == Paq.IdProyectoIndice) && Paq.ClaveProgramatica != "Sin clave")
            {
                try
                {
                    var proyecto = _sicop.ESCU.FirstOrDefault(x => x.CCT_TURNO == Paq.IdProyectoIndice);
                    proyecto.Estatus = Paq.ClaveProgramatica;
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
            Paq = CargaDatos(Paq);
            Paq.Error = Error;
            Paq.Exito = Exito;

            return View("Index", Paq);
        }

        public List<Junta> LlenaJunta(int ejercicio, int IdPaquete , int IdActa)
        {
            var junta = new List<Junta>();

            var datos = _sicop.VwJuntaAclaraciones.Where(x => x.idPaquete == IdPaquete && x.Ejercicio == ejercicio && x.IdActa == IdActa);
            var datos1 = datos.Select(x=>new {obra=  x.DescObra,trab= x.DescTrabajos , x.Paquete} ).Distinct();
            var descobra = "";
            var trab = "";
            var cont = 0;
            foreach (var item in datos1)
            { if (cont == 0)
                {
                    descobra = item.obra;
                    trab = item.trab;
                }
                else
                {
                    descobra +="&" +item.obra;
                    trab += "&" + item.trab;
                }
                cont++;
            }

            datos = datos.Where(x => x.DescObra == datos1.FirstOrDefault().obra);
             
            foreach (var item in datos)
            {
                junta.Add(new Junta()
                {
                    idcontratistaActas = item.idContratistasActas,
                    IdPaquete = item.idPaquete,
                    Modalidad = item.Modalidad,
                    NoProcedimiento = item.NoProcedimiento,
                    Paquete= item.Paquete.GetValueOrDefault(0),
                    NumPaquete = item.NumPaquete,
                    DescTrabajos = trab,
                    FechaJunta = item.fechajunta.GetValueOrDefault(DateTime.MinValue),
                    HoraJunta = item.hrjunta,
                    DescripcionObra = descobra,
                    Empresa = item.empresa,
                    RepreDependencia = item.RepDependencia,
                    RepreCargo= item.Cargo,
                    FechaInicio = item.FechaInicio.GetValueOrDefault(DateTime.MinValue),
                    HoraTermino = item.HoraTermino,
                    Director= item.director,
                    CargoDirector = item.CargoDirector,
                    Invitado1 = item.invitado1,
                    CargoInv1 = item.cargoinv1,
                    Nombre= item.NOMBRE,
                    Asistente= item.Asistente,
                    No_Preguntas = item.No_Preguntas.GetValueOrDefault(0),
                    Ejercicio = item.Ejercicio.GetValueOrDefault(0),     
                    Inv3 =  String.IsNullOrEmpty(item.inv3)?"": item.inv3,
                    CargoInv3 =string.IsNullOrEmpty(item.cargoinv3)?"":item.cargoinv3,
                    NoActa = item.NoActa.GetValueOrDefault(0),
                });

            }           
            return junta; 
        }

        public List<RportePrograma> LlenaPrograma(int ejercicio, int idpaquete)
        {
            var Dat = _sicop.VwProgramaCont.Where(x => x.ejercicio == ejercicio && x.idPaquete == idpaquete);
            var TipoInv = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Dat.FirstOrDefault().idPaquete).TipoInversion;
            var Prog = new List<RportePrograma>();
            var progcont = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == idpaquete);

            var mod = "";

            switch (progcont.Modalidad)
            {
                case 1:
                    mod = "FECHA DE CONVOCATORIA";
                    break;
                case 2:
                    mod = "OFICIO DE INVITACIÓN";
                    break;
               case 3:
                    mod = "OFICIO DE INVITACIÓN";
                    break;
                case 4:
                    mod = "OFICIO DE INVITACIÓN";
                    break;
            }
            
                

            var DescObra = "";
            


            var contador = 1;
            foreach (var item in Dat)
            {   
                if (contador == 1)
                {
                    if (TipoInv == "FEDERAL")
                    {
                        DescObra = item.descTrabajos;
                    }
                    else
                    {
                        DescObra = item.descTrabajos;
                    }
                    
                }
                else 
                {
                    if (TipoInv == "FEDERAL")
                    {
                        DescObra += "&" + item.descTrabajos;
                    }
                    else
                    {
                        DescObra += "&" + item.descTrabajos;
                    }

                    
                }
                contador++;
            }

            foreach (var item in Dat)
            {
                Prog.Add(new RportePrograma() {
                    FechaPrograma = item.Fechaoficio.GetValueOrDefault(DateTime.MinValue),
                    Licitacion = item.NoProcedimiento,
                    NumPaquete = item.numpaquete,
                    Descripcion = DescObra,
                    FechaInvitacion = item.fechaivitacion.GetValueOrDefault(DateTime.MinValue),
                    FechaVentaBases = item.fechaventabases.GetValueOrDefault(DateTime.MinValue),
                    FechaVisita = item.FechaVisita.GetValueOrDefault(DateTime.MinValue),
                    FechaJunta = item.FechaJunta.GetValueOrDefault(DateTime.MinValue),
                    FechaApertura = item.FechaApertura.GetValueOrDefault(DateTime.MinValue),
                    FechaFallo = item.FechaFallo.GetValueOrDefault(DateTime.MinValue),
                    HoraVentaBases = item.horaventabases,
                    HoraVisita = item.Horavisita,
                    HoraJunta = item.HoraJunta,
                    HoraApertura = item.HoraApertura,
                    HoraFallo = item.HoraFallo,
                    FirmaContrato = item.FechaContrato.GetValueOrDefault(DateTime.MinValue),
                    InicioEstimado = item.FechaInicioEstimada.GetValueOrDefault(DateTime.MinValue),
                    Proyecto = item.proyecto,
                    Periodo = item.PlazodeEjecucion.GetValueOrDefault(0),
                    Anticipo = item.Anticipo.GetValueOrDefault(0),
                    Reviso = item.Reviso,
                    CargoReviso = item.AargoRev,
                    Vobo= item.Vobo,
                    CargoVobo = item.CargoVobo,
                    Autorizo = item.Autorizo,
                    Cargoautorizo = item.CargoAut,
                    Localidad = item.localidad,
                    Municipio= item.municipio,   
                    Modalidad= mod
                }); 


            }

            return Prog;
        } 

        public List<ReporteInvitado> LlenaInvitacion(int ejercicio, int idpaquete)
        {
            var Rep = new List<ReporteInvitado>();

             
            var VwInvita = _sicop.VwInvitacion1.Where(x => x.ejercicio == ejercicio && x.idPaquete == idpaquete).Distinct();

            var datos1 = VwInvita.Select(x => new { obra = x.descobra, x.Anticipo, x.Proyecto, capital=x.CapitalContable}).OrderBy(x=>x.Proyecto).Distinct();
            var Con = 0;
            var meta = "";
            var proy = "";
            var anticipo = "";
            var PrimProy = "";
            var cap = 0d;
            foreach (var item in datos1)
            {
                if (Con == 0)
                {
                    meta = item.obra;
                    proy = item.Proyecto;
                    PrimProy = item.Proyecto;
                    anticipo = item.Anticipo.ToString();
                    cap = (double)item.capital;

                }
                else
                {
                    meta += "&" + item.obra;
                    proy += ", " + item.Proyecto;
                    anticipo += ", " + item.Anticipo.ToString();
                    cap += (double)item.capital;
                }
                Con++;
            }
            VwInvita = VwInvita.Where(x => x.Proyecto == PrimProy);
            foreach (var elem in VwInvita)
            {
             
                            
                Rep.Add(new ReporteInvitado()
                {
                    Id = (int)elem.id,
                    idPaquete = elem.idPaquete,
                    Oficio = elem.Oficio,
                    Clave = elem.Clave,
                    Nombre = elem.Nombre,
                    descobra = meta,
                    Domicilio = elem.Domicilio,
                    Telefono = elem.Telefono,
                    FechaOficio = elem.FechaOficio.GetValueOrDefault(DateTime.MinValue),
                    TipoPersona = elem.TipoPersona,
                    Padron = elem.Padron,
                    REPLEGAL = elem.REPLEGAL,
                    PUESTO = elem.PUESTO,
                    modalidad = elem.modalidad,
                    Articulo = elem.Articulo,
                    NoProcedimiento = elem.NoProcedimiento,
                    paquete = elem.paquete,
                    fchVisitaSitio = elem.fchVisitaSitio.GetValueOrDefault(DateTime.MinValue),
                    hrVisitaSitio = elem.hrVisitaSitio,
                    fchJuntaAclaracion = elem.fchJuntaAclaracion.GetValueOrDefault(DateTime.MinValue),
                    hrJuntaAclaracion = elem.hrJuntaAclaracion,
                    fchAperturaTecnicaEconomica = elem.fchAperturaTecnicaEconomica.GetValueOrDefault(DateTime.MinValue),
                    hrAperturaTecnicaEconomica = elem.hrAperturaTecnicaEconomica,
                    FechaInicioEstimada = elem.FechaInicioEstimada.GetValueOrDefault(DateTime.MinValue),
                    Anticipo = elem.Anticipo.GetValueOrDefault(0),                  
                    localidad = elem.localidad,
                    municipio = elem.municipio,
                    PlazoEjecucion = elem.PlazoEjecucion.GetValueOrDefault(0),
                    Director = elem.Director,
                    Direccion = elem.Direccion,
                    Cargo = elem.Cargo,
                    firmanombre = elem.firmanombre,
                    firmacargo = elem.firmacargo,
                    direccioncargo = elem.direccioncargo,
                    txtVoBo = elem.txtVoBo,
                    ejercicio = elem.ejercicio.GetValueOrDefault(0),
                    NumPaquete = elem.NumPaquete.GetValueOrDefault(0),
                    fchLimiteInscripcion = elem.fchLimiteInscripcion.GetValueOrDefault(DateTime.MinValue),
                    hrLimiteInscripcion = elem.hrLimiteInscripcion,
                    CapitalContable = (decimal)cap,
                    ExpAcred = elem.ExpAcred,
                    Recuperacion = elem.Recuperacion.GetValueOrDefault(0),
                    Proyecto = proy,

                });

            }
            return Rep;
        }

        public List<ReporteFallos> LlenaReporte(int ejercicio , int idPaquete, string Proyecto)           
        {
            var Rep = new List<ReporteFallos>();
            var falloactual = _sicop.VwFallos.Where(x => x.ejercicio == ejercicio && x.idPaquete ==idPaquete && x.Proyecto == Proyecto);
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
                    AutirizadoProyecto =  item.AutorizadoProyecto.GetValueOrDefault(0)
                    
                });
            }




                return Rep;
        }
        public List<RepCaratula> LlenaCaratula(int ejercicio, int idPaquete)
        {
            var Rep = new List<RepCaratula>();
            var valores  = _sicop.VwReporteCaratula.Where(x => x.ejercicio == ejercicio && x.idPaquete == idPaquete);



           foreach(var item in valores)
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
                    Representante = string.IsNullOrEmpty(item.Representante)?"": item.Representante,
                    Empresa = item.Empresa,
                    DomicilioEmpresa = item.domicilioempresa,
                    Localidad = item.empLocalidad, 
                    CPEmpresa = item.CPEmpresa,
                    Correo = item.Correo,
                    RFCEmpresa = item.RFCEmpresa,
                    Imss = item.Imss,
                    Infonavit = item.Infonavit,
                    Padron = item.Padron,
                    Depen= item.dep,
                    ClaveFinanzas = item.CadenaFianzas== null?"":item.CadenaFianzas,
                });
            }

                return Rep;
        }

        public List<ReporteApertura> LlenaAperturas(int Ejer, int Idpaq)
        {
            List<ReporteApertura> Apertura  = new List<ReporteApertura>();

            var datos01 = _sicop.VwAperturaTecnica.Where(x => x.ejercicio == Ejer && x.idPaquete == Idpaq);
           

            var valida = _sicop.ContratistasActas.Where(x => x.IdPaquete == Idpaq);

            var valida2 = _sicop.VwAperturaTecnica.Where(x => x.idPaquete == Idpaq).Select(x=> new {x.idContratistasActas, x.Orden,x.Proyecto,x.Importe}).Distinct().OrderBy(x=>x.Importe);
            

            var Proyecto = "";
            var Descripcion = "";
            var Con = 0;
            var datos2 = datos01.Select(x => new {x.Proyecto, x.DescObra}).Distinct();
            foreach (var it in datos2)
            {
                if (Con == 0)
                {
                    Descripcion = it.DescObra;
                    //Proyecto = it.Proyecto;
                  

                }
                else
                {
                    Descripcion += "& " + it.DescObra;
                    //Proyecto += ", " + it.Proyecto;
                    
                }
                Con++;
            }

            var contador = 1;
            foreach (var valor in valida2.OrderBy(x=>x.Proyecto).ThenBy(x=>x.Importe))
            {
                var act = _sicop.VwAperturaTecnica.FirstOrDefault(n =>n.ejercicio == Ejer && n.idPaquete == Idpaq && n.idContratistasActas == valor.idContratistasActas && n.Proyecto==valor.Proyecto);
                
                   act.Orden = contador;
                   contador ++;
              


                Apertura.Add(new ReporteApertura()
                {
                    Id= act.Id,
                    Modalidad = act.Modalidad,
                    NoProcedimiento= act.NoProcedimiento,
                    NumPaquete= act.NumPaquete,
                    DescObra= Descripcion,
                    Fechaapertura= act.Fechaapertura.GetValueOrDefault(DateTime.MinValue),
                    Horaapertura= act.horaapertura,
                    Lugar= act.Lugar,
                    Director= act.director,
                    CargoDirector= act.cargodirector,
                    IdContratistasActas= act.idContratistasActas.GetValueOrDefault(0),
                    IdPaquete= act.idPaquete,
                    Paquete= act.Paquete.GetValueOrDefault(0),
                    Proyecto=act.Proyecto,
                    Localidad= act.Localidad,
                    Municipio= act.Municipio,
                    Orden= act.Orden.GetValueOrDefault(0),
                    Empresa= act.Empresa,
                    Plazo= act.Plazo.GetValueOrDefault(0),
                    Importe=  act.Importe.GetValueOrDefault(0),
                    Inv1= act.inv1,
                    Inv1Cargo= act.inv1cargo,
                    Inv2= act.inv2,
                    Inv2Cargo= act.inv2cargo,
                    Oficio= act.Oficio,
                    ImporteAutorizado= act.ImporteAutorizado.GetValueOrDefault(0),
                    Fechaautorizacion= act.fechaautorizacion.GetValueOrDefault(DateTime.MinValue),
                    Asistente= act.Asistente,
                    Ejercicio= act.ejercicio.GetValueOrDefault(0),
                    MotivoDesechamiento= act.MotivoDesechamiento   ,
                    DesechamientoFallo= act.Desechamiento,
                    MotivoDesechamientoFallo= act.MotivoDesechamiento,
                    Recurso= act.recurso,
                    FechaContrato= act.fechaContrato.GetValueOrDefault(DateTime.MinValue),
                    HoraContrato= act.horaContrato,
                    FechaFallo= act.FechaFallo.GetValueOrDefault(DateTime.MinValue),
                    HoraFallo= act.HoraFallo,
                    AutorizadoProyecto= act.AutorizadoProyecto.GetValueOrDefault(0),
                    IdactaApertura= act.idactaapertura,
                    Invitado3= act.Invitado3,
                    CargoInv3= act.cargoInv3,
                    hrTerminaApertura = act.HrTerminoApertura,
                  });



            }

            return Apertura;
        }

        public List<ReporteContrato1> LlenaContrato1(int ejercicio, int IdPaquete )
        {
            var Rep = new List<ReporteContrato1>();
            var falloactual = _sicop.VwReporteContrato.Where(x => x.ejercicio == ejercicio && x.idPaquete == IdPaquete );
                         

            foreach (var item in falloactual)
            {
                

                if (_sicop.GeneradorContrato.Any(x => x.NumeroContrato == item.no_contrato))
                {
                    var GContrato = _sicop.GeneradorContrato.FirstOrDefault(x => x.NumeroContrato == item.no_contrato);
                       var  Cargo1 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo1)? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo1).Cargo:"NO SE ENCONTRO";
                       var  Cargo2 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo2) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo2).Cargo : "NO SE ENCONTRO";
                       var  Cargo3 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo3) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo3).Cargo : "NO SE ENCONTRO";
                    Rep.Add(new ReporteContrato1()
                    {
                        IdPaquete = item.idPaquete,
                        Dependencia = item.Dependencia,
                        TipoContrato = item.modalidad,
                        Programa = item.programa,
                        Ejercicio = item.ejercicio.GetValueOrDefault(0),
                        Proyecto = item.Proyecto,
                        Partida = "",
                        Procedimiento = item.Procedimiento,
                        FechaProcedimiento = GContrato.FechaProcedimiento.GetValueOrDefault(DateTime.MinValue) ,
                        Articulo = GContrato.Articulo,
                        Contrato = item.no_contrato,
                        FechaContrato = item.FechaContrato.GetValueOrDefault(DateTime.MinValue),
                        DescripcionObra = item.DescripcionTrabajo,
                        Localidad = item.Localidad,
                        Municipio = item.municipio,
                        PlazoEjecucion = item.plazo.GetValueOrDefault(0),
                        FechaInicio = item.fechainicio.GetValueOrDefault(DateTime.MinValue),
                        FechaTermino = item.FechaTermino.GetValueOrDefault(DateTime.MinValue),
                        ImporteContratado = item.ImporteContrato.GetValueOrDefault(0),
                        ImporteAutorizado = item.ImporteAutorizado.GetValueOrDefault(0),
                        ImporteAnticipo = item.anticipo.GetValueOrDefault(0),
                        AnticipoXcien = item.PorcentajeAnticipo.GetValueOrDefault(0),
                        AnticipotipoPago = GContrato.NumeroExhibicion,  // pendiente tipo pago anticipo
                        DependeciaAnticipo = GContrato.ExhibicionDependencia,
                        UbicacionPagoAnticipo = GContrato.ExhibicionUbicacion,
                        OficioAutorizaionInv = item.OficioAutorizacion,
                        FechaOficio = item.FechaOficio.GetValueOrDefault(DateTime.MinValue),
                        Contratista = item.Empresa,
                        Domicilio = item.domicilioempresa,
                        Correo = item.Correo,
                        CP = item.CPEmpresa,
                        RFC = item.RFCEmpresa,
                        RegistroIMSS = item.Imss,
                        RegistroInfonavit = item.Infonavit,
                        RegistroPadron = item.Padron,
                        Representante = item.REPLEGAL == null ? item.Empresa : item.REPLEGAL,
                        CargoRepresentante = item.CargoRep ==null ? "ADMINISTRADOR UNICO" : item.CargoRep,
                        Iva = item.iva.GetValueOrDefault(0) * 100,
                        Director = item.Director,
                        CargoDirector = item.CargoDireccion,
                        DireccionDependencia = item.DireccionDependencia,
                        RFCDependencia = item.RFCDependencia,
                        Recurso = item.programa,
                        Testigo1 = GContrato.Testigo1,
                        Testigo2 = GContrato.Testigo2,
                        Testigo3 = GContrato.Testigo3,
                        CargoTestigo1 = Cargo1,
                        CargoTestigo2 = Cargo2,
                        CargoTestigo3 = Cargo3,
                        FacturaA = GContrato.FacturadoA,
                        FacturaDireccion = GContrato.DomicilioFactura,
                        FacturaCP = GContrato.CPFactura,
                        FacturaRFCEstimacion= GContrato.RFCFactura,
                        FacturaCiudad = GContrato.CiudadFactura,
                        IFE= item.INE,
                        TipoPersona = item.TipoPersona,
                        NoEscritura = item.EscrituraPublica,
                        Volumen= item.VolumenEscritura,
                        FechaEscritura = item.FechaEscritura.GetValueOrDefault(DateTime.MinValue),
                        NotarioNum = item.NumNotario,
                        NotariaTitular= item.Notario,
                        NotariaCiudad = item.LugNot,
                        RPPCLugar = item.Ac_CiudadRPPC,
                        RPPCFolio = item.Ac_NumRPPC,
                        RPPCFecha = item.Ac_FechaRPPC.GetValueOrDefault(DateTime.MinValue),
                        TipoInversion = item.TipoInversion,
                        RepEscritura = item.RepEscritura,
                        RepVolumen = item.RepVolumen,
                        RepFechaEscritura = item.RepFechaEscritura.GetValueOrDefault(DateTime.MinValue) ,
                        RepNumNotario = item.RepNumNotario,
                        RepNotarioTitular = item.RepNombreNot,
                        RepCiudadNotario = item.CiudadNotariaRep,
                        RepRppcCiudad = item.RepCiudadRPPC,
                        RepRppcFecha = item.RepRppcFecha.GetValueOrDefault(DateTime.MinValue),
                        RepRppcFolio = item.RepRppcFolio,
                        RepLibroRPPC = item.repRpccLibro,
                        AplicaCMIC = item.AplicaCMIC,
                        NoInscripRPPCRep = item.inscripcionRPPCRep,
                        FoliosRPPCRep = item.FoliosRep,
                        LibroRppcRep = item.repRpccLibro,
                        FMERep = item.RepRppcFolio,
                        EmpFME  = item.FolioMercantilElectronico,
                        FechaFallo = item.fechafallo.GetValueOrDefault(DateTime.MinValue),
                        AutorizadoProyecto = item.AutorizadoProyecto.GetValueOrDefault(0),
                    });


                }
            
            }
            return Rep;
        }
        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Programa(DocumentosModelView prog)
        {

            var Datos = LlenaPrograma(prog.IdEjercicio, prog.IdPaquete);
            var paq = Datos.FirstOrDefault();
            ReportDocument rd = new ReportDocument();
            var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == prog.IdPaquete);
           
            if (primer.TipoInversion == "FEDERAL")
            { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "ProgramaEstatal.rpt")); }
            else
            { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "ProgramaEstatal.rpt")); }
            rd.SetDataSource(Datos);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Programa"+paq.NumPaquete+".pdf");
            }
            catch
            {
                throw;
            }
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Fallos(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaReporte(Paq.IdEjercicio, Paq.IdPaquete, Paq.IdProyecto).OrderBy(x => x.Orden);
            if (Datos.Count() > 0)
                {
                    var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);
                    paq = Datos.FirstOrDefault().NumPaquete;

                var invitados = new List<Rubricas>();
                var invitadosExt = _sicop.ControlInvitadosActas.Where(x => x.IdPaquete == Paq.IdPaquete && x.TipoActa == 4);
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



                if (primer.TipoInversion == "FEDERAL")
                    { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "FalloFederal.rpt")); }
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
                        return File(stream, "application/pdf", "Fallos " + paq + Paq.IdProyecto +".pdf");

                    }
                    catch
                    {
                        throw;
                    }

                }
                else
                {
                    Paq.Error = "No se encontró sufucuente información para generar este reporte.";
                    return RedirectToAction("Index");
                }
            
            Paq = CargaDatos(Paq);            
           return View("index",Paq);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Fallos2(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaReporte(Paq.IdEjercicio, Paq.IdPaquete, Paq.IdProyecto).OrderBy(x => x.Orden);
            if (Datos.Count() > 0)
            {
                var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);
                paq = Datos.FirstOrDefault().NumPaquete;

                if (primer.TipoInversion == "FEDERAL")
                { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "FalloFederal.rpt")); }
                else
                { rd.Load(Path.Combine(Server.MapPath("../Reportes"), "FalloEstatalReprog.rpt")); }
                rd.SetDataSource(Datos);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Fallos " + paq + Paq.IdProyecto + ".pdf");

                }
                catch
                {
                    throw;
                }

            }
            else
            {
                Paq.Error = "No se encontró sufucuente información para generar este reporte.";
                return RedirectToAction("Index");
            }

            Paq = CargaDatos(Paq);
            return View("index", Paq);
        }
        [Authorize(Roles = "Administrador,P-Programa2,J-Contratos")]
        [HttpPost]
        public ActionResult Caratula(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq ="";
            var Datos = LlenaCaratula(Paq.IdEjercicio, Paq.IdPaquete);
            if (Datos.Count() > 0)
            {


                var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);
                paq = Datos.FirstOrDefault().NumPaquete;

                if (primer.TipoInversion == "FEDERAL")
                { rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "Caratula1.rpt")); }
                else
                { rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "Caratula1.rpt")); }
                rd.SetDataSource(Datos);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/pdf", "Carátula PAQ" +paq.ToString()+ ".pdf");
                }
                catch
                {
                    throw;
                }

            }
            else
            {
                Paq.Error = "No se encontró sufucuente información para generar este reporte.";
                return RedirectToAction("Index");
            }
        }

        //----

        public ActionResult CaratulaWord(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaCaratula(Paq.IdEjercicio, Paq.IdPaquete);
            if (Datos.Count() > 0)
            {


                var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == Paq.IdPaquete);
                paq = Datos.FirstOrDefault().NumPaquete;

                if (primer.TipoInversion == "FEDERAL")
                { rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "Caratula1.rpt")); }
                else
                { rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "Caratula1.rpt")); }
                rd.SetDataSource(Datos);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                try
                {
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, "application/file", "Carátula PAQ" + paq.ToString() + ".doc");
                }
                catch
                {
                    throw;
                }

            }
            else
            {
                Paq.Error = "No se encontró sufucuente información para generar este reporte.";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Administrador,J-Contratos")]
        [HttpPost]
        public ActionResult Contrato1(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaContrato1(Paq.IdEjercicio, Paq.IdPaquete);

            if (Datos.Count() > 0)
            {
                paq = Datos.FirstOrDefault().Contrato.ToString();

                if (Datos.FirstOrDefault().TipoInversion == "ESTATAL")
                {

                    switch (Datos.FirstOrDefault().TipoContrato)
                    {
                        case "ADJUDICACION DIRECTA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                            }
                            break;
                        case "INVITACION A CUANDO MENOS TRES PERSONAS":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                            }
                            break;
                        case "LICITACION PUBLICA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                            }
                            break;
                        default:
                            {
                                if (Datos.FirstOrDefault().AnticipoXcien > 0)
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                                }
                                else
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                                }
                                break;
                            }

                    }

                }
                else
                {
                    //Paq.Error = "No se encontró reporte de tipo federal.";
                    //return RedirectToAction("Index");
                    switch (Datos.FirstOrDefault().TipoContrato)
                    {
                        case "ADJUDICACIÓN DIRECTA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo2.rpt"));
                            }
                            break;
                        case "INVITACION A CUANDO MENOS TRES PERSONAS":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo2.rpt"));
                            }
                            break;
                        case "LICITACION PUBLICA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo2.rpt"));
                            }
                            break;
                        default:
                            {
                                Paq.Error = "No se encontró reporte de tipo federal.";
                                return RedirectToAction("Index");

                            }
                    }





                }
                if (Datos.Count > 0)
                {
                    rd.SetDataSource(Datos);

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        return File(stream, "application/pdf", "Contrato Estatal" + paq + ".pdf");
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    Paq.Error = "No se encontraron datos suficientes para generar reporte";
                    return RedirectToAction("Index");
                }

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();



            }
            else
            {
                Paq.Error = "No se encontraron datos suficientes para generar reporte";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult Contrato1Word(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaContrato1(Paq.IdEjercicio, Paq.IdPaquete);

            if (Datos.Count() > 0)
            {
                paq = Datos.FirstOrDefault().Contrato.ToString();

                if (Datos.FirstOrDefault().TipoInversion == "ESTATAL")
                {

                    switch (Datos.FirstOrDefault().TipoContrato)
                    {
                        case "ADJUDICACION DIRECTA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                            }
                            break;
                        case "INVITACION A CUANDO MENOS TRES PERSONAS":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                            }
                            break;
                        case "LICITACION PUBLICA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                            }
                            break;
                        default:
                            {
                                if (Datos.FirstOrDefault().AnticipoXcien > 0)
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo1.rpt"));
                                }
                                else
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoTIpo2.rpt"));
                                }
                                break;
                            }

                    }

                }
                else
                {
                    //Paq.Error = "No se encontró reporte de tipo federal.";
                    //return RedirectToAction("Index");
                    switch (Datos.FirstOrDefault().TipoContrato)
                    {
                        case "ADJUDICACIÓN DIRECTA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            break;
                        case "INVITACION A CUANDO MENOS TRES PERSONAS":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            break;
                        case "LICITACION PUBLICA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedTipo1.rpt"));
                            }
                            break;
                        default:
                            {
                                Paq.Error = "No se encontró reporte de tipo federal.";
                                return RedirectToAction("Index");

                            }
                    }





                }
                if (Datos.Count > 0)
                {
                    rd.SetDataSource(Datos);

                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                        stream.Seek(0, SeekOrigin.Begin);
                        return File(stream, "application/file", "Contrato Estatal" + paq + ".doc");
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    Paq.Error = "No se encontraron datos suficientes para generar reporte";
                    return RedirectToAction("Index");
                }

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();



            }
            else
            {
                Paq.Error = "No se encontraron datos suficientes para generar reporte";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult Paquetes3(DocumentosModelView Paq)
        {
            Paq = CargaDatos(Paq);

            return View("Index", Paq);

        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AperturaTecnica(DocumentosModelView prog)
        {

            var Dat = LlenaAperturas(prog.IdEjercicio, prog.IdPaquete);
            var IdActa = Dat.FirstOrDefault().IdactaApertura;

            var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == prog.IdPaquete);
            var Notas = _sicop.NOTASACTAAPERTURA.Where(x => x.IDACTAAPERTURA == IdActa);
            var rubricas = new List<Rubricas>();
           

            var rub = _sicop.DetalleRubricas.Where(x => x.IdActaAperturaPresentacion == IdActa);
            foreach (var item in rub)
            {
                rubricas.Add(new Rubricas()
                {   Id  = IdActa,
                    Nombre = item.Nombre,
                    Cargo = item.Cargo
                });
            }
            var invitados = new List<Rubricas>();
            var invitadosExt = _sicop.ControlInvitadosActas.Where(x => x.IdPaquete == prog.IdPaquete && x.TipoActa == 3);
            foreach (var item in invitadosExt)
            {
                if (_sicop.InvitadosActas.Any(x => x.id == item.idInvitadoActa))
                {
                    var firma = _sicop.InvitadosActas.FirstOrDefault(x => x.id == item.idInvitadoActa); 
                    invitados.Add(new Rubricas()
                    {
                        Id =  firma.id,
                        Nombre = firma.Invitado,
                        Cargo = firma.Cargo
                    });

                }
                
            }


            var paq = Dat.FirstOrDefault();
            ReportDocument rd = new ReportDocument();          
            var NotPar = "";
            var cont = 1;
            foreach (var nt in  Notas)
            {
                if (cont == 1)
                {
                    NotPar = cont  +".- "+ nt.DESCRIPCION;
                }
                else
                {
                    NotPar += "&" +cont  +".- " + nt.DESCRIPCION;
                }
                cont++;
                
            }

            if (primer.TipoInversion == "FEDERAL")
            {
                rd.Load(Path.Combine(Server.MapPath("../Reportes"), "AperturaFederal.rpt"));
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("../Reportes"),  "AperturaEstatal.rpt")); }
            rd.SetDataSource(Dat);
            rd.Subreports["rubricas"].SetDataSource(rubricas);
            rd.Subreports["Subinforme"].SetDataSource(rubricas);
            rd.Subreports["InvitadosExternos"].SetDataSource(invitados);
            rd.SetParameterValue("Notas", NotPar);
            

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Apertura " + paq.NumPaquete + ".pdf");
            }
            catch
            {
                throw;
            }
        }


    }
}