# Core Rules
- There are **N tubes** (at least 8), each with a capacity of **4 color layers**
- A **move** = pour the top layer(s) of tube A into tube B
- A pour is **valid** if:
  - Tube B is not full
  - Tube B is empty, OR the top color of B matches the top color of A
  - Multiple layers of the same color on top of A all pour together
- The **win condition**: every tube is either empty or filled with a single color
- The **lose condition**: no valid moves exist (optional, you can skip this for prototype)

# Key Data Questions to Answer First
- How many layers per tube? → 4
- What is the state of a tube? → A stack of colors (max 4)
- What defines "same color group"? → Consecutive same-color layers from top
- When is a pour valid? → Check top of source vs top of destination

---

# Project Folder Structure

```
Assets/
│
├── Art/
│   ├── Materials/
│   ├── Sprites/
│   ├── Animations/
│   └── VFX/
│
├── Audio/
│   ├── Music/
│   └── SFX/
│
├── GameData/                        # ScriptableObject instances (Boss Room pattern)
│   ├── Levels/                      # LevelData assets (Level_01.asset, etc.)
│   ├── ColorPalette.asset
│   └── GameConfig.asset
│
├── Prefabs/
│   ├── Gameplay/
│   ├── UI/
│   └── VFX/
│
├── Scenes/
│   ├── Startup.unity                # Bootstrap — mirrors Boss Room's Startup
│   ├── MainMenu.unity
│   └── Gameplay.unity
│
└── Scripts/
    │
    ├── ApplicationLifecycle/        # Entry point, DI root — directly from Boss Room
    │   ├── ApplicationController.cs # Your existing file — stays here
  │   └── GameLifetimeScope.cs
    │
    ├── Gameplay/                    # Everything about the game itself
    │   │
    │   ├── Tube/                    # The core game object — like Boss Room's Character/
    │   │   ├── TubeModel.cs         # Pure logic (your Domain layer)
    │   │   ├── TubeView.cs          # MonoBehaviour visuals
    │   │   ├── TubePresenter.cs     # Connects model to view
    │   │   └── TubeColor.cs
    │   │
    │   ├── Board/                   # The game board — like Boss Room's GameplayObjects/
    │   │   ├── BoardModel.cs
    │   │   ├── BoardView.cs
    │   │   ├── BoardPresenter.cs
    │   │   └── ColorSegment.cs
    │   │
    │   ├── GameState/               # State machine — directly mirrors Boss Room's GameState/
    │   │   ├── IGameState.cs
    │   │   ├── GameStateMachine.cs
    │   │   └── States/
    │   │       ├── LoadingState.cs
    │   │       ├── GameplayState.cs
    │   │       ├── PausedState.cs
    │   │       ├── WinState.cs
    │   │       └── LoseState.cs
    │   │
    │   ├── Configuration/           # Config ScriptableObject definitions — from Boss Room
    │   │   ├── LevelData.cs
    │   │   ├── GameConfig.cs
    │   │   └── ColorPalette.cs
    │   │
    │   └── UI/                      # In-game UI — mirrors Boss Room's Gameplay/UI/
    │       ├── HUDView.cs
    │       ├── HUDPresenter.cs
    │       ├── WinScreenView.cs
    │       └── LoseScreenView.cs
    │
    ├── MainMenu/                    # Separate feature folder — Boss Room has this too
    │   └── MainMenuView.cs
    │
    ├── Infrastructure/              # Reusable tools — directly from Boss Room
    │   ├── PubSub/                  # Your existing MessageChannel code lives here
    │   │   ├── IMessageChannel.cs
    │   │   ├── MessageChannel.cs
    │   │   └── DisposableSubscription.cs
    │   ├── SceneLoader.cs
    │   └── DisposableGroup.cs
    │
    └── Utils/
        └── Extensions/
            └── TubeExtensions.cs

```


