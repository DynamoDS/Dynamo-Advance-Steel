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

namespace AdvanceSteel.Nodes.ConnectionObjects
{
	/// <summary>
	/// Advance Steel Connection
	/// </summary>
	[DynamoServices.RegisterForTrace]
	public class Connection: GraphicObject
	{
		internal Connection(SteelGeometry.Point3d[] astPoints, IEnumerable<string> handlesToConnect, string internalName)
		{
			lock(access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					Autodesk.AdvanceSteel.ConstructionTypes.UserAutoConstructionObject connection = null;
					string handle = SteelServices.ElementBinder.GetHandleFromTrace();

					FilerObject[] filerObjects = ObjectsConnection.GetFilerObjects(handlesToConnect).ToArray();
					ActiveConstructionElement[] elements = filerObjects.Where(fo => fo is ActiveConstructionElement).Cast<ActiveConstructionElement>().ToArray();

					/*
					if (elements[0].NumberOfDrivenConObj > 0)
					{
						PassiveConstructionObject conn = elements[0].GetDrivenConObj(0);
						// check if it is a joint
						var connectedObjects = conn.Drivens;

						var beamId = elements[0].GetObjectId();
					}
					*/
					var beamDrivensList = new List<Tuple<string, string>>();
					for (int i = 0; i < elements.Length; i++)
					{
						if (elements[i].NumberOfDrivenConObj > 0)
						{
							for (int j = 0; j < elements[i].NumberOfDrivenConObj; j++)
							{
								PassiveConstructionObject conn = elements[i].GetDrivenConObj(j);
								if (conn is Autodesk.AdvanceSteel.ConstructionTypes.UserAutoConstructionObject)
								{
									foreach (var drivensObj in conn.Drivens)
									{
										var beamId = elements[i].GetObjectId();
										beamDrivensList.Add(new Tuple<string, string>(beamId.Handle, drivensObj.Handle));
									}
								}
							}
						}
					}

					// check if it is a connection already between two objects (beam, plates, etc...)
					foreach(Tuple<string, string> tuple1 in beamDrivensList)
						foreach(Tuple<string, string> tuple2 in beamDrivensList)
						{
							if((tuple1.Item1 != tuple2.Item1) && (tuple1.Item2 == tuple2.Item2))
							{

							}

						}

					if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
					{
						if (filerObjects.Length != astPoints.Length)
							throw new System.Exception("Numbers of points should match the number of objects");

						var tupleList = new List<Tuple<FilerObject, Point3d>>();
						for(int i = 0; i < astPoints.Length; i++)
						{
							tupleList.Add(new Tuple<FilerObject, Point3d>(filerObjects[i], astPoints[i]));
						}
						connection = new Autodesk.AdvanceSteel.ConstructionTypes.UserAutoConstructionObject(internalName, tupleList);
						connection.WriteToDb();
					}
					else
					{
						connection = Utils.GetObject(handle) as Autodesk.AdvanceSteel.ConstructionTypes.UserAutoConstructionObject;
						if (connection != null && connection.IsKindOf(FilerObject.eObjectType.kAutoConstructionObject))
						{
							connection.NSAModuleInternalName = internalName;
							connection.AutoUpdate = true;
						}
						else
						{
							throw new System.Exception("Not a Bar Grating pattern");
						}
					}
					Handle = connection.Handle;
					SteelServices.ElementBinder.CleanupAndSetElementForTrace(connection);
				}
			}
		}
		/// <summary>
		/// Create an Advance Steel AutoConnection Pattern
		/// </summary>
		/// <param name="objectsToConnect">Input objects to be connected</param>
		/// <param name="internalName">Input internal name</param>
		/// <param name="points">Input list of points </param>
		/// <returns></returns>
		public static Connection ByInternalName(IEnumerable<SteelDbObject> objectsToConnect, string internalName, List<DynGeometry.Point> points)
		{
			var astPoints = Utils.ToAstPoints(points.ToArray(), true);
			IEnumerable<string> handles = objectsToConnect.Select(obj => obj.Handle);

			return new Connection(astPoints, handles, internalName);
		}

		[IsVisibleInDynamoLibrary(false)]
		public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
		{
			lock (access_obj)
			{
				using (var ctx = new SteelServices.DocContext())
				{
					var constructionPattern = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.ConstructionTypes.UserAutoConstructionObject;
					if (constructionPattern == null)
					{
						throw new Exception("Null construction pattern");
					}
				
					return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadius(Utils.ToDynPoint(constructionPattern.CenterPoint, true), 0.1);
				}
			}
		}

	}
}
