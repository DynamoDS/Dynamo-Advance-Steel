using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvanceSteel.Nodes
{
    /// <summary>
    /// this represent the Advance Steel Objects as graphical objects
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public abstract class GraphicObject : Object, IGraphicItem
    {
        public abstract Autodesk.DesignScript.Geometry.Curve Curve
        {
            get;
        }

        public new void Tessellate(IRenderPackage package, double tol, int gridLines)
        {
            this.Curve.Tessellate(package, tol);
        }
    }
}
