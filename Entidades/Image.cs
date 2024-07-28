using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class Image
    {
        // Properties
        public int Id { get; set; }
        [Display(Name = "Nombre de la Imagen")]
        public string FileName { get; set; }
        [Display(Name = "Imagen")]
        public byte[] Data { get; set; }

        
    }
}
