using System;
using AdvanceSteel.Nodes.Plates;
using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.DesignScript.Runtime;
using DynGeometry = Autodesk.DesignScript.Geometry;
using SteelGeometry = Autodesk.AdvanceSteel.Geometry;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using Autodesk.AdvanceSteel.ConstructionTypes;
using System.Collections.Generic;
using Autodesk.AdvanceSteel.Geometry;
using System.Linq;

namespace AdvanceSteel.Nodes.Gratings
{
	/// <summary>
	/// Advance Steel Bar Grating Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class BarGrating : GraphicObject
	{
		internal BarGrating(Plane plane, Point3d ptCenter, double dLength)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						gratings = new Autodesk.AdvanceSteel.Modelling.Grating("ADT", 11, 2, "3 / 16 inch", "10", "3/16", plane, ptCenter, dLength);
						gratings.WriteToDb();
					}
					else
					{
						gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
						if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
						{
							gratings.DefinitionPlane = plane;
							gratings.SetLength(dLength, true);
						}
						else
						{
							throw new System.Exception("Not a Bar Grating pattern");
						}
					}
					Handle = gratings.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
				}
			}
		}
		/// <summary>
		/// Create an Advance Steel Bar Grating
		/// </summary>
		/// <returns></returns>

		public static BarGrating ByLine(Autodesk.DesignScript.Geometry.Line line, Autodesk.DesignScript.Geometry.Vector planDirection)
		{
			var start = Utils.ToAstPoint(line.StartPoint, true);
			var end = Utils.ToAstPoint(line.EndPoint, true);
			var refPoint = start + (end - start) * 0.5;
			var planeNorm = Utils.ToAstVector3d(planDirection, true);

			if (!planeNorm.IsPerpendicularTo(Utils.ToAstVector3d(line.Direction, true)))
			{
				throw new System.Exception("Plan Direction must be perpendicular to line");
			}

			Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(refPoint, planeNorm);
			return new BarGrating(plane, refPoint, Utils.ToInternalUnits(line.Length, true));
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var grating = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.Grating;

					if (grating == null)
					{
						throw new Exception("Null Variable Grating pattern");
					}

					List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>(GratingDraw.GetPointsToDraw(grating));

					return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
				}
			}
		}
	}
}

