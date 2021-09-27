using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AngularCRUD.Models;
using NuevoSicop.Models.BD;


namespace AngularCRUD.Controllers
{
    public class InventariosController : Controller
    {
        // GET: Inventarios
        private ContratosSevrEntities _bd = new ContratosSevrEntities();
        public InventarioModelView CargaInventario(InventarioModelView Inv)
        {
            //Limpiar mensajes de Exito/Error
            Inv.Error = "";
            Inv.Exito = "";
            //carga de listas  
            Inv.ListaDireciones = new SelectList(_bd.Direcciones, "Id", "NombreDireccion");
            Inv.ListaEnagenacion = new SelectList(_bd.TiposEnagenacion, "Id", "TipoEnegenacion");
            Inv.ListaEstados = new SelectList(_bd.EstadoBien, "Id", "Estado");
            Inv.ListaPersonal = new SelectList(_bd.Personal.Select(x => new { Id = x.Id, Nombre = x.Trato + " " + x.Nombre + "" + x.ApellidoPaterno + " " + x.ApellidoMaterno }).OrderByDescending(x => x.Id), "Id", "Nombre");
            Inv.ListaUbicaciones = new SelectList(_bd.Ubicaciones, "Id", "Ubicacion");
            //Carga datos
            var fechfinal = Inv.FiltroFFinal.AddDays(1);
            var Inventario = _bd.Inventario.Where(x => x.FechaRegistro >= Inv.FiltroFInicio && x.FechaRegistro < fechfinal).ToList();
            //AplicaFiltros
            if (Inv.FiltroDireccion >= 1)
            {
                Inventario = Inventario.Where(x => x.IdDireccion == Inv.FiltroDireccion).ToList();
            }
            if (Inv.FiltroEnajenacion >= 1)
            {
                Inventario = Inventario.Where(x => x.IdTipoEnagenacion == Inv.FiltroEnajenacion).ToList();
            }
            if (!string.IsNullOrEmpty(Inv.FiltroMarca))
            {
                Inventario = Inventario.Where(x => x.Marca.Contains(Inv.FiltroMarca)).ToList();
            }
            if (Inv.FiltroPeronal >= 1)
            {
                Inventario = Inventario.Where(x => x.IdPersonal == Inv.FiltroPeronal).ToList();
            }
            //Pasa datos filtrados a modelview
            foreach (var item in Inventario)
            {
                var estado = item.EstadoBien.Estado;
                var ubicacion = item.Ubicaciones.Ubicacion;
                var direccion = item.Direcciones.NombreDireccion;
                var pers = item.Personal.Trato + " " + item.Personal.Nombre + " " + item.Personal.ApellidoPaterno + " " + item.Personal.ApellidoMaterno;
                var enagenacion = item.TiposEnagenacion.TipoEnegenacion;
                Inv.ListaBienes.Add(new Bien
                {
                    Id = item.Id,
                    Clave = item.Clave,
                    Nombre = item.NombreBien.ToUpper(),
                    Marca = item.Marca.ToUpper(),
                    Tipo = item.Tipo.ToUpper(),
                    Serie = item.Serie,
                    Modelo = item.Modelo,
                    Observaciones = item.Observacion.ToUpper(),
                    FechaFactura = (DateTime)item.FechaFactura,
                    FechaRegistro = (DateTime)item.FechaRegistro,
                    Valorfactura = (decimal)item.ValorFacturado,
                    ValorActual = (decimal)item.ValorActual,
                    Caracteristicas = item.Caracteristicas.ToUpper(),
                    IdEstado = item.IdEstadoBien,
                    IdUbicacion = item.IdUbicacion,
                    IdDireccion = item.IdDireccion,
                    IdPresonal = item.IdPersonal,
                    IdTipoEnagenacion = item.IdTipoEnagenacion,
                    Estado = estado,
                    Ubicacion = ubicacion,
                    Direccion = direccion,
                    Asignado = pers,
                    TipoEnagenacion = enagenacion,
                });
            }
            return Inv;
        }

        public ActionResult Index()
        {
            var Inv = new InventarioModelView();
            Inv = CargaInventario(Inv);
            return View(Inv);
        }

