using System.IO;
using Geometry.CSharp;
using netDxf;
using DxfDB = netDxf.Entities;

namespace TeklaTestDrawApp;

internal class DxfTool
{
    private DxfDocument _doc;
    private AciColor defaultColor = new AciColor(System.Drawing.Color.White);

    private bool _inchUnit;

    public DxfTool(string filePath, bool inch)
    {
        _inchUnit = inch;
        DxfFilePath = filePath;
        if (File.Exists(DxfFilePath))
            File.Delete(DxfFilePath);
        _doc = new DxfDocument();
    }

    public void Add(IPoint point, AciColor color = null)
    {
        var x = point.X;
        var y = point.Y;
        var z = point.Z;
        var dxfPoint = new netDxf.Entities.Point(x, y, z);
        SetColor(dxfPoint, color);
        _doc.Entities.Add(dxfPoint);
    }

    private void SetColor(in DxfDB.EntityObject entityObject, in AciColor color)
    {
        if (null == color)
        {
            entityObject.Color = defaultColor;
            return;
        }

        entityObject.Color = color;
        return;
    }

    public void AddPolyLine(IList<IPoint> points, AciColor color = null)
    {
        var polyline3D = new DxfDB.Polyline3D();
        var collection = points.ToList().ConvertAll(point => ToVector3(point));
        polyline3D.Vertexes.AddRange(collection);
    }

    public void Save()
    {
        _doc.Save(DxfFilePath);
    }

    private Vector3 ToVector3(in IPoint point)
    {
        return new Vector3(point.X, point.Y, point.Z);
    }

    public string DxfFilePath { get; set; } = string.Empty;
}