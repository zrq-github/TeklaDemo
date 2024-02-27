using System.Windows;
using System.Linq;
using TeklaUtils.Extend;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Catalogs;

namespace TeklaTestDrawApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TSM.Model _model;
    private TSD.DrawingHandler _drawingHandler;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var model = new TSM.Model();
        if (!model.GetConnectionStatus())
        {
            MessageBox.Show("Tekla Structure not connected");
            return;
        }
        _model = model;

        _drawingHandler = new TSD.DrawingHandler();
    }

    private void BtnGetAllDrawings_OnClick(object sender, RoutedEventArgs e)
    {
        if (!CheckDrawingConnectionStatus())
            return;
        var drawings = _drawingHandler.GetDrawings().ToList();
        foreach (var drawing in drawings)
        {
            var name = drawing.Name;
        }

        AssemblyDrawing assemblyDrawing;
        CastUnitDrawing castUnitDrawing;
        SinglePartDrawing singlePartDrawing;

        MultiDrawing multiDrawing;
        GADrawing gaDrawing;
    }

    private bool CheckDrawingConnectionStatus()
    {
        if (!_drawingHandler.GetConnectionStatus())
        {
            MessageBox.Show("图纸未连接");
            return false;
        }

        return true;
    }

    private void BtnGetDrawingObjects_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var drawing = _drawingHandler.GetActiveDrawing();

            var sheet = drawing.GetSheet();

            var views = sheet.GetViews().ToList<DrawingObject>();
            var allViews = sheet.GetAllViews().ToList<DrawingObject>();
            var sheetDrawing = sheet.GetDrawing();

            var objects = drawing.GetSheet().GetAllObjects();
            var drawingObjects = objects.ToList<DrawingObject>();
            foreach (var drawingObject in drawingObjects)
            {
                var type = drawingObject.GetType();

                if (drawingObject is Tekla.Structures.Drawing.View view)
                {
                    var viewHeight = view.Height;
                    var viewWidth = view.Width;
                    var viewName = view.Name;
                    var viewModelObjects = view.GetModelObjects().ToList<TSD.ModelObject>();
                    var drawingPart = viewModelObjects.FirstOrDefault() as TSD.Part;
                    var id = drawingPart.ModelIdentifier.ID;


                    var partRelatedObjects = drawingPart.GetRelatedObjects().ToList<TSD.DrawingObject>();
                }

            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void Test()
    {

    }

    private void BtnExportDwg_OnClick(object sender, RoutedEventArgs e)
    {
        DrawingItem item = new DrawingItem();

        //if (item.Select())
        //{
        //    string filename = System.IO.Path.GetTempPath() + item.Name;
        //    if (item.Export(ref filename))
        //    {
        //        Console.WriteLine(item.Name + " exported successfully");
        //    }
        //}
        return;

        var modelPath = _model.GetInfo().ModelPath;
        var modelName = _model.GetInfo().ModelName.Split('.')[0];

        ExportDrawingsHelper.ExportDrawings($"{modelPath}\\exportDWG.dwf");
    }

    private void BtnExportPDF_OnClick(object sender, RoutedEventArgs e)
    {
        bool result = false;
        DrawingHandler dh = new DrawingHandler();

        var drawings = dh.GetDrawings().ToList();
        var drawing = drawings.First();
        if (null == drawing)
        {
            return;
        }
        //if (!(activeDrawing is GADrawing) || !activeDrawing.Name.Equals("New Drawing")) return;

        var printAttributes = new PrintAttributes
        {
            ScalingType = DotPrintScalingType.Auto,
            NumberOfCopies = 1,
            Orientation = DotPrintOrientationType.Landscape,
            Scale = 1,
            PrintArea = DotPrintAreaType.EntireDrawing,
        };
        dh.PrintDrawing(drawing, printAttributes);

        // 以下是2022的调用方式
        //DPMPrinterAttributes printAttributes = new DPMPrinterAttributes();
        //printAttributes.ColorMode = DotPrintColor.BlackAndWhite;
        //printAttributes.NumberOfCopies = 1;
        //printAttributes.OpenFileWhenFinished = true;
        //printAttributes.Orientation = DotPrintOrientationType.Landscape;
        //printAttributes.OutputFileName = "ExampleDrawing.pdf";
        //printAttributes.OutputType = DotPrintOutputType.PDF;
        //printAttributes.PaperSize = DotPrintPaperSize.A4;
        //printAttributes.PrinterName = "PDF-XChange 3.0"; // Must use local PDF printer name
        //printAttributes.PrintToMultipleSheet = DotPrintToMultipleSheet.Off;
        //printAttributes.ScaleFactor = 1.0;
        //printAttributes.ScalingMethod = DotPrintScalingType.Auto;
        //result = dh.PrintDrawing(activeDrawing, printAttributes);

    }
}