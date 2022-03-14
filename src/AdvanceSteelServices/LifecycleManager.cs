using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  /// <summary>
  /// Used to manage the life of Advance Steel objects
  /// </summary>
  public class LifecycleManager
  {
    private static LifecycleManager manager;
    private static readonly object access_obj = new object();

    private Dictionary<string, List<Object>> wrappers;
    private Dictionary<string, bool> bindingEntities;

    private LifecycleManager()
    {
      wrappers = new Dictionary<string, List<object>>();
      bindingEntities = new Dictionary<string, bool>();
    }

    public static LifecycleManager GetInstance()
    {
      lock (access_obj)
      {
        if (manager == null)
        {
          manager = new LifecycleManager();
        }

        return manager;
      }
    }

    public static void DisposeInstance()
    {
      lock (access_obj)
      {
        if (manager != null)
        {
          manager = null;
        }
      }
    }

    /// <summary>
    /// Register a new dependency between an element handle and a wrapper
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <param name="wrapper"></param>
    public void RegisterAsssociation(string elementHandle, object wrapper)
    {
      List<object> existingWrappers;
      if (wrappers.TryGetValue(elementHandle, out existingWrappers))
      {
        //handle already existed, check we're not over adding
        DynamoServices.Validity.Assert(!existingWrappers.Contains(wrapper),
              "Lifecycle manager alert: registering the same Element Wrapper twice");
      }
      else
      {
        existingWrappers = new List<object>();
        wrappers.Add(elementHandle, existingWrappers);
      }

      existingWrappers.Add(wrapper);

      if (bindingEntities.ContainsKey(elementHandle))
      {
        bindingEntities[elementHandle] = true;
      }
      else
      {
        bindingEntities.Add(elementHandle, true);
      }
    }

    /// <summary>
    /// Remove an association between an element handle and
    /// </summary>
    /// <param name="elementHandle"></param>
    /// <param name="wrapper"></param>
    /// <returns>The number of remaining associations</returns>
    public int UnRegisterAssociation(string elementHandle, object wrapper)
    {
      List<object> existingWrappers;
      if (wrappers.TryGetValue(elementHandle, out existingWrappers))
      {
        //handle already existed, check we're not over adding
        if (existingWrappers.Contains(wrapper))
        {
          existingWrappers.Remove(wrapper);
          if (existingWrappers.Count == 0)
          {
            wrappers.Remove(elementHandle);
            bindingEntities.Remove(elementHandle);

            return 0;
          }
          else
          {
            return existingWrappers.Count;
          }
        }
        else
        {
          throw new InvalidOperationException(
              "Attempting to remove a wrapper that wasn't there registered");
        }
      }
      else
      {
        //The handle didn't exist

        throw new InvalidOperationException(
            "Attempting to remove a wrapper, but there were no ids registered");
      }
    }

    /// <summary>
    /// Checks whether an element binding
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool IsEntityBinding(string elementHandle)
    {
      if (!bindingEntities.ContainsKey(elementHandle))
      {
        throw new ArgumentException("Entity is not registered");
      }

      return bindingEntities[elementHandle];
    }

    /// <summary>
    /// This method tells the life cycle the element that needs to be unbinding
    /// </summary>
    /// <param name="elementHandle">The element that needs to be unbinding></param>
    public void NotifyOfUnbinding(string elementHandle)
    {
      if (bindingEntities.ContainsKey(elementHandle))
      {
        bindingEntities[elementHandle] = false;
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine("LifeCycleManager:");
      foreach (var kvp in wrappers)
      {
        sb.AppendLine(string.Format("\tHandle {0}:", kvp.Key));
        foreach (var item in kvp.Value)
        {
          sb.AppendLine(string.Format("\t\t{0}:", item));
        }
      }

      return sb.ToString();
    }

  }
}