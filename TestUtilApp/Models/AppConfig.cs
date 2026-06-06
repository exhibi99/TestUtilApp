using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Newtonsoft.Json;
using TestUtilApp.Editors;

namespace TestUtilApp.Models
{
    [DisplayName("Application Settings")]
    [Description("Overall settings for the image processing utility")]
    [JsonObject(MemberSerialization.OptIn)]
    public class AppConfig
    {
        [Category("3. General")]
        [DisplayName("Detection Classes")]
        [Description("List of classes to use for image detection")]
        [TypeConverter(typeof(StringListConverter))]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<string> DetectionClasses { get; set; }

        [Category("3. General")]
        [DisplayName("Default Padding")]
        [Description("Default padding value for image cropping (pixels)")]
        [JsonProperty]
        public int DefaultPadding { get; set; }

        [Category("3. General")]
        [DisplayName("Default Crop Area")]
        [Description("Default crop area settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public CropArea DefaultCropArea { get; set; }

        [Category("1. Deep Learning")]
        [DisplayName("DICE Model Settings")]
        [Description("DICE model paths and usage settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public DiceModelsConfig DiceModels { get; set; }

        [Category("4. Label Generation")]
        [DisplayName("Label Generation Settings")]
        [Description("Settings for automatic JSON label generation")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public LabelGenerationConfig LabelGeneration { get; set; }

        [Category("5. Classification")]
        [DisplayName("Classification Settings")]
        [Description("Settings for image classification")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public ClassificationConfig Classification { get; set; }

        [Category("2. File Filtering")]
        [DisplayName("File Filtering Settings")]
        [Description("Settings for file filtering")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public FileFilterConfig FileFilter { get; set; }

        [Category("6. Last Paths")]
        [DisplayName("Crop Source Folder")]
        [Description("Last used crop source folder path")]
        [JsonProperty]
        public string LastCropSourceFolder { get; set; }

        [Category("6. Last Paths")]
        [DisplayName("Classification Source Folder")]
        [Description("Last used classification source folder path")]
        [JsonProperty]
        public string LastClassifySourceFolder { get; set; }

        [Category("6. Last Paths")]
        [DisplayName("Label Generation Source Folder")]
        [Description("Last used label generation source folder path")]
        [JsonProperty]
        public string LastLabelGenSourceFolder { get; set; }

        [Category("7. Inspection")]
        [DisplayName("Inspection Spec JSON Path")]
        [Description("GMAP inspection spec JSON file path (GMAPInspSpec.json)")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [JsonProperty]
        public string InspectionSpecJsonPath { get; set; }

        [Category("7. Inspection")]
        [DisplayName("Inspection Conditions")]
        [Description("Rules that select detect classes and classification model by image filename")]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<InspectionConditionRule> InspectionConditions { get; set; }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Crop Area")]
    [JsonObject(MemberSerialization.OptIn)]
    public class CropArea
    {
        [DisplayName("X Coordinate")]
        [Description("Starting X position of the crop area")]
        [JsonProperty]
        public int X { get; set; }

        [DisplayName("Y Coordinate")]
        [Description("Starting Y position of the crop area")]
        [JsonProperty]
        public int Y { get; set; }

        [DisplayName("Width")]
        [Description("Width of the crop area")]
        [JsonProperty]
        public int Width { get; set; }

        [DisplayName("Height")]
        [Description("Height of the crop area")]
        [JsonProperty]
        public int Height { get; set; }

        public override string ToString()
        {
            return $"X={X}, Y={Y}, W={Width}, H={Height}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DICE Model Settings")]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiceModelsConfig
    {
        [DisplayName("Detection Model")]
        [Description("DICE detection model settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public DiceModelSetting DetectModel { get; set; }

        [DisplayName("Classification Model A")]
        [Description("DICE classification model A (top) settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public DiceModelSetting ClassifyModel_A { get; set; }

        [DisplayName("Classification Model B")]
        [Description("DICE classification model B (bottom) settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [JsonProperty]
        public DiceModelSetting ClassifyModel_B { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("DICE Model")]
    [JsonObject(MemberSerialization.OptIn)]
    public class DiceModelSetting
    {
        [DisplayName("Use")]
        [Description("Whether to use this model")]
        [JsonProperty]
        public bool Use { get; set; }

        [DisplayName("Path")]
        [Description("DICE model folder path (click the '...' button on the right)")]
        [Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public string Path { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Label Generation Settings")]
    [JsonObject(MemberSerialization.OptIn)]
    public class LabelGenerationConfig
    {
        [DisplayName("Minimum Confidence")]
        [Description("Minimum confidence to save as a label (0.0 to 1.0)")]
        [JsonProperty]
        public float MinConfidence { get; set; } = 0.5f;

        [DisplayName("Skip Existing JSON")]
        [Description("Skip images that already have a JSON file")]
        [JsonProperty]
        public bool SkipExistingJson { get; set; } = true;

        public override string ToString()
        {
            return $"Minimum confidence: {MinConfidence:F2}, skip existing: {SkipExistingJson}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Classification Settings")]
    [JsonObject(MemberSerialization.OptIn)]
    public class ClassificationConfig
    {
        [DisplayName("Minimum Confidence")]
        [Description("Minimum confidence to accept a classification result (0.0 to 1.0)")]
        [JsonProperty]
        public float MinConfidence { get; set; } = 0.5f;

        public override string ToString()
        {
            return $"Minimum confidence: {MinConfidence:F2}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File Filter Settings")]
    [JsonObject(MemberSerialization.OptIn)]
    public class FileFilterConfig
    {
        [Category("Path")]
        [DisplayName("Last Target Folder")]
        [Description("Last selected filtering target folder")]
        [JsonProperty]
        public string LastTargetFolder { get; set; }

        [Category("Path")]
        [DisplayName("Last Output Folder")]
        [Description("Last selected custom output folder")]
        [JsonProperty]
        public string LastOutputFolder { get; set; }

        [Category("Filter Conditions")]
        [DisplayName("Active Profile Index")]
        [Description("Selected filter condition profile index")]
        [JsonProperty]
        public int ActiveProfileIndex { get; set; }

        [Category("Filter Conditions")]
        [DisplayName("Condition Profiles")]
        [Description("Saved filter condition profiles")]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<FileFilterProfile> ConditionProfiles { get; set; }

        [Category("Output")]
        [DisplayName("Output Folder Postfix")]
        [Description("Postfix to add to the output folder name in copy mode")]
        [JsonProperty]
        public string OutputPostfix { get; set; }

        [Category("Output")]
        [DisplayName("Use Custom Output Folder")]
        [Description("Use the selected output folder instead of creating one next to the target folder")]
        [JsonProperty]
        public bool UseCustomOutputFolder { get; set; }

        [Category("Output")]
        [DisplayName("Copy Folder Count")]
        [Description("Maximum number of folders to copy randomly. 0 means disabled (copy all matching folders).")]
        [JsonProperty]
        public int CopyFolderCount { get; set; } = 30;

        public override string ToString()
        {
            int profileCount = ConditionProfiles?.Count ?? 0;
            return $"Profiles: {profileCount}, active: {ActiveProfileIndex + 1}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("File Filter Profile")]
    [JsonObject(MemberSerialization.OptIn)]
    public class FileFilterProfile
    {
        [Category("1. Profile")]
        [DisplayName("Profile Name")]
        [Description("Name displayed for this filter profile")]
        [JsonProperty]
        public string Name { get; set; }

        [Category("2. Folder Filter")]
        [DisplayName("Include Folder Keywords")]
        [Description("Folder keywords to include")]
        [TypeConverter(typeof(StringListConverter))]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<string> IncludeFolderKeywords { get; set; }

        [Category("2. Folder Filter")]
        [DisplayName("Include File Keywords")]
        [Description("File keywords that must exist in the folder")]
        [TypeConverter(typeof(StringListConverter))]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<string> IncludeFileKeywords { get; set; }

        [Category("3. File Filter")]
        [DisplayName("Exclude File Keywords")]
        [Description("File keywords to exclude")]
        [TypeConverter(typeof(StringListConverter))]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<string> ExcludeFileKeywords { get; set; }

        [Category("3. File Filter")]
        [DisplayName("Exclude Extensions")]
        [Description("Extensions to exclude, without dot")]
        [TypeConverter(typeof(StringListConverter))]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<string> ExcludeExtensions { get; set; }

        [Category("2. Folder Filter")]
        [DisplayName("Use Folder Include Filter")]
        [Description("Enable folder keyword include filtering")]
        [JsonProperty]
        public bool UseFolderIncludeFilter { get; set; } = true;

        [Category("2. Folder Filter")]
        [DisplayName("Use File Include Filter")]
        [Description("Enable file keyword include filtering")]
        [JsonProperty]
        public bool UseFileIncludeFilter { get; set; } = false;

        [Category("3. File Filter")]
        [DisplayName("Use File Exclude Filter")]
        [Description("Enable file keyword exclude filtering")]
        [JsonProperty]
        public bool UseFileExcludeFilter { get; set; } = true;

        [Category("3. File Filter")]
        [DisplayName("Use Extension Exclude Filter")]
        [Description("Enable extension exclude filtering")]
        [JsonProperty]
        public bool UseExtensionExcludeFilter { get; set; } = true;

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? "Filter Profile" : Name;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Inspection Condition")]
    [JsonObject(MemberSerialization.OptIn)]
    public class InspectionConditionRule
    {
        [Category("1. Condition")]
        [DisplayName("Use")]
        [Description("Whether this condition is enabled")]
        [JsonProperty]
        public bool Use { get; set; } = true;

        [Category("1. Condition")]
        [DisplayName("Filename Keyword")]
        [Description("Keyword that must be included in the image filename")]
        [JsonProperty]
        public string FileNameKeyword { get; set; }

        [Category("2. Detection")]
        [DisplayName("Detect Class Keywords")]
        [Description("Detection class keywords to inspect")]
        [TypeConverter(typeof(StringListConverter))]
        [Editor(typeof(StringCollectionEditor), typeof(UITypeEditor))]
        [JsonProperty]
        public List<string> DetectClassKeywords { get; set; }

        [Category("3. Classification")]
        [DisplayName("Classify Model")]
        [Description("Classification model to use for matching detections: A or B")]
        [JsonProperty]
        public string ClassifyModel { get; set; } = "A";

        [Category("3. Classification")]
        [DisplayName("Min Confidence")]
        [Description("Minimum classification confidence threshold (0.0–1.0). 0 = use global setting from Classification config.")]
        [JsonProperty]
        public float MinConfidence { get; set; } = 0.75f;

        public override string ToString()
        {
            string keyword = string.IsNullOrWhiteSpace(FileNameKeyword) ? "(all files)" : FileNameKeyword;
            return $"{keyword} -> Model {ClassifyModel}";
        }
    }
}


