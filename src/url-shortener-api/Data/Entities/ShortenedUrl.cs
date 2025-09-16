using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Url.Shortener.Api.Data.Entities;

[Table("shortened_urls", Schema = "url")]
public class ShortenedUrl
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("original_url")]
    public required string OriginalUrl { get; set; }
    
    [Required]
    [Column("generated_code")]
    public required string GeneratedCode { get; set; }
    
    [Column("created_date")]
    public DateTime CreatedDate { get; set; }
}