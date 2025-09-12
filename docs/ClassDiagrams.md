## 🤖 AI Generated C# Class Diagrams

### ./main/SugorokuModel.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class SugorokuModel {
        +int DiceValue
        +int DiceTotal
        +int PieceTileIndex
    }
    MonoBehaviour <|-- SugorokuModel
```
```
</details>

### ./main/TileManager.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class TileManager {
        -const string SHDER_PROPERTY_TEX_1
        -const string SHDER_PROPERTY_TEX_2
        -const float JUMP_TIME
        -List<GameObject> _tiles
        -Material _roadMaterial
        -List<Texture> _setTextures
        -Texture _defaultTextures
        -Shader _tileChange
        -List<Material> materials
        -int _changedCount
        +List<GameObject> Tiles
        -void Awake()
        -void SettingMaterialTile()
        -void OnDisable()
        +Tween ChangeRoadVisual(float, int)
        -void ApplyRoadMaterialChange()
        +void UpdateTiles()
        +Tween ChangeTile(float, Material)
        +void RoadMoving(bool)
    }
    TileManager --|> MonoBehaviour
    TileManager o-- GameObject
    TileManager o-- Material
    TileManager o-- Texture
    TileManager o-- Shader
    TileManager ..> DG.Tweening.Tween
    TileManager ..> DG.Tweening.Sequence
    TileManager ..> UnityEngine.Vector2
    TileManager ..> UnityEngine.Vector3
    TileManager ..> UnityEngine.Transform
    TileManager ..> UnityEngine.Renderer
    TileManager ..> UnityEngine.Random
    TileManager ..> UnityEngine.Debug
```
```
</details>

### ./main/PieceMoving.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class PieceMoving {
        -const float JUMP_POWER
        -const float JUMP_TIME
        -Transform _pieceTrans
        +List~GameObject~ Tiles
        +int CurrentTileIndex
        +ReactiveProperty~bool~ IsPieceMove
        +IObservable~(float, int)~ RoadChange
        +IObservable~Unit~ GetTile
        -int _currentTileIndex
        -Subject~(float, int)~ _roadChange
        -Subject~Unit~ _getTile
        +void Start()
        +void ProceedPiece(int number)
        -Vector3 NextPos(int tileIndex)
    }
```
```
</details>

### ./main/SugorokuAnime.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class SugorokuAnime {
        - List<Button> _buttons
        - Animator _anime
        - Transform _playerPos
        - int _previousSign
        - string[] _animationNames
        - string[] _selectedAnimationName
        - const float FADE_TIME
        - bool _isRunning
        - void Start()
        + UniTask StartAnimation()
        - void SettingButtons()
        - UniTaskVoid PlayTemporaryAnimation(string clipName)
        - UniTask PlayAndReturnToDefault(string clipName)
        - UniTaskVoid StartRandomMovement()
        - void DefaultAnimation(int newSign)
    }
```
```
</details>

### ./main/GameOpening.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class GameOpening {
        <<MonoBehaviour>>
        - Transform _cameraTrans
        - TMP_Text _countTxt
        - Vector3 _initialPos
        - Vector3 _initialRot
        - readonly Subject<Unit> _openingEnd
        + IObservable<Unit> OpeningEnd
        - readonly Subject<Unit> _tileChange
        + IObservable<Unit> TileChange
        - readonly Subject<Unit> _goSignal
        + IObservable<Unit> GoSignal
        - Start()
        - UniTask RunOpeningAsync()
        - UniTask PlayOpeningIntroAsync()
        - PlayCountdownSequence()
        - AddCountdownStep(Sequence seq, string text, Vector3 startPos, Vector3 startRot, Vector3? moveTarget)
        - SetCameraStart(Vector3 pos, Vector3 rot)
        - SetText(string txt)
        - OnDestroy()
    }
    GameOpening --|> MonoBehaviour
```
```
</details>

### ./main/CameraWorking.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class CameraWorking {
        - Transform _target
        - Transform _mainCameraTrans
        - GameObject CameraParent
        - Vector3 _defaultPos
        - SplineAnimate _splineAnime
        - Transform _moveTrans
        - CancellationTokenSource _standbyCts
        - void Start()
        + void BeginningWork()
        - void StartStandbyCameraLoop()
        + void StopStandbyCameraLoop()
        - UniTaskVoid StandbyCameraLoopAsync(CancellationToken token)
        + void BeginDiceRollEvent(int pieceTileIndex)
        + void EndDiceRollEvent()
        + void MoveToPiecePosition(int number)
        - void UpdateCameraTargetByTileIndex(int number)
    }
```
```
</details>

### ./main/RollingDice.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class RollingDice {
        -const float ROLL_TIME
        -const float RESULT_TIME
        -List<Sprite> _diceSprites
        -Image _diceDisplay
        -Button _diceBtn
        -Button _testBtn
        -readonly Subject<int> _diceResult
        -readonly Subject<Unit> _startDiceRoll
        +IObservable<int> DiceResult
        +IObservable<Unit> StartDiceRoll
        -void Start()
        -async UniTaskVoid StartDiceRollAsync()
        -async UniTask<int> AnimateDiceRollingAsync(TimeSpan duration)
        -async UniTask ShowResultAsync(int diceNumber, TimeSpan duration)
        -void SetDiceFace(int diceNumber)
    }

    MonoBehaviour <|-- RollingDice
    RollingDice --> List~Sprite~ : uses
    RollingDice --> Image : uses
    RollingDice --> Button : uses
    RollingDice --> Subject~int~ : uses
    RollingDice --> Subject~Unit~ : uses
    RollingDice ..> IObservable~int~ : exposes
    RollingDice ..> IObservable~Unit~ : exposes
```
```
</details>

### ./main/SugorokuPresenter.cs
<details><summary>Expand</summary>

```mermaid
```mermaid
classDiagram
    class SugorokuPresenter {
        -SugorokuModel _sugorokuModel
        -TileManager _tileManager
        -SugorokuAnime _sugorokuAnime
        -RollingDice _rollingDice
        -PieceMoving _pieceMoving
        -CameraWorking _cameraWorking
        -GameOpening _gameOpening
        -void Start()
    }

    class SugorokuModel
    class TileManager
    class SugorokuAnime
    class RollingDice
    class PieceMoving
    class CameraWorking
    class GameOpening

    SugorokuPresenter --> SugorokuModel
    SugorokuPresenter --> TileManager
    SugorokuPresenter --> SugorokuAnime
    SugorokuPresenter --> RollingDice
    SugorokuPresenter --> PieceMoving
    SugorokuPresenter --> CameraWorking
    SugorokuPresenter --> GameOpening
```
```
</details>

