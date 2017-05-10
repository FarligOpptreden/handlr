using System.Collections.Generic;

namespace Handlr.Framework.Web.ViewModel
{
    public class Validation
    {
        public Validation()
        {
            Errors = new List<string>();
        }

        public bool IsValid
        {
            get
            {
                return Errors.Count == 0;
            }
        }
        public List<string> Errors { get; private set; }
    }
}
