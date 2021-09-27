using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class SupervisoresController : Controller


    {
        private ContratosSevrEntities _bd = new ContratosSevrEntities();
        // GET: Supervisores

        public SupervisoresViewModel CargaSup(SupervisoresViewModel sup)
        {

            sup.Exito = "";
            sup.Error = "";
            sup.ListasRoles = new SelectList(_bd.RolesSupervisores, "Id", "RollSupervis");
            sup.ListaSupervisores = new List<PersonalSup>();
            sup.NuevoSupervisor= new PersonalSup();
           
           var  supervisores = sup.FiltroIdRoll >= 0 ? _bd.Supervisores.Where(x =>x.IdRollSpervisores == sup.FiltroIdRoll) : _bd.Supervisores.Where(x => x.IdRollSpervisores == 1);

            foreach (var supervisor in supervisores )
            {
                var Roll = supervisor.RolesSupervisores.RollSupervis;

                 sup.ListaSupervisores.Add(new PersonalSup()
                 {
                     Id= supervisor.Id,
                     Nombre = supervisor.Nombre,
                     Direccion = supervisor.Direccion,
                     Telefono =  supervisor.Telefono,
                     Cedula = supervisor.Cedula,
                     Roll = Roll,
                     Estado = supervisor.Estatus,
                     

                 }); 
                
            }


            
            return sup;
        }


       


        public ActionResult Index()
        {
            var sup = new SupervisoresViewModel();


            sup = CargaSup(sup);


            return View(sup);
        }

        [HttpPost]
        public ActionResult AgregaSupervisor(SupervisoresViewModel sup)
        {
            var exito = "";
            var error = "";

            if (!string.IsNullOrEmpty(sup.NuevoSupervisor.Nombre) && !string.IsNullOrEmpty(sup.NuevoSupervisor.Cedula) &&
                sup.NuevoSupervisor.IdRoll != 0)
            {
                if (String.IsNullOrEmpty(sup.NuevoSupervisor.Direccion))
                {
                    sup.NuevoSupervisor.Direccion = "Sin Direccion";

                }
                if (String.IsNullOrEmpty(sup.NuevoSupervisor.Telefono))
                {
                    sup.NuevoSupervisor.Telefono = "Sin Direccion";

                }

                try
                {
                    var nuevopersonal = new Supervisores()
                    {
                        Nombre = sup.NuevoSupervisor.Nombre.ToUpper(),
                        Cedula = sup.NuevoSupervisor.Cedula.ToUpper(),
                        IdRollSpervisores = sup.NuevoSupervisor.IdRoll,
                        Direccion = sup.NuevoSupervisor.Direccion,
                        Telefono = sup.NuevoSupervisor.Telefono,
                        Estatus = "ACTIVO"

                    };
                    _bd.Supervisores.Add(nuevopersonal);
                    _bd.SaveChanges();
                    exito = "Se guardó el nombre correctamente ";

                }
                catch
                {
                    error = "No fue posible guardar el dato";
                }


            
                

            }
            sup.FiltroIdRoll = sup.NuevoSupervisor.IdRoll;
            sup = CargaSup(sup);
            sup.Error = error;
            sup.Exito = exito;
            return View("Index", sup);

        }


        [HttpPost]
        public ActionResult Busqueda (SupervisoresViewModel sup)
        {
            

            sup = CargaSup(sup);

            return View("Index", sup);

        }

        [HttpPost]
        public ActionResult EliminaSupervisor(SupervisoresViewModel sup)
        {
            var exito = "";
            var error = "";
            if (sup.EliminaSup != 0)
            {
                var ElimSupervis = _bd.Supervisores.FirstOrDefault(x => x.Id == sup.EliminaSup);

                _bd.Supervisores.Remove(ElimSupervis);
                _bd.SaveChanges();

                exito = "Dato Eliminado correctamente";
            }
            else
            {
                error = "No es posible eliminar el dato";
            }
            
            sup = CargaSup(sup);
            sup.Error = error;
            sup.Exito = exito;
            return View("Index", sup);
        }

        [HttpPost]
        public ActionResult CambiaEstatusSupervisor(SupervisoresViewModel sup)
        {
            var exito = "";
            var error = "";
            if (sup.EliminaSup != 0)
            {
                var ElimSupervis = _bd.Supervisores.FirstOrDefault(x => x.Id == sup.EliminaSup);

                if (ElimSupervis.Estatus == "ACTIVO")
                {
                    ElimSupervis.Estatus = "IN ACTIVO";

                }
                else
                {
                    ElimSupervis.Estatus = "ACTIVO";
                }
                _bd.SaveChanges();
                exito = "Dato cambiado correctamente";
            }
            else
            {
                error = "No es posible Cambiar el dato";
            }

            sup = CargaSup(sup);
            sup.Error = error;
            sup.Exito = exito;
            return View("Index", sup);
        }
    }
}