using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using Autodesk.AdvanceSteel.CADAccess;
using static Autodesk.AdvanceSteel.CADAccess.FilerObject;
using System;
using Autodesk.AdvanceSteel.Modelling;

namespace AdvanceSteel.Nodes
{
  //TODO: Tentar tirar os contrutores e checar se funciona

  [NodeName("StraightBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Straight Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Beams.StraightBeam")]
  public class ASPropertiesStraightBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(StraightBeam);

    public ASPropertiesStraightBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesStraightBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("BentBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Bent Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Beams.BentBeam")]
  public class ASPropertiesBentBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(BentBeam);

    public ASPropertiesBentBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesBentBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("CompoundStraightBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Compound Straight Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Beams.CompoundBeam")]
  public class ASPropertiesCompoundStraightBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(CompoundStraightBeam);

    public ASPropertiesCompoundStraightBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesCompoundStraightBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("ConcreteBentBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Concrete Bent Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Concrete.ConcreteBentBeam")]
  public class ASPropertiesConcBentBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(ConcreteBentBeam);

    public ASPropertiesConcBentBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesConcBentBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("ConcreteStraightBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Concrete Straight Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Concrete.ConcreteStraightBeam")]
  public class ASPropertiesConcStraightBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(ConcreteBeam);

    public ASPropertiesConcStraightBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesConcStraightBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("GratingProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Grating")]
  [NodeCategory("AdvanceSteel.Nodes.Grating")]
  public class ASPropertiesGrating : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(Grating);

    public ASPropertiesGrating() : base() { }

    [JsonConstructor]
    public ASPropertiesGrating(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("ConcreteIsolatedFootingProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Concrete Isolated Footing")]
  [NodeCategory("AdvanceSteel.Nodes.Concrete.Footings")]
  public class ASPropertiesIsolatedFooting : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(FootingIsolated);

    public ASPropertiesIsolatedFooting() : base() { }

    [JsonConstructor]
    public ASPropertiesIsolatedFooting(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("PlateProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Plate")]
  [NodeCategory("AdvanceSteel.Nodes.Plates")]

  public class ASPropertiesPlate : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(Plate);

    public ASPropertiesPlate() : base() { }

    [JsonConstructor]
    public ASPropertiesPlate(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("PolyBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Poly Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Beams.PolyBeam")]

  public class ASPropertiesPolyBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(PolyBeam);

    public ASPropertiesPolyBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesPolyBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("ConcreteSlabProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Concrete Slabs")]
  [NodeCategory("AdvanceSteel.Nodes.Concrete.Slabs")]

  public class ASPropertiesSlab : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(Slab);

    public ASPropertiesSlab() : base() { }

    [JsonConstructor]
    public ASPropertiesSlab(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("TaperedBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Tapered Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Beams.TaperedBeam")]

  public class ASPropertiesTaperedBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(BeamTapered);

    public ASPropertiesTaperedBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesTaperedBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("UnfoldedBeamProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Unfolded Beam")]
  [NodeCategory("AdvanceSteel.Nodes.Beams.UnFoldedBeam")]

  public class ASPropertiesUnfoldedBeam : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(UnfoldedStraightBeam);

    public ASPropertiesUnfoldedBeam() : base() { }

    [JsonConstructor]
    public ASPropertiesUnfoldedBeam(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

  [NodeName("ConcreteWallProperties")]
  [NodeDescription("Lists all the property names of an Advance Steel Concrete Wall")]
  [NodeCategory("AdvanceSteel.Nodes.Concrete.Walls")]

  public class ASPropertiesWall : ASPropertiesBase
  {
    protected override Type GetObjectType => typeof(Wall);

    public ASPropertiesWall() : base() { }

    [JsonConstructor]
    public ASPropertiesWall(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(inPorts, outPorts) { }
  }

}