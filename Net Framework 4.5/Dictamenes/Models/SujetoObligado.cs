using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

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
            string cuilCuit = value.ToString();

            if (cuilCuit.Length != 11)
            {
                var errorMessage = "El Cuit/Cuil debe tener 11 carateres";
                return new ValidationResult(errorMessage);
            }
            Regex rg = new Regex("[A-Z_a-z]");
            cuilCuit = cuilCuit.Replace("-", "");
            if (rg.IsMatch(cuilCuit))
            {
                var errorMessage = "El Cuit/Cuil no es valido";
                return new ValidationResult(errorMessage);
            }

            char[] cuitArray = cuilCuit.ToCharArray();
            double sum = 0;
            int bint = 0;
            int j = 7;
            for (int i = 5, c = 0; c != 10; i--, c++)
            {
                if (i >= 2)
                    sum += (Char.GetNumericValue(cuitArray[c]) * i);
                else
                    bint = 1;
                if (bint == 1 && j >= 2)
                {
                    sum += (Char.GetNumericValue(cuitArray[c]) * j);
                    j--;
                }
            }
            if ((cuitArray.Length - (sum % 11)) != Char.GetNumericValue(cuitArray[cuitArray.Length - 1]))
            {
                var errorMessage = "El Cuit/Cuil no es valido";
                return new ValidationResult(errorMessage);
            }
            return ValidationResult.Success;
        }
    }

}