using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.AdvanceSteel.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes
{
    /// <summary>
    /// An AdvanceSteel Bent Beam
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class BentBeam : GraphicObject
    {
        internal BentBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd,  Autodesk.DesignScript.Geometry.Point ptOnArc)
        {
            //use lock just to be safe
            //AutoCAD does not support multithreaded access
            lock (myLock)
            {
                //lock the document and start transaction
                using (var x = new AdvanceSteelServices.ObjectAccess.CADContext())
                {
                    string handle = null;

                    handle = AdvanceSteelServices.ElementBinder.GetHandleFromTrace();

                    var beamStart = (ptStart == null ? new Point3d() : Utils.ToAstPoint(ptStart));
                    var beamEnd = (ptEnd == null ? new Point3d() : Utils.ToAstPoint(ptEnd));
                    var onArc = Utils.ToAstPoint(ptOnArc);

                    if (handle == null || Utils.GetObject(handle) == null)
                    {
                        ProfileName profName = new ProfileName();
                        ProfilesManager.GetProfTypeAsDefault("I", out profName);

                        var myBeam = new Autodesk.AdvanceSteel.Modelling.BentBeam(profName.Name, Vector3d.kZAxis, beamStart, onArc, beamEnd);

                        myBeam.WriteToDb();
                        handle = myBeam.Handle;
                    }

                    var beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

                    if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kBentBeam))
                    {
                        beam.SetSystemline(beamStart, onArc, beamEnd);
                    }
                    else
                        throw new SystemException("not a StraightBeam");

                    Handle = handle;

                    AdvanceSteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
                }
            }
        }
        /// <summary>
        /// Create an advance steel bent beam between two points and a point on arc
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onArc"></param>
        /// <returns></returns>
        public static BentBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Point ptOnArc)
        {
            return new BentBeam(start, end, ptOnArc);
        }

        [IsVisibleInDynamoLibrary(false)]
        public override Autodesk.DesignScript.Geometry.Curve Curve
        {
            get
            {
                //use lock just to be safe
                //AutoCAD does not support multithreaded access
                lock (myLock)
                {
                    using (var x = new AdvanceSteelServices.ObjectAccess.CADContext())
                    {
                        var beam = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.BentBeam;

                        var start = Utils.ToDynPoint(beam.GetPointAtStart(0));
                        var end = Utils.ToDynPoint(beam.GetPointAtEnd(0));

                        Point3d asCenterPt = new Point3d();
                        beam.GetArcCenter(out asCenterPt);

                        var centerPt = Utils.ToDynPoint(asCenterPt);

                        var arc = Autodesk.DesignScript.Geometry.Arc.ByCenterPointStartPointEndPoint(centerPt, start, end);

                        return arc;
                    }
                }
            }
        }
    }
  
}