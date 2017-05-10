namespace Handlr.Framework.Data.Soap
{
    public sealed class Command : Http.Command
    {
        public Command() :
            base()
        {
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                return _Connection;
            }
            set
            {
                if (value.GetType() != typeof(Connection))
                    throw new System.Exception("Please use a Handlr.Data.Soap.Connection type.");
                _Connection = (Connection)value;
            }
        }

        public override void Prepare()
        {
            
        }
    }
}
