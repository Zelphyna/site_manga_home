using System.ComponentModel.DataAnnotations;

namespace site_manga_home.Domain;

public sealed class Manga
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est obligatoire")]
    [MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "URL de couverture")]
    public string CoverUrl { get; set; } = string.Empty;

    [Range(1, 9999, ErrorMessage = "Le nombre de tomes doit être entre 1 et 9999")]
    [Display(Name = "Tomes total")]
    public int TomesTotal { get; set; }

    [Range(0, 9999, ErrorMessage = "Le nombre de tomes possédés doit être entre 0 et 9999")]
    [Display(Name = "Tomes possédés")]
    public int TomesPossedes { get; set; }

    public List<int> TomesPossedesNumeros { get; set; } = [];
}
