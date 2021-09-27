using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{

    public class ProgramasController : Controller
    {
        private ContratosSevrEntities _bd = new ContratosSevrEntities();

        public RecursosViewModel Carga(RecursosViewModel Rec )
        {
            Rec.NuevoRecurso = new Programa();
            Rec.ListaEjerciciosFiscales = new SelectList(_bd.Ejercicios, "Id", "Descripcion");
            Rec.ListaRecursos = new SelectList(_bd.Recursos, "Id", "Recurso");
            Rec.ListaNiveles = new SelectList(_bd.Nivel, "IdNivel", "Descripcion");
           
            var Listarec = _bd.ProgramasRecursos.Where(x=>x.Ejercicio>0);
            Listarec = Rec.FiltroEjercicio != 0 ? Listarec.Where(x => x.Ejercicio == Rec.FiltroEjercicio) : Listarec;
            if (Rec.FiltroNivel != 0)
            {
                Listarec = Listarec.Where(x => x.IdNivel == Rec.FiltroNivel);
            }    
            if (Rec.FiltroRecurso != 0)
            {
                Listarec = Listarec.Where(x => x.IdRecurso == Rec.FiltroRecurso);
            }
 
            foreach (var item in Listarec)
            {
                var oficios = new List<OficioRecurso>();
                var niv = _bd.Nivel.FirstOrDefault(x => x.IdNivel == item.IdNivel);
                var rec = _bd.Recursos.FirstOrDefault(x => x.Id == item.IdRecurso);
                var ejer = _bd.Ejercicios.FirstOrDefault(x => x.Id == item.Ejercicio).Descripcion; 
                if (_bd.OficiosRecursos.Any(x => x.IdRecurso == item.IdProgramasRecursos))
                {
                    var ofi = _bd.OficiosRecursos.Where(x => x.IdRecurso == item.IdProgramasRecursos);

                    foreach (var item2 in ofi)
                    {
                        oficios.Add(new OficioRecurso()
                        {
                            IdOficio = item2.IdOficio,
                            NumeroOficio = item2.Oficio,
                            FechaOficio = item2.Fecha,
                            ImporteAutorizado = (double) item2.ImporteAutorizado,
                            IdEjercicio = item.Ejercicio,
                            NombreRecurso = item.Descripcion,
                        });
                    }
                }

                Rec.Programas.Add(new Programa()
                {
                    Clave = item.IdProgramasRecursos.ToString(),
                    NombrePrograma = item.Descripcion.ToUpper(),
                    Oficios = oficios,
                    InversionMunicipal = (double) item.Inversion_Mun,
                    InversionEstatal = (double) item.Inversion_Est,
                    InversionFederal = (double) item.Inversion_Fed,
                    ClaveFF = item.ClaveFuenteFin.ToUpper(),
                    Procedencia = item.Procedencia.ToUpper(),
                    Estatus = item.Estatus.Value,
                    Nivel = niv.Descripcion.ToUpper(),
                    Ejercicio =ejer,
                    DescRecurso = rec.Recurso,
                });
            }

            return Rec;
        }


        // GET: Programas
        public ActionResult Index()
        {
            var Rec = new RecursosViewModel();
            Rec.FiltroEjercicio = _bd.Ejercicios.FirstOrDefault(x => x.Ejercicio == DateTime.Now.Year).Id;
            Rec = Carga(Rec);

            return View("index", Rec);
        }


        [HttpPost]
        public ActionResult Filtros(RecursosViewModel Rec)
        {
            Rec = Carga(Rec);
                return View("index",Rec);
        }

        // GET: Programas/Details/5

        [HttpPost]
        public ActionResult AgregaPrograma(RecursosViewModel Rec)
        {
            var exito = "";
            var error = "";
            if (string.IsNullOrEmpty(Rec.NuevoRecurso.Clave))
            {
                try
                {
                    ProgramasRecursos Nuevo = new ProgramasRecursos()
                    {

                        Ejercicio = Rec.NuevoRecurso.IdEjercicio,
                        Inversion_Mun = (decimal)Rec.NuevoRecurso.InversionMunicipal,
                        Inversion_Est = (float)Rec.NuevoRecurso.InversionEstatal,
                        Inversion_Fed = (float)Rec.NuevoRecurso.InversionFederal,
                        ClaveFuenteFin = Rec.NuevoRecurso.ClaveFF,
                        Descripcion = Rec.NuevoRecurso.DescRecurso,
                        Procedencia = Rec.NuevoRecurso.Procedencia == null ? "" : Rec.NuevoRecurso.Procedencia,
                        FechaRegistro = DateTime.Now,
                        IdRecurso = Rec.NuevoRecurso.IdRecurso,
                        IdNivel = (byte)Rec.NuevoRecurso.IdNivel,
                        IDUsuario = User.Identity.GetUserId(),
                        Estatus = true,


                    };
                    _bd.ProgramasRecursos.Add(Nuevo);
                    _bd.SaveChanges();
                    exito = "Se guardo correctamente en un nuevo Programa";
                }
                catch
                {
                    return RedirectToAction("Index");
                }
                
            }
            else
            {

                int clave = int.Parse(Rec.NuevoRecurso.Clave);

                if (_bd.ProgramasRecursos.Any(x => x.IdProgramasRecursos == clave))
                {
                    var prog = _bd.ProgramasRecursos.FirstOrDefault(x => x.IdProgramasRecursos == clave);
                    prog.Ejercicio = Rec.NuevoRecurso.IdEjercicio;
                    prog.Inversion_Mun = (decimal) Rec.NuevoRecurso.InversionMunicipal;
                    prog.Inversion_Est = (float) Rec.NuevoRecurso.InversionEstatal;
                    prog.Inversion_Fed = (float) Rec.NuevoRecurso.InversionFederal;
                    prog.ClaveFuenteFin = Rec.NuevoRecurso.ClaveFF;
                    prog.Descripcion = Rec.NuevoRecurso.DescRecurso;
                    prog.Procedencia = Rec.NuevoRecurso.Procedencia;
                    prog.FechaRegistro = DateTime.Now;
                    prog.IdRecurso = Rec.NuevoRecurso.IdRecurso;
                    prog.IdNivel = (byte) Rec.NuevoRecurso.IdNivel;
                    _bd.SaveChanges();
                    exito = "Se guardaron  correctamente los cambios en el programa";
                }
                else
                {
                    error = "No se encontro el programa ";
                }
            }
            Rec = Carga(Rec);
            Rec.Exito = exito;
            Rec.Error = error;
            return View("index", Rec);
        }


        [HttpPost]
        public ActionResult EliminaPrograma(RecursosViewModel rec)
        {
            //var Rec = new RecursosViewModel();
            var error = "";
            var Exito = "";
            if (_bd.ProgramasRecursos.Any(x => x.IdProgramasRecursos == rec.ProgEliminar))
            {
                var elimin = _bd.ProgramasRecursos.FirstOrDefault(x => x.IdProgramasRecursos == rec.ProgEliminar);

                _bd.ProgramasRecursos.Remove(elimin);
                _bd.SaveChanges();
                Exito = "Se eliminó correctamente el Programa";
            }
            else
            {
                error = "No se encontro el Programa ";
            }


            rec = Carga(rec);

            rec.Exito = Exito;
            rec.Error = error;

            return View("index", rec);
        }


        [HttpPost]
        public ActionResult EliminaOficio(RecursosViewModel rec)
        {
            //var Rec = new RecursosViewModel();
            var error = "";
            var Exito = "";
            if (_bd.OficiosRecursos.Any(x => x.IdOficio == rec.ElimarOfi))
            {
                var elimin = _bd.OficiosRecursos.FirstOrDefault(x => x.IdOficio == rec.ElimarOfi);

                _bd.OficiosRecursos.Remove(elimin);
                _bd.SaveChanges();
                Exito = "Se eliminó correctamente el Oficio";
            }
            else
            {
                error = "No se encontro el Oficio ";
            }


            rec = Carga(rec);

            rec.Exito = Exito;
            rec.Error = error;

            return View("index", rec);
        }


        [HttpPost]
        public ActionResult EditarPrograma(RecursosViewModel rec)
        {
            //var Rec = new RecursosViewModel();
            var error = "";
            var Exito = "";
            var editar = new ProgramasRecursos();
            var ed = rec.ProgEditar;
            rec = Carga(rec);
            if (_bd.ProgramasRecursos.Any(x => x.IdProgramasRecursos == ed))
            {
                 editar = _bd.ProgramasRecursos.FirstOrDefault(x => x.IdProgramasRecursos == ed);
                rec.NuevoRecurso = new Programa()
                {
                    Clave = ed.ToString(),
                    ClaveFF = editar.ClaveFuenteFin,
                    IdRecurso = editar.IdRecurso,
                    DescRecurso = editar.Descripcion,
                    IdEjercicio = editar.Ejercicio,
                    IdNivel = editar.IdNivel,
                    InversionMunicipal = (double)editar.Inversion_Mun,
                    InversionEstatal = (float)editar.Inversion_Est,
                    InversionFederal = (float)editar.Inversion_Fed,
                    Procedencia = editar.Procedencia,
                };

                rec.ModalEditar = "show";


                Exito = "Se cargo correcamente para su edición";
            }
            else
            {
                error = "No se encontro el Programa ";
            }


            

       


            rec.Exito = Exito;
            rec.Error = error;

            return View("index", rec);
        }


        [HttpPost]
        public ActionResult AgregarRecurso(RecursosViewModel Rec)
        {
            var exito = "";
            var error = "";
            if (!string.IsNullOrEmpty(Rec.AgregaRecurso))
            {
                if (_bd.Recursos.Any(x => x.Recurso == Rec.AgregaRecurso))
                {
                    error = "El recurso ya existe";

                }
                else
                {


                    var rNuevo = new Recursos() {Recurso = Rec.AgregaRecurso};
                    _bd.Recursos.Add(rNuevo);
                    _bd.SaveChanges();
                    exito = "Se guardo el recurso Correctamente";
                }
            }
            else
            {
                error = "Se requeire el agregar una descripcion de recurso";
                Rec.Error= error;
                RedirectToAction("Index");
            }
        Rec = Carga(Rec);
            Rec.Error = error;
            Rec.Exito = exito;
            return View("index", Rec);
        }

        [HttpPost]
        public ActionResult AgregarOficio(RecursosViewModel Rec)
        {
            var error = "";
            var exito = "";
            var Ususario = User.Identity.GetUserId();
            try
            {
                var NuevoOficio = new OficiosRecursos()
                {Oficio = Rec.NuevOficio.NumeroOficio,
                 Fecha=Rec.NuevOficio.FechaOficio,
                 Emisora = Rec.NuevOficio.Emisora,
                 ImporteAutorizado =  (decimal ) Rec.NuevOficio.ImporteAutorizado,
                 IdUsusario = Ususario,
                 FechaRegistro =  DateTime.Now,
                 IdRecurso = Rec.NuevOficio.IdPrograma,

                };
                _bd.OficiosRecursos.Add(NuevoOficio);
                _bd.SaveChanges();
                exito = "Se guardo El oficio " + Rec.NuevOficio.NumeroOficio +" con Exito";
            }
            catch
            { 


                Rec = Carga(Rec);
                Rec.Error = "No fue posible guardar el Oficio";
                Rec.Exito = exito;
                return RedirectToAction("Index");
            }


            Rec = Carga(Rec);
            Rec.Error = error;
            Rec.Exito = exito;

            return View("index", Rec);
        }


        [HttpPost]
        public ActionResult SubirOficio(IEnumerable<HttpPostedFileBase> archivos, string caract)
        {
            List<string> nomArchivos = new List<string>();
            var datos = caract.Split(',');
            int idConcepto = 0;
            int tipo = 0;
            // var Let = "";
            if (datos.Length >= 2 && int.TryParse(datos[0], out idConcepto) &&
                int.TryParse(datos[1], out tipo))
            {
                int tip;
                //Let = datos[2];
                //var concepto = _bdest.vwCat_Conceptos.First(x => x.IdConcepto == idConcepto);
                int.TryParse(datos[2], out tip);
                //if (_bdest.ImagenesGen.Any(x => x.IdConcepto == idConcepto && x.Tipo == tip))
                //{
                //    var idimagen = _bdest.ImagenesGen.First(x => x.IdConcepto == idConcepto && x.Tipo == tip).IdimagenCat;
                //    if (datos[2] != "5")
                //    {
                //        var resp = Eliminaimg(idimagen.ToString());
                //    }
                //}

                //var ubicacion = "";
                //var notas = "ANTES";


                //switch (datos[2])
                //{
                //    case "1":
                //        notas = "ANTES";
                //        break;
                //    case "2":
                //        notas = "DURANTE";
                //        break;
                //    case "3":
                //        notas = "DESPUES";
                //        break;
                //    case "4":
                //        notas = "OTRA";
                //        break;
                //    case "5":
                //        notas = "CROQUIS";
                //        break;
                //}
                foreach (var file in archivos)
                {
                    var resp = "";
                    //var img = new Imagenes();
                    //var imggen = new ImagenesGen();
                    //ubicacion = (tipo==1? "../Evidencias/" : "../Croquis/")+"/"+concepto.ClaveProyecto.Replace(".","").Replace("/","")+"/" + concepto.et_Clave.Replace(".", "").Replace("/", "") + "/" + concepto.pa_Desc.Replace(".", "").Replace("/", "") + "/";
                    // ubicacion = (tipo == 1 ? "../Evidencias/" : "../Croquis/") + "/" + concepto.ClaveProyecto.Replace(".", "").Replace("/", "").Replace(" ", "") + "/" + concepto.et_Clave.Replace(".", "").Replace("/", "").Replace(" ", "") + "/" + concepto.pa_Desc.Replace(".", "").Replace("/", "").Replace(" ", "") + "/";
                    //        if (file != null)
                    //        {
                    //            if (!string.IsNullOrEmpty(file.ToString()) && (file.FileName.ToUpper().Contains(".JPG") || file.FileName.ToUpper().Contains(".JPEG")) && file != null)
                    //            {
                    //                var direc = Path.Combine(HttpContext.Server.MapPath(ubicacion), "");
                    //                if (!System.IO.Directory.Exists(direc))
                    //                {
                    //                    System.IO.Directory.CreateDirectory(direc);
                    //                }
                    //                string arch = "";
                    //                string archmin = "";
                    //                do
                    //                {
                    //                    img.Archivo = Guid.NewGuid() + ".jpg";
                    //                    arch = Path.Combine(HttpContext.Server.MapPath(ubicacion), img.Archivo);
                    //                } while (System.IO.File.Exists(arch));

                    //                do
                    //                {
                    //                    img.ArchivoMiniatura = Guid.NewGuid() + ".jpg";
                    //                    archmin = Path.Combine(HttpContext.Server.MapPath(ubicacion), img.ArchivoMiniatura);
                    //                } while (System.IO.File.Exists(archmin));

                    //                img.Ubicacion = ubicacion;
                    //                img.TipoImagen = 2;
                    //                _bdest.Imagenes.Add(img);
                    //                _bdest.SaveChanges();
                    //                var idimg = _bdest.Imagenes.First(x => x.Archivo == img.Archivo && x.Ubicacion == img.Ubicacion).IdImagen;
                    //                resp = ubicacion + img.ArchivoMiniatura + "," + idimg;
                    //                imggen.IdConcepto = concepto.IdConcepto;
                    //                imggen.IdimagenCat = idimg;
                    //                imggen.IdUsuario = User.Identity.GetUserId();
                    //                imggen.Notas = notas;
                    //                imggen.Tipo = tip;
                    //                _bdest.ImagenesGen.Add(imggen);
                    //                _bdest.SaveChanges();
                    //                MemoryStream imagen = new MemoryStream();
                    //                file.InputStream.CopyTo(imagen);
                    //                var nuevaimagen = RedimensionarImagen(imagen, (datos[2] != "5"));
                    //                nuevaimagen.Save(arch, ImageFormat.Jpeg);
                    //                MemoryStream imagenmin = new MemoryStream();
                    //                //file.InputStream.CopyToAsync(nuevaimagen);
                    //                var nuevaimagenmin = RedimensionarImagenmin(imagen);
                    //                nuevaimagenmin.Save(archmin, ImageFormat.Jpeg);
                    //                nomArchivos.Add(resp);
                    //            }
                    //        }
                }
            }
                //else
                //{

                //    return Json("Fallo");
                //}
                //if (datos[2] == "5")
                //{
                //    nomArchivos = new List<string>();
                //    var arch = _bdest.ImagenesGen.Where(x => x.IdConcepto == idConcepto && x.Tipo == 5)
                //         .Join(_bdest.Imagenes, x => x.IdimagenCat, y => y.IdImagen, (x, y) => new { Archivo = y.Ubicacion + y.Archivo + "," + x.IdimagenCat.ToString() });
                //    foreach (var item in arch)
                //    {
                //        nomArchivos.Add(item.Archivo);
                //    }

                //}
                return Json(nomArchivos);
        }



     
    }
}
