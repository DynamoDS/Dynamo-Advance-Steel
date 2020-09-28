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
using System;

namespace AdvanceSteel.Nodes.ConnectionObjects.Anchors
{
	/// <summary>
	/// Advance Steel Rectangular Anchor Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class RectangularAnchorPattern : GraphicObject
	{

    internal RectangularAnchorPattern(SteelGeometry.Point3d anchorBoltPatternInsertPoint, IEnumerable<string> handlesToConnect,
                                      SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
                                      PropertiesAnchorBolts anchorBoltData)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          Autodesk.AdvanceSteel.Modelling.AnchorPattern anchors = null;
          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            anchors = new Autodesk.AdvanceSteel.Modelling.AnchorPattern(anchorBoltPatternInsertPoint, vx, vy);
            anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular;
            anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;

            if (string.IsNullOrEmpty(anchorBoltData.Standard) == false) { anchors.Standard = anchorBoltData.Standard; }
            if (string.IsNullOrEmpty(anchorBoltData.AnchorBoltAssembly) == false) { anchors.BoltAssembly = anchorBoltData.AnchorBoltAssembly; }
            if (string.IsNullOrEmpty(anchorBoltData.Grade) == false) { anchors.Material = anchorBoltData.Grade; }
            anchors.ScrewLength = anchorBoltData.AnchorBoltLength;
            anchors.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)anchorBoltData.AnchorBoltOrientationType;

            if (anchorBoltData.Diameter > 0) { anchors.ScrewDiameter = anchorBoltData.Diameter; }
            if (anchorBoltData.HoleTolerance > -1) { anchors.HoleTolerance = anchorBoltData.HoleTolerance; }
            if (anchorBoltData.XCount > -1) { anchors.Nx = anchorBoltData.XCount; }
            if (anchorBoltData.YCount > -1) { anchors.Ny = anchorBoltData.YCount; }
            if (anchorBoltData.XSpacing > 0) { anchors.Dx = anchorBoltData.XSpacing; }
            if (anchorBoltData.YSpacing > 0) { anchors.Dy = anchorBoltData.YSpacing; }

