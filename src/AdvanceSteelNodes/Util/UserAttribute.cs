using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;

namespace AdvanceSteel.Nodes.Util
{
	/// <summary>
	/// Custom User attributes for Advance Steel elements
	/// </summary>
	public class UserAttribute
	{
		internal UserAttribute()
		{
		}
		/// <summary>
		/// This node can set User attributes for Advance Steel elements from Dynamo
		/// </summary>
		/// <param name="element">Advance Steel element</param>
		/// <param name="AttIdx">The index of the User attribute. Is a number between 1 and 10</param>
		/// <param name="value">Attribute value</param>
		/// <returns></returns>
		public static void SetUserAttribute(AdvanceSteel.Nodes.Object element, int AttIdx, string value)
		{
			if (AttIdx < 1 || AttIdx > 10)
				throw new System.Exception("Attribute index is not in the range from 1 to 10");

			//lock the document and start transaction
			using (var _CADAccess = new AdvanceSteel.Services.ObjectAccess.CADContext())
			{
				string handle = element.Handle;

				FilerObject obj = Utils.GetObject(handle);
				AtomicElement atomic = obj as AtomicElement;

				if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kAtomicElem))
				{
					//[1, 10] ->[0 ,9]
					AttIdx = AttIdx - 1;

					atomic.SetUserAttribute(AttIdx, value);
				}
				else
					throw new System.Exception("Failed to set attribute");
			}
		}
	}
}