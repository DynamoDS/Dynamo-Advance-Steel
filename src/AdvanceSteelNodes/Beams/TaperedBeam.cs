using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Beams
{
	/// <summary>
	/// Advance Steel tapered beam
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class TaperedBeam : GraphicObject
	{
		internal TaperedBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation, double startHeight = 100, double endHeight=100, double webThickness=100)
		{
			//use lock just to be safe
			//AutoCAD does not support multithreaded access
			lock (myLock)
			{
				//lock the document and start transaction
				using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
				{
					string handle = AdvanceSteel.Services.ElementBinder.GetHandleFromTrace();

					Point3d beamStart = Utils.ToAstPoint(ptStart, true);
					Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						var myBeam = new Autodesk.AdvanceSteel.Modelling.BeamTapered(beamStart, beamEnd, Vector3d.kXAxis, startHeight, endHeight, webThickness);
						myBeam.CreateComponents();

						myBeam.WriteToDb();
						handle = myBeam.Handle;
					}

					BeamTapered beamTapered = Utils.GetObject(handle) as BeamTapered;

					if (beamTapered != null && beamTapered.IsKindOf(FilerObject.eObjectType.kBeamTapered))
					{
						Utils.AdjustBeamEnd(beamTapered, beamStart);
						beamTapered.SetSysStart(beamStart);
						beamTapered.SetSysEnd(beamEnd);

						Utils.SetOrientation(beamTapered, Utils.ToAstVector3d(vOrientation, true));
					}
					else
						throw new System.Exception("Not a tapered beam");

					this.Handle = handle;

					AdvanceSteel.Services.ElementBinder.CleanupAndSetElementForTrace(beamTapered);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel tapered beam
		/// </summary>
		/// <param name="start">Start point</param>
		/// <param name="end">End point</param>
		/// <param name="vOrientation">Section orientation</param>
		/// <returns></returns>
		public static TaperedBeam ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector vOrientation)
		{
			return new TaperedBeam(start, end, vOrientation);
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
					using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
					{
						var beam = Utils.GetObject(Handle) as Beam;

						Point3d asPt1 = beam.GetPointAtStart(0);
						Point3d asPt2 = beam.GetPointAtEnd(0);

						var pt1 = Utils.ToDynPoint(beam.GetPointAtStart(0), true);
						var pt2 = Utils.ToDynPoint(beam.GetPointAtEnd(0), true);

						var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(pt1, pt2);
						return line;
					}
				}
			}
		}
	}
}