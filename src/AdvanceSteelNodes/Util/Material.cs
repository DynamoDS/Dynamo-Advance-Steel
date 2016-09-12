using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;

namespace AdvanceSteel.Nodes.Util
{
	/// <summary>
	/// This node can be used to assign Material to Advance Steel elements from Dynamo
	/// </summary>
	public class Material
	{
		internal Material()
		{
		}
		/// <summary>
		/// This node can set the Material for Advance Steel elements from Dynamo
		/// </summary>
		/// <param name="element">Advance Steel element</param>
		/// <param name="materialName">Material</param>
		/// <returns></returns>
		public static void SetMaterial(AdvanceSteel.Nodes.Object element, string materialName)
		{
			//lock the document and start transaction
			using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
			{
				string handle = element.Handle;

				FilerObject obj = Utils.GetObject(handle);
				AtomicElement atomic = obj as AtomicElement;

				if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kAtomicElem))
				{
					atomic.Material = materialName;
				}

				else
					throw new System.Exception("Failed to set material");
			}
		}
	}
}