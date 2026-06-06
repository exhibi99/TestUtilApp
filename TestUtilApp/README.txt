# TestUtilApp Configuration Guide

## config.json Overview

The application uses DICE model settings directly.

```json
{
  "DetectionClasses": ["BELT_TOP", "BELT_BOTTOM"],
  "DefaultPadding": 10,
  "DefaultCropArea": {
    "X": 100,
    "Y": 100,
    "Width": 200,
    "Height": 200
  },
  "DiceModels": {
    "DetectModel": {
      "Use": true,
      "Path": "DICE detection model folder"
    },
    "ClassifyModel_A": {
      "Use": true,
      "Path": "DICE classification model A folder"
    },
    "ClassifyModel_B": {
      "Use": true,
      "Path": "DICE classification model B folder"
    }
  },
  "LabelGeneration": {
    "MinConfidence": 0.5,
    "SkipExistingJson": true
  },
  "Classification": {
    "MinConfidence": 0.5
  },
  "FileFilter": {
    "LastTargetFolder": "",
    "LastOutputFolder": "",
    "OutputPostfix": "_filtered",
    "UseCustomOutputFolder": false
  }
}
```

## Model Loading

- `Label Generate` loads the DICE detection model when needed.
- `Image Crop` loads the DICE detection model only for auto-detect crop mode.
- `Image Classification` loads the selected DICE classification model.

Model paths can be changed from each screen and are saved back to `config.json`.
