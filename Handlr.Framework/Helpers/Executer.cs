using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Handlr.Framework
{
    public static class CodeExecuter
    {
        public static string CSharp(string code, dynamic model)
        {
            string condition = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Data;
            using Handlr.Common;

            namespace Handlr.Framework
            {
                public class CodeExecutor
                {
                    public string CSharp(dynamic Model)
                    {
                        string result = " + "\"\"" + @";
                        " + (code.IndexOf(";") > 0 ? code : @"result = " + code + ";") + @"
                        return result;
                    }
                }
            }";
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>()
                {
                    {"CompilerVersion", "v4.0"}
                });
            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                CompilerOptions = "/optimize"
            };
            parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Web.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, condition);
            if (results.Errors.Count > 0)
                return "ERROR: " + new Func<string>(() =>
                    {
                        string errors = "";
                        foreach (CompilerError error in results.Errors)
                        {
                            errors += error.ErrorText + "\n";
                        }
                        return errors;
                    })();
            object o = results.CompiledAssembly.CreateInstance("Handlr.Framework.CodeExecutor");
            MethodInfo info = o.GetType().GetMethod("CSharp");
            string r = (string)info.Invoke(o, new object[] { (dynamic)model });
            return r;
        }
    }
}
