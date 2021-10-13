
// Reflection API
// Reference: https://johnlnelson.com/tag/assembly-gettypes/

using System;
using System.Reflection;
using System.Text;

namespace SystemReflection
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            Console.WriteLine(callingAssembly);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            StringBuilder sb = new StringBuilder();
            for(int i=0; i<assemblies.Length; i++)
            {
                Console.WriteLine(assemblies[i]);
                Console.WriteLine(assemblies[i].GetTypes().Length);
                Type[] types = assemblies[i].GetTypes();

                //iterate through the Type[] array
                foreach (Type type in types)
                {
                    sb.AppendLine("===============================================================");
                    sb.AppendLine(String.Format("Type Name: {0}", type.Name));
                    sb.AppendLine("===============================================================");

                    sb.AppendLine(String.Format("Type FullName: {0}", type.FullName));
                    sb.AppendLine(String.Format("Namespace: {0}", type.Namespace));

                    sb.AppendLine(String.Format("Is it a Class?: {0}", type.IsClass.ToString()));
                    sb.AppendLine(String.Format("Is it an Interface?: {0}", type.IsInterface.ToString()));
                    sb.AppendLine(String.Format("Is it Generic?: {0}", type.IsGenericType.ToString()));
                    sb.AppendLine(String.Format("Is it Public?: {0}", type.IsPublic.ToString()));
                    sb.AppendLine(String.Format("Is it Sealed?: {0}", type.IsSealed.ToString()));

                    sb.AppendLine(String.Format("Qualified Name: {0}", type.AssemblyQualifiedName));

                    if (type.BaseType != null && !String.IsNullOrEmpty(type.BaseType.Name))
                    {
                        sb.AppendLine(String.Format("Base Type: {0}", type.BaseType.Name));
                    }

                    string output = sb.ToString();
                    Console.WriteLine(output);


                    //there are many, many more properties that an be shown...
                }
            }
  

        }
    }
}
