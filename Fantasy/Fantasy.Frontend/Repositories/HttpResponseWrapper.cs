using System.Net;

namespace Fantasy.Frontend.Repositories;

public class HttpResponseWrapper<T>
{
    public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
    {
        Response = response;
        Error = error;
        HttpResponseMessage = httpResponseMessage;
    }

    public T? Response { get; }
    public bool Error { get; }
    public HttpResponseMessage HttpResponseMessage { get; }

    public async Task<string?> GetErrorMessageAsync()
    {
        if (!Error)
        {
            return null;
        }

        switch (HttpResponseMessage.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return "Recurso no encontrado.";

            case HttpStatusCode.BadRequest:
                return await HttpResponseMessage.Content.ReadAsStringAsync();

            case HttpStatusCode.Unauthorized:
                return "Tienes que estar logueado para ejecutar esta operación.";

            case HttpStatusCode.Forbidden:
                return "No tienes permisos para hacer esta operación.";

            default:
                return "Ha ocurrido un error inesperado.";
        }
    }
}