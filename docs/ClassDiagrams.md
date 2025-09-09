## 🤖 AI Generated C# Class Diagrams\n\n### main/RollingDice.cs\n<details><summary>Expand</summary>\n\n```mermaid\n```mermaid
classDiagram
    class RollingDice {
        - const float ROLL_TIME
        - const float RESULT_TIME
        - List~Sprite~ _diceSprites
        - Image _diceDisplay
        - Button _diceBtn
        - readonly Subject~int~ _diceResult
        - readonly Subject~Unit~ _startDiceRoll
        + IObservable~int~ DiceResult
        + IObservable~Unit~ StartDiceRoll
        - Start()
        - StartDiceRollAsync() UniTaskVoid
        - AnimateDiceRollingAsync(TimeSpan duration) UniTask~int~
        - ShowResultAsync(int diceNumber, TimeSpan duration) UniTask
        - SetDiceFace(int diceNumber)
    }

    MonoBehaviour <|-- RollingDice
```\n```\n</details>\n\n
