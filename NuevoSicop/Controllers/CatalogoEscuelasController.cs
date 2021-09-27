using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class CatalogoEscuelasController : Controller
    {
        private ContratosSevrEntities _bd = new ContratosSevrEntities();
        // GET: CatalogoEscuelas
        public CatalogoEscuelasViewModels Carga(CatalogoEscuelasViewModels Escu)
        {



            Escu.NuevaEscuela = new Escuela();
            Escu.ListaEntidades = new SelectList(_bd.Entidades.Where(x=>x.idPais == 1).Select(x => new { x.id, nombre = x.nombre.ToUpper() }), "id", "nombre");
            Escu.ListaMunicipios = new SelectList(_bd.Municipios.Where(x => x.idEntidad == 27).Select(y => new { y.id,nombre=y.nombre.ToUpper()}), "id", "nombre");
            Escu.ListaModalidad = new SelectList(_bd.ModalidadEsc, "Id", "Modalidad");
            Escu.ListaNivelesEsc = new SelectList(_bd.NivelEsc, "Id", "Nivel");
            Escu.ListaDescrsost = new SelectList(_bd.DescSostEsc, "Id", "Descripcion");
            var datos = new List<SelectListItem>();
            for (var i = 1; i <= 100; i++)
            {
                datos.Add(new SelectListItem(){Value = i.ToString("##"),
                    Text = i.ToString("##")
                });
            }
            Escu.ListaZonas = new SelectList(datos,"value","text" );
            datos = new List<SelectListItem>();
            for (var i = 1; i <= 1000; i++)
            {
                datos.Add(new SelectListItem()
                {
                    Value = i.ToString("##"),
                    Text = i.ToString("##")
                });
            }
            Escu.ListaSectores = new SelectList(datos, "value", "text");

            var ListaEsc = !String.IsNullOrEmpty(Escu.FiltroNombre)
                ? _bd.Escuelas.Where(x => x.Nombre.Contains(Escu.FiltroNombre))
                : _bd.Escuelas;


            ListaEsc = Escu.FiltroNivel != 0 ? ListaEsc.Where(x => x.IdNivel == Escu.FiltroNivel) : ListaEsc;



            if (Escu.FiltroModalidad != 0)
            {
                ListaEsc = ListaEsc.Where(x => x.IdModalidad == Escu.FiltroModalidad);
            }

            if (Escu.FiltroMunicipio != 0)
            {
                ListaEsc = ListaEsc.Where(x => x.Localidades.IdMunicipio  == Escu.FiltroMunicipio);
            }

            foreach (var item in ListaEsc.Take(10))
            {
                var oficios = new List<OficioRecurso>();
                var niv = _bd.NivelEsc.FirstOrDefault(x => x.Id == item.IdNivel).Nivel;
                var modalidad = _bd.ModalidadEsc.FirstOrDefault(x => x.Id == item.IdModalidad).Modalidad;
                var loc = _bd.Localidades.FirstOrDefault(x => x.id == item.Localidad);
                var municipio = _bd.Municipios.FirstOrDefault(x => x.id == loc.IdMunicipio).nombre;
                var Descsost = _bd.DescSostEsc.FirstOrDefault(x => x.Id == item.DescSost).Descripcion;
                Escu.ListaEscuelas.Add(new Escuela()
                {
                    Id= item.Id,
                    Clave   = item.Clave,
                    Nombre = item.Nombre.ToUpper(),
                    Modalidad =  modalidad,
                    IdModalidad =  item.IdModalidad,
                    Nivel = niv,
                    Sector = item.Sector,
                    ZonaEscolar = item.Zona,
                    IdNivel = item.IdNivel,
                    Direccion =  item.Domicilio,
                    Localidad = loc.nombre.ToUpper(),
                    IdLocalidad = item.Localidad,
                    Municipio = municipio.ToUpper(),
                    IdMunicipio = loc.IdMunicipio.Value,
                    Descrsost= Descsost.ToUpper(),
                    IdDescrsost = item.DescSost,
                    ClaveLocalidad= item.ClaveLocalidad,
                    
                });
            }

            return Escu;
        }
        public ActionResult Index()
        {
            var Escu = new CatalogoEscuelasViewModels();
            
            Escu = Carga(Escu);

            return View("index", Escu);
        }
        [HttpPost]
        public ActionResult Filtros(CatalogoEscuelasViewModels Escu)
        {
            Escu = Carga(Escu);
            return View("index", Escu);
        }
        [HttpPost]
        public ActionResult AgregaEscuela(CatalogoEscuelasViewModels escu)
        {
            var exito = "";
            var error = "";
            if (escu.NuevaEscuela.Id>0)
            {
                try
                {
                    Escuelas Nuevo = new Escuelas()
                    {

                        Id = escu.NuevaEscuela.Id,
                        IdNivel = escu.NuevaEscuela.IdNivel,
                        IdModalidad = escu.NuevaEscuela.IdModalidad,
                        Clave= escu.NuevaEscuela.Clave,
                        Nombre = escu.NuevaEscuela.Nombre,
                        Sector = escu.NuevaEscuela.Sector,
                        Zona = escu.NuevaEscuela.ZonaEscolar,
                        Domicilio= escu.NuevaEscuela.Direccion,
                        Localidad = escu.NuevaEscuela.IdLocalidad,
                        DescSost = escu.NuevaEscuela.IdDescrsost,
                        ClaveLocalidad = escu.NuevaEscuela.ClaveLocalidad,


                    };
                    _bd.Escuelas.Add(Nuevo);
                    _bd.SaveChanges();
                    exito = "Se guardo correctamente en una Nueva Escuela";
                }
                catch
                {
                    return RedirectToAction("Index");
                }

            }
            else
            {

                

                if (_bd.Escuelas.Any(x => x.Id== escu.NuevaEscuela.Id))
                {
                    var Esc = _bd.Escuelas.FirstOrDefault(x => x.Id == escu.NuevaEscuela.Id);
                    Esc.IdNivel = escu.NuevaEscuela.IdNivel;
                    Esc.IdModalidad = escu.NuevaEscuela.IdModalidad;
                    Esc.Clave = escu.NuevaEscuela.Clave;
                    Esc.Nombre = escu.NuevaEscuela.Nombre;
                    Esc.Sector = escu.NuevaEscuela.Sector;
                    Esc.Zona = escu.NuevaEscuela.ZonaEscolar;
                    Esc.Domicilio = escu.NuevaEscuela.Direccion;
                    Esc.Localidad = escu.NuevaEscuela.IdLocalidad;
                    Esc.DescSost = escu.NuevaEscuela.IdDescrsost;
                    Esc.ClaveLocalidad = escu.NuevaEscuela.ClaveLocalidad;
                    _bd.SaveChanges();
                    exito = "Se guardaron  correctamente los cambios en la escuela";
                }
                else
                {
                    error = "No se encontro el la Escuela ";
                }
            }
            escu = Carga(escu);
            escu.Exito = exito;
            escu.Error = error;
            return View("index",escu);
        }
        [HttpPost]
        public ActionResult EditarEscuela(CatalogoEscuelasViewModels escu)
        {
            var error = "";
            var Exito = "";
            var editar = new Escuelas();
            var ed = escu.EditarEscuela;
            escu = Carga(escu);
            if (_bd.Escuelas.Any(x => x.Id == ed))
            {
                editar = _bd.Escuelas.FirstOrDefault(x => x.Id == ed);
                var muni = (int ) ( editar.Localidades.IdMunicipio==null ?0: editar.Localidades.IdMunicipio);

                escu.NuevaEscuela = new Escuela()
                {
                    Id = ed,
                    Clave = editar.Clave,
                    IdNivel = editar.IdNivel,
                    IdModalidad = editar.IdModalidad,
                    Nombre = editar.Nombre,
                    Sector = editar.Sector,
                    ZonaEscolar = editar.Zona,
                    Direccion = editar.Domicilio,
                    IdLocalidad = editar.Localidad,
                    IdMunicipio = muni,
                    IdDescrsost = editar.DescSost,
                    ClaveLocalidad= editar.ClaveLocalidad,
                };

                escu.ModalEditar = "show";


                Exito = "Se cargó la escuela correctamente para su edición";
            }
            else
            {
                error = "No se encontro La escuela ";
            }







            escu.Exito = Exito;
            escu.Error = error;

            return View("index", escu);
        }
        public JsonResult CargarMunicipios(int ent)
        {
            var resp = new Listas();
            var res = _bd.Municipios.Where(x => x.idEntidad == ent);
            resp.Municipios = new SelectList(res.Select(x => new { x.id, nombre = x.nombre.ToUpper() }).OrderBy(x => x.nombre), "id", "nombre");
          return Json(resp);
        }
        public JsonResult CargarLocalidades(int ent)
        {
            var resp = new Listas();
            var res2 = _bd.Localidades.Where(x => x.IdMunicipio == ent);
           
            resp.Localidades =new SelectList(res2.Select(x => new { x.id, nombre = x.nombre.ToUpper() }).OrderBy(x => x.nombre), "id","nombre");

            return Json(resp);
        }
        [HttpPost]
        public ActionResult AgregarModalidad(CatalogoEscuelasViewModels escu)
        {

            var exito = "";
            var error = "";
            if (!string.IsNullOrEmpty(escu.NuevaModalidad))
            {
                if (!_bd.ModalidadEsc.Any(x => x.Modalidad == escu.NuevaModalidad))
                {
                    _bd.ModalidadEsc.Add(new ModalidadEsc()
                    {
                        Modalidad = escu.NuevaModalidad.ToUpper()
                    });
                    _bd.SaveChanges();

                    exito = "Se guardo la modalidad correctamente";
                }
                else
                {
                    error = "La modalidad ya existe en el catálogo";
                }

            }
            else
            {
                error = "No se puede guardar una modalidad vacia";
            }


            escu = Carga(escu);
            escu.Exito = exito;
            escu.Error = error;
            return View("index", escu);

        }
        [HttpPost]
        public ActionResult AgregarNivel(CatalogoEscuelasViewModels escu)
        {
            var exito = "";
            var error = "";
            if (!string.IsNullOrEmpty(escu.NuevoNivel))
            {
                if (!_bd.NivelEsc.Any(x => x.Nivel == escu.NuevoNivel))
                {
                    _bd.NivelEsc.Add(new NivelEsc()
                    {
                        Nivel = escu.NuevoNivel.ToUpper()
                    });
                    _bd.SaveChanges();

                    exito = "Se guardo la Nivel correctamente";
                }
                else
                {
                    error = "El Nivel ya existe en el catálogo  ";
                }

            }
            else
            {
                error = "No se puede guardar una Nivel vacia";
            }



            escu = Carga(escu);
            escu.Exito = exito;
            escu.Error = error;
            return View("index", escu);

        }
        [HttpPost]
        public ActionResult AgregarDescSost(CatalogoEscuelasViewModels escu)
        {
            var exito = "";
            var error = "";
            if (!string.IsNullOrEmpty(escu.NuevoDescSost))
            {
                if (!_bd.DescSostEsc.Any(x => x.Descripcion == escu.NuevoDescSost))
                {
                    _bd.DescSostEsc.Add(new DescSostEsc()
                    {
                        Descripcion = escu.NuevoDescSost.ToUpper()
                    });
                    _bd.SaveChanges();

                    exito = "Se guardo la DescSost correctamente";

                }

                else
                {
                    error = "La DescSost ya existe en el catálogo";
                }
              
            }
            else
            {
                error = "No se puede guardar una DescSost vacia";
            }



            escu = Carga(escu);
            escu.Exito = exito;
            escu.Error = error;
            return View("index", escu);

        }

    }



}