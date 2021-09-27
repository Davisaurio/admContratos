using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.Ajax.Utilities;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class AdquisicionesController : Controller
    {
        sicop_tempEntities _sicop = new sicop_tempEntities();
        // GET: Adquisiciones
        public AdquisicionesModelView ObtenDatos(AdquisicionesModelView Adq)
        {
            var ejercicios =_sicop.PaquetesProcedimiento.Select(x => new { anio = x.Anio }).Distinct().OrderBy(x => x.anio);
            Adq.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");
            if (Adq.IdEjercicio > 0)
            {
                if (string.IsNullOrEmpty(Adq.BuscarContrato))
                {
                    Adq.ListaContratos = new SelectList(_sicop.vwReportesContratoAdq.Where(x => x.ejercicio == Adq.IdEjercicio && x.no_contrato.Contains("CV")).Select(x => new { x.no_contrato, x.ID }).OrderBy(x => x.no_contrato), "no_contrato", "no_contrato");
                }
                else
                {
                    Adq.ListaContratos = new SelectList(_sicop.vwReportesContratoAdq.Where(x => x.ejercicio == Adq.IdEjercicio && x.no_contrato.Contains(Adq.BuscarContrato)&& x.no_contrato.Contains("CV")).Select(x => new { x.no_contrato, x.ID }).OrderBy(x => x.no_contrato), "Id", "no_contrato");
                }
            }

            if (!string.IsNullOrEmpty(Adq.IdContrato))
            {
                var itAdq = _sicop.vwReportesContratoAdq.FirstOrDefault(x => x.ejercicio == Adq.IdEjercicio && x.no_contrato == Adq.IdContrato);
                var GContrato = _sicop.GeneradorContrato.FirstOrDefault(x => x.NumeroContrato == Adq.IdContrato);

                if (itAdq != null && GContrato != null)
                {

                    Adq.IdContrato = Adq.IdContrato;
                    Adq.modalidad = itAdq.modalidad;
                    Adq.Recurso = itAdq.programa;
                    Adq.Procedimiento = itAdq.Procedimiento;
                    Adq.Contrato = itAdq.no_contrato;
                    Adq.Descripcion = itAdq.DescripcionTrabajo;
                    Adq.Plazo = itAdq.plazo.GetValueOrDefault(0);
                    Adq.Inicia = itAdq.fechainicio.GetValueOrDefault(DateTime.MinValue);
                    Adq.Termina = itAdq.FechaTermino.GetValueOrDefault(DateTime.MinValue);
                    Adq.Monto = itAdq.ImporteContrato.GetValueOrDefault(0);
                    Adq.Anticipo = itAdq.anticipo.GetValueOrDefault(0);
                    Adq.AnticipoxCien = itAdq.PorcentajeAnticipo.GetValueOrDefault(0);
                    Adq.Iva = itAdq.iva.GetValueOrDefault(0) * 100;
                    Adq.TipoPersona = itAdq.TipoPersona;
                    Adq.TipoInversion = itAdq.Inversion;
                    Adq.Contratista = itAdq.Empresa;
                }
            }
            return Adq;
        }
        public ActionResult Index()
        {
            AdquisicionesModelView Adq = new AdquisicionesModelView();

            Adq = ObtenDatos(Adq); 
            return View ("Index", Adq);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Ejercicio( AdquisicionesModelView adq)
        {
            adq = ObtenDatos(adq);
            return View("Index", adq);
        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult BuscarContratos( AdquisicionesModelView adq)
        {
            adq = ObtenDatos(adq);
            return View("Index", adq);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Contratos( AdquisicionesModelView adq)
        {
            adq = ObtenDatos(adq);
            return View("Index", adq);
        }
        public List<RepCaratula> LlenaCaratula(int ejercicio, string Contrato)
        {
            var Rep = new List<RepCaratula>();
            var contratos = _sicop.vwRecursosEnContrato.Where(x => x.Contrato == Contrato);
            var oficios = "";
            var fechas = "";
            var partidas = "";
           // decimal importeautorizado = 0;
            var proyectos = "";
            var desc = "";
            var  cont = 0;
            foreach (var it in contratos.Select(x=>new {  x.Oficio, x.AutorizadoOficio , x.fechaoficio}).Distinct().OrderBy(x=>x.Oficio))
            {
                if (cont == 0)
                {
                    oficios += it.Oficio;
                    fechas += it.fechaoficio.GetValueOrDefault(DateTime.MinValue).ToString("dd DE MMMMM DE yyyy");
                
                   
                }
                else
                {
                    oficios +=", "+ it.Oficio;
                    fechas +=", "+ it.fechaoficio.GetValueOrDefault(DateTime.MinValue).ToString("dd DE MMMMM DE yyyy");
                }
                
                cont++;
            }
            cont = 0;
            foreach (var item in contratos.Distinct())
            {
               
                if (cont == 0)
                {
                    proyectos += item.ClaveProyecto;
                    desc += (item.Alcance.Contains(item.ClaveProyecto) ? item.Alcance : item.ClaveProyecto + ".-" + item.Alcance);
                  
                }
                else
                {
                    proyectos += ", "+item.ClaveProyecto;
                    desc +="@"+ (item.Alcance.Contains(item.ClaveProyecto)?item.Alcance : item.ClaveProyecto+".-"+item.Alcance ); 
                }
                //importeautorizado += item.AutorizadoProyecto.GetValueOrDefault(0);
                cont++;
            }

            var valores = _sicop.vwReporteCaratulaAdq.Where(x => x.ejercicio == ejercicio && x.no_contrato == Contrato).Select(x=>x.proyecto ).Distinct();
            foreach (var it in valores)
            {
                var item = _sicop.vwReporteCaratulaAdq.FirstOrDefault(x => x.ejercicio == ejercicio && x.no_contrato == Contrato && x.proyecto== it);
                var cveFinan = _sicop.ESCU.Any(x => x.CCT_TURNO == item.proyecto)
                    ? _sicop.ESCU.FirstOrDefault(x => x.CCT_TURNO == item.proyecto).Estatus
                    : "";
                var importeProyecto = contratos.FirstOrDefault(x => x.ClaveProyecto == item.proyecto).importeProyecto;
                Rep.Add(new RepCaratula()
                {

                    NumPaquete = item.paqueteDesc,
                    PaqueteDesc = item.paqueteDesc,
                    Partida = item.clavepartida,
                    FuenteFianciera = item.ClaveFF,
                    Procedencia = item.Procedencia,
                    ClaveProcedencia = "xx" ,
                    Proyecto = proyectos,
                    ClaveProgramatica = item.claveProgramatica,
                    Contrato = item.no_contrato,
                    ImporteContratado = (decimal)item.monto_contrato.GetValueOrDefault(0),
                    Partidadesc = string.IsNullOrEmpty(item.Partidadesc) ? "" : item.Partidadesc,
                    TipoInversion = item.TipoInversion,
                    Programa = item.Programa,
                    Modalidad = item.modalidad,
                    NoProcedimiento = item.NoProcedimiento,
                    FechaContrato = item.FechaContrato.GetValueOrDefault(DateTime.MinValue),
                    DescripcionTrabajo = desc,
                    DescripcionTrabajo2 = item.DescripcionTrabajo2,
                    Plazo = item.plazo.GetValueOrDefault(0),
                    FechaInicio = item.fechainicio.GetValueOrDefault(DateTime.MinValue),
                    FechaTermino = item.FechaTermino.GetValueOrDefault(DateTime.MinValue),
                    MontoAutorizado =  (decimal)item.MontoAutorizado,
                    Anticipo = (decimal)item.anticipo,
                    Oficio = oficios,
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
                    Imss = fechas.ToUpper(),
                    Infonavit = item.Infonavit,
                    Padron = item.Padron,
                    Depen = item.dep,
                    ClaveFinanzas = cveFinan,
                    ImporteProyecto = (decimal) importeProyecto,
                });
            }

            return Rep;
        }
        public List<ReporteContratoAdq> LlenaContrato1(int ejercicio, string Contrato , bool cumplimiento )
        {
            var Rep = new List<ReporteContratoAdq>();
            var falloactual = _sicop.vwReportesContratoAdq.Where(x => x.ejercicio == ejercicio && x.no_contrato == Contrato);
            var recursos = _sicop.vwRecursosEnContrato.Where(x => x.Contrato == Contrato);
            var descripcion = "";
            var Oficios = "";
            var AutorizadoPy =recursos.Sum(x=>x.AutorizadoProyecto);
            var Alcances = "";
            var cont = 0; 



            foreach (var it  in recursos.Select(x => new { x.Oficio, x.recurso ,x.fechaoficio, x.AutorizadoOficio }).Distinct().OrderBy(x => x.Oficio))
            {
                var  autori = Moneda.Convertir(it.AutorizadoOficio.ToString(), true, "PESOS"); 

                if (cont == 0)
                {
                    descripcion = it.Oficio + " DE FECHA  " +
                                  it.fechaoficio.GetValueOrDefault(DateTime.MinValue).ToString("dd DE MMMMM DE yyyy").ToUpper() +
                                  ", SU AUTORIZACIÓN ES POR UN MONTO DE " +it.AutorizadoOficio.GetValueOrDefault(0).ToString("$ ##,###.##") + " (" + autori+ "), CON CARGO AL PROGRAMA: " +it.recurso.ToUpper();   
                    Oficios += it.Oficio;
                   
                }
                else
                {
                    descripcion += ", " +it.Oficio + " DE FECHA  " +
                                 it.fechaoficio.GetValueOrDefault(DateTime.MinValue).ToString("dd DE MMMMM DE yyyy").ToUpper() +
                                 ", SU AUTORIZACIÓN ES POR UN MONTO DE " + it.AutorizadoOficio.GetValueOrDefault(0).ToString("$ ##,###.##") + " (" + autori + "), CON CARGO AL PROGRAMA: " + it.recurso.ToUpper();
                    Oficios += ", "+ it.Oficio;
                   
                }        
                cont++;

            }
             cont = 0;
            foreach (var it in recursos)
            {
                if (cont == 0)
                {
                    Alcances += it.Alcance;

                }
                else
                {
                    Alcances += ", "+it.Alcance;
                }
                cont++;
            }


            foreach (var item in falloactual)
            {


                if (_sicop.GeneradorContrato.Any(x => x.NumeroContrato == item.no_contrato))
                {
                    var GContrato = _sicop.GeneradorContrato.FirstOrDefault(x => x.NumeroContrato == item.no_contrato);
                    

                    var Cargo1 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo1) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo1).Cargo : "NO SE ENCONTRO";
                    var Cargo2 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo2) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo2).Cargo : "NO SE ENCONTRO";
                    var Cargo3 = _sicop.DC.Any(x => x.Nombre == GContrato.Testigo3) ? _sicop.DC.FirstOrDefault(x => x.Nombre == GContrato.Testigo3).Cargo : "NO SE ENCONTRO";
                    Rep.Add(new ReporteContratoAdq()
                    {
                        Dependencia = item.Dependencia,
                        TipoContrato = item.modalidad,
                        Programa =descripcion, // item.programa,
                        Ejercicio = item.ejercicio.GetValueOrDefault(0),
                        Proyecto = item.Proyecto,
                        Partida = "",
                        Procedimiento = item.Procedimiento,
                        FechaProcedimiento = GContrato.FechaProcedimiento.GetValueOrDefault(DateTime.MinValue),
                        Articulo = GContrato.Articulo,
                        Contrato = item.no_contrato,
                        FechaContrato = item.FechaContrato.GetValueOrDefault(DateTime.MinValue),
                        DescripcionObra = Alcances,//item.DescripcionTrabajo,
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
                        OficioAutorizaionInv = Oficios,//item.OficioAutorizacion,
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
                        TipoInversion = item.Inversion,
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
                        FechaFallo = item.FechaContrato.GetValueOrDefault(DateTime.MinValue),
                        AutorizadoProyecto = AutorizadoPy.GetValueOrDefault(0),
                        SupervisaContrato = item.SupervisorContrato,
                        EntregaBienes = item.EntregadeBienes,
                        FianzaCumplimiento = cumplimiento,
                       
                    });


                }

            }
            return Rep;
        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Caratula(AdquisicionesModelView adq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaCaratula(adq.IdEjercicio, adq.Contrato);
            if (Datos.Count() > 0)
            {
                var valores = _sicop.vwReporteCaratulaAdq.FirstOrDefault(x => x.ejercicio == adq.IdEjercicio && x.no_contrato == adq.Contrato);

                var primer = _sicop.ContratoObra.FirstOrDefault(x => x.no_contrato == valores.no_contrato);
                paq = Datos.FirstOrDefault().NumPaquete;

                if (primer.Aportacion == "FEDERAL")
                { rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "CaratulaAdq.rpt")); }
                else
                { rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "CaratulaAdq.rpt")); }
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
                adq.Error = "No se encontró sufucuente información para generar este reporte.";
                return RedirectToAction("Index");
            }
        }

        public string Ceros(int num, int dig)
        {
            var cadena="";

            var  digitos = dig - num.ToString().Length ;
            for (var x = 0; x < digitos; x++)
            {
                cadena += "0";
            }
            
            return cadena+num.ToString();
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Contrato1(AdquisicionesModelView adq)
        {
            ReportDocument rd = new ReportDocument();
            var paq = "";
            var Datos = LlenaContrato1(adq.IdEjercicio, adq.Contrato, adq.FianzaCumplimineto);
             
            var proyecto = Datos.FirstOrDefault().Proyecto;
            var idproyCont = _sicop.vwReportesContratoAdq.Where(x => x.no_contrato == adq.Contrato);
                var con = new List<ConceptosContratoAdq>();


            foreach (var it in idproyCont)
            {   var concep = _sicop.ConceptosAdjudicacion.Where(x => x.IDProyectoContrato == it.idproyectosencontrato);
                foreach (var item  in concep)
                {
                   
                    con.Add(new ConceptosContratoAdq()
                    {
                        
                        Proyecto =it.Proyecto,
                        Partida = Ceros(item.Partida.GetValueOrDefault(0),2),
                        PartidaPresupuestal =  item.PartidaPresupuestal.GetValueOrDefault(0).ToString(), 
                        Cantidad = item.Cantidad.GetValueOrDefault(0),
                        ClaveArt = item.ClaveArticulo,
                        Descripcion = item.Descripcion,
                        PrecioUnidad = (decimal) item.PrecioUnitario,
                        Importe = (decimal) item.Importe,
                        IVA = (decimal) item.ImporteIVA,
                        ImporteTotal = (decimal) item.ImporteTotal
                    });
                }
            }
            con.OrderBy(x => x.PartidaPresupuestal).ThenBy(x => x.Partida);
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
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq2.rpt"));
                            }
                            break;
                        case "INVITACION A CUANDO MENOS TRES PERSONAS":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq2.rpt"));
                            }
                            break;
                        case "LICITACION PUBLICA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq2.rpt"));
                            }
                            break;
                        default:
                            {
                                if (Datos.FirstOrDefault().AnticipoXcien > 0)
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq1.rpt"));
                                }
                                else
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoEstAdq2.rpt"));
                                }
                                break;
                            }
                    }

                }
                else
                {                    
                    switch (Datos.FirstOrDefault().TipoContrato)
                    {
                        case "ADJUDICACIÓN DIRECTA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq2.rpt"));
                            }
                            break;
                        case "INVITACION A CUANDO MENOS TRES PERSONAS":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq2.rpt"));
                            }
                            break;
                        case "LICITACIÓN PÚBLICA":
                            if (Datos.FirstOrDefault().AnticipoXcien > 0)
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq1.rpt"));
                            }
                            else
                            {
                                rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq2.rpt"));
                            }
                            break;
                        default:
                            {

                                if (Datos.FirstOrDefault().AnticipoXcien > 0)
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq1.rpt"));
                                }
                                else
                                {
                                    rd.Load(Path.Combine(Server.MapPath("../Reportes/Contratos"), "ContratoFedAdq2.rpt"));
                                }
                                break;
                            }
                    }
                }
                if (Datos.Count > 0)
                {
                    rd.SetDataSource(Datos);
                    rd.Subreports["Conceptos"].SetDataSource(con.OrderBy(x=>x.Partida));
                    try
                    {
                        Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        stream.Seek(0, SeekOrigin.Begin);
                        return File(stream, "application/pdf", "ADQ-" + adq.Contrato + ".pdf");
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    adq.Error = "No se encontraron datos suficientes para generar reporte";
                    return RedirectToAction("Index");
                }

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();



            }
            else
            {
                adq.Error = "No se encontraron datos suficientes para generar reporte";
                return RedirectToAction("Index");
            }

        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult ActualizaAlcance( AdquisicionesModelView adq)
        {
            var exito = "";
            var error = "";
            try
            {
                if (_sicop.VwReporteContrato.Any(x => x.ejercicio == adq.IdEjercicio && x.no_contrato == adq.IdContrato))
                {
                    var contrato = _sicop.contratos.FirstOrDefault(x => x.no_contrato == adq.IdContrato);
                    contrato.desc_obra = adq.Descripcion;
                    _sicop.SaveChanges();
                    exito = "Se realizó el cambio con exito";
                }
            }
            catch
            {
                error = "No fue posible guardar el registro";
            }
            adq = ObtenDatos(adq);
            adq.Error = error;
            adq.Exito = exito;
            return View("index", adq);
        }

    }
}