using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class EmpleadosController : Controller
    {
     
            // GET: Empleados
            private ContratosSevrEntities _bd = new ContratosSevrEntities();

            public EmpleadosModelView CargaPer(EmpleadosModelView Per)
            {

                var Idpersona = Per.NuevoEmpleado != null ? Per.NuevoEmpleado.Id : 0;

                Per.Exito = "";
                Per.Error = "";
                Per.ListaDirecciones = new SelectList(_bd.Direcciones, "Id", "NombreDireccion");
                Per.ListaDepartamentos = new SelectList(_bd.Departamentos, "Id", "NombreDepto");
                Per.ListaJefes = new SelectList(_bd.Personal.Where(x => x.Estatus == true).Select(x => new { x.Id, Nombre = x.Trato + " " + x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno }), "Id", "Nombre");
                Per.ListaEmpleados = new List<Empleado>();

                var Empleados = _bd.Personal.OrderBy(x => x.ApellidoPaterno).ThenBy(x => x.ApellidoMaterno).ThenBy(x => x.Nombre).ToList();

                // _bd.Personal.Where(x => x.Estatus == true).OrderBy(x => x.ApellidoPaterno).ThenBy(x => x.ApellidoMaterno).ThenBy(x => x.Nombre).ToList();

                if (!String.IsNullOrEmpty(Per.FiltroNombre))
                {
                    Per.FiltroNombre = Per.FiltroNombre.ToUpper();
                    Empleados = Empleados.Where(x => x.Nombre.Contains(Per.FiltroNombre) || x.ApellidoPaterno.Contains(Per.FiltroNombre) || x.ApellidoMaterno.Contains(Per.FiltroNombre)).ToList();
                }

                if (Per.FiltroDepartamento > 0)
                {
                    Empleados = Empleados.Where(x => x.IdDepartamento == Per.FiltroDepartamento).ToList();
                }
                if (Per.FiltroDireccion > 0)
                {
                    Empleados = Empleados.Where(x => x.IdDireccion == Per.FiltroDireccion).ToList();
                }

                foreach (var item in Empleados)

                {
                    var dire = _bd.Direcciones.FirstOrDefault(x => x.Id == item.IdDireccion).NombreDireccion;
                    var depto = _bd.Direcciones.FirstOrDefault(x => x.Id == item.IdDireccion).NombreDireccion;

                    Per.ListaEmpleados.Add(new Empleado()
                    {
                        Id = item.Id,
                        Trato = item.Trato,
                        Nombre = item.Nombre,
                        ApellidoP = item.ApellidoPaterno,
                        ApellidoM = item.ApellidoMaterno,
                        Cargo = item.Cargo,
                        Direccion = dire,
                        IdDireccion = item.IdDireccion,
                        Depto = depto,
                        IdDepto = item.IdDepartamento,
                        Telefono = item.Telefono,
                        Correo = item.Correo,
                        GeneroTexto = item.Sexo == false ? "MASCULINO" : "FEMENINO",
                        Domicilio = item.Domicilio,
                        Estatus = item.Estatus == true ? "ACTIVO " : "SUSPENDIDO",
                        Foto = item.Foto,
                        FechaNacimiento = (DateTime)item.FechaNacimiento,
                        IdJefe = item.IdJefeInmediato.GetValueOrDefault(0),

                    });

                }
                if (_bd.Personal.Any(x => x.Id == Idpersona))
                {
                    var persona = _bd.Personal.FirstOrDefault(x => x.Id == Idpersona);

                    Per.NuevoEmpleado.Id = persona.Id;
                    Per.NuevoEmpleado.Trato = persona.Trato;
                    Per.NuevoEmpleado.Nombre = persona.Nombre;
                    Per.NuevoEmpleado.ApellidoP = persona.ApellidoPaterno;
                    Per.NuevoEmpleado.ApellidoM = persona.ApellidoMaterno;
                    Per.NuevoEmpleado.Cargo = persona.Cargo;
                    Per.NuevoEmpleado.Domicilio = persona.Domicilio;
                    Per.NuevoEmpleado.RFC = persona.RFC;
                    Per.NuevoEmpleado.Genero = persona.Sexo;
                    Per.NuevoEmpleado.IdDireccion = persona.IdDireccion;
                    Per.NuevoEmpleado.IdDepto = persona.IdDepartamento;
                    Per.NuevoEmpleado.Telefono = persona.Telefono;
                    Per.NuevoEmpleado.Correo = persona.Correo;
                    Per.NuevoEmpleado.IdJefe = (int)persona.IdJefeInmediato;
                    Per.NuevoEmpleado.FechaNacimiento = (DateTime)persona.FechaNacimiento;

                }
                return Per;
            }
            [HttpPost]
            public ActionResult AgregaEmpleado(EmpleadosModelView Per)
            {
                var error = "";
                var exito = "";

                try
                {
                    if (!(Per.NuevoEmpleado.Id > 0))
                    {
                        _bd.Personal.Add(new Personal
                        {
                            Trato = Per.NuevoEmpleado.Trato.ToUpper(),
                            Nombre = Per.NuevoEmpleado.Nombre.ToUpper(),
                            ApellidoPaterno = Per.NuevoEmpleado.ApellidoP.ToUpper(),
                            ApellidoMaterno = Per.NuevoEmpleado.ApellidoM.ToUpper(),
                            Cargo = Per.NuevoEmpleado.Cargo.ToUpper(),
                            Domicilio = Per.NuevoEmpleado.Domicilio.ToUpper(),
                            RFC = Per.NuevoEmpleado.RFC.ToUpper(),
                            Sexo = Per.NuevoEmpleado.Genero,
                            Telefono = Per.NuevoEmpleado.Telefono,
                            Correo = Per.NuevoEmpleado.Correo,
                            IdDireccion = Per.NuevoEmpleado.IdDireccion,
                            IdDepartamento = Per.NuevoEmpleado.IdDepto,
                            FechaNacimiento = (DateTime)Per.NuevoEmpleado.FechaNacimiento,
                            FechaRegistro = DateTime.Now,
                            Estatus = true,
                            IdJefeInmediato = Per.NuevoEmpleado.IdJefe,
                        });
                        _bd.SaveChanges();
                        exito = "Se guardó Exitosamente el personal ";
                    }
                    else
                    {
                        if (_bd.Personal.Any(x => x.Id == Per.NuevoEmpleado.Id))
                        {
                            var persona = _bd.Personal.FirstOrDefault(x => x.Id == Per.NuevoEmpleado.Id);

                            persona.Trato = Per.NuevoEmpleado.Trato.ToUpper();
                            persona.Nombre = Per.NuevoEmpleado.Nombre.ToUpper();
                            persona.ApellidoPaterno = Per.NuevoEmpleado.ApellidoP.ToUpper();
                            persona.ApellidoMaterno = Per.NuevoEmpleado.ApellidoM.ToUpper();
                            persona.Cargo = Per.NuevoEmpleado.Cargo.ToUpper();
                            persona.Domicilio = Per.NuevoEmpleado.Domicilio.ToUpper();
                            persona.RFC = Per.NuevoEmpleado.RFC.ToUpper();
                            persona.Sexo = Per.NuevoEmpleado.Genero;
                            persona.Telefono = Per.NuevoEmpleado.Telefono;
                            persona.Correo = Per.NuevoEmpleado.Correo;
                            persona.IdDireccion = Per.NuevoEmpleado.IdDireccion;
                            persona.IdDepartamento = Per.NuevoEmpleado.IdDepto;
                            persona.FechaNacimiento = (DateTime)Per.NuevoEmpleado.FechaNacimiento;
                            persona.Estatus = true;
                            persona.IdJefeInmediato = Per.NuevoEmpleado.IdJefe;

                            _bd.SaveChanges();
                        }

                    }



                }
                catch
                {
                    Per.Error = "no fue posible guardar el dato favor de verificar";
                    return RedirectToAction("Index");
                }


                Per = CargaPer(Per);
                Per.Exito = exito;
                Per.Error = error;
                return View("Index", Per);
            }
            [HttpPost]
            public ActionResult EditarPersonal(EmpleadosModelView Per)
            {

                Per = CargaPer(Per);

                return View("Index", Per);
            }
            [HttpPost]
            public ActionResult Busqueda(EmpleadosModelView Per)
            {
                Per = CargaPer(Per);
                return View("Index", Per);
            }

            [HttpPost]
            public ActionResult Deshabilitar(EmpleadosModelView Per)
            {
                var exito = "";

                if (Per.NuevoEmpleado.Id > 0)
                {
                    try
                    {
                        var empleado = _bd.Personal.FirstOrDefault(x => x.Id == Per.NuevoEmpleado.Id);
                        empleado.Estatus = !empleado.Estatus;
                        _bd.SaveChanges();
                        exito = "El empleado fue Deshabilitado";
                    }
                    catch
                    {
                        Per.Error = "No se pudo dehabilitar a esta persona";
                        return RedirectToAction("Index");
                    }
                }

                Per = CargaPer(Per);
                Per.Exito = exito;
                return View("Index", Per);
            }

            [HttpPost]
            public ActionResult AgregaDepartamento(EmpleadosModelView Per)
            {
                var error = "";
                var exito = "";
                try
                {
                    if (!string.IsNullOrEmpty(Per.NombDepto) && !string.IsNullOrEmpty(Per.NombDepto) && !_bd.Departamentos.Any(x => x.NombreDepto == Per.NombDepto))
                    {
                        _bd.Departamentos.Add(new Departamentos
                        {
                            NombreDepto = Per.NombDepto,
                            Abreviatura = Per.AbreviaDepto
                        });
                        _bd.SaveChanges();
                        exito = "Se guardó exitosamente la en departamento " + Per.NombDepto;
                    }
                    else
                    {
                        error = "No fue posible agregar el departameto por que ya existe o faltan datos";
                    }
                }
                catch
                {
                    Per.Error = "no fue posible guardar el dato favor de verificar";
                    return RedirectToAction("Index");
                }
                Per = CargaPer(Per);
                Per.Exito = exito;
                Per.Error = error;
                return View("Index", Per);
            }
            public ActionResult Index()
            {
                var Per = new EmpleadosModelView();
                Per = CargaPer(Per);
                return View(Per);
            }
        }
    }