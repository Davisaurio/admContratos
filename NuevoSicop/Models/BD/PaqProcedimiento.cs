//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuevoSicop.Models.BD
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaqProcedimiento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PaqProcedimiento()
        {
            this.ContratistasInvitadosPaquete = new HashSet<ContratistasInvitadosPaquete>();
            this.DetallePaquetesProyecto = new HashSet<DetallePaquetesProyecto>();
            this.InvitacionCCP = new HashSet<InvitacionCCP>();
            this.ProgramasContratacion = new HashSet<ProgramasContratacion>();
        }
    
        public int Id { get; set; }
        public Nullable<int> NoPaquete { get; set; }
        public Nullable<int> IdProyecto { get; set; }
        public string NoProcedimiento { get; set; }
        public string Descripcion { get; set; }
        public Nullable<System.DateTime> Convocatoria { get; set; }
        public Nullable<System.DateTime> LimiteInscripcion { get; set; }
        public Nullable<System.DateTime> VisitaSitio { get; set; }
        public Nullable<System.DateTime> JuntaAclaraciones { get; set; }
        public Nullable<System.DateTime> AperturaTecnica { get; set; }
        public Nullable<System.DateTime> Fallo { get; set; }
        public Nullable<System.DateTime> Contrato { get; set; }
        public Nullable<System.DateTime> InicioEstimada { get; set; }
        public Nullable<int> IdResidente { get; set; }
        public Nullable<decimal> Recuperacion { get; set; }
        public Nullable<decimal> ImportePresupuestoBase { get; set; }
        public Nullable<int> IdEjercicio { get; set; }
    
        public virtual Ejercicios Ejercicios { get; set; }
        public virtual Supervisores Supervisores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContratistasInvitadosPaquete> ContratistasInvitadosPaquete { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetallePaquetesProyecto> DetallePaquetesProyecto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvitacionCCP> InvitacionCCP { get; set; }
        public virtual Proyectos Proyectos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgramasContratacion> ProgramasContratacion { get; set; }
    }
}