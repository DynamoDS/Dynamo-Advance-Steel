﻿using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.DocumentManagement;
using Autodesk.DesignScript.Runtime;
using Dynamo.Applications.AdvanceSteel.Services;
using System;
using Autodesk.AdvanceSteel.Geometry;

namespace AdvanceSteel.Nodes
{
	[IsVisibleInDynamoLibrary(false)]
	public static class Utils
	{
		private static readonly string separator = "#@§@#";

		public static double RadToDegree(double rad)
		{
			return 180.0 * rad / System.Math.PI;
		}
		static public Autodesk.AdvanceSteel.Geometry.Point3d ToAstPoint(Autodesk.DesignScript.Geometry.Point pt, bool bConvertToAstUnits)
		{
			double factor = 1.0;
			if (bConvertToAstUnits)
			{
				var units = AppResolver.Instance.Resolve<IAppInteraction>().DbUnits;
				factor = units.UnitOfDistance.Factor;
			}

			return new Autodesk.AdvanceSteel.Geometry.Point3d(pt.X, pt.Y, pt.Z) * factor;
		}

		static public Double ToInternalUnits(double value, bool bConvert)
		{
			double factor = 1.0;
			if (bConvert)
			{
				var units = AppResolver.Instance.Resolve<IAppInteraction>().DbUnits;
				factor = units.UnitOfDistance.Factor;
			}

			return (value * factor);
		}

		static public Double FromInternalUnits(double value, bool bConvertFromAstUnits)
		{
			double factor = 1.0;
			if(bConvertFromAstUnits)
			{
				var units = AppResolver.Instance.Resolve<IAppInteraction>().DbUnits;
				factor = units.UnitOfDistance.Factor;
			}
			return (value * (1 / factor));
		}

		static public Autodesk.DesignScript.Geometry.Point ToDynPoint(Autodesk.AdvanceSteel.Geometry.Point3d pt, bool bConvertFromAstUnits)
		{
			double factor = 1.0;
			if (bConvertFromAstUnits)
			{
				var units = AppResolver.Instance.Resolve<IAppInteraction>().DbUnits;
				factor = units.UnitOfDistance.Factor;
			}
			pt = pt * (1 / factor);
			return Autodesk.DesignScript.Geometry.Point.ByCoordinates(pt.x, pt.y, pt.z);
		}
		static public Autodesk.DesignScript.Geometry.Vector ToDynVector(Autodesk.AdvanceSteel.Geometry.Vector3d vect, bool bConvertFromAstUnits)
		{
			double factor = 1.0;
			if (bConvertFromAstUnits)
			{
				var units = AppResolver.Instance.Resolve<IAppInteraction>().DbUnits;
				factor = units.UnitOfDistance.Factor;
			}
			vect = vect * (1 / factor);
			return Autodesk.DesignScript.Geometry.Vector.ByCoordinates(vect.x, vect.y, vect.z);
		}

		static public Autodesk.AdvanceSteel.Geometry.Vector3d ToAstVector3d(Autodesk.DesignScript.Geometry.Vector v, bool bConvertToAstUnits)
		{
			double factor = 1.0;
			if (bConvertToAstUnits)
			{
				var units = AppResolver.Instance.Resolve<IAppInteraction>().DbUnits;
				factor = units.UnitOfDistance.Factor;
			}
			return new Autodesk.AdvanceSteel.Geometry.Vector3d(v.X, v.Y, v.Z) * factor;
		}

		static public Autodesk.AdvanceSteel.Geometry.Point3d[] ToAstPoints(Autodesk.DesignScript.Geometry.Point[] pts, bool bConvertToAstUnits)
		{
			if (pts == null)
				return new Autodesk.AdvanceSteel.Geometry.Point3d[0];

			Autodesk.AdvanceSteel.Geometry.Point3d[] astPts = new Autodesk.AdvanceSteel.Geometry.Point3d[pts.Length];
			for (int nIdx = 0; nIdx < pts.Length; nIdx++)
			{
				astPts[nIdx] = ToAstPoint(pts[nIdx], bConvertToAstUnits);
			}

			return astPts;
		}

		static public Autodesk.DesignScript.Geometry.Point[] ToDynPoints(Autodesk.AdvanceSteel.Geometry.Point3d[] astPts, bool bConvertToAstUnits)
		{
			if (astPts == null)
				return new Autodesk.DesignScript.Geometry.Point[0];

			Autodesk.DesignScript.Geometry.Point[] pts = new Autodesk.DesignScript.Geometry.Point[astPts.Length];
			for (int nIdx = 0; nIdx < pts.Length; nIdx++)
			{
				pts[nIdx] = ToDynPoint(astPts[nIdx], bConvertToAstUnits);
			}

			return pts;
		}

		static public FilerObject GetObject(string handle)
		{
			return FilerObject.GetFilerObjectByHandle(handle);
		}

		internal static void SetOrientation(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Vector3d vOrientation)
		{
			beam.PhysCSStart.GetCoordSystem(out _, out Vector3d vXAxis, out Vector3d vYAxis, out Vector3d vZAxis);
			if (!vXAxis.IsParallelTo(vOrientation))
			{
				Vector3d vProj = vOrientation.OrthoProject(vXAxis);

				double dAngle = vZAxis.GetAngleTo(vProj, vXAxis);

				beam.SetXRotation(dAngle * 180 / Math.PI);
			}
		}

		internal static void AdjustBeamEnd(Autodesk.AdvanceSteel.Modelling.Beam beam, Autodesk.AdvanceSteel.Geometry.Point3d newPtStart)
		{
			Autodesk.AdvanceSteel.Geometry.Point3d beamPtStart = beam.GetPointAtStart();
			Autodesk.AdvanceSteel.Geometry.Point3d beamPtEnd = beam.GetPointAtEnd();

			if (beamPtEnd.IsEqualTo(newPtStart))
			{
				Autodesk.AdvanceSteel.Geometry.Point3d newBeamEnd = beamPtEnd + (beamPtEnd - beamPtStart) * 0.5;
				beam.SetSysEnd(newBeamEnd);
			}
		}

		internal static string Separator
		{
			get { return separator; }
		}

		internal static string[] SplitSectionName(string sectionName)
		{
			string[] result = sectionName.Split(new string[] { Separator }, System.StringSplitOptions.None);

			if (2 == result.Length)
			{
				return result;
			}
			else
			{
				throw new System.Exception("Invalid section name");
			}
		}

		internal static bool CompareCompoundSectionTypes(string first, string second)
		{
			if (first.Equals(second) || (first.Contains("Welded") && second.Contains("Welded")) || (first.Contains("Compound") && second.Contains("Compound")) || (first.Contains("Tapered") && second.Contains("Tapered")))
			{
				return true;
			}
			  return false;
		}
	}
}