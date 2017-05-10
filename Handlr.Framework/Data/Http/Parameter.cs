namespace Handlr.Framework.Data.Http
{
    public sealed class Parameter : System.Data.Common.DbParameter
    {
        public override System.Data.DbType DbType { get; set; }

        public override System.Data.ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        public override string ParameterName { get; set; }

        public override void ResetDbType()
        {
        }

        public override int Size { get; set; }

        public override string SourceColumn { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override System.Data.DataRowVersion SourceVersion { get; set; }

        public override object Value { get; set; }
    }
}
