using MatrixUtils.EventBus;
using UnityEngine;

public class GameOverNotifier : MonoBehaviour
{
    public void NotifyGameOver() => EventBus<GameOverEvent>.Raise(new());
}