        [HttpPost]

        public ActionResult GuardarBien(InventarioModelView Inv)
        {
            var error = "";
            var exito = "";
            if (!string.IsNullOrEmpty(Inv.NuevoBien.Nombre) && !string.IsNullOrEmpty(Inv.NuevoBien.Clave) && Inv.NuevoBien.IdEstado > 0 && Inv.NuevoBien.IdUbicacion > 0 && Inv.NuevoBien.IdDireccion > 0 && Inv.NuevoBien.IdPresonal > 0 && Inv.NuevoBien.IdTipoEnagenacion > 0)
            {
                try
                {
                    if (_bd.Inventario.Any(x => x.Id == Inv.NuevoBien.Id))
                    {
                        var BienActual = _bd.Inventario.FirstOrDefault(x => x.Id == Inv.NuevoBien.Id);
                        BienActual.Clave = Inv.NuevoBien.Clave.ToUpper();
                        BienActual.NombreBien = Inv.NuevoBien.Nombre.ToUpper();
                        BienActual.Marca = Inv.NuevoBien.Marca.ToUpper();
                        BienActual.Tipo = Inv.NuevoBien.Tipo.ToUpper();
                        BienActual.Serie = Inv.NuevoBien.Serie.ToUpper();
                        BienActual.Modelo = Inv.NuevoBien.Modelo.ToUpper();
                        BienActual.IdEstadoBien = Inv.NuevoBien.IdEstado;
                        BienActual.IdUbicacion = Inv.NuevoBien.IdUbicacion;
                        BienActual.IdDireccion = Inv.NuevoBien.IdDireccion;
                        BienActual.Observacion = Inv.NuevoBien.Observaciones.ToUpper();
                        BienActual.FechaFactura = Inv.NuevoBien.FechaFactura;
                        BienActual.ValorFacturado = Inv.NuevoBien.Valorfactura;
                        BienActual.ValorActual = Inv.NuevoBien.ValorActual;
                        BienActual.IdPersonal = Inv.NuevoBien.IdPresonal;
                        BienActual.IdTipoEnagenacion = Inv.NuevoBien.IdTipoEnagenacion;
                        BienActual.Caracteristicas = Inv.NuevoBien.Caracteristicas.ToUpper();
                        BienActual.FechaRegistro = DateTime.Now;

                        _bd.SaveChanges();
                        exito = "Se actualizó el bien correctamente";
                    }
                    else
                    {
                        _bd.Inventario.Add(new Inventario()
                        {
                            Clave = Inv.NuevoBien.Clave.ToUpper(),
                            NombreBien = Inv.NuevoBien.Nombre.ToUpper(),
                            Marca = Inv.NuevoBien.Marca.ToUpper(),
                            Tipo = Inv.NuevoBien.Tipo.ToUpper(),
                            Serie = Inv.NuevoBien.Serie.ToUpper(),
                            Modelo = Inv.NuevoBien.Modelo.ToUpper(),
                            IdEstadoBien = Inv.NuevoBien.IdEstado,
                            IdUbicacion = Inv.NuevoBien.IdUbicacion,
                            IdDireccion = Inv.NuevoBien.IdDireccion,
                            Observacion = Inv.NuevoBien.Observaciones.ToUpper(),
                            FechaFactura = Inv.NuevoBien.FechaFactura,
                            ValorFacturado = Inv.NuevoBien.Valorfactura,
                            ValorActual = Inv.NuevoBien.ValorActual,
                            IdPersonal = Inv.NuevoBien.IdPresonal,
                            IdTipoEnagenacion = Inv.NuevoBien.IdTipoEnagenacion,
                            Caracteristicas = Inv.NuevoBien.Caracteristicas.ToUpper(),
                            FechaRegistro = DateTime.Now
                        });
                        _bd.SaveChanges();
                        exito = "Se guardó el bien correctamente";
                    }
                }
                catch
                {
                    error = "No fue posible guardar el Bien";
                }
            }
            else
            {
                error = "Porfavor revise los datos son nincorrectos o estan incompletos";
                RedirectToAction("Index");
            }
            Inv = CargaInventario(Inv);
            Inv.NuevoBien = new Bien();
            Inv.Error = error;
            Inv.Exito = exito;
            return View("Index", Inv);
        }


