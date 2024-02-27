using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using CsvHelper;
using Tekla.Structures;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;
using TeklaTestApplication.Helper;
using Localization = Tekla.Structures.Dialog.Localization;

namespace TeklaTestApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TSM.Model _model;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Create a model instance and check connection status
        var model = new TSM.Model();
        if (!model.GetConnectionStatus())
        {
            MessageBox.Show("Tekla Structure not connected");
            return;
        }

        _model = model;
    }

    private void BtnSendCommandPrompt_OnClick(object sender, RoutedEventArgs e)
    {
        if (_model == null)
            return;

        // get model info and send a "Hello world" message to the message box
        var modelInfo = _model.GetInfo();
        var name = modelInfo.ModelName;

        MessageBox.Show($"Hello world! your current model is named: {name}");

        // send a hello world message to the Tekla Structures user command prompt
        Operation.DisplayPrompt($"Hello world! your current model is named: {name}");
    }

    private void BtnCreateBeam_OnClick(object sender, RoutedEventArgs e)
    {
        var myModel = _model;
        var myBeam = new TSM.Beam(new TSG.Point(1000, 1000, 1000),
            new TSG.Point(6000, 6000, 1000))
        {
            Material =
            {
                MaterialString = "S235JR"
            },
            Profile =
            {
                ProfileString = "HEA400"
            }
        };
        myBeam.Insert();
        myModel.CommitChanges();
    }

    private void BtnFindAllObjects_OnClick(object sender, RoutedEventArgs e)
    {
        var objectSelector = _model.GetModelObjectSelector();
        var allObjects = objectSelector.GetAllObjects();
        foreach (var allObject in allObjects)
        {
            TSM.PolyBeam polyBeam = null;
            TSM.Beam beam = null;
            var type = allObject.GetType();
            var typeofName = type.Name;
            Debug.WriteLine(typeofName);

            TSM.Assembly assembly = null;
            TSM.Component component = null;
            TSM.Part part = null;

            var assemblyID = assembly.Identifier;
            var componentId = component.Identifier;
            var partId = part.Identifier;
        }
    }

    private void BtnGetColor_OnClick(object sender, RoutedEventArgs e)
    {
        var picker = new Picker();
        var selection = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT);

        if (selection is not TSM.Part part)
            return;

        // 颜色分类 1- 14 每一个颜色不可修改，是固定的
        var strClass = part.Class;

        //ViewHandler
        var color = new Color();
        ModelObjectVisualization.GetRepresentation(part, ref color);

        // send a hello world message to the Tekla Structures user command prompt
        Operation.DisplayPrompt($"{part.Identifier.ID} color is ({color.Blue},{color.Green},{color.Blue},{color.Transparency})");
    }

    private void BtnGetObjectProperty_OnClick(object sender, RoutedEventArgs e)
    {
        var picker = new Picker();
        var selection = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT);
        if (null == selection)
            return;
        var part = selection as TSM.Part;
        if (null == part)
            return;

        var stringNames = new ArrayList();
        var doubleNames = new ArrayList();
        var integerNames = new ArrayList();
        var values = new Hashtable();
        var allUserHashTable = new Hashtable();
        part.GetAllUserProperties(ref allUserHashTable);

        //UserPropertyItem

        foreach (DictionaryEntry dictionaryEntry in allUserHashTable)
        {
            var key = dictionaryEntry.Key;
            var value = dictionaryEntry.Value;
        }
    }

    private void BtnExportIFC_OnClick(object sender, RoutedEventArgs e)
    {
        var selection = new Picker()
            .PickObjects(Picker.PickObjectsEnum.PICK_N_OBJECTS, "Pick Objects")
            ?.ToList();
        if (null == selection)
            return;

        var selector = new ModelObjectSelector();
        selector.Select(new ArrayList(selection), false);

        var modelPath = _model.GetInfo().ModelPath;
        var modelName = _model.GetInfo().ModelName.Split('.')[0];
        ExportIFCHelper.ExportIFC($"{modelPath}\\IFC\\OUT_{modelName}");
    }

    private void BtnExportDWG_OnClick(object sender, RoutedEventArgs e)
    {
        var modelPath = _model.GetInfo().ModelPath;
        var modelName = _model.GetInfo().ModelName.Split('.')[0];
        var outputFileName = $"{modelPath}\\DWG\\OUT_{modelName}";

        var componentInput = new TSM.ComponentInput();
        var comp = new TSM.Component(componentInput)
        {
            Name = "ExportDWG",
            Number = TSM.BaseComponent.PLUGIN_OBJECT_NUMBER
        };

        comp.SetAttribute("OutputFile", outputFileName);

        comp.Insert();
    }

    private void BtnExportCSV_OnClick(object sender, RoutedEventArgs e)
    {
        TSM.ModelObjectEnumerator.AutoFetch = true;
        var modelObjectEnumerator = new TSM.Model().GetModelObjectSelector().GetAllObjects();
        var modelObjects = modelObjectEnumerator.ToList();

        var beams = modelObjects.OfType<TSM.Beam>().ToList();
        var beamsReport = beams.Select(b => new
        {
            b.Name,
            b.Profile.ProfileString,
            b.Class,
            b.Finish,
            b.Material.MaterialString
        }).ToList();

        var modelPath = _model.GetInfo().ModelPath;
        var modelName = _model.GetInfo().ModelName.Split('.')[0];

        var filePath = $"{modelPath}\\{modelName}_BeamReport.csv";
        if (!File.Exists(filePath))
            using (var fileStream = File.Create(filePath))
            {
                fileStream.Close();
            }

        using (var writer = new CsvWriter(new StreamWriter(filePath), new CultureInfo("zh-cn")))
        {
            writer.WriteRecords(beamsReport);
        }
    }

    private void BetGetSelectId_OnClick(object sender, RoutedEventArgs e)
    {
        var identifier = new Picker()
            .PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick Objects").Identifier;

        Operation.DisplayPrompt($"{identifier.ID} {identifier.GUID}");
    }

    private void BtnExportDxf_OnClick(object sender, RoutedEventArgs e)
    {
        var exportDxf = new TeklaExportDxf(_model);

        var selection = new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART);
        exportDxf.Export(new List<TSM.ModelObject> { selection });

        //var objectSelector = _model.GetModelObjectSelector();
        //var allObjects = objectSelector.GetAllObjects().ToList();
        //exportDxf.Export(allObjects);
    }

    private void BtnSetCustomProperty_OnClick(object sender, RoutedEventArgs e)
    {
        void CreateUserPropertyItem(List<string> names)
        {
            names.ForEach(name =>
            {
                var up = new UserPropertyItem
                {
                    Name = name,
                    Level = UserPropertyLevelEnum.LEVEL_MODEL,
                    FieldType = UserPropertyFieldTypeEnum.FIELDTYPE_TEXT,
                    Type = PropertyTypeEnum.TYPE_STRING,
                    Visibility = UserPropertyVisibilityEnum.VISIBILITY_READONLY,
                    Unique = true,
                    AffectsNumbering = true
                };
                up.Insert();
                up.SetLabel(name);
                up.AddToObjectType(CatalogObjectTypeEnum.STEEL_BEAM);
            });
        }


        try
        {
            var selection = new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART);

            var modelObject = selection;
            CreateUserPropertyItem(new List<string> { "zrq_customProperty" });
            modelObject.SetUserProperty("zrq_customProperty", "设置自定义属性测试");
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.ToString());
        }
    }

    private void Test()
    {
    }

    private void BtnLocalization_OnClick(object sender, RoutedEventArgs e)
    {
        // "D:\\Program Files\\Tekla Structures\\2016\\\\messages\\DotAppsStrings\\DotNetDialogStrings.ail"
        var defaultLocalizationFile = Localization.DefaultLocalizationFile;

        // 主动去获取翻译
        var ailFile = "D:\\Program Files\\Tekla Structures\\2016\\messages\\joints.ail";
        var localization = new Localization();
        localization.Language = "chs";
        localization.LoadAilFile(ailFile);
        var test = localization.GetText("j_d_j_xs_shorten");

        // 被动去获取翻译
        var catalogHandler = new CatalogHandler();
        var userPropertyItemEnumerator = catalogHandler.GetUserPropertyItems();
        while (userPropertyItemEnumerator.MoveNext())
        {
            var current = userPropertyItemEnumerator.Current;
            if (current is not { } userPropertyItem)
                continue;

            if (current.Name.Equals("xs_shorten"))
            {
                var label = current.GetLabel();
            }
        }
    }

    private void BtnGetModelCatalogs_OnClick(object sender, RoutedEventArgs e)
    {
        var catalogHandler = new CatalogHandler();
        var userPropertyItemEnumerator = catalogHandler.GetUserPropertyItems();
        while (userPropertyItemEnumerator.MoveNext())
        {
            var current = userPropertyItemEnumerator.Current;
            if (current is not { } userPropertyItem)
                continue;

            if (current.Name.Equals("xs_shorten"))
            {
            }
        }
    }
}