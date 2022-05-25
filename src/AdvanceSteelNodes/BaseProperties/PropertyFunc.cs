using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes
{
  public class PropertyMethods
  {
    internal PropertyMethods(Type typeProperties, string methodInfoGet, string methodInfoSet)
    {
      if (!string.IsNullOrEmpty(methodInfoGet))
      {
        MethodInfoGet = typeProperties.GetMethod(methodInfoGet, BindingFlags.Static);
        if (MethodInfoGet == null || MethodInfoGet.GetParameters().Length != 1 && MethodInfoGet.ReturnType == typeof(void))
        {
          throw new Exception(string.Format("Method Get '{0}' must have 1 parameter, 1 return and be static", methodInfoGet));
        }
      }

      if (!string.IsNullOrEmpty(methodInfoSet))
      {
        MethodInfoSet = typeProperties.GetMethod(methodInfoSet, BindingFlags.Static);
        if (MethodInfoSet == null || MethodInfoSet.GetParameters().Length != 2 && MethodInfoSet.ReturnType != typeof(void))
        {
          throw new Exception(string.Format("Method Set '{0}' must have 2 parameters, void return and be static", MethodInfoSet));
        }
      }

      if(MethodInfoGet == null && MethodInfoSet == null)
      {
        throw new Exception("Must have at least 1 get or set function");
      }

      if (MethodInfoGet != null && MethodInfoSet != null)
      {
        //Compare set parameter with get return
        if (MethodInfoGet.ReturnType != MethodInfoSet.GetParameters()[1].ParameterType)
        {
          throw new Exception("The get return type must be the same second parameter of set method");
        }
      }

    }

    internal MethodInfo MethodInfoGet { get; private set; }

    internal MethodInfo MethodInfoSet { get; private set; }
  }
}
