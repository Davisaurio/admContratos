using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using Antlr.Runtime;
using NuevoSicop.Models;
using NuevoSicop.Models.BD;

namespace NuevoSicop.Controllers
{
    public class InvitacionesController : Controller
    {
        // GET: Invitaciones

        private sicop_tempEntities _sicop = new sicop_tempEntities();

        public Cap_InvitacionModelView CargaDatos(Cap_InvitacionModelView inv)
        {
            var eje = inv.IdEjercicio;
            var paq = inv.IdPaquete;

            var ejercicios =
                _sicop.PaquetesProcedimiento.Select(x => new {anio = x.Anio}).Distinct().OrderBy(x => x.anio);

            inv.ListaEjercicios = new SelectList(ejercicios, "anio", "anio");
            if (inv.IdEjercicio > 0)
            {
                var paquetes =
                    _sicop.PaquetesProcedimiento.Where(x => x.Anio == inv.IdEjercicio)
                        .Select(x => new {NumPaquete = x.idPaquete, Paquete = x.Descripcion})
                        .Distinct()
                        .OrderBy(x => x.Paquete);
                inv.ListaPaquetes = new SelectList(paquetes, "NumPaquete", "paquete");
            }


            if (inv.IdPaquete > 0)
            {
                var paquete = _sicop.PaquetesProcedimiento.FirstOrDefault(x => x.idPaquete == inv.IdPaquete);
                var Programa = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == inv.IdPaquete);

                var Modalidad = Programa != null?_sicop.Modalidad.FirstOrDefault(x => x.Clave_Modalidad == Programa.Modalidad).NomModalidad:"";
                inv.Anio = paquete.Anio.GetValueOrDefault(0);
                var Proys = _sicop.DetallePaqueteProyecto.Where(x => x.IdPaquete == inv.IdPaquete);


                foreach (var Pro in Proys)
                {
                    var py = _sicop.ESCU.FirstOrDefault(x => x.id == Pro.IdProyecto);
                    inv.ListaProyectos.Add(new ProyInv()
                    {
                        DescripcionObra = py.NOMESC,
                        PlazoEjecucion = Pro.PlazoEjecucion.GetValueOrDefault(0),
                        Anticipo = Pro.Anticipo.GetValueOrDefault(0),
                        ImporteAut = Pro.ImporteAutorizado.GetValueOrDefault(0),
                        CapitalContable = Pro.CapitalContable.GetValueOrDefault(0),
                    });
                }
                var Copias =
                    _sicop.Detalle_CcpDesignacion.Where(x => x.contrato == inv.IdPaquete.ToString())
                        .OrderBy(x => x.Numero);
                foreach (var copia in Copias)
                {
                    inv.ListaCcp.Add(new CopiaPara()
                    {
                        Id = copia.Id,
                        Orden = copia.Numero.GetValueOrDefault(0),
                        Nombre = copia.nombre_ccp,
                        Puesto = copia.pueto_ccp
                    });
                }
                
                inv.VerInvitacion = _sicop.VwProgramaCont.Any(x => x.ejercicio == inv.IdEjercicio && x.idPaquete == inv.IdPaquete);

                inv.Paquete = paquete.Paquete.GetValueOrDefault(0);
                inv.NoPaquete = paquete.Descripcion;
                inv.Procedimiento = paquete.NoProcedimiento;
                inv.Recuperacion = paquete.Recuperacion.GetValueOrDefault(0);
                inv.TipoInversion = Programa.TipoInversion;
                inv.Modalidad = Modalidad;
                inv.FundamentoLegal = Programa.Articulo;
                inv.OficioPrograma = Programa.Fechaoficio.GetValueOrDefault(DateTime.MinValue);
                inv.Convocatoria = paquete.fchConvocatoria.GetValueOrDefault(DateTime.MinValue);
                inv.LimiteVentaBases = paquete.fchLimiteInscripcion.GetValueOrDefault(DateTime.MinValue);
                inv.Visita = paquete.fchVisitaSitio.GetValueOrDefault(DateTime.MinValue);
                inv.JuntaAclaraciones = paquete.fchJuntaAclaracion.GetValueOrDefault(DateTime.MinValue);
                inv.Apertura = paquete.fchAperturaTecnicaEconomica.GetValueOrDefault(DateTime.MinValue);
                inv.Fallo = paquete.fchFallo.GetValueOrDefault(DateTime.MinValue);
                inv.InicioEstimada = paquete.FechaInicioEstimada.GetValueOrDefault(DateTime.MinValue);
                inv.FirmaContrato = paquete.fchContrato.GetValueOrDefault(DateTime.MinValue);
                inv.IdExperienciaAcreditada = Programa.idExperienciaAcreditada.GetValueOrDefault(0);
                inv.IdDirectorDeArea = Programa.Director.GetValueOrDefault(0);
                inv.FirmaOficio = Programa.DirectorFirma.GetValueOrDefault(0);
                inv.VoBo = Programa.txtVoBo;

                var invitados = _sicop.ContratistasProgramaContratacion.Where(x => x.IdPaquete == inv.IdPaquete);
                foreach (var cont in invitados)
                {
                    var emp = _sicop.EMPRESAS.Any(x => x.Clave == cont.idContratista)
                        ? _sicop.EMPRESAS.FirstOrDefault(x => x.Clave == cont.idContratista).Nombre
                        : "";
                    inv.ListaInvitados.Add(new Licitante()
                    {
                        IdInvitado = cont.id,
                        Invitado = emp,
                        Oficio = cont.Oficio,
                        FechaOficio = cont.FechaOficio.GetValueOrDefault(DateTime.MinValue),
                    });
                }
                var contratista = _sicop.EMPRESAS.Select(x => new {Id = x.Clave, x.Nombre}).OrderBy(x => x.Nombre);
                inv.ListaContratistas = new SelectList(contratista, "Id", "Nombre");
                var firmas =
                    _sicop.DC.Select(x => new {Id = x.Clave, Nombre = x.Nombre + "-" + x.Cargo}).OrderBy(x => x.Nombre);
                inv.ListaFirmas = new SelectList(firmas, "Id", "Nombre");
                var Exp = _sicop.CatalogoExperienciaAcreditada.Select(x => new {Id = x.Id, clave = x.Clave + "-" + x.Nombre})
                        .OrderBy(x => x.clave);
                inv.ListaExpAcreditada = new SelectList(Exp, "Id", "clave");
                var leyendas = _sicop.Leyendas.Select(x => new {Id = x.id, leyenda = x.Anio + " - " + x.Leyendas1});
                inv.ListaLeyendas = new SelectList(leyendas, "Id", "Leyenda");

            }
            inv.IdPaquete = paq;
            inv.IdEjercicio = eje;

