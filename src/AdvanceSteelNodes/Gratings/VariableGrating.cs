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
	/// Advance Steel Variable Grating Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class VariableGrating : GraphicObject
	{

		internal VariableGrating(string strClass, string strName, Plane plane, Point3d ptCenter, double dWidth, double dLength)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{

					Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, ptCenter, dLength, dWidth);
						gratings.WriteToDb();
					}
					else
					{
						gratings = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.Grating;
						if (gratings != null && gratings.IsKindOf(FilerObject.eObjectType.kGrating))
						{
							gratings.GratingClass = strClass;
							gratings.GratingSize = strName;
							gratings.DefinitionPlane = plane;
							gratings.SetLength(dLength, true);
							gratings.SetWidth(dWidth, true);
						}
						else
						{
							throw new System.Exception("Not a Variable Grating pattern");
						}
					}

					Handle = gratings.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Variable grating
		/// </summary>

		/// <returns></returns>
		public static VariableGrating ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle, string strClass, string strName)
		{
			var dynCorners = rectangle.Corners();
			var astCorners = Utils.ToAstPoints(dynCorners, true);
			var refPoint = astCorners[0] + (astCorners[2] - astCorners[0]) * 0.5;
			var vx = astCorners[1] - astCorners[0];
			var vy = astCorners[3] - astCorners[0];

			Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(refPoint, vx, vy);
			return new VariableGrating(strClass, strName, plane, refPoint, Utils.ToInternalUnits(rectangle.Width, true), Utils.ToInternalUnits(rectangle.Height, true));
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

