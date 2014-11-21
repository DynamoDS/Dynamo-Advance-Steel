using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.CADAccess;
namespace AdvanceSteel.Nodes
{
    [IsVisibleInDynamoLibrary(false)]
    internal static class Utils
    {
        static public Autodesk.AdvanceSteel.Geometry.Point3d ToAstPoint(Autodesk.DesignScript.Geometry.Point pt)
        {
            return new Autodesk.AdvanceSteel.Geometry.Point3d(pt.X, pt.Y, pt.Z);
        }
        static public Autodesk.DesignScript.Geometry.Point ToDynPoint(Autodesk.AdvanceSteel.Geometry.Point3d pt)
        {
            return Autodesk.DesignScript.Geometry.Point.ByCoordinates(pt.x, pt.y, pt.z);
        }
        static public FilerObject GetObject( string handle)
        {
            //var objectId = CADDocumentManager.GetCurrentDatabase().getFilerObjectIdFromHandle(handle);
            //return FilerObject.GetFilerObject(objectId);
            return FilerObject.GetFilerObjectByHandle(handle);
        }
    }
}