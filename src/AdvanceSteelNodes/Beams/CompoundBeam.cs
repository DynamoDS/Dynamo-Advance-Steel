using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;

namespace AdvanceSteel.Nodes.Beams
{
	/// <summary>
	/// Advance Steel straight beam
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class CompoundBeam : GraphicObject
	{
		internal CompoundBeam(Autodesk.DesignScript.Geometry.Point ptStart, Autodesk.DesignScript.Geometry.Point ptEnd, Autodesk.DesignScript.Geometry.Vector vOrientation, string compoundClassName, string compoundTypeName)
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
						var myBeam = new Autodesk.AdvanceSteel.Modelling.CompoundStraightBeam(beamStart, beamEnd, Vector3d.kXAxis);
						myBeam.CreateComponents(compoundClassName, compoundTypeName);

						myBeam.WriteToDb();
						handle = myBeam.Handle;
					}

					CompoundStraightBeam beamCompound = Utils.GetObject(handle) as CompoundStraightBeam;

					if (beamCompound != null && beamCompound.IsKindOf(FilerObject.eObjectType.kCompoundStraightBeam))
					{
						Utils.AdjustBeamEnd(beamCompound, beamStart);
						beamCompound.SetSysStart(beamStart);
						beamCompound.SetSysEnd(beamEnd);
						Utils.SetOrientation(beamCompound, Utils.ToAstVector3d(vOrientation, true));
					}
					else
						throw new System.Exception("Not a compound beam");

					this.Handle = handle;

					AdvanceSteel.Services.ElementBinder.CleanupAndSetElementForTrace(beamCompound);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel compound beam made of rolled beams
		/// </summary>
		/// <param name="start">Start point</param>
		/// <param name="end">End point</param>
		/// <param name="vOrientation">Section orientation</param>
		/// <param name="compoundClassName">Compound class name</param>
		/// <param name="compoundTypeName">Compound type name</param>
		/// <returns></returns>
		public static CompoundBeam ByStartPointEndPointWithBeams(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector vOrientation, string compoundClassName = "Compound2U", string compoundTypeName = "Default")
		{
			return new CompoundBeam(start, end, vOrientation, compoundClassName, compoundTypeName);
		}

		/// <summary>
		/// Create an Advance Steel compound beam made of welded plates
		/// </summary>
		/// <param name="start">Start point</param>
		/// <param name="end">End point</param>
		/// <param name="vOrientation">Section orientation</param>
		/// <param name="compoundClassName">Compound class name</param>
		/// <param name="compoundTypeName">Compound type name</param>
		/// <returns></returns>
		public static CompoundBeam ByStartPointEndPointWithPlates(Autodesk.DesignScript.Geometry.Point start, Autodesk.DesignScript.Geometry.Point end, Autodesk.DesignScript.Geometry.Vector vOrientation, string compoundClassName = "WeldedISymmetric", string compoundTypeName = "Default")
		{
			return new CompoundBeam(start, end, vOrientation, compoundClassName, compoundTypeName);
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