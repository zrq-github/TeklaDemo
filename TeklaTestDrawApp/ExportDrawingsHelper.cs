namespace TeklaTestDrawApp;

public class ExportDrawingsHelper
{
    public static void ExportDrawings(string outputFileName)
    {
        var componentInput = new TSM.ComponentInput();
        componentInput.AddOneInputPosition(new TSG.Point(0, 0, 0));

        var comp = new TSM.Component(componentInput)
        {
            Name = "ExportDrawings",
            Number = TSM.BaseComponent.PLUGIN_OBJECT_NUMBER
        };

        comp.SetAttribute("Name", outputFileName);
        comp.SetAttribute("Type", 0);

        comp.Insert();
    }
}