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
	/// Advance Steel Circular Anchor Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class CircularAnchorPattern : GraphicObject
	{

		internal CircularAnchorPattern(SteelGeometry.Point3d astPointRef, IEnumerable<string> handlesToConnect, 
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
						anchors = new Autodesk.AdvanceSteel.Modelling.AnchorPattern(astPointRef, vx, vy);
						anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle;
            anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;
            anchors.NumberOfScrews = anchorBoltData.XCount;
            anchors.Radius = Utils.ToInternalUnits(anchorBoltData.Radius, true);

            if (string.IsNullOrEmpty(anchorBoltData.Standard) == false) { anchors.Standard = anchorBoltData.Standard; }
            if (string.IsNullOrEmpty(anchorBoltData.AnchorBoltAssembly) == false) { anchors.BoltAssembly = anchorBoltData.AnchorBoltAssembly; }
            if (string.IsNullOrEmpty(anchorBoltData.Grade) == false) { anchors.Material = anchorBoltData.Grade; }

            if (anchorBoltData.Diameter > 0) { anchors.ScrewDiameter = Utils.ToInternalUnits(anchorBoltData.Diameter, true); }
            if (anchorBoltData.HoleTolerance > -1) { anchors.HoleTolerance = Utils.ToInternalUnits(anchorBoltData.HoleTolerance, true); }
            anchors.ScrewLength = Utils.ToInternalUnits(anchorBoltData.AnchorBoltLength, true);
            anchors.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)anchorBoltData.AnchorBoltOrientationType;
            anchors.IsInverted = anchorBoltData.AnchorBoltInverted;

            anchors.WriteToDb();
					}
					else
					{
						anchors = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

						if (anchors != null && anchors.IsKindOf(FilerObject.eObjectType.kAnchorPattern))
						{
						  anchors.ArrangerType = Autodesk.AdvanceSteel.Arrangement.Arranger.eArrangerType.kCircle;
              anchors.BoundedSide = Autodesk.AdvanceSteel.Modelling.AnchorPattern.eBoundedSide.kAll;
							anchors.RefPoint = astPointRef;
              anchors.XDirection = vx;
							anchors.YDirection = vy;
              anchors.NumberOfScrews = anchorBoltData.XCount;

              if (string.IsNullOrEmpty(anchorBoltData.Standard) == false) { anchors.Standard = anchorBoltData.Standard; }
              if (string.IsNullOrEmpty(anchorBoltData.AnchorBoltAssembly) == false) { anchors.BoltAssembly = anchorBoltData.AnchorBoltAssembly; }
              if (string.IsNullOrEmpty(anchorBoltData.Grade) == false) { anchors.Material = anchorBoltData.Grade; }

              if (anchorBoltData.Diameter > 0) { anchors.ScrewDiameter = Utils.ToInternalUnits(anchorBoltData.Diameter, true); }
              if (anchorBoltData.HoleTolerance > -1) { anchors.HoleTolerance = Utils.ToInternalUnits(anchorBoltData.HoleTolerance, true); }
              anchors.ScrewLength = Utils.ToInternalUnits(anchorBoltData.AnchorBoltLength, true);
              anchors.OrientationType = (Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType)anchorBoltData.AnchorBoltOrientationType;

              anchors.Radius = Utils.ToInternalUnits(anchorBoltData.Radius, true);
              anchors.IsInverted = anchorBoltData.AnchorBoltInverted;
            }
            else
							throw new System.Exception("Not a circular pattern");
					}

          FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
					anchors.Connect(filerObjects, (AtomicElement.eAssemblyLocation)anchorBoltData.AnchorBoltConnectionType);

					Handle = anchors.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(anchors);
				}
			}
		}

    /// <summary>
    /// Create an Advance Steel Circular Anchor Pattern By Circle
    /// </summary>
    /// <param name="circle"> Input circle</param>
    /// <param name="referenceVector"> Input Dynamo Vector for alignment of circle</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="anchorBoltData"> Input Bolt Build Properties </param>
    /// <returns></returns>
    public static CircularAnchorPattern ByCircle(DynGeometry.Circle circle,
                                                  DynGeometry.Vector referenceVector,
                                                  IEnumerable<SteelDbObject> objectsToConnect, 
                                                  PropertiesAnchorBolts anchorBoltData)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
			var vx = Utils.ToAstVector3d(referenceVector, true);
      var vy = norm.CrossProduct(vx);

      vx = vx.Normalize();
      vy = vy.Normalize();

      anchorBoltData.Radius = circle.Radius;

      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);
			return new CircularAnchorPattern(Utils.ToAstPoint(circle.CenterPoint, true), handlesList, vx, vy, anchorBoltData);
		}

    /// <summary>
    /// Create an Advance Steel Circular Anchor Pattern By Center Point and Dynamo Coordinate System
    /// </summary>
    /// <param name="point"> Input radius center point</param>
    /// <param name="anchorCS"></param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="anchorBoltData"> Input Bolt Build Properties </param>
    /// <returns></returns>
    public static CircularAnchorPattern AtCentrePoint(DynGeometry.Point point,
                                                      DynGeometry.CoordinateSystem anchorCS,
                                                      IEnumerable<SteelDbObject> objectsToConnect, 
                                                      PropertiesAnchorBolts anchorBoltData)
		{
			SteelGeometry.Point3d astPointRef = Utils.ToAstPoint(point, true);

      var vx = Utils.ToAstVector3d(anchorCS.XAxis, true);
      var vy = Utils.ToAstVector3d(anchorCS.YAxis, true);

      vx = vx.Normalize();
      vy = vy.Normalize();

      IEnumerable<string> handles = Utils.GetSteelDbObjectsToConnect(objectsToConnect);

      return new CircularAnchorPattern(astPointRef, handles, vx, vy, anchorBoltData);
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
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

					using (var point = Utils.ToDynPoint(anchorPattern.RefPoint, true))
					using (var norm = Utils.ToDynVector(anchorPattern.Normal, true))
					{
						return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalUnits(anchorPattern.Radius, true), norm);
					}
				}
			}
		}
	}
}