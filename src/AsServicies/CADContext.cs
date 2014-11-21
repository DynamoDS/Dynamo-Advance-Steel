using Autodesk.AdvanceSteel.DocumentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteelServices.ObjectAccess
{
    /// <summary>
    /// used to manage the documents locking and transcations while accesiing advance steel objects
    /// </summary>
    public class CADContext:IDisposable
    {
        public CADContext()
        {
            bool bNotFail = true;
            bNotFail &= DocumentManager.lockCurrentDocument();

            if( bNotFail == true)
                bNotFail &= Autodesk.AdvanceSteel.CADAccess.TransactionManager.startTransaction();
        }
        public void Dispose()
        {
            bool bNotFail = true;
            bNotFail &= Autodesk.AdvanceSteel.CADAccess.TransactionManager.endTransaction();
            bNotFail &= DocumentManager.unlockCurrentDocument();
        }
    }
}
