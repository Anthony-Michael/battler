# Minimal Main Scene Setup Instructions

## In Unity Editor:
1. Create new scene: File > New Scene
2. Add Canvas: UI > Canvas
3. Add Button: UI > Button (name: 'ScanBarcodeButton')
4. Add Text: UI > Text (name: 'MonsterStatsOutput') 
5. Add InputField: UI > Input Field (name: 'ManualBarcodeInput')
6. Create empty GameObject: 'MainMenuController'
7. Attach MainMenuController.cs script to GameObject
8. Create empty GameObject: 'BarcodeScanner' 
9. Attach BarcodeScanner.cs script to GameObject
10. Wire up UI references in Inspector

## UI Layout:
- Canvas (Screen Space Overlay)
  - ScanBarcodeButton (Position: Center, Text: 'Scan Barcode')
  - MonsterStatsOutput (Position: Below button, Multi-line text)
  - ManualBarcodeInput (Position: Below text, Placeholder: 'Enter barcode...')

## Component References:
MainMenuController:
- Scan Barcode Button: drag ScanBarcodeButton
- Monster Stats Output: drag MonsterStatsOutput  
- Manual Barcode Input: drag ManualBarcodeInput
- Barcode Scanner: drag BarcodeScanner GameObject

BarcodeScanner:
- Camera Display: (optional RawImage for camera preview)
- Scan Button: drag ScanBarcodeButton  
- Cancel Button: (optional)
- Status Text: drag MonsterStatsOutput
- Manual Barcode Input: drag ManualBarcodeInput

## Testing:
- Play scene, click 'Scan Barcode' or type barcode in input field
- Monster stats should appear in the text panel
