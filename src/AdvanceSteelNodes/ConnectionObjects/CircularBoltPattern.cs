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

namespace AdvanceSteel.Nodes.ConnectionObjects.Bolts
{
	/// <summary>
	/// Advance Steel Circular Bolt Pattern
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class CircularBoltPattern : GraphicObject
	{

		internal CircularBoltPattern(SteelGeometry.Point3d holeInsertPoint, IEnumerable<string> handlesToConnect, 
                                  SteelGeometry.Vector3d vx, SteelGeometry.Vector3d vy, 
                                  PropertiesBolts boltData)
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern bolts = null;

					string handle = SteelServices.ElementBinder.GetHandleFromTrace();
					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						bolts = new Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern(holeInsertPoint, vx, vy);
            bolts.NumberOfScrews = boltData.XCount;
            bolts.Radius = Utils.ToInternalUnits(boltData.Radius, true);
            bolts.BindingLengthAddition = boltData.LengthAddition;

            if (string.IsNullOrEmpty(boltData.Standard) == false) { bolts.Standard = boltData.Standard; }
            if (string.IsNullOrEmpty(boltData.BoltAssembly) == false) { bolts.BoltAssembly = boltData.BoltAssembly; }
            if (string.IsNullOrEmpty(boltData.Grade) == false) { bolts.Material = boltData.Grade; }

            if (boltData.Diameter > 0) { bolts.ScrewDiameter = boltData.Diameter; }
            if (boltData.HoleTolerance > -1) { bolts.HoleTolerance = boltData.HoleTolerance; }
            bolts.IsInverted = boltData.BoltInverted;

            bolts.WriteToDb();
					}
					else
					{
						bolts = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern;

						if (bolts != null && bolts.IsKindOf(FilerObject.eObjectType.kCircleScrewBoltPattern))
						{
							bolts.RefPoint = holeInsertPoint;
							bolts.XDirection = vx;
							bolts.YDirection = vy;
              bolts.NumberOfScrews = boltData.XCount;
              bolts.BindingLengthAddition = boltData.LengthAddition;

              if (string.IsNullOrEmpty(boltData.Standard) == false) { bolts.Standard = boltData.Standard; }
              if (string.IsNullOrEmpty(boltData.BoltAssembly) == false) { bolts.BoltAssembly = boltData.BoltAssembly; }
              if (string.IsNullOrEmpty(boltData.Grade) == false) { bolts.Material = boltData.Grade; }

              if (boltData.Diameter > 0) { bolts.ScrewDiameter = boltData.Diameter; }
              if (boltData.HoleTolerance > -1) { bolts.HoleTolerance = boltData.HoleTolerance; }

              bolts.Radius = Utils.ToInternalUnits(boltData.Radius, true);
              bolts.IsInverted = boltData.BoltInverted;
            }
						else
							throw new System.Exception("Not a circular pattern");
					}

          FilerObject[] filerObjects = Utils.GetFilerObjects(handlesToConnect);
					bolts.Connect(filerObjects, (AtomicElement.eAssemblyLocation)boltData.BoltConnectionType);

					Handle = bolts.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(bolts);
				}
			}
		}

    /// <summary>
    /// Create an Advance Steel Circular Bolt Pattern By Circle
    /// </summary>
    /// <param name="circle"> Input circle</param>
    /// <param name="referenceVector"> Input Dynamo Vector for alignment of circle</param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="boltData"> Input Bolt Build Properties </param>
    /// <returns></returns>
    public static CircularBoltPattern ByCircle(DynGeometry.Circle circle,
                                                DynGeometry.Vector referenceVector,
                                                IEnumerable<SteelDbObject> objectsToConnect,
                                                PropertiesBolts boltData)
		{
			var norm = Utils.ToAstVector3d(circle.Normal, true);
			var vx = Utils.ToAstVector3d(referenceVector, true);
      var vy = norm.CrossProduct(vx);

      vx = vx.Normalize();
      vy = vy.Normalize();

      boltData.Radius = circle.Radius;

      List<string> handlesList = Utils.GetSteelDbObjectsToConnect(objectsToConnect);
			return new CircularBoltPattern(Utils.ToAstPoint(circle.CenterPoint, true), handlesList, vx, vy, boltData);
		}

    /// <summary>
    /// Create an Advance Steel Circular Bolt Pattern By Center Point Radius Normal
    /// </summary>
    /// <param name="point"> Input radius center point</param>
    /// <param name="boltCS"> Input Coordinate System </param>
    /// <param name="objectsToConnect"> Input Objects to be bolted </param>
    /// <param name="boltData"> Input Bolt Build Properties </param>
    /// <returns></returns>
    public static CircularBoltPattern AtCentrePoint(DynGeometry.Point point,
                                                    DynGeometry.CoordinateSystem boltCS,
                                                    IEnumerable<SteelDbObject> objectsToConnect, 
                                                    PropertiesBolts boltData)
		{
			SteelGeometry.Point3d astPointRef = Utils.ToAstPoint(point, true);

      var vx = Utils.ToAstVector3d(boltCS.XAxis, true);
      var vy = Utils.ToAstVector3d(boltCS.YAxis, true);

      vx = vx.Normalize();
      vy = vy.Normalize();

      IEnumerable<string> handles = Utils.GetSteelDbObjectsToConnect(objectsToConnect); 

			return new CircularBoltPattern(Utils.ToAstPoint(point, true), handles, vx, vy, boltData );
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var boltPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.CircleScrewBoltPattern;
					if (boltPattern == null)
					{
						throw new Exception("Null bolt pattern");
					}
						
					using (var point = Utils.ToDynPoint(boltPattern.RefPoint, true))
					using (var norm = Utils.ToDynVector(boltPattern.Normal, true))
					{
						return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadiusNormal(point, Utils.FromInternalUnits(boltPattern.Radius, true), norm);
					}
				}
			}
		}
	}
}