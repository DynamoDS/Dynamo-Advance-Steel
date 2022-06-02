using System;
using System.Collections.Generic;

namespace Dynamo.Applications.AdvanceSteel.Services
{
  /// <summary>
  /// Used to manage the life of Advance Steel objects
  /// </summary>
  public class LifecycleManager
  {
    private static LifecycleManager manager;
    private Dictionary<string, List<Object>> wrappers;

    private LifecycleManager()
    {
      wrappers = new Dictionary<string, List<object>>();
    }

    public static LifecycleManager GetInstance()
    {
      if (manager == null)
      {
        manager = new LifecycleManager();
      }

      return manager;
    }

    public static void DisposeInstance()
    {
      if (manager != null)
      {
        manager = null;
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
  }
}