            return inv;
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        
        public ActionResult Index()
        {
            var inv = new Cap_InvitacionModelView();
            inv = CargaDatos(inv);
            return View("Index", inv);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Ejercicio(Cap_InvitacionModelView inv)
        {
            inv = CargaDatos(inv);

            return View("Index", inv);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult Paquetes(Cap_InvitacionModelView inv)
        {
            inv = CargaDatos(inv);

            return View("Index", inv);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult GuardarInvitado(Cap_InvitacionModelView inv)
        {
            var Error = "";
            var Exito = "";
            if (
                _sicop.ContratistasProgramaContratacion.Any(
                    x => x.idContratista == inv.NuevoInvitado.IdInvitado && x.IdPaquete == inv.IdPaquete))
            {
                Error = "Ya se encuentra registrado el Invitado";
            }
            else
            {
                if (!String.IsNullOrEmpty(inv.NuevoInvitado.Oficio) && inv.NuevoInvitado.IdInvitado > 0 &&
                    inv.IdPaquete > 0)
                {
                    try
                    {
                        var Nuevoinvitado = new ContratistasProgramaContratacion();

                        Nuevoinvitado.idContratista = inv.NuevoInvitado.IdInvitado;
                        Nuevoinvitado.IdPaquete = inv.IdPaquete;
                        Nuevoinvitado.Oficio = inv.NuevoInvitado.Oficio;
                        Nuevoinvitado.FechaOficio = inv.NuevoInvitado.FechaOficio;

                        _sicop.ContratistasProgramaContratacion.Add(Nuevoinvitado);
                        _sicop.SaveChanges();
                        Exito = "Se guardo correctamente el nuevo invitado";

                    }
                    catch
                    {
                        Error = "No fue posible guardar el nuevo invitado";
                    }
                }
            }
            inv = CargaDatos(inv);
            inv.Error = Error;
            inv.Exito = Exito;
            return View("Index", inv);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult ELiminaInvitado(Cap_InvitacionModelView inv)
        {
            var Error = "";
            var Exito = "";
            if (_sicop.ContratistasProgramaContratacion.Any(
                x => x.IdPaquete == inv.IdPaquete && x.id == inv.IdEliminaInvitado))
            {
                var elim =
                    _sicop.ContratistasProgramaContratacion.FirstOrDefault(
                        x => x.IdPaquete == inv.IdPaquete && x.id == inv.IdEliminaInvitado);
                _sicop.ContratistasProgramaContratacion.Remove(elim);
                _sicop.SaveChanges();
                Exito = "Se eliminó correctamente al invitado";
            }
            else
            {
                Error = "No se Encontó el Contratista";
            }
            inv = CargaDatos(inv);
            inv.Error = Error;
            inv.Exito = Exito;
            return View("Index", inv);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult GuardarDatos(Cap_InvitacionModelView inv)
        {
            var Error = "";
            var Exito = "";
            try
            {
                var paq = _sicop.ProgramaDeContratacion.FirstOrDefault(x => x.IdPaquete == inv.IdPaquete);
                if (inv.IdExperienciaAcreditada > 0)
                {
                    paq.idExperienciaAcreditada = inv.IdExperienciaAcreditada;
                }
                if (inv.IdDirectorDeArea > 0)
                {
                    paq.Director = inv.IdDirectorDeArea;
                }
                if (inv.FirmaOficio > 0)
                {
                    paq.DirectorFirma = inv.FirmaOficio;
                }
                if (!String.IsNullOrEmpty(inv.VoBo))
                {
                    paq.txtVoBo = inv.VoBo;
                }
                _sicop.SaveChanges();
                Exito = "Se actualizó la informacion de firmas correctamente";
            }
            catch
            {
                Error = "No fue posible cambiar la infromacion de firmas";
            }
            inv = CargaDatos(inv);
            inv.Error = Error;
            inv.Exito = Exito;
            return View("Index", inv);
        }

        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult AgregaCCP(Cap_InvitacionModelView inv)
        {
            var Error = "";
            var Exito = "";
            var Ord = _sicop.Detalle_CcpDesignacion.Any(x => x.contrato == inv.IdPaquete.ToString())
                ? _sicop.Detalle_CcpDesignacion.Where(x => x.contrato == inv.IdPaquete.ToString()).Max(x => x.Numero)
                : 0;
            Ord = Ord + 1;
            if (inv.NuevaCopia.Id > 0)
            {
              var nom = _sicop.DC.FirstOrDefault(x => x.Clave == inv.NuevaCopia.Id);
              var Registro = new Detalle_CcpDesignacion()
            {
                Numero = Ord,
                contrato = inv.IdPaquete.ToString(),
                nombre_ccp = nom.Nombre+" .- "+ nom.Cargo,
                TipoDe = "OFICIO DE INVITACIÓN A LICITANTES"
              };
                _sicop.Detalle_CcpDesignacion.Add(Registro);
                _sicop.SaveChanges();
                Exito = "Se guardó el destinatario correctamente";
            }
        else
            {
                Error = "No se encuentra el registro";
            }                        
            inv = CargaDatos(inv);
            inv.Error = Error;
            inv.Exito = Exito;
            return View("Index", inv);
        }


        [Authorize(Roles = "Administrador,P-Programa")]
        [HttpPost]
        public ActionResult EliminaCCP(Cap_InvitacionModelView inv)
        {
            var Error = "";
            var Exito = "";
            if (
                _sicop.Detalle_CcpDesignacion.Any(
                    x => x.Id == inv.IdEliminaCcp && x.contrato == inv.IdPaquete.ToString()))
            {
                var elimCcp =
                    _sicop.Detalle_CcpDesignacion.FirstOrDefault(
                        x => x.Id == inv.IdEliminaCcp && x.contrato == inv.IdPaquete.ToString());
                _sicop.Detalle_CcpDesignacion.Remove(elimCcp);
                _sicop.SaveChanges();

                Exito = "Se elimino correctamente destinatario de copia";
            }
            else
            {
                Error = "No se encontró el destinatario de copia para eliminarse";
            }

            inv = CargaDatos(inv);
            inv.Error = Error;
            inv.Exito = Exito;
            return View("Index", inv);
        }
    }
}