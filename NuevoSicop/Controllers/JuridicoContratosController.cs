using CrystalDecisions.CrystalReports.Engine;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuevoSicop.Controllers
{
    public class JuridicoContratosController : Controller
    {
        // GET: JuridicoContratos

        sicop_tempEntities _sicop = new sicop_tempEntities();
        public ContratosModelView ObtenDatos(ContratosModelView Cont)
        {
            var ejercicios = _sicop.PaquetesProcedimiento.Select(x => new { anio = x.Anio }).Distinct().OrderBy(x => x.anio);
            Cont.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");
            if (Cont.IdEjercicio > 0)
            {
                if (string.IsNullOrEmpty(Cont.BuscarContrato))
                {
                    Cont.ListaContratos =new  SelectList( _sicop.VwReporteContrato.Where(x=>x.ejercicio == Cont.IdEjercicio).Select(x=>new { x.no_contrato, x.Id }).OrderBy(x=>x.no_contrato),"no_contrato","no_contrato" );
                }
                else
                {
                    Cont.ListaContratos = new SelectList(_sicop.VwReporteContrato.Where(x => x.ejercicio == Cont.IdEjercicio && x.no_contrato.Contains(Cont.BuscarContrato) ).Select(x => new { x.no_contrato, x.Id }).OrderBy(x => x.no_contrato), "Id", "no_contrato");
                }
            }

            if (!string.IsNullOrEmpty(Cont.IdContrato))
            {
                var itcont = _sicop.VwReporteContrato.FirstOrDefault(x => x.ejercicio == Cont.IdEjercicio && x.no_contrato == Cont.IdContrato);
                var GContrato = _sicop.GeneradorContrato.FirstOrDefault(x => x.NumeroContrato == Cont.IdContrato);

                if (itcont != null && GContrato != null)
                {                   

                    Cont.IdContrato = Cont.IdContrato;                    
                    Cont.modalidad = itcont.modalidad;
                    Cont.Recurso = itcont.programa;
                    Cont.Procedimiento = itcont.Procedimiento;                   
                    Cont.Contrato = itcont.no_contrato;
                    Cont.Descripcion = itcont.DescripcionTrabajo;                   
                    Cont.Plazo = itcont.plazo.GetValueOrDefault(0);
                    Cont.Inicia = itcont.fechainicio.GetValueOrDefault(DateTime.MinValue);
                    Cont.Termina = itcont.FechaTermino.GetValueOrDefault(DateTime.MinValue);
                    Cont.Monto = itcont.ImporteContrato.GetValueOrDefault(0);                   
                    Cont.Anticipo = itcont.anticipo.GetValueOrDefault(0);
                    Cont.AnticipoxCien = itcont.PorcentajeAnticipo.GetValueOrDefault(0);
                    Cont.Iva = itcont.iva.GetValueOrDefault(0) * 100;
                    Cont.TipoPersona = itcont.TipoPersona;
                    Cont.TipoInversion = itcont.TipoInversion;
                    Cont.Contratista = itcont.Empresa;                  
                }
            }
            return Cont;
        }

        [Authorize(Roles = "Administrador,J-Contratos")]
        
        public ActionResult Index()
        {
            ContratosModelView Cont = new ContratosModelView();

           Cont = ObtenDatos(Cont);
            return View("Index", Cont);
        }
        [Authorize(Roles = "Administrador,J-Contratos")]
        [HttpPost]
        public ActionResult Ejercicio(ContratosModelView Cont)
        {
            Cont = ObtenDatos(Cont);
            return View("Index", Cont);
        }
        [Authorize(Roles = "Administrador,J-Contratos")]
        [HttpPost]
        public ActionResult BuscarContratos(ContratosModelView Cont)
        {
            Cont = ObtenDatos(Cont);
            return View("Index", Cont);
        }

        [Authorize(Roles = "Administrador,J-Contratos")]
        [HttpPost]
        public ActionResult Contratos(ContratosModelView Cont)
        {
            Cont = ObtenDatos(Cont);
            return View("Index", Cont);
        }


        public List<RepCaratula> LlenaCaratula(int ejercicio, string  Contrato)
        {
            var Rep = new List<RepCaratula>();


            if (_sicop.VwReporteCaratula.Any(x => x.ejercicio == ejercicio && x.no_contrato == Contrato))
            {
                var valores = _sicop.VwReporteCaratula.FirstOrDefault(x => x.ejercicio == ejercicio && x.no_contrato == Contrato);
            }
            var valores2 = _sicop.VwReporteCaratula.Where(x => x.ejercicio == ejercicio && x.no_contrato == Contrato);
            foreach (var item in valores2)
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
        public List<ReporteContrato1> LlenaContrato1(int ejercicio, string Contrato)
        {
            var Rep = new List<ReporteContrato1>();
            var falloactual = _sicop.VwReporteContrato.Where(x => x.ejercicio == ejercicio && x.no_contrato == Contrato);


            foreach (var item in falloactual)
            {


                if (_sicop.GeneradorContrato.Any(x => x.NumeroContrato == item.no_contrato))
                {
                    var GContrato = _sicop.GeneradorContrato.FirstOrDefault(x => x.NumeroContrato == item.no_contrato);
                    var Cargo1 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo1) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo1).Cargo : "NO SE ENCONTRO";
                    var Cargo2 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo2) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo2).Cargo : "NO SE ENCONTRO";
                    var Cargo3 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo3) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo3).Cargo : "NO SE ENCONTRO";
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
                        FechaProcedimiento = GContrato.FechaProcedimiento.GetValueOrDefault(DateTime.MinValue),
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
                        CargoRepresentante = item.CargoRep == null ? "ADMINISTRADOR UNICO" : item.CargoRep,
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
                        FacturaRFCEstimacion = GContrato.RFCFactura,
                        FacturaCiudad = GContrato.CiudadFactura,
                        IFE = item.INE,
                        TipoPersona = item.TipoPersona,
                        NoEscritura = item.EscrituraPublica,
                        Volumen = item.VolumenEscritura,
                        FechaEscritura = item.FechaEscritura.GetValueOrDefault(DateTime.MinValue),
                        NotarioNum = item.NumNotario,
                        NotariaTitular = item.Notario,
                        NotariaCiudad = item.LugNot,
                        RPPCLugar = item.Ac_CiudadRPPC,
                        RPPCFolio = item.Ac_NumRPPC,
                        RPPCFecha = item.Ac_FechaRPPC.GetValueOrDefault(DateTime.MinValue),
                        TipoInversion = item.TipoInversion,
                        RepEscritura = item.RepEscritura,
                        RepVolumen = item.RepVolumen,
                        RepFechaEscritura = item.RepFechaEscritura.GetValueOrDefault(DateTime.MinValue),
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
                        EmpFME = item.FolioMercantilElectronico,
                        FechaFallo = item.fechafallo.GetValueOrDefault(DateTime.MinValue),
                        AutorizadoProyecto = item.AutorizadoProyecto.GetValueOrDefault(0),
                    });


                }

            }
            return Rep;
        }

        [Authorize(Roles = "Administrador,J-Contratos")]
        [HttpPost]
        public ActionResult Caratula(DocumentosModelView Paq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaCaratula(Paq.IdEjercicio, Paq.Contrato);
            if (Datos.Count() > 0)
            {
                var valores = _sicop.VwReporteCaratula.FirstOrDefault(x => x.ejercicio == Paq.IdEjercicio && x.no_contrato == Paq.Contrato);

                var primer = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete ==  valores.idPaquete);
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
                    return File(stream, "application/pdf", "Carátula PAQ" + paq.ToString() + ".pdf");
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
            var Datos = LlenaContrato1(Paq.IdEjercicio, Paq.Contrato);

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
                        case "ADJUDICACION DIRECTA":
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
        [Authorize(Roles = "Administrador,J-Contratos")]
        [HttpPost]
        public ActionResult ActualizaAlcance(ContratosModelView Paq)
        {
            var exito = "";
            var error = "";
            try
            {
                if (_sicop.VwReporteContrato.Any(x => x.ejercicio == Paq.IdEjercicio && x.no_contrato == Paq.IdContrato))
                {
                    var contrato = _sicop.contratos.FirstOrDefault(x=>x.no_contrato == Paq.IdContrato);
                    contrato.desc_obra = Paq.Descripcion;
                    _sicop.SaveChanges();
                    exito = "Se realizó el cambio con exito";
                }
            }
            catch
            {
                error = "No fue posible guardar el registro";
            }
            Paq = ObtenDatos(Paq);
            Paq.Error = error;
            Paq.Exito = exito;
            return View("index", Paq);
        }
    }
}