# 🧪 Magic Bloom: Sort Water — Game Architecture Guide

> **Goal:** Build a clean, scalable Water Sort prototype using **SOLID principles**, **VContainer (DI)**, **Pub/Sub messaging**, and a **State Machine** — inspired by Unity's Boss Room sample project structure.

---

## 📋 Table of Contents

1. [Understanding the Game Mechanics](#1-understanding-the-game-mechanics)
2. [Project Folder Structure](#2-project-folder-structure)
3. [Architecture Overview](#3-architecture-overview)
4. [State Machine Design](#4-state-machine-design)
5. [Core Domain Layer](#5-core-domain-layer)
6. [Pub/Sub Event System](#6-pubsub-event-system)
7. [VContainer Setup & Scopes](#7-vcontainer-setup--scopes)
8. [Scene & Prefab Layout](#8-scene--prefab-layout)
9. [Implementation Hints & Guides](#9-implementation-hints--guides)
10. [Bonus: Level Generation](#10-bonus-level-generation)
11. [SOLID Checklist](#11-solid-checklist)
12. [Recommended Implementation Order](#12-recommended-implementation-order)

---

## 1. Understanding the Game Mechanics

Before writing a single line of code, fully understand the rules:

### Core Rules
- There are **N tubes** (at least 8), each with a capacity of **4 color layers**
- A **move** = pour the top layer(s) of tube A into tube B
- A pour is **valid** if:
  - Tube B is not full
  - Tube B is empty, OR the top color of B matches the top color of A
  - Multiple layers of the same color on top of A all pour together
- The **win condition**: every tube is either empty or filled with a single color
- The **lose condition**: no valid moves exist (optional, you can skip this for prototype)

### Key Data Questions to Answer First
- How many layers per tube? → 4
- What is the state of a tube? → A stack of colors (max 4)
- What defines "same color group"? → Consecutive same-color layers from top
- When is a pour valid? → Check top of source vs top of destination

---

## 2. Project Folder Structure

Inspired by Boss Room's feature-based folder organization:

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

> **Boss Room Inspiration:** Boss Room uses feature-based folders (`GameplayObjects/`, `ConnectionManagement/`, `ApplicationLifecycle/`) instead of type-based folders (`Scripts/`, `Managers/`). Follow the same idea here.

---

## 3. Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                     VIEW LAYER                      │
│    TubeView  ←→  TubePresenter  ←→  BoardPresenter  │
│    (MonoBehaviour, animation, input click)           │
└─────────────────────┬───────────────────────────────┘
                      │ Pub/Sub Messages
┌─────────────────────▼───────────────────────────────┐
│                  CONTROLLER LAYER                   │
│             GameplayController (EntryPoint)          │
│        Listens to messages, drives StateMachine      │
└─────────────────────┬───────────────────────────────┘
                      │ calls
┌─────────────────────▼───────────────────────────────┐
│                   DOMAIN LAYER                      │
│   MoveValidator  │  WinChecker  │  LevelService     │
│   (pure C#, no Unity, fully unit testable)           │
└─────────────────────────────────────────────────────┘
```

**Flow of a single move:**
```
User clicks TubeView
  → TubeView publishes TubeSelectedMessage
    → GameplayController receives it
      → If first selection: store as "source"
      → If second selection: validate move via IMoveValidator
        → If valid: apply move to Tube domain models
          → Publish PourCompletedMessage
            → BoardPresenter updates TubeViews
              → Check win via IWinChecker
                → If won: publish GameWonMessage → StateMachine → WinState
```

---

## 4. State Machine Design

### States

```
┌──────────────┐     Start Game    ┌───────────────┐
│  MainMenu    │ ────────────────► │   Gameplay    │
└──────────────┘                   └───────┬───────┘
                                           │
                          ┌────────────────┼──────────────────┐
                          ▼                ▼                  ▼
                      ┌───────┐       ┌────────┐        ┌──────────┐
                      │  Win  │       │  Lose  │        │ (Paused) │
                      └───────┘       └────────┘        └──────────┘
                          │                │
                    Next Level /       Retry /
                    Main Menu          Main Menu
```

### Code Skeleton

```csharp
// IState.cs
public interface IState
{
    void OnEnter();
    void OnExit();
    void OnTick(); // optional
}

// StateMachine.cs
public class StateMachine
{
    IState _current;

    public void TransitionTo(IState next)
    {
        _current?.OnExit();
        _current = next;
        _current.OnEnter();
    }

    public void Tick() => _current?.OnTick();
}
```

```csharp
// GameplayState.cs
public class GameplayState : IState
{
    readonly IMessageBus _bus;
    readonly BoardPresenter _board;
    readonly ILevelService _levelService;

    public GameplayState(IMessageBus bus, BoardPresenter board, ILevelService levelService)
    { ... } // Injected by VContainer

    public void OnEnter()
    {
        var level = _levelService.GetCurrentLevel();
        _board.SetupLevel(level);
        // subscribe to events
    }

    public void OnExit()
    {
        // unsubscribe
        _board.Teardown();
    }
}
```

> **Hint:** Register each State as a plain class via VContainer. The StateMachine holds references to each state. GameplayController drives the machine.

---

## 5. Core Domain Layer

This is your most important layer — **zero Unity dependency**, fully testable.

### TubeColor.cs
```csharp
public enum TubeColor
{
    None = 0,
    Red,
    Blue,
    Yellow,
    Purple,
    Green,
    Orange,
    Pink,
    Teal,
}
```

### Tube.cs
```csharp
// Immutable-friendly domain model
public class Tube
{
    private readonly Stack<TubeColor> _layers;
    public const int MaxCapacity = 4;

    public int Id { get; }
    public bool IsEmpty => _layers.Count == 0;
    public bool IsFull => _layers.Count >= MaxCapacity;
    public TubeColor TopColor => IsEmpty ? TubeColor.None : _layers.Peek();
    public int Count => _layers.Count;

    // How many consecutive layers of the same color are on top?
    public int TopGroupCount { get; private set; } // compute on state change

    public bool TryPour(Tube destination) { ... }
    // hint: validate before calling this! Keep domain dumb, let service validate
}
```

### IMoveValidator.cs
```csharp
public interface IMoveValidator
{
    bool IsValidMove(Tube source, Tube destination);
    bool HasAnyValidMove(IReadOnlyList<Tube> tubes);
}
```

```csharp
// MoveValidator.cs — pure logic, no Unity
public class MoveValidator : IMoveValidator
{
    public bool IsValidMove(Tube source, Tube destination)
    {
        if (source.IsEmpty) return false;
        if (destination.IsFull) return false;
        // Special case: pouring into empty tube is always allowed
        // but be careful: don't allow pouring a single-color full tube into empty
        // (it's a wasted move — optional optimization)
        if (destination.IsEmpty) return true;
        return source.TopColor == destination.TopColor;
    }

    public bool HasAnyValidMove(IReadOnlyList<Tube> tubes)
    {
        foreach (var source in tubes)
            foreach (var dest in tubes)
                if (source != dest && IsValidMove(source, dest))
                    return true;
        return false;
    }
}
```

### LevelData.cs (ScriptableObject)
```csharp
[CreateAssetMenu(fileName = "Level", menuName = "WaterSort/Level")]
public class LevelData : ScriptableObject
{
    public int tubeCount = 8;
    public int emptyTubeCount = 2;
    // Each element is a tube: list of colors from bottom to top
    public List<TubeColorList> tubeConfigs;
}

[Serializable]
public class TubeColorList
{
    public List<TubeColor> colors; // index 0 = bottom
}
```

---

## 6. Pub/Sub Event System

Keep all messages as **simple structs** (no inheritance, just data).

### IMessageBus.cs
```csharp
public interface IMessageBus
{
    void Subscribe<T>(Action<T> handler);
    void Unsubscribe<T>(Action<T> handler);
    void Publish<T>(T message);
}
```

### MessageBus.cs (simple implementation)
```csharp
public class MessageBus : IMessageBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<T>(Action<T> handler)
    {
        var key = typeof(T);
        if (!_handlers.ContainsKey(key))
            _handlers[key] = new List<Delegate>();
        _handlers[key].Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var key = typeof(T);
        if (_handlers.TryGetValue(key, out var list))
            list.Remove(handler);
    }

    public void Publish<T>(T message)
    {
        var key = typeof(T);
        if (!_handlers.TryGetValue(key, out var list)) return;
        foreach (var handler in list.ToArray()) // ToArray: safe iteration
            ((Action<T>)handler)(message);
    }
}
```

### Message Structs

```csharp
// Messages/TubeSelectedMessage.cs
public readonly struct TubeSelectedMessage
{
    public readonly int TubeId;
    public TubeSelectedMessage(int tubeId) => TubeId = tubeId;
}

// Messages/PourRequestMessage.cs
public readonly struct PourRequestMessage
{
    public readonly int SourceTubeId;
    public readonly int DestinationTubeId;
    public PourRequestMessage(int src, int dst) { SourceTubeId = src; DestinationTubeId = dst; }
}

// Messages/PourCompletedMessage.cs
public readonly struct PourCompletedMessage
{
    public readonly int SourceTubeId;
    public readonly int DestinationTubeId;
    public readonly int LayersPoured;
    // Add snapshot of new tube states for presenters
}

public readonly struct GameWonMessage { }
public readonly struct GameLostMessage { }
public readonly struct RestartRequestMessage { }
public readonly struct NextLevelRequestMessage { }
```

> **Boss Room Inspiration:** Boss Room uses `NetworkedMessageChannel<T>` for messaging. Here we use a simpler in-process version with the same idea — typed messages, subscribe/unsubscribe, publish.

---

## 7. VContainer Setup & Scopes

### AppLifetimeScope.cs (Root — lives in Main.unity, persistent)
```csharp
public class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Singleton services shared across scenes
        builder.Register<IMessageBus, MessageBus>(Lifetime.Singleton);
        builder.Register<ILevelService, LevelService>(Lifetime.Singleton);
        // Register GameConfig SO
        builder.RegisterInstance(gameConfig); // drag in inspector
    }
}
```

### GameplayLifetimeScope.cs (Per-game scene)
```csharp
public class GameplayLifetimeScope : LifetimeScope
{
    [SerializeField] GameConfig gameConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        // Set parent to AppLifetimeScope (drag in inspector or set via code)

        // Domain services (scoped to gameplay)
        builder.Register<IMoveValidator, MoveValidator>(Lifetime.Singleton);
        builder.Register<IWinChecker, WinChecker>(Lifetime.Singleton);

        // State machine & states
        builder.Register<StateMachine>(Lifetime.Singleton);
        builder.Register<GameplayState>(Lifetime.Singleton);
        builder.Register<WinState>(Lifetime.Singleton);
        builder.Register<LoseState>(Lifetime.Singleton);

        // Presenters
        builder.Register<BoardPresenter>(Lifetime.Singleton);

        // EntryPoint — GameplayController drives everything
        builder.RegisterEntryPoint<GameplayController>(Lifetime.Singleton);
    }
}
```

### GameplayController.cs (The brain)
```csharp
public class GameplayController : IStartable, IDisposable
{
    readonly StateMachine _stateMachine;
    readonly GameplayState _gameplayState;
    readonly IMessageBus _bus;

    public GameplayController(StateMachine sm, GameplayState gps, IMessageBus bus)
    {
        _stateMachine = sm;
        _gameplayState = gps;
        _bus = bus;
    }

    public void Start()
    {
        _bus.Subscribe<RestartRequestMessage>(OnRestart);
        _stateMachine.TransitionTo(_gameplayState);
    }

    void OnRestart(RestartRequestMessage _) => _stateMachine.TransitionTo(_gameplayState);

    public void Dispose()
    {
        _bus.Unsubscribe<RestartRequestMessage>(OnRestart);
    }
}
```

---

## 8. Scene & Prefab Layout

### Scenes

| Scene | LifetimeScope | Contains |
|-------|--------------|----------|
| `Main.unity` | `AppLifetimeScope` | Persistent objects, music, DontDestroyOnLoad |
| `MainMenu.unity` | *(none or simple)* | Menu UI |
| `Gameplay.unity` | `GameplayLifetimeScope` | Board, tubes, HUD |

### Tube Prefab Structure
```
Tube (GameObject)
├── TubeView.cs (MonoBehaviour) ← handles click, triggers animation
├── Background (sprite — glass tube visual)
├── LiquidLayers (parent)
│   ├── LiquidLayer_0 (bottom)
│   ├── LiquidLayer_1
│   ├── LiquidLayer_2
│   └── LiquidLayer_3 (top)
└── SelectionIndicator (highlight ring, hidden by default)
```

### TubeView.cs sketch
```csharp
public class TubeView : MonoBehaviour
{
    [SerializeField] LiquidLayerView[] layers; // 4 layers
    [SerializeField] GameObject selectionIndicator;

    IMessageBus _bus; // injected

    [Inject]
    public void Construct(IMessageBus bus) => _bus = bus;

    void OnMouseDown() // or use IPointerClickHandler
    {
        _bus.Publish(new TubeSelectedMessage(TubeId));
    }

    public void SetSelected(bool selected)
    {
        selectionIndicator.SetActive(selected);
        // optional: DOTween bounce animation
    }

    public void UpdateDisplay(Tube tubeData)
    {
        // Update each LiquidLayerView based on tube stack
        // Animate the pour using DOTween or coroutine
    }
}
```

---

## 9. Implementation Hints & Guides

### 🟢 Start Here (Day 1)
1. **Build Tube.cs first** — no Unity, no DI. Just a class with a `Stack<TubeColor>`. Write tests or debug.log to verify pour logic works.
2. **Build MoveValidator.cs** — again pure C#. Test edge cases: empty tube, full tube, color mismatch.
3. **Build WinChecker.cs** — check that every tube is either empty or all same color.

> This is the "core" — if the logic here is wrong, nothing else matters. Get it bulletproof first.

### 🟡 Day 2: Connect to Unity
4. Create **LevelData ScriptableObjects** for 2-3 test levels manually.
5. Create a **TubeView prefab** — just colored quads or sprites, no animation yet.
6. Create **BoardPresenter** — reads LevelData, spawns TubeView prefabs, feeds them data from Tube domain models.

### 🟠 Day 3: Wiring
7. Set up **VContainer scopes** (AppLifetimeScope + GameplayLifetimeScope).
8. Set up **MessageBus** and wire TubeView → GameplayController → domain → BoardPresenter.
9. Make a complete move work end-to-end without animation.

### 🔵 Day 4: State Machine + UI
10. Implement **StateMachine** with GameplayState, WinState, LoseState.
11. Add **Win/Lose screen** UI that responds to GameWonMessage / GameLostMessage.
12. Add Restart and Next Level buttons that publish messages.

### 🟣 Day 5: Polish
13. Add **pour animations** (DOTween recommended — arc the liquid from tube A to tube B).
14. Add **selection highlight** when a tube is selected.
15. Add **particle effects** when a tube completes (all one color).
16. Add sound effects.

---

### 🎯 Key Design Decisions to Make

**Q: How does TubeView know its ID?**
> Assign it during `BoardPresenter.SetupLevel()`. Each spawned TubeView gets `tubeId = i`.

**Q: Where does click input live?**
> In `TubeView` (MonoBehaviour). It just publishes a message. It knows nothing about game rules.

**Q: Where does selection logic live?**
> In `GameplayController` or `GameplayState`. It tracks which tube was selected first and second.

**Q: How does pour animation coordinate with game state?**
> Two options:
> - Simple: Play animation, game state updates immediately. (Good for prototype)
> - Clean: Lock input during animation. Use `async/await` + `UniTask` or a coroutine. Publish "PourAnimationCompleted" when done, then check win.

**Q: What if the player selects the same tube twice?**
> Deselect it (set selection back to null). Publish a `TubeDeselectedMessage` or handle in GameplayState.

---

### ⚠️ Common Pitfalls

| Pitfall | Fix |
|---------|-----|
| Putting game logic in MonoBehaviours | Move all rules to pure C# services |
| Tight coupling between View and Model | Always go through messages or presenters |
| Forgetting to Unsubscribe from MessageBus | Always unsubscribe in `OnDestroy` or `Dispose()` |
| Animating and updating state simultaneously | Sequence them: update state → then animate |
| Making Tube a MonoBehaviour | Keep it pure C# — it's just data + logic |

---

## 10. Bonus: Level Generation

For the "auto-generate levels" bonus point:

```csharp
public class LevelGenerator
{
    public LevelData Generate(int colorCount, int emptyTubes, int seed = -1)
    {
        // 1. Create colorCount * 4 tiles (4 per color)
        // 2. Shuffle them (use seed for reproducibility)
        // 3. Distribute 4 tiles per tube into colorCount tubes
        // 4. Add emptyTubes empty tubes
        // 5. Validate: ensure level is solvable (optional — hard, skip for now)
        //    Simple approach: always generate, and let the retry button handle unsolvable
    }
}
```

**Hint:** Register `LevelGenerator` as a Singleton in VContainer. Call it from `LevelService` when no `LevelData` asset is found for the requested level number.

```csharp
// In LevelService:
public LevelData GetLevel(int index)
{
    if (index < _predefinedLevels.Count)
        return _predefinedLevels[index];

    // Auto-generate beyond predefined levels
    int colorCount = Mathf.Min(4 + index, 10); // scale difficulty
    return _generator.Generate(colorCount, emptyTubes: 2, seed: index);
}
```

---

## 11. SOLID Checklist

| Principle | How it's applied |
|-----------|-----------------|
| **S** — Single Responsibility | `Tube` = data only. `MoveValidator` = validation only. `TubeView` = visuals only. |
| **O** — Open/Closed | Add new states by implementing `IState`, not modifying `StateMachine`. Add new win conditions by implementing `IWinChecker`. |
| **L** — Liskov Substitution | All `IState` implementations are interchangeable in `StateMachine`. Mock `IMoveValidator` in tests. |
| **I** — Interface Segregation | `IMoveValidator` only validates. `IWinChecker` only checks win. `ILevelService` only provides levels. No fat interfaces. |
| **D** — Dependency Inversion | `GameplayController` depends on `IMessageBus`, not `MessageBus`. `GameplayState` depends on `IMoveValidator`, not `MoveValidator`. VContainer injects all. |

---

## 12. Recommended Implementation Order

```
Week 1 — Core & Skeleton
  ☐ 1. Domain models: TubeColor, Tube, LevelData
  ☐ 2. Services: MoveValidator, WinChecker
  ☐ 3. MessageBus (IMessageBus + MessageBus)
  ☐ 4. StateMachine + IState
  ☐ 5. LevelData ScriptableObjects (2-3 hand-crafted levels)

Week 2 — Unity Integration
  ☐ 6. TubeView prefab (static visuals, no animation)
  ☐ 7. BoardPresenter (spawn + layout tubes)
  ☐ 8. VContainer scopes (App + Gameplay)
  ☐ 9. GameplayController as EntryPoint
  ☐ 10. Full move flow working end-to-end (click → validate → update → display)

Week 3 — States & UI
  ☐ 11. GameplayState, WinState, LoseState
  ☐ 12. Win/Lose UI screens
  ☐ 13. Restart + Next Level buttons
  ☐ 14. Main Menu scene

Week 4 — Polish & Bonus
  ☐ 15. Pour animation (DOTween arc or Lerp)
  ☐ 16. Particle FX on tube completion
  ☐ 17. Sound effects (click, pour, win)
  ☐ 18. Level auto-generation (bonus)
  ☐ 19. Multiple level layouts (bonus)
```

---

## 📚 References

| Resource | Link |
|----------|------|
| Boss Room Sample | https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop |
| VContainer Docs | https://vcontainer.hadashikick.jp |
| DOTween | http://dotween.demigiant.com |
| UniTask (async Unity) | https://github.com/Cysharp/UniTask |
| Unity ScriptableObjects | https://docs.unity3d.com/Manual/class-ScriptableObject.html |

---

> **Final advice:** Build the domain layer first and test it in isolation. Once the pure C# logic is solid, connecting Unity visuals is just plumbing. The architecture above is designed so that every layer is independently testable and replaceable — that's what makes it "clean code" the evaluators are looking for.

*Good luck! 🚀*
