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

namespace AdvanceSteel.Nodes.ConnectionObjects
{
  [IsVisibleInDynamoLibrary(false)]
  public class ObjectsConnection
  {
    public static FilerObject[] GetSteelObjectsToConnect(IEnumerable<string> handlesToConnect)
		{
			var ret = new List<FilerObject>();
      bool Objs = false;
			foreach (var objHandle in handlesToConnect)
			{
				FilerObject obj = Utils.GetObject(objHandle);
				if (obj != null && (obj.IsKindOf(FilerObject.eObjectType.kBeam) ||
														obj.IsKindOf(FilerObject.eObjectType.kBentBeam) ||
														obj.IsKindOf(FilerObject.eObjectType.kPlate)))
				{
          Objs = true;
          ret.Add(obj);
				}
				else
				{
					throw new System.Exception("Object is empty");
				}
			}
			return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
		}
		
		public static FilerObject[] GetFilerObjects(IEnumerable<string> handles)
		{
			var ret = new List<FilerObject>();
      bool Objs = false;
			foreach (var objHandle in handles)
			{
        Objs = true;
				ret.Add(Utils.GetObject(objHandle));
			}

			return Objs ? ret.ToArray() : Array.Empty<FilerObject>();
		}
		
		public static List<string> GetSteelDbObjectsToConnect(IEnumerable<SteelDbObject> objectsToConnect)
		{
			List<string> handlesList = new List<string>();
			foreach (var obj in objectsToConnect)
			{
				if (obj is AdvanceSteel.Nodes.Beams.BentBeam ||
				    obj is AdvanceSteel.Nodes.Beams.StraightBeam ||
				    obj is AdvanceSteel.Nodes.Plates.Plate)
				{
					handlesList.Add(obj.Handle);
				}
				else
				{
					throw new Exception("Only beams and plates can be connected");
				}
			}
			return handlesList;
		}
		
		/// <summary>
		/// Set Assembly Location 
		/// </summary>
		/// <param name="location">Input assembly location</param>
		/// <param name="aux">Input aux </param>
		/// <returns></returns>
		public static void SetAssemblyLocation(Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern aux, int location)
		{
			var temp = Utils.GetObject(aux.Handle) as Autodesk.AdvanceSteel.Modelling.ScrewBoltPattern;
			if (temp.IsKindOf(FilerObject.eObjectType.kCircleScrewBoltPattern) || temp.IsKindOf(FilerObject.eObjectType.kFinitRectScrewBoltPattern))
			{
				temp.AssemblyLocation = GetSteelAssemblyLocation(location);
			}
		}
		/// <summary>
		/// Get Assembly Location From Dynamo to Advance Steel
		/// </summary>
		public static AtomicElement.eAssemblyLocation GetSteelAssemblyLocation(int assemblyLocation)
		{
			if (assemblyLocation == (int)AtomicElement.eAssemblyLocation.kWrong)
				return AtomicElement.eAssemblyLocation.kWrong;
			else if (assemblyLocation == (int)AtomicElement.eAssemblyLocation.kUnknown)
				return AtomicElement.eAssemblyLocation.kUnknown;
			else if (assemblyLocation == (int)AtomicElement.eAssemblyLocation.kOnSite)
				return AtomicElement.eAssemblyLocation.kOnSite;
			else if (assemblyLocation == (int)AtomicElement.eAssemblyLocation.kSiteDrill)
				return AtomicElement.eAssemblyLocation.kSiteDrill;
			else
				return AtomicElement.eAssemblyLocation.kInShop;
		}
		/// <summary>
		/// Get Assembly Location From Advance Steel to Dynamo
		/// </summary>
		public static int GetDynAssemblyLocation(AtomicElement.eAssemblyLocation assemblyLocation)
		{
			if (assemblyLocation == AtomicElement.eAssemblyLocation.kWrong)
				return (int)AtomicElement.eAssemblyLocation.kWrong;
			else if (assemblyLocation == AtomicElement.eAssemblyLocation.kUnknown)
				return (int)AtomicElement.eAssemblyLocation.kUnknown;
			else if (assemblyLocation == AtomicElement.eAssemblyLocation.kOnSite)
				return (int)AtomicElement.eAssemblyLocation.kOnSite;
			else if (assemblyLocation == AtomicElement.eAssemblyLocation.kSiteDrill)
				return (int)AtomicElement.eAssemblyLocation.kSiteDrill;
			else
				return (int)AtomicElement.eAssemblyLocation.kInShop;
		}
		// <summary>
		// Get Orientation Type From Dynamo to Advance Steel
		// </summary>
		//public static Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType GetSteelOrientationType(AnchorOrientationTypes.OrientationType orientationType)
		//{
		//	if (orientationType == AnchorOrientationTypes.OrientationType.kNormalOrientation)
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kNormalOrientation;
		//	else if (orientationType == AnchorOrientationTypes.OrientationType.kDiagonalInside)
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kDiagonalInside;
		//	else if (orientationType == AnchorOrientationTypes.OrientationType.kDiagonalOutside)
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kDiagonalOutside;
		//	else if (orientationType == AnchorOrientationTypes.OrientationType.kAllOutside)
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kAllOutside;
		//	else if (orientationType == AnchorOrientationTypes.OrientationType.kAllInside)
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kAllInside;
		//	else if (orientationType == AnchorOrientationTypes.OrientationType.kInsideRotated)
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kInsideRotated;
		//	else
		//		return Autodesk.AdvanceSteel.Modelling.AnchorPattern.eOrientationType.kOutsideRotated;

