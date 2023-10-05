using System.ComponentModel.DataAnnotations;

namespace Bwr.Exchange.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}