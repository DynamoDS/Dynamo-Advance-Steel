
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.CADLink.Database;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes
{
    /// <summary>DynamoServices
    /// An AdvanceSteel Straight Beam
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class StraightBeam: GraphicObject
    {
        internal StraightBeam( Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd)
        {
            //use lock just to be safe
            //AutoCAD does not support multithreaded access
            lock (myLock)
            {

                //lock the documnet and start transaction
                using (var x = new AdvanceSteelServices.ObjectAccess.CADContext())
                {
                    string handle = null;

                    handle = AdvanceSteelServices.ElementBinder.GetHandleFromTrace();

                    Point3d beamStart = Utils.ToAstPoint(ptStart);
                    Point3d beamEnd = Utils.ToAstPoint(ptEnd);

                    if (handle == null || Utils.GetObject(handle) == null)
                    {
                        ProfileName profName = new ProfileName();
                        ProfilesManager.GetProfTypeAsDefault("I", out profName);
                        var myBeam = new Autodesk.AdvanceSteel.Modelling.StraightBeam(profName.Name, beamStart, beamEnd, Vector3d.kXAxis);

                        myBeam.WriteToDb();
                        handle = myBeam.Handle;
                    }

                    Beam beam = Utils.GetObject(handle) as Beam;

                    if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kStraightBeam))
                    {
                        bool bNotSet = beam.SetSysEnd(beamEnd);
                        bNotSet &= beam.SetSysStart(beamStart);

                        if (bNotSet == false)
                            throw new SystemException("Create Advance Steel setsystemline");
                    }
                    else
                        throw new SystemException("not a StraightBeam");

                    this.Handle = handle;

                    AdvanceSteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
                }
            }
        }

        /// <summary>
        /// Create an advance steel beam between two end points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static StraightBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end)
        {
            return new StraightBeam(start, end);
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
                        var beam = Utils.GetObject(Handle) as Beam;

                        Point3d asPt1 = beam.GetPointAtStart(0);
                        Point3d asPt2 = beam.GetPointAtEnd(0);

                        var pt1 = Utils.ToDynPoint(beam.GetPointAtStart(0));
                        var pt2 = Utils.ToDynPoint(beam.GetPointAtEnd(0));

                        var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(pt1, pt2);
                        return line;
                    }
                }
            }
        }
    }
}
