# PocketBattler - Unity 2D Mobile Game

A turn-based monster battling game where players scan real-world barcodes to create unique monsters and battle them locally or online.

## Features

- **Barcode Scanning**: Scan real-world UPC/EAN barcodes to generate unique monsters
- **Deterministic Stats**: Same barcode always creates the same monster stats
- **Turn-Based Combat**: Strategic battles with speed-based initiative
- **Firebase Integration**: Cloud storage for monsters, battles, and leaderboards
- **Local Multiplayer**: Battle against other monsters on the same device
- **Cross-Platform**: Android first, with iOS and WebGL support planned

## Project Structure

```
Assets/
├── Scripts/
│   ├── Barcode/
│   │   ├── BarcodeGenerator.cs    # Converts barcodes to monster stats
│   │   └── BarcodeScanner.cs      # Camera barcode scanning
│   ├── Battle/
│   │   └── BattleEngine.cs        # Turn-based combat logic
│   ├── Firebase/
│   │   └── FirebaseService.cs     # Cloud services integration
│   └── UI/
│       ├── MainMenuController.cs  # Main menu UI
│       ├── BattleController.cs    # Battle scene UI
│       ├── PlayerConnector.cs     # Local multiplayer
│       ├── MonsterListItem.cs     # Monster list UI
│       └── OpponentListItem.cs    # Opponent selection UI
├── Scenes/
│   ├── Main.unity                 # Main menu scene
│   └── Battle.unity               # Battle scene
├── Resources/
│   ├── MonsterFrames/             # Monster sprite frames
│   └── ArchetypeIcons/            # Archetype icons
└── Plugins/
    └── Android/
        └── AndroidManifest.xml    # Android permissions
```

## Setup Instructions

### 1. Unity Setup
- Open project in Unity 2021.3+ (recommended)
- Import required packages via Package Manager

### 2. Firebase Configuration
1. Create a Firebase project at https://console.firebase.google.com/
2. Enable Firestore Database
3. Download `google-services.json` and replace the template in `Assets/Firebase/`
4. Update the config values with your Firebase project details

### 3. Dependencies
Required Unity packages (already configured in Packages/manifest.json):
- Firebase App, Auth, Firestore
- External Dependency Manager
- TextMeshPro
- ZXing.NET for barcode scanning

### 4. Scene Setup
Create two scenes:
- **Main Scene** (`Assets/Scenes/Main.unity`):
  - Canvas with UI elements for menu buttons
  - Attach `MainMenuController`, `BarcodeScanner`, `FirebaseService` scripts
  - Configure UI references in inspectors

- **Battle Scene** (`Assets/Scenes/Battle.unity`):
  - Canvas with battle UI elements
  - Attach `BattleController`, `PlayerConnector` scripts
  - Configure UI references in inspectors

### 5. Build Settings
- Target Platform: Android
- Minimum API Level: 21 (Android 5.0)
- Scripting Backend: IL2CPP
- Target Architectures: ARM64, ARMv7

## Monster Generation

Monsters are created from barcode digits using deterministic algorithms:

- **Archetype** (last 3 digits):
  - 000-199: Beast
  - 200-399: Robot
  - 400-599: Undead
  - 600-799: Alien
  - 800-999: Mystic

- **Stats** (remaining digits via SHA256 hash):
  - HP: 50-500
  - Attack: 5-99
  - Defense: 5-99
  - Speed: 1-10
  - Crit Rate: 1%-25%

## Battle System

Turn-based combat with:
- Speed determines attack order
- Damage = Attack × (0.8-1.2) - Defense × 0.5
- Critical hits: 1.5× damage based on crit rate
- Battle ends when HP ≤ 0

## Firebase Collections

- **monsters**: Monster data with stats and creation timestamps
- **battles**: Battle results with winner/loser and scores
- **leaderboard**: Computed from battle results (daily/weekly/all-time)

## Testing

1. **Barcode Generation**: Test with sample barcodes like "123456789012"
2. **Battle Simulation**: Create two monsters and simulate battles
3. **Firebase**: Ensure cloud saving and leaderboard loading works
4. **UI Flow**: Test complete flow from scanning to battling

## Development Notes

- MVP focuses on core gameplay loop
- No complex 3D graphics yet
- Built-in Unity UI used throughout
- Camera permissions handled automatically on Android
- Local multiplayer via shared device monster selection

## Future Enhancements

- iOS and WebGL builds
- Online multiplayer
- Monster sprites and animations
- Marketplace for trading monsters
- Guild/clan system
- Tournament modes

## License

This project is for educational purposes. Modify and distribute as needed.
