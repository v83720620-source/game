# 🎮 Flump Game - Mobile 5v5 Shooter

**Mobile tactical shooter for iOS & Android**

## 📊 Project Info

- **Engine:** Unity 6.3 LTS (6000.3.3f1)
- **Render Pipeline:** Universal Render Pipeline 17.3.0
- **Language:** C# (.NET Standard 2.1)
- **Platforms:** iOS 12.0+, Android API 24+
- **Target:** 30-60 FPS on mobile devices

## 🎯 Features

### Single-Player (Completed ✅)
- Player movement (walk, sprint, jump, crouch)
- First-person camera system
- Mobile controls (virtual joystick, buttons)
- Advanced weapon system (recoil, spread, magazine)
- Health & damage system (HP, armor, hit zones)
- AI bots (patrol, chase, attack)
- Team system (5v5)
- Game modes:
  - Team Deathmatch (50 kills)
  - Hardpoint (zone capture, 150 points)
- VFX & Audio systems
- Mobile optimization (3 quality levels)

### Multiplayer (In Development 🚧)
- Main menu & lobby
- Matchmaking system
- Duel 1v1 mode
- Team 3v3 TDM mode
- Server-authoritative combat
- Lag compensation
- Smart AI bots (4 difficulty levels)

## 🛠️ Tech Stack

- Unity 6.3 LTS
- Universal Render Pipeline (URP) 17.3.0
- Input System 1.17.0
- Netcode for GameObjects 2.8.0 (multiplayer)
- AI Navigation Package

## 📝 Development Status

**Progress:** 11/12 stages completed (91.7%)

**Milestone 4:** POLISH & MOBILE ✅
- Stage 10: VFX & Audio ✅
- Stage 11: Mobile Optimization ✅
- Stage 12: Build & Test 🚧

**Milestone 5:** MULTIPLAYER (Planned)
- Stage 13: Network Setup & Lobby
- Stage 14: Player Networking
- Stage 15: Game Modes Networking
- Stage 16: Matchmaking & Bots
- Stage 17: Server Hosting & Testing

## 📦 Project Structure

```
Assets/
├── _Project/
│   ├── Scripts/         # All C# scripts (44 files)
│   │   ├── Player/      # Movement, Health
│   │   ├── Camera/      # First person camera
│   │   ├── Weapons/     # Weapon system
│   │   ├── Combat/      # Hit detection, damage
│   │   ├── AI/          # Bot AI, Vision
│   │   ├── GameModes/   # TDM, Hardpoint, Teams
│   │   ├── UI/          # HUD, menus
│   │   ├── Input/       # Mobile controls
│   │   ├── VFX/         # Visual effects
│   │   ├── Audio/       # Sound system
│   │   ├── Managers/    # Quality, Match, Team
│   │   └── Debug/       # FPS counter
│   ├── Prefabs/         # Game objects
│   ├── Scenes/          # Game scenes
│   ├── Settings/        # URP assets, Input
│   └── Materials/       # Materials, Textures
├── Art/                 # 3D models, textures
├── Audio/               # Sound effects, music
└── Plugins/             # Third-party code
```

## 🚀 Getting Started

### Prerequisites
- Unity 6.3 LTS (6000.3.0f1)
- Android Build Support (for mobile builds)
- Git LFS (for large files)

### Setup
1. Clone the repository
2. Open project in Unity Hub
3. Wait for initial import (5-10 minutes)
4. Open MainScene
5. Press Play!

## 📖 Documentation

- [Development Plan](FLUMP_DEVELOPMENT_PLAN.md) - Full roadmap
- [Multiplayer Plan](MULTIPLAYER_PLAN.md) - Network architecture
- [Technologies 2026](UNITY_6_TECHNOLOGIES_2026.md) - Modern Unity features
- [Stage Guides](Assets/_Project/STAGE_*_SETUP.md) - Step-by-step instructions

## 🎮 Controls

### PC (Editor):
- WASD - Move
- Shift - Sprint
- Space - Jump
- Ctrl - Crouch
- Mouse - Look around
- LMB - Fire
- R - Reload

### Mobile:
- Virtual Joystick - Move
- Touch & Drag - Camera
- Fire Button - Shoot
- Reload Button - Reload
- Jump, Run, Crouch buttons

## 📊 Performance

### Targets:
- PC: 60+ FPS ✅
- Mobile (Flagship): 60 FPS 🎯
- Mobile (Mid-range): 45-60 FPS 🎯
- Mobile (Budget): 30 FPS 🎯

### Optimizations:
- Object pooling (VFX, Audio)
- 3 Quality levels (Low/Medium/High)
- ASTC texture compression
- Static batching
- URP optimized shaders

## 📝 License

Private project - All rights reserved

## 👤 Author

[Your Name]

## 📅 Timeline

- **Started:** January 2026
- **Single-Player Completed:** January 30, 2026
- **Multiplayer (Expected):** March 2026
- **Release (Target):** Q2 2026

---

**Status:** Active Development 🚀
