using System.ComponentModel.DataAnnotations;
using MovieScreeningApp.Api.Attributes;

namespace MovieScreeningApp.Api.Entities;

public class ApiKey : BaseEntity
{
    
    public string ApiName { get; set; }
    [Encrypted]
    public string Value { get; set; }
    
    private ApiKey()
    {
        
    }

    public ApiKey(string apiName, string value)
    {
        ApiName = apiName;
        Value = value;
    }
}