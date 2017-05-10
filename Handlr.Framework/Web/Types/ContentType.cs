using Handlr.Framework.Web.Attributes;

namespace Handlr.Framework.Web.Types
{
    public enum ContentType
    {
        [Display("none")]
        None,
        [Display("*/*")]
        Any,
        [Display("text/plain")]
        Other,
        [Display("text/html")]
        Html,
        [Display("application/x-www-form-urlencoded", "multipart/form-data")]
        Form,
        [Display("text/xml", "application/xml")]
        Xml,
        [Display("application/json", "text/json", "text/json; charset=utf-8", "application/json; charset=utf-8")]
        Json,
        [Display("application/soap+xml")]
        SOAP,
        [Display("text/css")]
        Css,
        [Display("text/javascript", "application/javascript")]
        JavaScript,
        [Display("text/plain")]
        PlainText
    }
}
