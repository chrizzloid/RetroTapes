using System.ComponentModel.DataAnnotations;

namespace RetroTapes.ViewModels
{
    public class FilmEditVm
    {
        public int? FilmId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Description { get; set; }
        [RegularExpression("^\\d{4}$", ErrorMessage = "Ange ett år (YYYY).")]
        public string? ReleaseYear { get; set; }

        [Required(ErrorMessage = "Välj språk")]
        public byte LanguageId { get; set; }

        public byte? OriginalLanguageId { get; set; }

        public List<byte> CategoryIds { get; set; } = new();
        public List<int> ActorIds { get; set; } = new();
        public byte StoreId { get; set; } = 1;
        public int? StockDesired { get; set; }

        // Concurrency
        public DateTime? LastUpdate { get; set; }

    }

    public class FilmListItemVm
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ReleaseYear { get; set; }
        public string LanguageName { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
    }


    public class FilmDetailVm
    {
        public int FilmId;
        public string Title;
        public string? Description;
        public string? ReleaseYear;
        public string Language;
        public string Categories;
        public string Actors;
        public DateTime LastUpdate;
    }
}
