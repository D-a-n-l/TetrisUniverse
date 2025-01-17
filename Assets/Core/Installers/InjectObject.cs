using UnityEngine;

public class InjectObject : MonoBehaviour
{
    private void Start() => SceneContextSingleton.Instance.Container.InjectGameObject(gameObject);
}