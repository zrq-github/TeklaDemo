using System.IO;
using CsvHelper;
using netDxf;
using netDxf.Entities;
using Tekla.Structures.Model;
using TeklaTestDrawApp;
using TeklaUtils.Extend;
using Mesh = Tekla.Structures.Model.UI.Mesh;

namespace TeklaTestApplication;

public class TeklaExportDxf
{
    private readonly DxfTool _dxfTool;
    private readonly TSM.Model _model;

    public TeklaExportDxf(TSM.Model model)
    {
        _model = model;
        var modelPath = _model.GetInfo().ModelPath;

        var dxfFile = Path.Combine(modelPath, "TeklaExportDxf.dxf");
        DxfTool dxfTool = new(dxfFile, false);
        _dxfTool = dxfTool;
    }

    public void Export(List<TSM.ModelObject> modelObjects)
    {
        foreach (var modelObject in modelObjects)
        {
            switch (modelObject)
            {
                case TSM.Part part:
                    WritePart(part);
                    break;
            }

            //if (modelObject is TSM.Assembly assembly)
            //{
            //    WriteGeometry(assembly);
            //    continue;
            //}

            //TSM.Connection connection = null;
        }

        _dxfTool.Save();
    }

    private void WritePart(TSM.Part part)
    {
        var assembly = part.GetAssembly();
        WriteGeometry(assembly); // write face geometry of assembly part solids

        //WriteEmbeds(assembly);          // write subassemblies
        WriteHoles(assembly); // write holes and bolts
    }

    private void WriteHoles(TSM.Assembly piece)
    {
        var mainPart = piece.GetMainPart() as TSM.Part;
        var alist = piece.GetSecondaries();
        WriteHoles(mainPart);
        WriteBolts(mainPart);
        foreach (ModelObject modelObject in alist)
        {
            var part = modelObject as Part;
            if (part != null)
            {
                WriteHoles(part);
                WriteBolts(part);
            }
        }
    }

    private void WriteBolts(TSM.Part part)
    {
        var myBoltEnum = part.GetBolts();
        while (myBoltEnum.MoveNext())
        {
            var myBolt = myBoltEnum.Current as TSM.BoltArray;

            WriteGeometry(myBolt);


            //planeHandler.SetCurrentTransformationPlane(BoltPlane);
            //planeHandler.SetCurrentTransformationPlane(CurrentPlane);
        }
    }

    private void WriteGeometry(TSM.BoltGroup boltGroup)
    {
        var solid = boltGroup.GetSolid();
        var faces = solid.GetFaceEnumerator();
        while (faces.MoveNext())
        {
            var face = faces.Current;
            if (null == face)
                continue;

            var loops = face.GetLoopEnumerator();
            while (loops.MoveNext())
            {
                var loop = loops.Current;
                if (null == loop) continue;

                var vertexEnumerator = loop.GetVertexEnumerator();
                var points = vertexEnumerator.ToList();
                points.Add(points.First()); // 形成一个回环

                var polyline3D = new netDxf.Entities.Polyline3D();
                var vertexes = points.ToList().ConvertAll(ToDxf);
                polyline3D.Vertexes.AddRange(vertexes);

                _dxfTool.AddPolyLine(polyline3D);
            }
        }
    }

    private void WriteHoles(TSM.Part part)
    {
        //if (part != null)
        //{
        //    ModelObjectEnumerator myHoleEnum = part.GetChildren();
        //    while (myHoleEnum.MoveNext())
        //    {
        //        BooleanPart myHole = myHoleEnum.Current as BooleanPart;
        //        if (myHole != null)
        //        {
        //            double Width = 0;
        //            double Height = 0;

        //            myHole.GetReportProperty("WIDTH", ref Width);
        //            myHole.GetReportProperty("HEIGHT", ref Height);

        //            writer.WriteStartElement("hole", null);
        //            writer.WriteAttributeString("id", myHole.Identifier.ID.ToString());
        //            writer.WriteElementString("width", Width.ToString("######."));
        //            writer.WriteElementString("height", Height.ToString("######."));
        //            writer.WriteEndElement();
        //        }
        //    }
        //}
    }

    private void WriteGeometry(in TSM.Assembly assembly)
    {
        // get main part
        var mainPart = assembly.GetMainPart() as TSM.Part;
        GetGeometry(mainPart);

        // get secondaries
        var secondaries = assembly.GetSecondaries();
        foreach (TSM.ModelObject modelObject in secondaries)
            if (modelObject is TSM.Part part)
                GetGeometry(part);

        // get 
        var subAssemblies = assembly.GetSubAssemblies();
        foreach (var subAssembly in subAssemblies)
            switch (subAssembly)
            {
                case TSM.Assembly caseSubAssembly:
                    WriteGeometry(caseSubAssembly);
                    break;
                case TSM.Part casePart:
                    GetGeometry(casePart);
                    break;
            }
    }

    private void GetGeometry(TSM.BaseComponent baseComponent)
    {
    }

    private void GetGeometry(TSM.Part part)
    {
        if (part == null) return;

        var solid = part.GetSolid();
        var faces = solid.GetFaceEnumerator();
        while (faces.MoveNext())
        {
            var face = faces.Current;
            if (null == face)
                continue;

            var loops = face.GetLoopEnumerator();
            while (loops.MoveNext())
            {
                var loop = loops.Current;
                if (null == loop) continue;

                var vertexEnumerator = loop.GetVertexEnumerator();
                var points = vertexEnumerator.ToList();
                points.Add(points.First()); // 形成一个回环

                var polyline3D = new Polyline3D();
                var vertexes = points.ToList().ConvertAll(ToDxf);
                polyline3D.Vertexes.AddRange(vertexes);

                _dxfTool.AddPolyLine(polyline3D);
            }
        }
    }

    private Vector3 ToDxf(TSG.Point point)
    {
        return new Vector3(point.X, point.Y, point.Z);
    }
}