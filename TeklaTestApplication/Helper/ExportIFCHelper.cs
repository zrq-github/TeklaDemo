namespace TeklaTestApplication.Helper;

internal class ExportIFCHelper
{
    #region Static Members

    public static void ExportIFC(string outputFileName)
    {
        var componentInput = new TSM.ComponentInput();
        componentInput.AddOneInputPosition(new TSG.Point(0, 0, 0));
        var comp = new TSM.Component(componentInput)
        {
            Name = "ExportIFC",
            Number = TSM.BaseComponent.PLUGIN_OBJECT_NUMBER
        };

        // Parameters
        comp.SetAttribute("OutputFile", outputFileName);
        comp.SetAttribute("Format", 0);
        comp.SetAttribute("ExportType", 1);
        //comp.SetAttribute("AdditionalPSets", "");
        comp.SetAttribute("CreateAll", 0); // 0 to export only selected objects

        // Advanced
        comp.SetAttribute("Assemblies", 1);
        comp.SetAttribute("Bolts", 1);
        comp.SetAttribute("Welds", 0);
        comp.SetAttribute("SurfaceTreatments", 1);

        comp.SetAttribute("BaseQuantities", 1);
        comp.SetAttribute("GridExport", 1);
        comp.SetAttribute("ReinforcingBars", 1);
        comp.SetAttribute("PourObjects", 1);

        comp.SetAttribute("LayersNameAsPart", 1);
        comp.SetAttribute("PLprofileToPlate", 0);
        comp.SetAttribute("ExcludeSnglPrtAsmb", 0);

        comp.SetAttribute("LocsFromOrganizer", 0);

        comp.Insert();
    }

    #endregion
}