using UnityEngine;

public class InjectObject : MonoBehaviour
{
    private void Awake() => SceneContextSingleton.Instance.Container.InjectGameObject(gameObject);
}