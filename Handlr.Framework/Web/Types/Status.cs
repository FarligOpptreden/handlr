namespace Handlr.Framework.Web.Types
{
    public enum Status
    {
        OK = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        Redirect = 302,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        ContentNotAccepted = 406,
        UnsupportedMediaType = 415,
        GeneralException = 500,
        NotImplemented = 501
    }
}
