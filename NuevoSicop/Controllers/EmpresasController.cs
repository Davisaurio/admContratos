using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;
using Root.Reports;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AngularCRUD.Controllers
{
    public class EmpresasController : Controller
    {
        // GET: Empresas

        public ContratosSevrEntities _Control = new ContratosSevrEntities();
        public EmpresasModelView CargaDatos(EmpresasModelView Emp)
        {
            var idempresa = Emp.NuevaEmpresa.Id;
            Emp.TipoEmpresa = new SelectList(_Control.Tamanio, "Id", "Tamaño");
            Emp.TipoInscripcion = new SelectList(_Control.TipoInscripcion, "Id", "TipoIncripcion");
            Emp.TipoPersona = new SelectList(_Control.TipoPersona, "Id", "TipoPersona1");
            Emp.TipoServicio = new SelectList(_Control.TipoServicio, "Id", "TipoServicio1");
            Emp.TipoExperienciaAcreditada = new SelectList(_Control.ExperienciaAcreditada, "Id", "Descripcion");
            Emp.ListaEstados = new SelectList(_Control.Entidades.Where(x => x.idPais == 1).Select(x => new { x.id, nombre = x.nombre.ToUpper() }), "id", "nombre");
            Emp.ListaFirmas = new SelectList(_Control.Supervisores, "Id", "Nombre");
            Emp.NuevaEmpresa = new EmpresaCompleta();
            var listaempresas = _Control.Contratistas.ToList();

            if (Emp.FiltroTipoServicio > 0)
            {
                listaempresas = listaempresas.Where(x => x.IdTipoServicio == Emp.FiltroTipoServicio).ToList();
            }
            if (Emp.FiltroTamanio > 0)
            {
                listaempresas = listaempresas.Where(x => x.IdTamaño == Emp.FiltroTamanio).ToList();
            }
            if (!string.IsNullOrEmpty(Emp.FiltroBuscar))
            {
                listaempresas = listaempresas.Where(x => x.Nombre == Emp.FiltroBuscar).ToList();
            }

            if (Emp.FiltroPersona > 0)
            {
                listaempresas = listaempresas.Where(x => x.IdTipoPersona == Emp.FiltroPersona).ToList();
            }

            foreach (var item in listaempresas)
            {
                Emp.ListaEmpresas.Add(new Empresavw
                {
                    AvanceCaptura = (double)Math.Round((decimal)(item.EstatusCaptura * 100 / 7), 2),
                    Pendiente = 100 - (double)Math.Round((decimal)(item.EstatusCaptura * 100 / 7), 2),
                    ClaveRegistro = item.ClaveRegistro,
                    IniciaVigencia = item.FechaInicioVigencia,
                    TerminaVigencia = item.FechaTerminoVigencia,
                    Id = item.Id,
                    Nombre = item.Nombre,
                    RFC = item.RFC,
                    Tipo = item.TipoPersona.TipoPersona1,
                    Tamanio = item.Tamanio.Tamaño,
                    TipoServicio = item.TipoServicio.TipoServicio1,
                    CapitalContable = item.CapitalContable.GetValueOrDefault(0),
                });
            }
            Emp.NuevaEmpresa.Id = idempresa;


            return Emp;
        }
        public ActionResult Index()
        {
            var emp = new EmpresasModelView();
            emp = CargaDatos(emp);
            return View(emp);
        }
        public JsonResult CargarMunicipios(int ent)
        {
            var resp = new Listas();
            var res = _Control.Municipios.Where(x => x.idEntidad == ent);
            resp.Municipios = new SelectList(res.Select(x => new { x.id, nombre = x.nombre.ToUpper() }).OrderBy(x => x.nombre), "id", "nombre");
            return Json(resp);
        }
        [HttpPost]
        public ActionResult AgregaEmpresa1(EmpresasModelView Emp)
        {
            var exito = "";
            var error = "";
            var nombre = Emp.NuevaEmpresa.Nombre;
            var Estatus = 0;
            if (Emp.IdEditarEmpresa == 0)
            {
                try
                {
                    _Control.Contratistas.Add(new Contratistas()
                    {
                        Nombre = Emp.NuevaEmpresa.Nombre.ToUpper(),
                        RFC = Emp.NuevaEmpresa.Rfc.ToUpper(),
                        ClaveRegistro = Emp.NuevaEmpresa.ClaveRegistro,
                        IdTamaño = Emp.NuevaEmpresa.IdTamañoEmp,
                        IdEstado = 1,
                        IdMunicipio = 1,
                        IdTipoInscripcion = Emp.NuevaEmpresa.IdTipoInscripcion,
                        IdTipoPersona = Emp.NuevaEmpresa.IdTipoPersona,
                        IdTipoServicio = Emp.NuevaEmpresa.IdTipoServicio,
                        FechaInicioVigencia = Emp.NuevaEmpresa.IniciaVigencia,
                        FechaTerminoVigencia = Emp.NuevaEmpresa.TerminaVigencia,
                        FechaRegistro = DateTime.Now,
                        NoExterior = "S/N",
                        EstatusCaptura = 1,
                        IdExperiencia = 1

                    });
                    Estatus = 1;
                    _Control.SaveChanges();
                    exito = "Se guardo Correctamente la empresa: " + Emp.NuevaEmpresa.Nombre.ToUpper();
                }

                catch
                {
                    Emp.Error = "No de guardó la empresa: " + Emp.NuevaEmpresa.Nombre.ToUpper() + "revise sus datos";
                    return RedirectToAction("Index");
                }
                var EstaEmpresa = _Control.Contratistas.FirstOrDefault(x => x.RFC == Emp.NuevaEmpresa.Rfc);
                Emp.IdEditarEmpresa = EstaEmpresa.Id;
            }
            else
            {
                var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.IdEditarEmpresa);
                if (empresa != null)
                {
                    empresa.Nombre = Emp.NuevaEmpresa.Nombre;
                    empresa.RFC = Emp.NuevaEmpresa.Rfc;
                    empresa.ClaveRegistro = Emp.NuevaEmpresa.ClaveRegistro;
                    empresa.IdTamaño = Emp.NuevaEmpresa.IdTamañoEmp;
                    empresa.IdTipoInscripcion = Emp.NuevaEmpresa.IdTipoInscripcion;
                    empresa.IdTipoPersona = Emp.NuevaEmpresa.IdTipoPersona;
                    empresa.IdTipoServicio = Emp.NuevaEmpresa.IdTipoServicio;
                    empresa.FechaInicioVigencia = Emp.NuevaEmpresa.IniciaVigencia;
                    empresa.FechaTerminoVigencia = Emp.NuevaEmpresa.TerminaVigencia;
                    if (Emp.IdEditarEmpresa == 6) { Estatus = 6; }
                    else { empresa.EstatusCaptura = 1; Estatus = 1; }
                    _Control.SaveChanges();

                    exito = "Se guardo Correctamentelos cambios de la empresa: " + Emp.NuevaEmpresa.Nombre.ToUpper();
                }
            }
            Emp.NuevaEmpresa.Id = _Control.Contratistas.OrderByDescending(x => x.Id).Any(x => x.Nombre == nombre) ? _Control.Contratistas.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Nombre == nombre).Id : 0;
            Emp = CargarEmpresa(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        [HttpPost]
        public ActionResult AgregaGenerales(EmpresasModelView Emp)
        {
            var exito = "";
            var error = "";

            var Estatus = Emp.IdEditarEmpresa;
            if ((Emp.IdEditarEmpresa == 1 || Emp.IdEditarEmpresa == 7) && Emp.NuevaEmpresa.Id != 0)
            {
                try
                {
                    var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);
                    empresa.CapitalContable = Emp.NuevaEmpresa.CapitalContable;
                    empresa.IMSS = Emp.NuevaEmpresa.Imss;
                    empresa.Infonavit = Emp.NuevaEmpresa.Infonavit;
                    empresa.CMIC = Emp.NuevaEmpresa.CMIC;
                    empresa.SIEM = Emp.NuevaEmpresa.SIEM;
                    empresa.FolioMercantilElectronico = Emp.NuevaEmpresa.FolioMercantilElectronico.ToUpper();
                    empresa.IdEstado = (short)Emp.NuevaEmpresa.IdEstado;
                    empresa.IdMunicipio = Emp.NuevaEmpresa.IdMunicipio;
                    empresa.Colonia = Emp.NuevaEmpresa.Colonia.ToUpper();
                    empresa.Telefono = Emp.NuevaEmpresa.Telefono;
                    empresa.Fax = Emp.NuevaEmpresa.Fax;
                    empresa.Correo = Emp.NuevaEmpresa.Correo;
                    empresa.CURP = Emp.NuevaEmpresa.Curp;
                    empresa.Domicilio = Emp.NuevaEmpresa.Domicilio.ToUpper();
                    empresa.IdExperiencia = Emp.NuevaEmpresa.IdExperienciaAcreditada;
                    if (empresa.IdTipoPersona == 1)
                    {
                        empresa.EstatusCaptura = 2;
                        Estatus = 2;
                    }
                    else
                    {
                        if (Emp.IdEditarEmpresa == 7)
                        { Estatus = 7; }
                        else
                        {
                            empresa.EstatusCaptura = 3;
                            Estatus = 3;
                        }
                    }
                    _Control.SaveChanges();

                    exito = "Se guardo Correctamente los datos generales de la empresa : " + empresa.Nombre;

                }
                catch
                {
                    Emp.Error = "Problemas al querer guardar los datos generales de la empresa Revisrar datos";
                    return RedirectToAction("Index");
                }

            }
            else
            {
                Emp = CargarEmpresa(Emp);
                Emp.IdEditarEmpresa = Estatus;
                Emp.Error = error;
                Emp.Exito = exito;
                return View("Index", Emp);

            }
            Emp = CargarEmpresa(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        [HttpPost]
        public ActionResult AgregaPersonaFisica(EmpresasModelView Emp)
        {
            var exito = "";
            var error = "";
            var Estatus = Emp.IdEditarEmpresa;
            if ((Emp.IdEditarEmpresa == 2 || Emp.IdEditarEmpresa == 7) && Emp.NuevaEmpresa.Id != 0)
            {
                try
                {
                    var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);
                    empresa.NumActaNacimiento = Emp.NuevaEmpresa.NumActaNacimiento;
                    empresa.FechaActaNacimiento = Emp.NuevaEmpresa.FechaActaNacimiento;
                    empresa.LugarExpedicion = Emp.NuevaEmpresa.LugarExpedicion;
                    empresa.LibroActanacimiento = Emp.NuevaEmpresa.LibroActanacimiento;
                    if (Emp.IdEditarEmpresa == 6) { Estatus = 6; }
                    else { empresa.EstatusCaptura = 5; Estatus = 5; }

                    _Control.SaveChanges();
                    exito = "Se guardo Correctamente los datos de persona fiasica de la empresa : " + empresa.Nombre;

                }
                catch
                {
                    error = "Problemas al querer guardar los datos de persona fisica, favor de revisar";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                error = "Problemas al querer guardar los datos de persona fisica, favor de revisar";
                Emp = CargarEmpresa(Emp);
                Emp.IdEditarEmpresa = Estatus;
                Emp.Error = error;
                Emp.Exito = exito;
                return View("Index", Emp);

            }
            Emp = CargaDatos(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        [HttpPost]
        public ActionResult AgregaEscritura(EmpresasModelView Emp)
        {
            var exito = "";
            var error = "";

            var Estatus = Emp.IdEditarEmpresa;
            if ((Emp.IdEditarEmpresa == 3 || Emp.IdEditarEmpresa == 7) && Emp.NuevaEmpresa.Id != 0)
            {
                try
                {
                    var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);
                    empresa.ActaNumEscritura = Emp.NuevaEmpresa.ActaNumEscritura;
                    empresa.ActaVolumenEscritura = Emp.NuevaEmpresa.VolumenEscritura;
                    empresa.ActaFechaConstancia = Emp.NuevaEmpresa.ActaFechaConstancia;
                    empresa.EstatusCaptura = 4;
                    if (Emp.IdEditarEmpresa == 6) { Estatus = 6; }
                    else { empresa.EstatusCaptura = 4; Estatus = 4; }
                    _Control.SaveChanges();
                    exito = "Se guardo Correctamente los datos de persona física de la empresa : " + empresa.Nombre;


                }
                catch
                {
                    error = "No fue posible guardar Escritura";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                error = "No fue posible guardar Escritura";
                Emp = CargarEmpresa(Emp);
                Emp.IdEditarEmpresa = Estatus;
                Emp.Error = error;
                Emp.Exito = exito;
                return View("Index", Emp);

            }
            Emp = CargarEmpresa(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        [HttpPost]
        public ActionResult AgregaNotario(EmpresasModelView Emp)
        {
            var error = "";
            var exito = "";
            var Estatus = Emp.IdEditarEmpresa;
            if ((Emp.IdEditarEmpresa == 4 || Emp.IdEditarEmpresa == 7) && Emp.NuevaEmpresa.Id != 0)
            {
                try
                {
                    var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);

                    empresa.NumNotaria = Emp.NuevaEmpresa.NumNotaria;
                    empresa.CiudadNotaria = Emp.NuevaEmpresa.CiudadNotaria;
                    empresa.Modalidad = Emp.NuevaEmpresa.Modalidad;
                    empresa.NombreTitularNotaria = Emp.NuevaEmpresa.TitularNotaria;
                    empresa.NombreAdscrito = Emp.NuevaEmpresa.AdscritoNotaria;
                    empresa.DireccionNoraria = Emp.NuevaEmpresa.DireccionNotaria;
                    empresa.NumRPPC = Emp.NuevaEmpresa.NumRPPC;
                    empresa.VolRPPC = Emp.NuevaEmpresa.VolRPPC;
                    empresa.FechaRPPC = Emp.NuevaEmpresa.FechaRPPC;
                    empresa.CiudadNotaria = Emp.NuevaEmpresa.CiudadNotaria;
                    empresa.FolioMercantilElectronico = Emp.NuevaEmpresa.FolioMercantilElectronico;
                    empresa.Folios = Emp.NuevaEmpresa.Folios;
                    empresa.LibroNum = Emp.NuevaEmpresa.LibroNum;
                    if (Emp.IdEditarEmpresa == 7) { Estatus = 7; }
                    else { empresa.EstatusCaptura = 5; Estatus = 5; }
                    _Control.SaveChanges();
                    exito = "Se guardó correctamente el notario ";
                }
                catch
                {
                    error = "problemas al cargar los datos de Notario";
                    Emp.Error = error;
                    Emp.Exito = exito;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                error = "problemas al cargar los datos de Notario";
                Emp = CargarEmpresa(Emp);
                Emp.IdEditarEmpresa = Estatus;
                Emp.Error = error;
                Emp.Exito = exito;
                return View("Index", Emp);

            }
            Emp = CargarEmpresa(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        [HttpPost]
        public ActionResult AgregaFirmas(EmpresasModelView Emp)
        {
            var exito = "";
            var error = "";
            var Estatus = Emp.IdEditarEmpresa;
            if ((Emp.IdEditarEmpresa == 3 || Emp.IdEditarEmpresa == 5 || Emp.IdEditarEmpresa == 7) && Emp.NuevaEmpresa.Id != 0)

            {
                try
                {
                    var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);
                    empresa.Registro = Emp.NuevaEmpresa.IdRegistro;
                    empresa.Reviso = Emp.NuevaEmpresa.IdReviso;
                    empresa.VoBo = Emp.NuevaEmpresa.IdVoBo;
                    empresa.Autorizo = Emp.NuevaEmpresa.IdAutorizo;

                    if (_Control.TipoPersona.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdTipoPersona).TipoPersona1 == "MORAL")
                    {
                        empresa.EstatusCaptura = 6;
                    }
                    else
                    {
                        empresa.EstatusCaptura = 7;
                    }



                    _Control.SaveChanges();
                    Estatus = 7;
                    exito = "Se guardaron correctamente las Firmas";
                }
                catch
                {
                    error = "Problemas al guardar las firmas";
                    Emp.Error = error;
                    Emp.Exito = exito;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                error = "Problemas al guardar las firmasfavor de revisar ";
                Emp = CargarEmpresa(Emp);
                Emp.IdEditarEmpresa = Estatus;
                Emp.Error = error;
                Emp.Exito = exito;
                return View("Index", Emp);

            }
            Emp = CargaDatos(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        [HttpPost]
        public ActionResult AgregaRepresentante(EmpresasModelView Emp)
        {
            var exito = "";
            var error = "";

            var Estatus = 0;
            if (Emp.NuevaEmpresa.Id > 0)
            {
                try
                {
                    var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);

                    _Control.RepresentanteLegal.Add(new RepresentanteLegal
                    {
                        Nombre = Emp.NuevoRepresentante.Nombre.ToUpper(),
                        Puesto = Emp.NuevoRepresentante.Puesto.ToUpper(),
                        FolioElectoral = Emp.NuevoRepresentante.FolioElectoral.ToUpper(),
                        RFC = Emp.NuevoRepresentante.RFC.ToUpper(),
                        FolioMercantilElectronico = Emp.NuevoRepresentante.FolioMercantil.ToUpper(),
                        Direccion = Emp.NuevoRepresentante.Direccion,
                        Colonia = Emp.NuevoRepresentante.Colonia,
                        CP = Emp.NuevoRepresentante.CP,
                        Ciudad = Emp.NuevoRepresentante.Ciudad.ToUpper(),
                        Telefono = Emp.NuevoRepresentante.Telefono,
                        Correo = Emp.NuevoRepresentante.Correo,
                        NumEscritura = Emp.NuevoRepresentante.NumEscritura,
                        VolEscritura = Emp.NuevoRepresentante.VolEscritura,
                        FechaEscritura = Emp.NuevoRepresentante.FechaEscritura,
                        NumNotario = Emp.NuevoRepresentante.NumNotario,
                        NombreNotario = Emp.NuevoRepresentante.NombreNotario.ToUpper(),
                        LugarNotario = Emp.NuevoRepresentante.LugarNotario.ToUpper(),
                        NumRPPC = Emp.NuevoRepresentante.NumRPPC,
                        VolRPPC = Emp.NuevoRepresentante.VolRPPC,
                        FechaRPPC = Emp.NuevoRepresentante.FechaRPPC,
                        LugarRPPC = Emp.NuevoRepresentante.LugarRPPC,
                        FechaNac = Emp.NuevoRepresentante.FechaNacimiento,
                        LugarExpedicionActa = Emp.NuevoRepresentante.LugarExpedicionActa,
                        NombreAdscrito = Emp.NuevoRepresentante.NombreAdscrito.ToUpper(),
                        NombreSustituto = Emp.NuevoRepresentante.NombreSustituto.ToUpper(),
                        IdEmpresa = Emp.NuevaEmpresa.Id,
                    });
                    Estatus = 1;
                    empresa.EstatusCaptura = 7;
                    _Control.SaveChanges();
                    exito = "Se guardo Correctamente la empresa: " + Emp.NuevaEmpresa.Nombre.ToUpper();
                }

                catch
                {
                    error = "No de guardó revise sus datos";
                    return RedirectToAction("Index");
                }
                var EstaEmpresa = _Control.Contratistas.FirstOrDefault(x => x.RFC == Emp.NuevaEmpresa.Rfc);
                Emp.IdEditarEmpresa = EstaEmpresa.Id;
            }
            else
            {
                var Repre = _Control.RepresentanteLegal.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);
                if (Repre != null)
                {

                    Repre.Nombre = Emp.NuevoRepresentante.Nombre.ToUpper();
                    Repre.Puesto = Emp.NuevoRepresentante.Puesto.ToUpper();
                    Repre.FolioElectoral = Emp.NuevoRepresentante.FolioElectoral.ToUpper();
                    Repre.RFC = Emp.NuevoRepresentante.RFC.ToUpper();
                    Repre.FolioMercantilElectronico = Emp.NuevoRepresentante.FolioMercantil.ToUpper();
                    Repre.Direccion = Emp.NuevoRepresentante.Direccion;
                    Repre.Colonia = Emp.NuevoRepresentante.Colonia;
                    Repre.CP = Emp.NuevoRepresentante.CP;
                    Repre.Ciudad = Emp.NuevoRepresentante.Ciudad;
                    Repre.Telefono = Emp.NuevoRepresentante.Telefono;
                    Repre.Correo = Emp.NuevoRepresentante.Correo;
                    Repre.NumEscritura = Emp.NuevoRepresentante.NumEscritura;
                    Repre.VolEscritura = Emp.NuevoRepresentante.VolEscritura;
                    Repre.FechaEscritura = Emp.NuevoRepresentante.FechaEscritura;
                    Repre.NumNotario = Emp.NuevoRepresentante.NumNotario;
                    Repre.NombreNotario = Emp.NuevoRepresentante.NombreNotario;
                    Repre.LugarNotario = Emp.NuevoRepresentante.LugarNotario;
                    Repre.NumRPPC = Emp.NuevoRepresentante.NumRPPC;
                    Repre.VolRPPC = Emp.NuevoRepresentante.VolRPPC;
                    Repre.FechaRPPC = Emp.NuevoRepresentante.FechaRPPC;
                    Repre.LugarRPPC = Emp.NuevoRepresentante.LugarRPPC;
                    Repre.FechaNac = Emp.NuevoRepresentante.FechaNacimiento;
                    Repre.LugarExpedicionActa = Emp.NuevoRepresentante.LugarExpedicionActa;
                    Repre.NombreAdscrito = Emp.NuevoRepresentante.NombreAdscrito;
                    Repre.NombreSustituto = Emp.NuevoRepresentante.NombreSustituto;

                    if (Emp.IdEditarEmpresa == 7) { Estatus = 7; }
                    _Control.SaveChanges();

                    exito = "Se guardo Correctamentelos cambios del Representante legal: " + Emp.NuevaEmpresa.Nombre.ToUpper();
                }
            }
            Emp = CargarEmpresa(Emp);
            Emp.IdEditarEmpresa = Estatus;
            Emp.Error = error;
            Emp.Exito = exito;
            return View("Index", Emp);
        }
        public EmpresasModelView CargarEmpresa(EmpresasModelView Emp)
        {
            if (_Control.Contratistas.Any(x => x.Id == Emp.NuevaEmpresa.Id))
            {
                var empresa = _Control.Contratistas.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.Id);
                //CAtalogo 
                Emp = CargaDatos(Emp);
                Emp.NuevaEmpresa.Id = empresa.Id;
                Emp.NuevaEmpresa.Nombre = empresa.Nombre;
                Emp.NuevaEmpresa.Rfc = empresa.RFC;
                Emp.NuevaEmpresa.ClaveRegistro = empresa.ClaveRegistro;
                Emp.NuevaEmpresa.IniciaVigencia = empresa.FechaInicioVigencia;
                Emp.NuevaEmpresa.TerminaVigencia = empresa.FechaTerminoVigencia;
                Emp.NuevaEmpresa.IdTipoInscripcion = empresa.IdTipoInscripcion;
                Emp.NuevaEmpresa.IdTipoPersona = empresa.IdTipoPersona;
                Emp.NuevaEmpresa.IdTamañoEmp = empresa.IdTamaño;
                Emp.NuevaEmpresa.IdTipoServicio = empresa.IdTipoServicio;
                //Generales
                Emp.NuevaEmpresa.CapitalContable = (decimal)empresa.CapitalContable.GetValueOrDefault(0);
                Emp.NuevaEmpresa.Infonavit = empresa.Infonavit;
                Emp.NuevaEmpresa.Imss = empresa.IMSS;
                Emp.NuevaEmpresa.SIEM = empresa.SIEM;
                Emp.NuevaEmpresa.CMIC = empresa.CMIC;
                Emp.NuevaEmpresa.FolioMercantilElectronico = empresa.FolioMercantilElectronico;
                Emp.NuevaEmpresa.Domicilio = empresa.Domicilio;
                Emp.NuevaEmpresa.IdEstado = (int)empresa.IdEstado;
                Emp.NuevaEmpresa.IdMunicipio = (int)empresa.IdMunicipio;
                Emp.NuevaEmpresa.Colonia = empresa.Colonia;
                Emp.NuevaEmpresa.Fax = empresa.Fax;
                Emp.NuevaEmpresa.Telefono = empresa.Telefono;
                Emp.NuevaEmpresa.Correo = empresa.Correo;
                Emp.NuevaEmpresa.Curp = empresa.CURP;
                Emp.NuevaEmpresa.IdExperienciaAcreditada = empresa.IdExperiencia;
                //PErsona Física
                Emp.NuevaEmpresa.NumActaNacimiento = empresa.NumActaNacimiento;
                Emp.NuevaEmpresa.LugarExpedicion = empresa.LugarExpedicion;
                Emp.NuevaEmpresa.LibroActanacimiento = empresa.LibroActanacimiento;
                Emp.NuevaEmpresa.FechaActaNacimiento = empresa.FechaActaNacimiento.GetValueOrDefault(DateTime.Now);
                //Escritura constututiva 
                Emp.NuevaEmpresa.ActaNumEscritura = empresa.ActaNumEscritura;
                Emp.NuevaEmpresa.VolumenEscritura = empresa.ActaVolumenEscritura;
                Emp.NuevaEmpresa.ActaFechaConstancia = empresa.ActaFechaConstancia.GetValueOrDefault(DateTime.Now);
                //notario Publico 
                Emp.NuevaEmpresa.NumNotaria = empresa.NumNotaria;
                Emp.NuevaEmpresa.CiudadNotaria = empresa.CiudadNotaria;
                Emp.NuevaEmpresa.Modalidad = empresa.Modalidad;
                Emp.NuevaEmpresa.TitularNotaria = empresa.NombreTitularNotaria;
                Emp.NuevaEmpresa.AdscritoNotaria = empresa.NombreAdscrito;
                //Emp.NuevaEmpresa.SustitutoNotaria = empresa.Sustituto;
                Emp.NuevaEmpresa.DireccionNotaria = empresa.DireccionNoraria;
                Emp.NuevaEmpresa.NumRPPC = empresa.NumRPPC;
                Emp.NuevaEmpresa.VolRPPC = empresa.VolRPPC;
                Emp.NuevaEmpresa.FechaRPPC = empresa.FechaRPPC.GetValueOrDefault(DateTime.Now);
                Emp.NuevaEmpresa.FolioMercantilElectronico = empresa.FolioMercantilElectronico;
                Emp.NuevaEmpresa.Folios = empresa.Folios;
                Emp.NuevaEmpresa.LibroNum = empresa.LibroNum;
                Emp.NuevaEmpresa.CiudadRPPC = empresa.CiudadRppc;
                //Firmas
                Emp.NuevaEmpresa.IdRegistro = (int)empresa.Registro.GetValueOrDefault(0);
                Emp.NuevaEmpresa.IdVoBo = (int)empresa.VoBo.GetValueOrDefault(0);
                Emp.NuevaEmpresa.IdReviso = (int)empresa.Reviso.GetValueOrDefault(0);
                Emp.NuevaEmpresa.IdAutorizo = (int)empresa.Autorizo.GetValueOrDefault(0);
                Emp.IdEditarEmpresa = (byte)empresa.EstatusCaptura;
            }
            if (_Control.RepresentanteLegal.Any(x => x.IdEmpresa == Emp.NuevaEmpresa.Id))
            {
                var Repre = _Control.RepresentanteLegal.OrderByDescending(x => x.Id).FirstOrDefault(x => x.IdEmpresa == Emp.NuevaEmpresa.Id);
                Emp.NuevoRepresentante.Nombre = Repre.Nombre;
                Emp.NuevoRepresentante.Puesto = Repre.Puesto;
                Emp.NuevoRepresentante.FolioElectoral = Repre.FolioElectoral;
                Emp.NuevoRepresentante.RFC = Repre.RFC;
                Emp.NuevoRepresentante.FolioMercantil = Repre.FolioMercantilElectronico;
                Emp.NuevoRepresentante.Direccion = Repre.Direccion;
                Emp.NuevoRepresentante.Colonia = Repre.Colonia;
                Emp.NuevoRepresentante.CP = Repre.CP;
                Emp.NuevoRepresentante.Ciudad = Repre.Ciudad;
                Emp.NuevoRepresentante.Telefono = Repre.Telefono;
                Emp.NuevoRepresentante.Correo = Repre.Correo;
                Emp.NuevoRepresentante.NumEscritura = Repre.NumEscritura;
                Emp.NuevoRepresentante.VolEscritura = Repre.VolEscritura;
                Emp.NuevoRepresentante.FechaEscritura = (DateTime)Repre.FechaEscritura;
                Emp.NuevoRepresentante.NumNotario = Repre.NumNotario;
                Emp.NuevoRepresentante.NombreNotario = Repre.NombreNotario;
                Emp.NuevoRepresentante.LugarNotario = Repre.LugarNotario;
                Emp.NuevoRepresentante.NumRPPC = Repre.NumRPPC;
                Emp.NuevoRepresentante.VolRPPC = Repre.VolRPPC;
                Emp.NuevoRepresentante.FechaRPPC = (DateTime)Repre.FechaRPPC;
                Emp.NuevoRepresentante.LugarRPPC = Repre.LugarRPPC;
                Emp.NuevoRepresentante.FechaNacimiento = (DateTime)Repre.FechaNac;
                Emp.NuevoRepresentante.LugarExpedicionActa = Repre.LugarExpedicionActa;
                Emp.NuevoRepresentante.NombreAdscrito = Repre.NombreAdscrito;
                Emp.NuevoRepresentante.NombreSustituto = Repre.NombreSustituto;
            }
            if (Emp.NuevaEmpresa.IdEstado >= 0)
            {
                Emp.ListaMunicipios = new SelectList(_Control.Municipios.Where(x => x.idEntidad == Emp.NuevaEmpresa.IdEstado).Select(x => new { x.id, nombre = x.nombre.ToUpper() }), "id", "nombre");
            }
            return Emp;
        }
        [HttpPost]
        public ActionResult EditarEmpresa(EmpresasModelView Emp)
        {


            Emp = CargarEmpresa(Emp);

            return View("Index", Emp);
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
        [HttpPost]
        public ActionResult RegistroContratista(EmpresasModelView Emp)
        {
            Emp = CargarEmpresa(Emp);
            var FormFecha = "dd-MMMM-yyyy";
            var RegistroContratista = new ReportePDF();
            RegistroContratista.NewPageVertical();
            var fuente = new FontDef(RegistroContratista, FontDef.StandardFont.Helvetica);
            FontProp fp1 = new FontPropMM(fuente, 1.75);
            FontProp fp2 = new FontPropMM(fuente, 2.5);


            var logoitife = ToStream(Path.Combine(HttpContext.Server.MapPath("../img/"), "LOGOITIFE1.jpg"), ImageFormat.Jpeg);
            ListLayoutManager Datos1 = null;
            using (Datos1 = new ListLayoutManager(RegistroContratista))
            {
                PenProp lineaBorde = new PenPropMM(RegistroContratista, 0.001, Color.Transparent);
                Datos1.tlmColumnDef_Default.penProp_BorderH = lineaBorde;
                Datos1.tlmCellDef_Default.penProp_Line = lineaBorde;
                TlmColumn encabezado1 = new TlmColumnMM(Datos1, 45);
                encabezado1.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                //col_partidas.tlmCellDef_Default.rAlignV = RepObj.p;
                encabezado1.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn encabezado2 = new TlmColumnMM(Datos1, 100);
                encabezado2.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                //col_partidas.tlmCellDef_Default.rAlignV = RepObj.p;
                encabezado2.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn encabezado3 = new TlmColumnMM(Datos1, 45);
                encabezado3.tlmCellDef_Default.rAlignH = RepObj.rAlignRight;
                //col_partidas.tlmCellDef_Default.rAlignV = RepObj.p;
                encabezado3.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                Datos1.container_CreateMM(RegistroContratista.page_Cur, 10, 10);
                Datos1.NewRow();
                encabezado1.Add(new RepString(fp1, ""));
                encabezado1.NewLineMM(25);
                encabezado1.Add(new RepImageMM(logoitife, 50, 20));
                encabezado2.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                fp2.bBold = true;
                encabezado2.Add(new RepString(fp1, ""));
                encabezado2.NewLineMM(10);

                encabezado2.Add(new RepString(fp2, "GOBIERNO DEL ESTADO TABASCO "));
                encabezado2.NewLine();
                //encabezado2.Add(new RepString(fp2, "SECRETARIA DE ASENTAMIENTOS Y OBRAS PUBLICAS "));
                //Datos1.NewRow();
                encabezado2.Add(new RepString(fp2, "INSTITUTO TABASQUEÑO DE LA INFRAESTRUCTURA FÍSICA EDUCATIVA "));
                fp2.bBold = false;

                //col_partidas.Add(new RepString(fp1, "VILLAHERMOSA, TAB., A " +  Emp.ProgramaContratacion.ToLongDateString().ToUpper()));
                //col_partidas.NewLine();
                fp2.bBold = true;

                encabezado3.Add(new RepString(fp2, ""));
                encabezado3.NewLineMM(15);
                encabezado3.Add(new RepString(fp2, ""));
                encabezado3.Add(new RepString(fp2, Emp.NuevaEmpresa.ClaveRegistro));
                fp2.bBold = false;
                encabezado3.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                Datos1.NewRow();
                encabezado2.Add(new RepString(fp2, "REGISTRO DE CONTRATISTA"));

            }
            ListLayoutManager Datos2 = null;
            using (Datos2 = new ListLayoutManager(RegistroContratista))
            {
                BrushProp fondo1 = new BrushProp(RegistroContratista, Color.FromArgb(190, 255, 190));
                BrushProp fondo2 = new BrushProp(RegistroContratista, Color.FromArgb(255, 255, 255));

                PenProp lineaBorde = new PenPropMM(RegistroContratista, 0.001, Color.Transparent);
                Datos2.tlmColumnDef_Default.penProp_BorderH = lineaBorde;
                Datos2.tlmCellDef_Default.penProp_Line = lineaBorde;
                TlmColumn col1 = new TlmColumnMM(Datos2, 90);
                col1.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col1.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                TlmColumn col2 = new TlmColumnMM(Datos2, 90);
                col2.tlmCellDef_Default.rAlignH = RepObj.rAlignLeft;
                //col1.tlmCellDef_Default.rAlignV = RepObj.p;
                col2.tlmCellDef_Default.tlmTextMode = TlmTextMode.MultiLine;
                Datos2.container_CreateMM(RegistroContratista.page_Cur, 10, Datos1.rCurY_MM + 10);
                col1.tlmCellDef_Default.brushProp_Back = fondo1;
                col2.tlmCellDef_Default.brushProp_Back = fondo1;
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "REGISTRO"));
                col2.Add(new RepString(fp1, "DATO CAPTURADO"));
                fp1.bBold = false;
                col1.tlmCellDef_Default.brushProp_Back = fondo2;
                col2.tlmCellDef_Default.brushProp_Back = fondo2;
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "TIPO"));
                var Tipo = _Control.TipoServicio.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdTipoServicio).TipoServicio1;
                col2.Add(new RepString(fp1, Tipo));
                col1.NewLine();
                col2.NewLine();
                col1.Add(new RepString(fp1, "FECHA DE ELABORACIÓN"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.FechaRegistro.ToString(FormFecha).ToUpper()));
                col1.NewLine();
                col2.NewLine();
                col1.Add(new RepString(fp1, "CLAVE DE REGISTRO"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.ClaveRegistro));
                col1.NewLine();
                col2.NewLine();
                var inscripcion = _Control.TipoInscripcion.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdTipoInscripcion).TipoIncripcion;
                col1.Add(new RepString(fp1, "INSCRIPCIÓN"));
                col2.Add(new RepString(fp1, inscripcion));
                col1.NewLine();
                col2.NewLine();
                col1.Add(new RepString(fp1, "FECHA DE INICIO DE LA VIGENCIA"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.IniciaVigencia.ToString(FormFecha).ToUpper()));
                col1.NewLine();
                col2.NewLine();
                col1.Add(new RepString(fp1, "FECHA DE TERMINO DE LA VIGENCIA "));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.TerminaVigencia.ToString(FormFecha).ToUpper()));
                col1.NewLine();
                col2.NewLine();
                var clasificaion = _Control.Tamanio.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdTamañoEmp).Tamaño;
                col1.Add(new RepString(fp1, "CLASIFICACIÓN "));
                col2.Add(new RepString(fp1, clasificaion));
                col1.NewLine();
                col2.NewLine();
                col1.Add(new RepString(fp1, "CAPITAL CONTABLE "));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.CapitalContable.ToString("$ #,###.00")));
                col1.NewLine();
                col2.NewLine();
                var persona = _Control.TipoPersona.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdTipoPersona).TipoPersona1;
                col1.Add(new RepString(fp1, "PERSONA  "));
                col2.Add(new RepString(fp1, persona));
                col1.NewLine();
                col2.NewLine();
                col1.Add(new RepString(fp1, "NOMBRE"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Nombre));





                col1.tlmCellDef_Default.brushProp_Back = fondo1;
                col2.tlmCellDef_Default.brushProp_Back = fondo1;
                Datos2.NewRow();
                fp1.bBold = true;
                if (persona == "MORAL")
                {
                    col1.Add(new RepString(fp1, "CONSTITUCION DE LA PERSONA MORAL "));
                    col2.Add(new RepString(fp1, ""));
                }
                else
                {
                    col1.Add(new RepString(fp1, "CONSTITUCION DE LA PERSONA FISICA "));
                    col2.Add(new RepString(fp1, ""));
                }
                fp1.bBold = false;
                col1.tlmCellDef_Default.brushProp_Back = fondo2;
                col2.tlmCellDef_Default.brushProp_Back = fondo2;
                Datos2.NewRow();
                if (persona == "MORAL")
                {


                    col1.Add(new RepString(fp1, "ACTA NO."));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.ActaNumEscritura));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "VOLUMEN"));

                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.VolumenEscritura));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "FECHA CONSTITUCIÓN"));

                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.FechaRPPC.ToString(FormFecha).ToUpper()));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "NOMBRE DEL NOTARIO O FEDETARIO PUBLICO QUE LA PROTOCOLIZÓ"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.TitularNotaria));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "NÚMERO Y CIRCUNSCRIPCION DEL NOTARIO"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.NumNotaria));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "NÚMERO DE LA INSCRIPCIÓN EN EL REGISRTO PÚBLICO DE LA PROPIEDAD Y EL COMERCIO"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.NumRPPC));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "DOMICILIO FISCAL"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Domicilio));
                    Datos2.NewRow();
                    var municipio = _Control.Municipios.FirstOrDefault(x => x.id == Emp.NuevaEmpresa.IdMunicipio).nombre.ToUpper();
                    col1.Add(new RepString(fp1, "MUNICIPIO"));
                    col2.Add(new RepString(fp1, municipio));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "TELEFONO Y FAX"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Telefono + ", " + Emp.NuevaEmpresa.Fax));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "CORREO ELECTRÓNICO"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Correo));
                    Datos2.NewRow();
                }
                else
                {
                    col1.Add(new RepString(fp1, "DOMICILIO FISCAL"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Domicilio));
                    Datos2.NewRow();
                    var municipio = _Control.Municipios.FirstOrDefault(x => x.id == Emp.NuevaEmpresa.IdMunicipio).nombre.ToUpper();
                    col1.Add(new RepString(fp1, "MUNICIPIO "));
                    col2.Add(new RepString(fp1, municipio));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "TELEFONO Y FAX"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Telefono + ", " + Emp.NuevaEmpresa.Fax));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "CORREO ELECTRÓNICO"));
                    col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Correo));
                    Datos2.NewRow();

                }


                col1.tlmCellDef_Default.brushProp_Back = fondo1;
                col2.tlmCellDef_Default.brushProp_Back = fondo1;
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "REGISTRO FEDERAL DE CONTRIBUYENTES "));
                col2.Add(new RepString(fp1, ""));
                fp1.bBold = false;
                col1.tlmCellDef_Default.brushProp_Back = fondo2;
                col2.tlmCellDef_Default.brushProp_Back = fondo2;
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "RFC"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Rfc));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "IMSS"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Imss));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "CMIC"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.CMIC));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "COLEGIO"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.SIEM));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "SIEM"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.SIEM));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "INFONAVIT"));
                col2.Add(new RepString(fp1, Emp.NuevaEmpresa.Infonavit));
                Datos2.NewRow();

                col1.tlmCellDef_Default.brushProp_Back = fondo1;
                col2.tlmCellDef_Default.brushProp_Back = fondo1;
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "REPRESENTANTE LEGAL "));
                col2.Add(new RepString(fp1, ""));
                fp1.bBold = false;
                col1.tlmCellDef_Default.brushProp_Back = fondo2;
                col2.tlmCellDef_Default.brushProp_Back = fondo2;
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "NOOMBRE"));
                col2.Add(new RepString(fp1, Emp.NuevoRepresentante.Nombre));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "NUMEO ACTA"));
                col2.Add(new RepString(fp1, Emp.NuevoRepresentante.NumEscritura));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "VOLUMEN ACTA"));
                col2.Add(new RepString(fp1, Emp.NuevoRepresentante.VolEscritura));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "FECHA DE CONSTITUCION"));
                col2.Add(new RepString(fp1, Emp.NuevoRepresentante.FechaEscritura.ToString(FormFecha)));
                Datos2.NewRow();

                if (_Control.ExperienciaAcreditada.Any(x => x.Id == Emp.NuevaEmpresa.IdExperienciaAcreditada))
                {
                    col1.tlmCellDef_Default.brushProp_Back = fondo1;
                    col2.tlmCellDef_Default.brushProp_Back = fondo1;
                    Datos2.NewRow();
                    fp1.bBold = true;
                    col1.Add(new RepString(fp1, "EXPERIENCIA ACREDITADA"));
                    col2.Add(new RepString(fp1, ""));
                    fp1.bBold = false;
                    col1.tlmCellDef_Default.brushProp_Back = fondo2;
                    col2.tlmCellDef_Default.brushProp_Back = fondo2;
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "CLAVE"));
                    var Exp = _Control.ExperienciaAcreditada.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdExperienciaAcreditada);
                    col2.Add(new RepString(fp1, Exp.Clave));
                    Datos2.NewRow();
                    col1.Add(new RepString(fp1, "DESCRIPCIÓN"));
                    col2.Add(new RepString(fp1, Exp.Descripcion));
                    Datos2.NewRow();
                }



                col1.tlmCellDef_Default.brushProp_Back = fondo1;
                col2.tlmCellDef_Default.brushProp_Back = fondo1;
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "FIRMAS"));
                col2.Add(new RepString(fp1, ""));
                fp1.bBold = false;
                col1.tlmCellDef_Default.brushProp_Back = fondo2;
                col1.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                col2.tlmCellDef_Default.brushProp_Back = fondo2;
                col2.tlmCellDef_Default.rAlignH = RepObj.rAlignCenter;
                Datos2.NewRow();
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "REGISTRÓ "));
                col2.Add(new RepString(fp1, "REVISÓ"));
                fp1.bBold = false;
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "____________________________________ "));
                col2.Add(new RepString(fp1, "____________________________________ "));
                Datos2.NewRow();

                var Registro = _Control.Supervisores.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdRegistro);
                var Reviso = _Control.Supervisores.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdReviso);
                var vobo = _Control.Supervisores.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdVoBo);
                var Autorizo = _Control.Supervisores.FirstOrDefault(x => x.Id == Emp.NuevaEmpresa.IdAutorizo);

                col1.Add(new RepString(fp1, Registro.Nombre));
                col2.Add(new RepString(fp1, Reviso.Nombre));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, Registro.RolesSupervisores.RollSupervis));
                col2.Add(new RepString(fp1, Reviso.RolesSupervisores.RollSupervis));
                Datos2.NewRow();
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                fp1.bBold = true;
                col1.Add(new RepString(fp1, "Vo. Bo."));
                col2.Add(new RepString(fp1, "AUTORIZÓ"));
                fp1.bBold = false;
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "  "));
                col2.Add(new RepString(fp1, " "));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, "____________________________________ "));
                col2.Add(new RepString(fp1, "____________________________________ "));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, vobo.Nombre));
                col2.Add(new RepString(fp1, Autorizo.Nombre));
                Datos2.NewRow();
                col1.Add(new RepString(fp1, vobo.RolesSupervisores.RollSupervis));
                col2.Add(new RepString(fp1, Autorizo.RolesSupervisores.RollSupervis));
                Datos2.NewRow();




            }





            RT.ResponsePDF(RegistroContratista, System.Web.HttpContext.Current.Response);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Busqueda(EmpresasModelView Emp)
        {
            Emp = CargaDatos(Emp);
            return View("Index", Emp);
        }
    }
}