        public JsonResult BuscaNombre(string ent)
        {
            var resp = new Listas();
            var res = string.IsNullOrEmpty(ent) ? _bd.Personal.Take(30).ToList() : _bd.Personal.Where(x => x.Nombre.Contains(ent) || x.ApellidoPaterno.Contains(ent) || x.ApellidoMaterno.Contains(ent)).Take(30).ToList();
            resp.Genericos = new SelectList(res.Select(x => new { Id = x.Id, nombre = x.Trato.ToUpper() + " " + x.Nombre.ToUpper() + " " + x.ApellidoPaterno.ToUpper() + " " + x.ApellidoMaterno }).OrderBy(x => x.nombre), "Id", "nombre");
            return Json(resp);
        }



        public ActionResult FiltraBienes(InventarioModelView Inv)
        {
            Inv = CargaInventario(Inv);
            return View("Index", Inv);
        }
        public ActionResult AsignaPersonal(InventarioModelView Inv)
        {
            var error = "";
            var exito = "";

            if (Inv.NuevoBien.IdPresonal > 0 && Inv.NuevoBien.Id > 0)
            {
                if (_bd.Inventario.Any(x => x.Id == Inv.NuevoBien.Id))
                {
                    var BienActual = _bd.Inventario.FirstOrDefault(x => x.Id == Inv.NuevoBien.Id);
                    BienActual.IdPersonal = Inv.NuevoBien.IdPresonal;
                    _bd.SaveChanges();
                    exito = "Se guardó correctamente el cambio de personal";
                }
                else
                {
                    error = "El bien no se encontró en la base de datos";
                }
            }
            else
            {
                error = "No fue posible cambiar la asignación";
            }
            Inv = CargaInventario(Inv);
            Inv.Error = error;
            Inv.Exito = exito;
            return View("Index", Inv);
        }


        public ActionResult EditarBien(InventarioModelView Inv)
        {
            var exito = "";
            var error = "";

            if (_bd.Inventario.Any(x => x.Id == Inv.EditarBien))
            {
                var BienAct = _bd.Inventario.FirstOrDefault(x => x.Id == Inv.EditarBien);
                Inv.NuevoBien.Id = BienAct.Id;
                Inv.NuevoBien.Clave = BienAct.Clave;
                Inv.NuevoBien.Nombre = BienAct.NombreBien;
                Inv.NuevoBien.Marca = BienAct.Marca;
                Inv.NuevoBien.Tipo = BienAct.Tipo;
                Inv.NuevoBien.Serie = BienAct.Serie;
                Inv.NuevoBien.Modelo = BienAct.Modelo;
                Inv.NuevoBien.IdEstado = BienAct.IdEstadoBien;
                Inv.NuevoBien.IdUbicacion = BienAct.IdUbicacion;
                Inv.NuevoBien.IdDireccion = BienAct.IdDireccion;
                Inv.NuevoBien.Observaciones = BienAct.Observacion;
                Inv.NuevoBien.FechaFactura = (DateTime)BienAct.FechaFactura;
                Inv.NuevoBien.Valorfactura = (decimal)BienAct.ValorFacturado;
                Inv.NuevoBien.ValorActual = (decimal)BienAct.ValorActual;
                Inv.NuevoBien.IdPresonal = BienAct.IdPersonal;
                Inv.NuevoBien.IdTipoEnagenacion = BienAct.IdTipoEnagenacion;
                Inv.NuevoBien.Caracteristicas = BienAct.Caracteristicas;
                exito = "Se cargo el bien correctamente";
            }
            else
            {
                error = "NO se encontró el bien";
            }

            Inv = CargaInventario(Inv);
            Inv.Error = error;
            Inv.Exito = exito;
            return View("Index", Inv);
        }

        public ActionResult LimpiaForm()
        {
            var Inv = new InventarioModelView();

            Inv = CargaInventario(Inv);
            return View("Index", Inv);
        }


    }
}