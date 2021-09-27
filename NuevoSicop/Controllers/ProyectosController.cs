using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class ProyectosController : Controller
    {
        // GET: Proyectos

        private ContratosSevrEntities _bd = new ContratosSevrEntities();
        public ProyectosModelView CargaProyectos(ProyectosModelView Pro)
        {
            Pro.Exito = "";
            Pro.Error = "";
            var ProyectoActivo = Pro.NuevoProyecto != null ? Pro.NuevoProyecto.Id : 0;
            Pro.ListaEjercicios = new SelectList(_bd.Ejercicios, "Id", "Ejercicio");
            Pro.ListaProgramas = new SelectList(_bd.ProgramasRecursos, "IdProgramasRecursos", "Descripcion");
            var Proyectos = _bd.Proyectos.ToList();

            if (!String.IsNullOrEmpty(Pro.FiltroClaveProyecto))
            {
                Proyectos = Proyectos.Where(x => x.ClaveProyecto.Contains(Pro.FiltroClaveProyecto)).ToList();
            }

            if (Pro.IdEjercicio > 0)
            {
                var ProgRecursos = _bd.ProgramasRecursos.Where(x => x.Ejercicio == Pro.IdEjercicio).Select(x => x.IdProgramasRecursos);
                var Proyecs = new List<Proyectos>();
                foreach (var item in Proyectos)
                {
                    if (ProgRecursos.Any(x => x == item.IdProgramadeRecursos))
                    {
                        Proyecs.Add(item);
                    }
                }

                Proyectos = Proyecs;
            }


            foreach (var item in Proyectos)
            {
                var escu = _bd.Escuelas.FirstOrDefault(x => x.Id == item.IdEscuela);
                var prog = _bd.ProgramasRecursos.FirstOrDefault(x => x.IdProgramasRecursos == item.IdProgramadeRecursos);
                Pro.ListaProyectos.Add(new ProyectoInd
                {
                    Id = item.Id,
                    Clave = item.ClaveProyecto,
                    Descripcion = item.Descripcion,
                    ImporteAutorizado = item.ImporteEutorizado,
                    Anticipo = item.Anticipo,
                    Escuela = item.IdEscuela,
                    ClaveEscuela = escu.Clave,
                    NombreEscuela = escu.Nombre,
                    FechaInicio = (DateTime)item.FechaInicio,
                    FechaTermino = (DateTime)item.FechaFinal,
                    ProgramaRecursos = item.IdProgramadeRecursos,
                    ProgramaRecursosT = prog.Descripcion,

                });
            }

            if (_bd.Proyectos.Any(x => x.Id == ProyectoActivo))
            {
                var proyect = _bd.Proyectos.FirstOrDefault(x => x.Id == ProyectoActivo);
                var escu = _bd.Escuelas.FirstOrDefault(x => x.Id == proyect.IdEscuela);
                var prog = _bd.ProgramasRecursos.FirstOrDefault(x => x.IdProgramasRecursos == proyect.IdProgramadeRecursos);


                Pro.NuevoProyecto.Id = proyect.Id;
                Pro.NuevoProyecto.Clave = proyect.ClaveProyecto;
                Pro.NuevoProyecto.Descripcion = proyect.Descripcion;
                Pro.NuevoProyecto.ImporteAutorizado = proyect.ImporteEutorizado;
                Pro.NuevoProyecto.Anticipo = proyect.Anticipo;
                Pro.NuevoProyecto.FechaInicio = (DateTime)proyect.FechaInicio;
                Pro.NuevoProyecto.FechaTermino = (DateTime)proyect.FechaFinal;
                Pro.NuevoProyecto.FechaAviso = (DateTime)proyect.FechaAviso;
                Pro.NuevoProyecto.Escuela = proyect.IdEscuela;
                Pro.NuevoProyecto.ProgramaRecursos = proyect.IdProgramadeRecursos;
                Pro.IdEjercicio = prog.Ejercicio;
                Pro.ListaEscuelas = new SelectList(_bd.Escuelas.Where(x => x.Id == proyect.IdEscuela).Select(x => new { Id = x.Id, nombre = x.Clave.ToUpper() + "-" + x.Nombre.ToUpper() + "-" + x.Domicilio }).OrderBy(x => x.nombre), "Id", "nombre");


            }




            return Pro;
        }


        public JsonResult CargaRecursos(int ent)
        {
            var resp = new Listas();
            var res = _bd.ProgramasRecursos.Where(x => x.Ejercicio == ent);
            resp.Genericos = new SelectList(res.Select(x => new { Id = x.IdProgramasRecursos, nombre = x.Descripcion.ToUpper() }).OrderBy(x => x.nombre), "Id", "nombre");
            return Json(resp);
        }

        public JsonResult BuscaEscu(string ent)
        {
            var resp = new Listas();
            var res = _bd.Escuelas.Where(x => x.Nombre.Contains(ent) || x.Clave.Contains(ent) || x.Domicilio.Contains(ent)).Take(20);
            resp.Genericos = new SelectList(res.Select(x => new { Id = x.Id, nombre = x.Clave.ToUpper() + "-" + x.Nombre.ToUpper() + "-" + x.Domicilio }).OrderBy(x => x.nombre), "Id", "nombre");
            return Json(resp);
        }

        // GET: Proyectos
        public ActionResult Index()
        {
            var Pro = new ProyectosModelView();
            Pro = CargaProyectos(Pro);
            return View(Pro);
        }


        [HttpPost]
        public ActionResult AgregarProyecto(ProyectosModelView Pro)
        {
            var error = "";
            var exito = "";

            try
            {
                if (!(Pro.NuevoProyecto.Id > 0))
                {
                    _bd.Proyectos.Add(new Proyectos
                    {
                        ClaveProyecto = Pro.NuevoProyecto.Clave.ToUpper(),
                        Descripcion = Pro.NuevoProyecto.Descripcion.ToUpper(),
                        ImporteEutorizado = Pro.NuevoProyecto.ImporteAutorizado,
                        Anticipo = Pro.NuevoProyecto.Anticipo,
                        FechaInicio = Pro.NuevoProyecto.FechaInicio,
                        FechaFinal = Pro.NuevoProyecto.FechaTermino,
                        FechaAviso = Pro.NuevoProyecto.FechaAviso,
                        IdProgramadeRecursos = Pro.NuevoProyecto.ProgramaRecursos,
                        IdEscuela = Pro.NuevoProyecto.Escuela
                    });
                    _bd.SaveChanges();
                    exito = "Se guardó exitosamente el Proyecto ";
                }
                else
                {
                    if (_bd.Proyectos.Any(x => x.Id == Pro.NuevoProyecto.Id))
                    {
                        var Proy = _bd.Proyectos.FirstOrDefault(x => x.Id == Pro.NuevoProyecto.Id);

                        Proy.ClaveProyecto = Pro.NuevoProyecto.Clave.ToUpper();
                        Proy.Descripcion = Pro.NuevoProyecto.Descripcion.ToUpper();
                        Proy.ImporteEutorizado = Pro.NuevoProyecto.ImporteAutorizado;
                        Proy.Anticipo = Pro.NuevoProyecto.Anticipo;
                        Proy.FechaInicio = Pro.NuevoProyecto.FechaInicio;
                        Proy.FechaFinal = Pro.NuevoProyecto.FechaTermino;
                        Proy.FechaAviso = Pro.NuevoProyecto.FechaAviso;
                        Proy.IdProgramadeRecursos = Pro.NuevoProyecto.ProgramaRecursos;
                        Proy.IdEscuela = Pro.NuevoProyecto.Escuela;

                        _bd.SaveChanges();
                        exito = " Se guardaron los cambios del proyecto correctamente ";
                    }

                }



            }
            catch
            {
                Pro.Error = "No fue posible guardar el dato favor de verificar";
                return RedirectToAction("Index");
            }


            Pro = CargaProyectos(Pro);
            Pro.Exito = exito;
            Pro.Error = error;
            return View("Index", Pro);
        }


        [HttpPost]
        public ActionResult EditarProyecto(ProyectosModelView Pro)
        {

            Pro = CargaProyectos(Pro);
            return View("Index", Pro);
        }

        [HttpPost]
        public ActionResult Busqueda(ProyectosModelView Pro)
        {

            Pro = CargaProyectos(Pro);
            return View("Index", Pro);
        }


    }
}