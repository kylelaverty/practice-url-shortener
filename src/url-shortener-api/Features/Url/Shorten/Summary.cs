namespace Url.Shortener.Api.Features.Url.Shorten;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Shortens a given URL.";
        Description = "Creates a shortened version of the provided original URL.";
        Response(StatusCodes.Status201Created, "URL shortened successfully.");
        Response(StatusCodes.Status400BadRequest, "Invalid request data.");
        Response(StatusCodes.Status500InternalServerError, "Internal server error.");
    }
}