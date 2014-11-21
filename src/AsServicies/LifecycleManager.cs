using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteelServices
{
    /// <summary>
    /// used to manage the life of Advance Steel objects
    /// for the moment it contains some death functionalities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LifecycleManager<T>
    {
        //Note this is only mutex for a specific type param
        private static Object singletonMutex = new object();
        private static LifecycleManager<T> manager;

        private Dictionary<T, List<Object>> wrappers;
        private Dictionary<T, bool> advanceSteelDeleted;


        private LifecycleManager()
        {
            wrappers = new Dictionary<T, List<object>>();
            advanceSteelDeleted = new Dictionary<T, bool>();
        }

        public static LifecycleManager<T> GetInstance()
        {
            lock (singletonMutex)
            {
                if (manager == null)
                {
                    manager = new LifecycleManager<T>();
                }

                return manager;
            }
        }


        /// <summary>
        /// Register a new dependency between an element handle and a wrapper
        /// </summary>
        /// <param name="elementHandle"></param>
        /// <param name="wrapper"></param>
        public void RegisterAsssociation(T elementHandle, Object wrapper)
        {

            List<Object> existingWrappers;
            if (wrappers.TryGetValue(elementHandle, out existingWrappers))
            {
                //handle already existed, check we're not over adding
                DSNodeServices.Validity.Assert(!existingWrappers.Contains(wrapper),
                    "Lifecycle manager alert: registering the same Element Wrapper twice");
            }
            else
            {
                existingWrappers = new List<object>();
                wrappers.Add(elementHandle, existingWrappers);
            }

            existingWrappers.Add(wrapper);
            if (!advanceSteelDeleted.ContainsKey(elementHandle))
            {
                advanceSteelDeleted.Add(elementHandle, false);
            }
        }

        /// <summary>
        /// Remove an association between an element handle and 
        /// </summary>
        /// <param name="elementHandle"></param>
        /// <param name="wrapper"></param>
        /// <returns>The number of remaining associations</returns>
        public int UnRegisterAssociation(T elementHandle, Object wrapper)
        {
            List<Object> existingWrappers;
            if (wrappers.TryGetValue(elementHandle, out existingWrappers))
            {
                //handle already existed, check we're not over adding
                if (existingWrappers.Contains(wrapper))
                {
                    existingWrappers.Remove(wrapper);
                    if (existingWrappers.Count == 0)
                    {
                        wrappers.Remove(elementHandle);
                        advanceSteelDeleted.Remove(elementHandle);
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
        /// Get the number of wrappers that are registered
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int GetRegisteredCount(T handle)
        {
            if (!wrappers.ContainsKey(handle))
            {
                return 0;
            }
            else
            {
                return wrappers[handle].Count;
            }

        }

        /// <summary>
        /// Checks whether an element has been deleted in AdvanceSteel
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool IsAdvanceSteelDeleted(T handle)
        {
            if (!advanceSteelDeleted.ContainsKey(handle))
            {
                throw new ArgumentException("Element is not registered");
            }

            return advanceSteelDeleted[handle];
        }


        /// <summary>
        /// This method tells the life cycle 
        /// </summary>
        /// <param name="handle"The element that needs to be deleted></param>
        public void NotifyOfDeletion(T handle)
        {
            advanceSteelDeleted[handle] = true;

        }
    }
}
