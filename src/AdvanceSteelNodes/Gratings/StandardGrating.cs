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
	/// Advance Steel Standard Grating Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class StandardGrating : GraphicObject
	{

		internal StandardGrating(string strClass, string strName, Plane plane, Point3d ptCenter)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					
					Autodesk.AdvanceSteel.Modelling.Grating gratings = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						gratings = new Autodesk.AdvanceSteel.Modelling.Grating(strClass, strName, plane, ptCenter);
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
						}
						else
						{
							throw new System.Exception("Not a Standard Grating pattern");
						}
					}
			
					Handle = gratings.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(gratings);
				}
			}
		}

		/// <summary>
		/// Create an Advance Steel Standard Grating
		/// </summary>

		/// <returns></returns>
		public static StandardGrating ByCS(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem, string gratingClass, string gratingName)
		{
			Autodesk.AdvanceSteel.Geometry.Plane plane = new Plane(Utils.ToAstPoint(coordinateSystem.Origin, true), Utils.ToAstVector3d(coordinateSystem.XAxis, true), Utils.ToAstVector3d(coordinateSystem.YAxis, true));
			return new StandardGrating(gratingClass, gratingName, plane, Utils.ToAstPoint(coordinateSystem.Origin, true));
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
						throw new Exception("Null Standard Grating pattern");
					}

					List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>(GratingDraw.GetPointsToDraw(grating));

					return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
					
				}
			}
		}
	}
}

