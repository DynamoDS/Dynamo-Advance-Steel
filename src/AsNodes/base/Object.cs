using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.CADAccess;

namespace AdvanceSteel.Nodes
{
    /// <summary>
    /// the representant of an Advance Steel object in Dynamo
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public abstract class Object : IDisposable, IGraphicItem, IFormattable
    {
        private string ObjectHandle;
        internal readonly object myLock = new object();

        /// <summary>
        /// property that holds the handle of the object
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        protected string Handle 
        { 
            get
            {
                return ObjectHandle;
            }
            set
            {
                ObjectHandle = value;

                var elementManager = AdvanceSteelServices.LifecycleManager<string>.GetInstance();
                elementManager.RegisterAsssociation(ObjectHandle, this);
            }
        }
        [IsVisibleInDynamoLibrary(false)]
        public virtual void Dispose()
        {
            //use lock just to be safe
            //AutoCAD does not support multithreaded access
            lock (myLock)
            {
                // Do not cleanup elements if we are shutting down Dynamo.
                if (DSNodeServices.DisposeLogic.IsShuttingDown)
                    return;

                //this function is not implemented for the moment
                bool didAdvanceSteelDelete = AdvanceSteelServices.LifecycleManager<string>.GetInstance().IsAdvanceSteelDeleted(Handle);

                var elementManager = AdvanceSteelServices.LifecycleManager<string>.GetInstance();
                int remainingBindings = elementManager.UnRegisterAssociation(Handle, this);

                // Do not delete owned elements
                if (remainingBindings == 0 && !didAdvanceSteelDelete)
                {
                    //lock the document and start a transaction
                    using(var x = new AdvanceSteelServices.ObjectAccess.CADContext())
                    {
                        if (Handle != null)
                        {
                            Autodesk.AdvanceSteel.DocumentManagement.CADDocumentManager.GetCurrentDatabase().getFilerObjectIdFromHandle("");
                            var filerObject = Utils.GetObject(Handle);

                            if (filerObject != null)
                                filerObject.DelFromDb();

                            ObjectHandle = string.Empty;
                        }
                    }
                }
                else
                {
                    //This element has gone
                    ObjectHandle = string.Empty;
                }
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, double tol, int gridLines)
        {
        }
        [IsVisibleInDynamoLibrary(false)]
        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }
}