using System.ComponentModel.DataAnnotations;

namespace MovieScreeningApp.Api.DTOs;

public class SetApiKeyRequestDto
{
    [Required]
    public string Value { get; set; } = string.Empty;
}