            anchors.RefPoint = anchorBoltPatternInsertPoint;
            anchors.IsInverted = anchorBoltData.AnchorBoltInverted;
            anchors.WriteToDb();
          }
          else
          {
            anchors = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

            if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
            {
              anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kRectangular;
              anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;

              if (string.IsNullOrEmpty(anchorBoltData.Standard) == false) { anchors.Standard = anchorBoltData.Standard; }
              if (string.IsNullOrEmpty(anchorBoltData.AnchorBoltAssembly) == false) { anchors.BoltAssembly = anchorBoltData.AnchorBoltAssembly; }
              if (string.IsNullOrEmpty(anchorBoltData.Grade) == false) { anchors.Material = anchorBoltData.Grade; }
              anchors.ScrewLength = anchorBoltData.AnchorBoltLength;
              anchors.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)anchorBoltData.AnchorBoltOrientationType;

              if (anchorBoltData.Diameter > 0) { anchors.ScrewDiameter = anchorBoltData.Diameter; }
              if (anchorBoltData.HoleTolerance > -1) { anchors.HoleTolerance = anchorBoltData.HoleTolerance; }
              if (anchorBoltData.XCount > -1) { anchors.Nx = anchorBoltData.XCount; }
              if (anchorBoltData.YCount > -1) { anchors.Ny = anchorBoltData.YCount; }
              if (anchorBoltData.XSpacing > 0) { anchors.Dx = anchorBoltData.XSpacing; }
              if (anchorBoltData.YSpacing > 0) { anchors.Dy = anchorBoltData.YSpacing; }

              anchors.RefPoint = anchorBoltPatternInsertPoint;
              anchors.IsInverted = anchorBoltData.AnchorBoltInverted;
            }
            else
              throw new System.Exception("Not an anchor pattern");
          }

          FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
          anchors.Connect(filerObjects, (AtomicElement.eAssemblyLocation)anchorBoltData.AnchorBoltConnectionType);

          Handle = anchors.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
        }
      }
    }

    internal RectangularAnchorPattern(SteelGeometry.Point3d astPoint1, SteelGeometry.Point3d astPoint2, IEnumerable<string> handlesToConnect, 
                                      SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy,
																			PropertiesAnchorBolts anchorBoltData)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.AnchorPattern anchors = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					var astPointRef = astPoint1 + (astPoint2 - astPoint1) * 0.5;

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						anchors = new Autodesk.AdvanceSteel.Modelling.AnchorPattern(astPoint1, astPoint2, vx, vy);
            anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kBounded;
            anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;

            if (string.IsNullOrEmpty(anchorBoltData.Standard) == false) { anchors.Standard = anchorBoltData.Standard; }
            if (string.IsNullOrEmpty(anchorBoltData.AnchorBoltAssembly) == false) { anchors.BoltAssembly = anchorBoltData.AnchorBoltAssembly; }
            if (string.IsNullOrEmpty(anchorBoltData.Grade) == false) { anchors.Material = anchorBoltData.Grade; }
            anchors.ScrewLength = anchorBoltData.AnchorBoltLength;
            anchors.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)anchorBoltData.AnchorBoltOrientationType;

            if (anchorBoltData.Diameter > 0) { anchors.ScrewDiameter = anchorBoltData.Diameter; }
            if (anchorBoltData.HoleTolerance > -1) { anchors.HoleTolerance = anchorBoltData.HoleTolerance; }
            if (anchorBoltData.XCount > -1) { anchors.Nx = anchorBoltData.XCount; }
            if (anchorBoltData.YCount > -1) { anchors.Ny = anchorBoltData.YCount; }
            if (anchorBoltData.XSpacing > 0) { anchors.Dx = anchorBoltData.XSpacing; }
            if (anchorBoltData.YSpacing > 0) { anchors.Dy = anchorBoltData.YSpacing; }

            anchors.RefPoint = astPoint1 + (astPoint2 - astPoint1) * 0.5;

            double aLengthX = Utils.GetRectangleLength(astPoint1, astPoint2, vx);// / (anchors.Nx - 1);
            double aLengthY = Utils.GetRectangleLength(astPoint1, astPoint2, vy);// / (anchors.Ny - 1);
            anchors.SetArrangerLength(aLengthX, 0);
            anchors.SetArrangerLength(aLengthY, 1);
            anchors.IsInverted = anchorBoltData.AnchorBoltInverted;

            anchors.WriteToDb();
					}
					else
					{
						anchors = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

						if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
						{
              anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kBounded;
              anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;

              if (string.IsNullOrEmpty(anchorBoltData.Standard) == false) { anchors.Standard = anchorBoltData.Standard; }
              if (string.IsNullOrEmpty(anchorBoltData.AnchorBoltAssembly) == false) { anchors.BoltAssembly = anchorBoltData.AnchorBoltAssembly; }
              if (string.IsNullOrEmpty(anchorBoltData.Grade) == false) { anchors.Material = anchorBoltData.Grade; }
              anchors.ScrewLength = anchorBoltData.AnchorBoltLength;
              anchors.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)anchorBoltData.AnchorBoltOrientationType;

              if (anchorBoltData.Diameter > 0) { anchors.ScrewDiameter = anchorBoltData.Diameter; }
              if (anchorBoltData.HoleTolerance > -1) { anchors.HoleTolerance = anchorBoltData.HoleTolerance; }
              if (anchorBoltData.XCount > -1) { anchors.Nx = anchorBoltData.XCount; }
              if (anchorBoltData.YCount > -1) { anchors.Ny = anchorBoltData.YCount; }
              if (anchorBoltData.XSpacing > 0) { anchors.Dx = anchorBoltData.XSpacing; }
              if (anchorBoltData.YSpacing > 0) { anchors.Dy = anchorBoltData.YSpacing; }

              anchors.RefPoint = astPoint1 + (astPoint2 - astPoint1) * 0.5;

              double aLengthX = Utils.GetRectangleLength(astPoint1, astPoint2, vx);
              double aLengthY = Utils.GetRectangleLength(astPoint1, astPoint2, vy);
              anchors.SetArrangerLength(aLengthX , 0);
              anchors.SetArrangerLength(aLengthY, 1);
              anchors.IsInverted = anchorBoltData.AnchorBoltInverted;

            }
            else
							throw new System.Exception("Not an anchor pattern");
					}

          FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
					anchors.Connect(filerObjects, (AtomicElement.eAssemblyLocation)anchorBoltData.AnchorBoltConnectionType);

					Handle = anchors.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
				}
			}
		}

    /// <summary>
    /// Create an Advance Steel Rectangular Anchor Pattern bound By Rectangle
    /// </summary>
    /// <param name="rectangle"> Input rectangle </param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="anchorBoltData"> Input Anchor Bolt Build Properties </param>
    public static RectangularAnchorPattern ByRectangle(Autodesk.DesignScript.Geometry.Rectangle rectangle, 
                                                      IEnumerable<SteelDbObject> objectsToConnect,
																											PropertiesAnchorBolts anchorBoltData)
		{
			List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

			var dynCorners = rectangle.Corners();
			var astCorners = Utils.ToAstPoints(dynCorners, true);
			var vx = astCorners[1] - astCorners[0];
			var vy = astCorners[3] - astCorners[0];

			return new RectangularAnchorPattern(astCorners[0], astCorners[2], handlesList, vx, vy, anchorBoltData);
		}

    /// <summary>
    /// Create an Advance Steel unbounded Rectangular Pattern at a Point
    /// </summary>
    /// <param name="connectionPoint"> Input Insertion point of Anchor Bolt Pattern </param>
    /// <param name="boltCS"> Input Anchor Bolt Coordinate System </param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="anchorBoltData"> Input Anchor Bolt Build Properties </param>
    /// <returns></returns>
    public static RectangularAnchorPattern AtCentrePoint(Autodesk.DesignScript.Geometry.Point connectionPoint,
                                                  Autodesk.DesignScript.Geometry.CoordinateSystem boltCS,
                                                  IEnumerable<SteelDbObject> objectsToConnect,
                                                  PropertiesAnchorBolts anchorBoltData)
    {
      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      var vx = Utils.ToAstVector3d(boltCS.XAxis, true);
      var vy = Utils.ToAstVector3d(boltCS.YAxis, true);

      return new RectangularAnchorPattern(Utils.ToAstPoint(connectionPoint, true), handlesList, vx, vy, anchorBoltData);
    }

    [IsVisibleInDynamoLibrary(false)]
		public override DynGeometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var anchorPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

					if (anchorPattern == null)
					{
						throw new Exception("Null anchor pattern");
					}

					var temp1 = anchorPattern.XDirection * anchorPattern.Dx / 2.0;
					var temp2 = anchorPattern.YDirection * anchorPattern.Dy / 2.0;

					var pt1 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt1.Add(temp1 + temp2);
					
					var pt2 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt2.Add(temp1 - temp2);

					var pt3 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt3.Add(-temp1 - temp2);

					var pt4 = new SteelGeometry.Point3d(anchorPattern.RefPoint);
					pt4.Add(-temp1 + temp2);

					{
						List<DynGeometry.Point> polyPoints = new List<DynGeometry.Point>
						{
							Utils.ToDynPoint(pt1, true),
							Utils.ToDynPoint(pt2, true),
							Utils.ToDynPoint(pt3, true),
							Utils.ToDynPoint(pt4, true)
						};

						return Autodesk.DesignScript.Geometry.Polygon.ByPoints(polyPoints);
					}
				}
			}
		}

	}
}