		//}

		// <summary>
		// Set anchors Orientation for Rectangular Anchor Pattern
		// </summary>
		//public static void SetRectangularAnchorPatternOrientation(AdvanceSteel.Nodes.ConnectionObjects.RectangularAnchorPattern element, AnchorOrientationTypes.OrientationType orientationType)
		//{
		//	//lock the document and start transaction
		//	using (var ctx = new SteelServices.DocContext())
		//	{
		//		string handle = element.Handle;
		//		var anchors = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

		//		if (handle != null)
		//		{
		//			anchors.OrientationType = GetSteelOrientationType(orientationType);
				
		//		}
		//		else
		//			throw new System.Exception("Failed to set anchor type");
		//	}
		//}

		// <summary>
		// Set anchors Orientation for Circular Anchor Pattern
		// </summary>
		//public static void SetRectangularAnchorPatternOrientation(AdvanceSteel.Nodes.ConnectionObjects.CircularAnchorPattern element, AnchorOrientationTypes.OrientationType orientationType)
		//{
		//	//lock the document and start transaction
		//	using (var ctx = new SteelServices.DocContext())
		//	{
		//		string handle = element.Handle;
		//		var anchors = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.AnchorPattern;

		//		if (handle != null)
		//		{
		//			anchors.OrientationType = GetSteelOrientationType(orientationType);

		//		}
		//		else
		//			throw new System.Exception("Failed to set anchor type");
		//	}
		//}

		/// <summary>
		/// Functions to draw a rectangle OXYZ
		/// </summary>
		/// <returns></returns>
		public static double GetDiagonalLength(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2)
		{
			return (point2 - point1).GetLength();
		}
		public static double GetRectangleAngle(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, SteelGeometry.Vector3d vx)
		{
			return (point2 - point1).GetAngleTo(vx);
		}
		public static double GetRectangleLength(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, SteelGeometry.Vector3d vx)
		{
			var diagLen = GetDiagonalLength(point1, point2);
			var alpha = GetRectangleAngle(point1, point2, vx);

			return diagLen * Math.Cos(alpha);
		}
		public static double GetRectangleHeight(SteelGeometry.Point3d point1, SteelGeometry.Point3d point2, SteelGeometry.Vector3d vx)
		{
			var diagLen = GetDiagonalLength(point1, point2);
			var alpha = GetRectangleAngle(point1, point2, vx);

			return diagLen * Math.Sin(alpha);
		}

	}
}



