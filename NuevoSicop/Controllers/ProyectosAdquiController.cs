using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.ViewerObjectModel;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Security.Provider;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace NuevoSicop.Controllers
{
    public  class ProyectosAdquiController : Controller
    {
        // GET: ProyectosAdqui
        sicop_tempEntities _sicop = new sicop_tempEntities();



        public RegistroAdquisicion ObtenDatos(RegistroAdquisicion proy)
        {
            

            var ejercicios =
                _sicop.PaquetesProcedimiento.Select(x => new {anio = x.Anio}).Distinct().OrderBy(x => x.anio);
            proy.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");

            if (proy.IdEjercicio > 0)
            {
                var recursos =
                    _sicop.PGO.Where(x => x.ANIO == proy.IdEjercicio)
                        .Join(_sicop.DetalleOficiosRecurso, x => x.Clave, y => y.IdRecurso,
                            (x, y) => new {id = x.Clave, desc = y.Oficio+" - "+ x.DESCRIPCION , anio = x.ANIO})
                        .OrderBy(x => x.desc)
                        .ThenBy(x => x.id);
                proy.ListaRecursos = new SelectList(recursos, "id", "desc");
            }
            proy.ListaPersonal = new SelectList(_sicop.DC.Select(x => new { x.Nombre, x.Clave }).OrderBy(x => x.Nombre), "clave", "Nombre");
            proy.ListaModalidad = new SelectList(new List<String> { "LICITACIÓN PÚBLICA", "INVITACIÓN A CUANDO MENOS TRES PERSONAS", "INVITACIÓN A CUANDO MENOS CINCO PERSONAS", "ADJUDICACIÓN DIRECTA", "ADMINISTRACIÓN DIRECTA", "SIMPLIFICADO MAYOR", "SIMPLIFICADO MENOR", "COMPRA DIRECTA" });
            proy.ListaProveedor = new SelectList(_sicop.EMPRESAS.Select(x => new { x.Clave, Nombre = x.Nombre + "-" + x.Clave }).OrderBy(x => x.Nombre), "Clave", "Nombre");
            proy.ListaTiposContrato = new SelectList(new List<string> { "FEDERAL", "ESTATAL" });
            if (proy.Recurso > 0)
            {
               // proy.ListaContratos = new SelectList(_sicop.contratos.Where(x => x.recurso == proy.Recurso).Select(x=> new { x.no_contrato , x.Id }).OrderBy(x=>x.no_contrato) , "id", "no_contrato");              
                proy.ListaProyectos = new SelectList(_sicop.ESCU.Where(x=>x.ClaveRecurso == proy.Recurso).Select(x=>new { Id= x.id,Clave =  x.CCT_TURNO, } ),"Id","Clave");
            }
            if (proy.IdEjercicio > 0)
            {
                if (string.IsNullOrEmpty(proy.BuscarContrato))
                {
                   proy.ListaContratos =
                        new SelectList(
                            _sicop.contratos.Where(x =>  x.FechaAlta.Value.Year  == proy.IdEjercicio && x.no_contrato.Contains("CV"))
                                .Select(x => new {x.no_contrato}).Distinct().OrderBy(x => x.no_contrato), "no_contrato", "no_contrato");
                }
                else
                {
                    proy.ListaContratos =
                        new SelectList(
                            _sicop.contratos.Where(x =>x.FechaAlta.Value.Year == proy.IdEjercicio && x.no_contrato.Contains(proy.BuscarContrato) &&
                                    x.no_contrato.Contains("CV"))
                                .Select(x => new {x.no_contrato}).Distinct().OrderBy(x => x.no_contrato), "no_contrato", "no_contrato");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(proy.BuscarContrato))
                {
                    proy.ListaContratos =
                        new SelectList(
                            _sicop.contratos.Where(
                                x => x.no_contrato.Contains("CV"))
                                .Select(x => new { x.no_contrato}).OrderBy(x => x.no_contrato), "no_contrato", "no_contrato");
                }
                else
                {
                    proy.ListaContratos =
                        new SelectList(
                            _sicop.contratos.Where(
                                x =>
                                    x.no_contrato.Contains(proy.BuscarContrato) &&
                                    x.no_contrato.Contains("CV"))
                                .Select(x => new { x.no_contrato}).OrderBy(x => x.no_contrato), "no_contrato", "no_contrato");
                }
            }

            if (_sicop.contratos.Any(x=>x.no_contrato == proy.NoContrato))
            {  
                var cont = _sicop.contratos.FirstOrDefault(x => x.no_contrato == proy.NoContrato);

                var idproyecto = _sicop.ESCU.Any(x => x.CCT_TURNO == cont.cve_escuela)? _sicop.ESCU.FirstOrDefault(x => x.CCT_TURNO == cont.cve_escuela).id:0;

                if (_sicop.ContratoObra.Any(x => x.no_contrato == cont.no_contrato))
                {
                    var contObra = _sicop.ContratoObra.FirstOrDefault(x => x.no_contrato == cont.no_contrato);
                    proy.FundamentoLegal = contObra.Articulo;
                    proy.AplicaAnticipo = contObra.Anticipo == "SI" ? true : false;
                    proy.TipoContrato = contObra.Aportacion;
                    proy.FundamentoLegal = contObra.Articulo;
                    proy.Garantia = contObra.Cumplimiento == "SI" ? true : false;
                    proy.Fianza = contObra.ViciosOcultos == "SI" ? true : false;
                    proy.FechaProcedimiento=contObra.FechaProcedimiento.GetValueOrDefault(DateTime.MinValue);                 
                }

                proy.Recurso = cont.recurso > 0 ? (int) cont.recurso : proy.Recurso;
                proy.NoContrato = cont.no_contrato;
                proy.IdModalidad = cont.modalidad_contrato;
                proy.IdProveedor =  cont.Contratista.GetValueOrDefault(0);
                proy.MontoTotal = cont.monto_contrato.GetValueOrDefault(0);
                proy.DescripcionTrabajo = cont.desc_obra;
                proy.NumeroProcedimiento = cont.no_licitacion;
                proy.FechaProcedimiento = cont.FFirma.GetValueOrDefault(DateTime.MinValue);
                proy.FechaInicio = cont.Inicia.GetValueOrDefault(DateTime.MinValue);
                proy.FechaTermino = cont.termina.GetValueOrDefault(DateTime.MinValue);
                proy.IdDirector = cont.IdDirectorGeneral.GetValueOrDefault(0);
                proy.Observaciones = cont.observaciones;
                proy.PorcentajeAnticipo = cont.PorcentajeAnticipo.GetValueOrDefault(0);
                proy.IdProyecto = idproyecto;
                //proy.ListaFundamentoLegal = cont.
            }
            if (_sicop.ProyectosEnContrato.Any(x => x.CveContrato == proy.NoContrato))
            {
                var proyectos = _sicop.ProyectosEnContrato.Where(x => x.CveContrato == proy.NoContrato);
               foreach (var item in proyectos)
                {
                    var importeT = _sicop.ConceptosAdjudicacion.Where(x => x.IDProyectoContrato == item.ID).Sum(x=>x.ImporteTotal).GetValueOrDefault(0);
                    proy.Proyectos.Add(new Proy()
                    {
                       
                       IdProyectosEncontrato = item.ID,
                       CveProyecto = item.CveProyecto,
                       CveContrato = proy.NoContrato,
                       Lote =  item.TieneLote.GetValueOrDefault(false),
                       NombreLote =  item.NombreLote,
                       ImporteTotal =(decimal) importeT,
                       Conceptos =  ObtenConceptos(item.ID)
                    });
                }
                proy.ListaPartidaPresupuestal= new SelectList(_sicop.CuentaPartida.Select(x=> new { x.clave, partida =x.clave+" - "+ x.partida }).OrderBy(x=>x.partida), "clave", "partida");
            }
            return proy;
        }

        public  List<ConceptoAdj> ObtenConceptos(int idProyeeEnContr)
        {
            List<ConceptoAdj> conc = new List<ConceptoAdj>();
            if  (_sicop.ConceptosAdjudicacion.Any(x => x.IDProyectoContrato == idProyeeEnContr))
            {
                var conceptos = _sicop.ConceptosAdjudicacion.Where(x => x.IDProyectoContrato == idProyeeEnContr);
                var concep = new ConceptoAdj();
                var cont = 0;
                foreach (var item in conceptos.OrderBy(x=>x.Partida))
                {

                    if (item.Cantidad == 0)
                    {
                        concep.Descripcion += item.Descripcion.ToUpper();
                        concep.IdClave += ',' + item.ID.ToString();
                    }
                    else
                    {
                        if (cont>0)
                        {
                            conc.Add(concep);
                        }
                        
                        concep= new ConceptoAdj();
                        concep.IdConcepto = item.ID;
                        concep.IdProyectoContrato = idProyeeEnContr;
                        concep.Partida = item.Partida.GetValueOrDefault(0);
                        concep.ClaveArticulo = item.ClaveArticulo;
                        concep.Descripcion = item.Descripcion.ToUpper();
                        concep.Cantidad = item.Cantidad.GetValueOrDefault(0);
                        concep.PrecioUnitario = (decimal) item.PrecioUnitario.GetValueOrDefault(0);
                        concep.Importe = (decimal) item.Importe.GetValueOrDefault(0);
                        concep.ImporteIva = (decimal) item.ImporteIVA.GetValueOrDefault(0);
                        concep.ImporteTotal = (decimal) item.ImporteTotal.GetValueOrDefault(0);
                        concep.Iva = (decimal) item.IVA.GetValueOrDefault(0);
                        concep.IdPartidaPresupuestal = item.PartidaPresupuestal.GetValueOrDefault(0);
                        concep.IdClave += ',' + item.ID.ToString();
                    }

                    cont ++;
                }
                if (concep.Cantidad > 0)
                {
                    conc.Add(concep);
                }
            }            
            return conc;
        }

        public JsonResult DescripcionProy(int proy)
        {
            string resp ="";
            if (_sicop.ESCU.Any(x => x.id == proy))
            {
                resp = _sicop.ESCU.FirstOrDefault(x => x.id == proy).NOMESC;
            }
            return Json(resp);
        }

        public JsonResult PyEnPrograma(int proy)
        {
            var resp = new Listas();
             resp.Proyectos = new SelectList(new List<string>() { " -Seleccione Proyectos- " });
            if (_sicop.ESCU.Any(x => x.ClaveRecurso== proy))
            {
               resp.Proyectos = new SelectList(_sicop.ESCU.Where(x=>x.ClaveRecurso == proy).Select(x=> new  {Id= x.id, Proyecto = x.CCT_TURNO } ).OrderBy(x=>x.Proyecto), "Id", "Proyecto" );                
            }
            return Json(resp);
        }

        public ActionResult Index()
        {
            var proy = new RegistroAdquisicion();
            proy = ObtenDatos(proy);   
            return View(proy);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult BuscarContratos(RegistroAdquisicion proy)
        {         
            proy = ObtenDatos(proy);
            return View( "Index" ,proy);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Ejercicio(RegistroAdquisicion proy)
        {
            proy = ObtenDatos(proy);   
            return View("Index", proy);
        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Recurso(RegistroAdquisicion proy)
        {
            proy = ObtenDatos(proy);
            return View("Index", proy);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult Contrato(RegistroAdquisicion proy)
        {
            proy = ObtenDatos(proy);
            return View("Index", proy);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult  GuardaContrato(RegistroAdquisicion proy)
        {
            var error = "";
            var exito = "";
            //proy = ObtenDatos(proy);
            try
            {
                if (_sicop.contratos.Any(x => x.no_contrato == proy.NoContrato))
                {
                    var cont = _sicop.contratos.FirstOrDefault(x => x.no_contrato == proy.NoContrato);
                    
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == proy.IdProyecto);
                    cont.no_licitacion = proy.NumeroProcedimiento;
                    cont.modalidad_contrato = proy.IdModalidad;
                    cont.Contratista = proy.IdProveedor;
                    cont.cve_escuela = py.CCT_TURNO;
                    cont.desc_obra = proy.DescripcionTrabajo;
                    cont.monto_contrato = proy.MontoTotal;
                    cont.monto_final_contrato = proy.MontoTotal;
                    cont.anticipo = proy.PorcentajeAnticipo * proy.MontoTotal / 100;
                    cont.monto_convenio = 0;
                    cont.plazo_conv = 0;
                    cont.plazo_final_cont = 0;
                    cont.recurso = proy.Recurso;
                    cont.FFirma = proy.FechaFirma;
                    cont.Inicia = proy.FechaInicio;
                    cont.termina = proy.FechaTermino;
                    cont.FechaInicioReal = proy.FechaInicio;
                    cont.FechaTerminoReal = proy.FechaTermino;
                    cont.plazo_dias_nat = 5;
                    cont.Obra_Adquisicion = "ADQUISICION";
                    cont.PerteneceCMIC = "NO";
                    cont.FechaAlta = DateTime.Now;
                    cont.iva = proy.Iva;
                    cont.IdDirectorGeneral = proy.IdDirector;
                    cont.TipoObra = "SUPERIOR";
                    cont.tomo = "1";
                    cont.observaciones = proy.Observaciones;

                    if (_sicop.ContratoObra.Any(x => x.no_contrato == proy.NoContrato))
                    {
                        var contobra = _sicop.ContratoObra.FirstOrDefault(x => x.no_contrato == proy.NoContrato);
                        contobra.Anticipo = "NO";
                        contobra.Aportacion = proy.TipoContrato;
                        contobra.Articulo = String.IsNullOrEmpty(proy.FundamentoLegal) ? "" : proy.FundamentoLegal;
                        contobra.Cumplimiento = proy.Garantia ? "SI" : "NO";
                        contobra.ViciosOcultos = proy.Fianza ? "SI" : "NO";
                        contobra.cveArticulo = 1;
                        contobra.FechaProcedimiento = proy.FechaProcedimiento;
                        contobra.Cambioarticulo = 1;
                      }
                    else
                    {
                        _sicop.ContratoObra.Add(new ContratoObra()
                        {
                            no_contrato = proy.NoContrato,
                            Anticipo = "NO",
                            Aportacion = proy.TipoContrato,
                            Articulo = String.IsNullOrEmpty(proy.FundamentoLegal) ? "" : proy.FundamentoLegal,
                            Cumplimiento = proy.Garantia ? "SI" : "NO",
                            ViciosOcultos = proy.Fianza ? "SI" : "NO",
                            cveArticulo = 1,
                            FechaProcedimiento = proy.FechaProcedimiento,
                            Cambioarticulo = 1

                        });
                    }                    
                    exito = "Se Modifico correctamente el proyecto";
                }
                else
                {
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == proy.IdProyecto);
                    _sicop.contratos.Add(new contratos()
                    {
                        no_contrato = proy.NoContrato.ToUpper(),
                        no_licitacion = proy.NumeroProcedimiento,
                        modalidad_contrato = proy.IdModalidad,
                        Contratista = proy.IdProveedor,
                        cve_escuela = py.CCT_TURNO,
                        desc_obra = proy.DescripcionTrabajo,
                        monto_contrato = proy.MontoTotal,
                        monto_final_contrato = proy.MontoTotal,
                        anticipo = proy.PorcentajeAnticipo * proy.MontoTotal / 100,
                        monto_convenio = 0,
                        plazo_conv = 0,
                        plazo_final_cont = 0,
                        recurso = proy.Recurso,
                        FFirma = proy.FechaFirma,
                        Inicia = proy.FechaInicio,
                        termina = proy.FechaTermino,
                        FechaInicioReal = proy.FechaInicio,
                        FechaTerminoReal = proy.FechaTermino,
                        plazo_dias_nat = 6,
                        Obra_Adquisicion = "ADQUISICION",
                        PerteneceCMIC = "NO",
                        FechaAlta = DateTime.Now,
                        iva = proy.Iva,
                        IdDirectorGeneral = proy.IdDirector,
                        TipoObra = "SUPERIOR",
                        tomo = "1",
                        observaciones = proy.Observaciones,


                    });
                    _sicop.ContratoObra.Add(new ContratoObra()
                    {
                        no_contrato = proy.NoContrato,
                        Anticipo = "NO",
                        Aportacion = proy.TipoContrato,
                        Articulo = String.IsNullOrEmpty(proy.FundamentoLegal) ? "" : proy.FundamentoLegal,
                        Cumplimiento = proy.Garantia ? "SI" : "NO",
                        ViciosOcultos = proy.Fianza ? "SI" : "NO",
                        cveArticulo = 1,
                        FechaProcedimiento = proy.FechaProcedimiento,
                        Cambioarticulo = 1

                    });
                    _sicop.checklist.Add(new checklist()
                    {
                        cve_check_list = proy.NoContrato,
                        d1 = "No",
                        resolucion_inah = "No aplica",
                        d3_1 = "No",
                        d4_1 = "No",
                        d5_1 = "No",
                        d6_1 = "No",
                        d7_1 = "No",
                        d8_1 = "No",
                        d9_1 = "No",
                        d10_1 = "No aplica",
                        d4 = "No",

                        d6 = "No",
                        d7 = "No",
                        d8 = "No",
                        d9 = "No",
                        d10 = "No",
                        d11 = "No",
                        d14 = "No",
                        d15 = "No aplica",
                        d16 = "No",
                        d17 = "No",
                        d18 = "No",
                        d23 = "No",
                        d24 = "No",
                        d25 = "No",
                        d26 = "No",
                        d28 = "No",
                        d12 = "No",
                        Inicio_Obra = "No",
                        Est_Residencia = "No",
                        Design_Residente = "No",
                        Entrega_ATrabajo = "No",
                        Aviso_Estim = "No aplica",
                        Aviso_Atraso = "No aplica",
                        d31 = "No",
                        no_monto_anticipo = 0,
                        d32 = "No",
                        d33 = "No aplica",
                        d34 = "No aplica",
                        d36 = "No aplica",
                        d39 = "No aplica",
                        d40 = "No aplica",
                        d41 = "No aplica",
                        d43 = "No aplica",
                        d47 = "No aplica",
                        no_endoso = "No aplica",
                        d48 = "No aplica",
                        d50 = "No aplica",
                        d54 = "No aplica",
                        d55 = "No aplica",
                        d56 = "No aplica",
                        ajuste_de_costos = "No aplica",

                        d57 = "No aplica",
                        d58 = "Si",
                        d59 = "No aplica",
                        d60 = "No",
                        d61 = "No",
                        d69 = "No",
                        d71 = "No",
                        Inv_EntRecep = "No aplica",
                        d72 = "No",
                        d73 = "No aplica",
                        d74 = "No",
                        d75 = "No",
                        solicitudpubconv = "No aplica",
                        acuserecibojunacla = "No aplica",
                        acuserecibopreape = "No aplica",
                        acuserecibofallo = "No aplica",
                        acuserecibodatrelev = "No aplica",
                        afianzadora2 = "No aplica",
                        polizaoriginal = "No aplica",
                        
                        vigencia = "No aplica",
                        contescontratproved = "No aplica",
                        escritoterminacion = "No aplica",
                        anotacionenbitacora = "No aplica",
                        actacircunst = "No aplica",
                        pagogastosnrecu = "No aplica",
                        ofisoliajuscost = "No aplica",
                        analisiscalculoajuscost = "No aplica",
                        pagoajustcost = "No aplica",
                        conveniosuspension = "No aplica",
                        polizareposabicivil = "No aplica",
                        nodepoliza = "No aplica",
                        oficioresoluc = "No aplica",
                        endozofianzcumplim = "No aplica"

                    });
                    _sicop.ProyectosEnContrato.Add(new ProyectosEnContrato()
                    {
                        CveContrato = proy.NoContrato.ToUpper(),
                        CveProyecto = py.CCT_TURNO,
                        TieneLote = true,
                        NombreLote = "",
                    });
                    _sicop.Detalle_Proyectos.Add(new Detalle_Proyectos()
                    {
                        Contrato = proy.NoContrato.ToUpper(),
                        Proyecto = py.CCT_TURNO,
                        idproyecto = py.id,
                    });
                    _sicop.SaveChanges();
                    exito = "se guardo correctamente el proyecto";
                }
            }
            catch
            {
                error = "No fue posible guardar el contrato";
            }
           
            proy = ObtenDatos(proy);
            proy.Error = error;
            proy.Exito = exito;
            return View("Index", proy);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult NuevoContrato(RegistroAdquisicion proy)
        {
            proy= new RegistroAdquisicion();


            proy = ObtenDatos(proy);
            return View("Index", proy);
        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult AgregaProyecto(RegistroAdquisicion proy)
        {
            var Exito = "";
            var Error = "";


            if (_sicop.ESCU.Any(x => x.id == proy.NuevoProy.IdProyecto))
            {
                var py = _sicop.ESCU.FirstOrDefault(x => x.id == proy.NuevoProy.IdProyecto);
                try
                {

                    _sicop.Detalle_Proyectos.Add(new Detalle_Proyectos()
                    {
                        Contrato = proy.NoContrato,
                        Proyecto = py.CCT_TURNO,
                        idproyecto = py.id

                    });

                    _sicop.ProyectosEnContrato.Add(new ProyectosEnContrato()
                    {
                        CveProyecto = py.CCT_TURNO,
                        CveContrato = proy.NoContrato,
                        TieneLote = proy.NuevoProy.Lote,
                        NombreLote = proy.NuevoProy.NombreLote
                    });
                    _sicop.SaveChanges();
                    Exito = " Se ha agregado correctamente el proyecto.";
                }
                catch
                {
                    Error = " No fue posible agregar el proyecto en el contrato";
                }
            }
            else
            {
                Error = " No se tiene un proyecto seleccionado";
            }


            proy = ObtenDatos(proy);
            proy.Error = Error;
            proy.Exito = Exito;
            return View("Index", proy);
        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult EliminaProyecto(RegistroAdquisicion proy)
        {
            var Exito = "";
            var Error = "";

            if (!string.IsNullOrEmpty(proy.QuitaProyecto))
            {
                if (_sicop.ProyectosEnContrato.Any(
                    x => x.CveContrato == proy.NoContrato && x.CveProyecto == proy.QuitaProyecto))
                {
                    var proycontra =
                        _sicop.ProyectosEnContrato.FirstOrDefault(
                            x => x.CveContrato == proy.NoContrato && x.CveProyecto == proy.QuitaProyecto);
                    if (!_sicop.ConceptosAdjudicacion.Any(x => x.IDProyectoContrato == proycontra.ID))
                    {
                        _sicop.ProyectosEnContrato.Remove(proycontra);
                        if (_sicop.Detalle_Proyectos.Any(
                            x => x.Proyecto == proy.QuitaProyecto && x.Contrato == proy.NoContrato))
                        {
                            var detproy =
                                _sicop.Detalle_Proyectos.FirstOrDefault(
                                    x => x.Proyecto == proy.QuitaProyecto && x.Contrato == proy.NoContrato);
                            _sicop.Detalle_Proyectos.Remove(detproy);
                            _sicop.SaveChanges();
                            Exito = "Se sacó correctamente el proyecto";

                        }
                        else
                        {
                            Error = "El proyecto no se encuentra en detalles proyecto";
                        }

                    }
                    else
                    {
                        Error = "El proyecto Contiene conceptos";
                    }

                }
                else
                {
                    Error = "No se encontro el proyecto";
                }                
            }
            else
            {
                Error = "No se encuentra el proyecto";
            }            
            proy = ObtenDatos(proy);
            proy.Error = Error;
            proy.Exito = Exito;
            return View("Index", proy);
        }
        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult AgregaLote(RegistroAdquisicion proy)
        {
           
                  if (proy.NuevoConcepto.Cantidad > 0 && proy.NuevoConcepto.Descripcion.Length > 1 &&
                      proy.NuevoConcepto.Importe > 0 && proy.NuevoConcepto.IdProyectoContrato>0)
                  {
                      var descrip = "";
                      var cont = 0;
                     while (proy.NuevoConcepto.Descripcion.Length > 0)
                      {
                          var desc = "";
                          var nuevaCadena = "";
                                               
                      if (proy.NuevoConcepto.Descripcion.Length > 350)
                          {
                              desc = proy.NuevoConcepto.Descripcion.Substring(0, 349);
                              nuevaCadena = proy.NuevoConcepto.Descripcion.Substring(349,
                                  proy.NuevoConcepto.Descripcion.Length - 349);
                              proy.NuevoConcepto.Descripcion = nuevaCadena;
                             descrip = descrip + desc;
                          }
                          else
                          {
                              desc = proy.NuevoConcepto.Descripcion;
                             descrip = descrip + desc;
                             proy.NuevoConcepto.Descripcion = "";
                          }
                    if (cont == 0)
                    {
                        _sicop.ConceptosAdjudicacion.Add(new ConceptosAdjudicacion()
                        {
                            IDProyectoContrato = proy.NuevoConcepto.IdProyectoContrato,
                            Partida = proy.NuevoConcepto.Partida,
                            Descripcion = desc,
                            ClaveArticulo = proy.NuevoConcepto.ClaveArticulo,
                            Cantidad = (int)proy.NuevoConcepto.Cantidad,
                            PrecioUnitario = (double)proy.NuevoConcepto.PrecioUnitario,
                            Importe = (double) proy.NuevoConcepto.Importe,
                            ImporteIVA = (double)proy.NuevoConcepto.ImporteIva,
                            ImporteTotal = (double)proy.NuevoConcepto.ImporteTotal,
                            PartidaPresupuestal = (int)proy.NuevoConcepto.IdPartidaPresupuestal,
                            IVA = 16,
                        });
                        _sicop.SaveChanges();
                        cont++;
                       
                    }
                    else
                    {
                        _sicop.ConceptosAdjudicacion.Add(new ConceptosAdjudicacion()
                        {
                            IDProyectoContrato = proy.NuevoConcepto.IdProyectoContrato,
                            Partida = proy.NuevoConcepto.Partida,
                            Descripcion = desc,
                            ClaveArticulo = proy.NuevoConcepto.ClaveArticulo,
                            Cantidad = 0,
                            PrecioUnitario = 0,
                            Importe = 0,
                            ImporteIVA = 0,
                            ImporteTotal = 0,
                            PartidaPresupuestal = (int)proy.NuevoConcepto.IdPartidaPresupuestal
                        });
                        _sicop.SaveChanges();
                        cont++;
                       
                    }

                }
                      
                  }
                  proy = ObtenDatos(proy);
            return View("Index", proy);
        }

        [Authorize(Roles = "Administrador,A-Adquisiciones")]
        [HttpPost]
        public ActionResult EliminaConcepto(RegistroAdquisicion proy)
        {
            var error = "";
            var exito = "";
            try
            {
                var ides = proy.EliminaLote.Split(',');
                foreach (var  item in ides ) 
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        int Id = Convert.ToInt32(item);
                        if (_sicop.ConceptosAdjudicacion.Any(x => x.ID == Id))
                        {
                            var Dato =_sicop.ConceptosAdjudicacion.FirstOrDefault(x => x.ID == Id);
                            _sicop.ConceptosAdjudicacion.Remove(Dato);
                            _sicop.SaveChanges();
                            exito = "Se eliminó correctamente el Concepto";
                        }                                               
                    }                    
                }

            }
            catch
            {
                error = "No fue posible eliminar el Concepto";
            }        
            proy = ObtenDatos(proy);
            proy.Error = error;
            proy.Exito = exito;
            return View("Index", proy);
        }

    }

    
}