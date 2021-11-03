using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Models
{
    
    [Table("SujetosObligados")]
    public class SujetoObligado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [CuilCuitValidator]
        public long CuilCuit { get; set; }

        [MaxLength(50, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [MaxLength(50, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Apellido { get; set; }

        [DisplayName("Razon Social")]
        [MaxLength(80, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string RazonSocial { get; set; }

        [DisplayName("Tipo de Sujeto Obligado")]
        [ForeignKey(nameof(TipoSujetoObligado))]
        public int IdTipoSujetoObligado { get; set; }

        [DisplayName("Tipo de Sujeto Obligado")]
        public TipoSujetoObligado TipoSujetoObligado { get; set; }

        [DisplayName("Esta habilitado")]
        public bool EstaHabilitado { get; set; }

        public DateTime FechaModificacion { get; set; }

        
        public int? IdUsuarioModificacion { get; set; }

        

    }

    public class CuilCuitValidator : ValidationAttribute
    {
       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                var errorMessage = "El Cuit/Cuil no debe der nulo";
                return new ValidationResult(errorMessage);
            }
            string cuilCuit = value.ToString() ;

            if (cuilCuit.Length != 11)
            {
                var errorMessage = "El Cuit/Cuil debe tener 11 carateres";
                return new ValidationResult(errorMessage);
            }
            string tipoPersona = cuilCuit.Substring(0, 2);
            if (tipoPersona != "20" && tipoPersona != "27" && tipoPersona != "23" && tipoPersona != "30")
            {
                var errorMessage = "El Cuit/Cuil debe tener un tipo de persona valido";
                return new ValidationResult(errorMessage);
            }
            //var array = cuilCuit.Split();
            //var algo = array[0].ValidoInt();
            //int sumaComprobacion = array[0].ValidoInt() * 5 + (int)array[1] * 4 + (int)array[2] * 3 + (int)array[3] * 2 + (int)array[4] * 7 + (int)array[5] * 6 + (int)array[6] * 5 + (int)array[7] * 4 + (int)array[8] * 3 + (int)array[9] * 2;
            //int numeroComprobacion = sumaComprobacion % 11;
            //int numeroComprobacionRestado = 11 - numeroComprobacion;
            //if (numeroComprobacion != (int)array[10])
            //{
            //    var errorMessage = "El Cuit/Cuil debe ser valido";
            //    return new ValidationResult(errorMessage);
            //}
            return ValidationResult.Success;
        }
    }
}