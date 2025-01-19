using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NextFigureView : MonoBehaviour
{
    [SerializeField]
    private Transform _root;

    [SerializeField]
    private bool _isSpin = true;

    private GameObject _previousNextShape;

    private Spawner _spawner;

    [Inject]
    public void Construct(Spawner spawner) 
    {  
        _spawner = spawner; 
    }

    private void Start()
    {
        Subscription();
    }

    public void Subscription()
    {
        _spawner.OnSpawned += DisplayNextShape;
    }

    public void Unsubscription()
    {
        _spawner.OnSpawned -= DisplayNextShape;
    }

    private void OnDisable()
    {
        Unsubscription();
    }

    public void DestroyPrevious()
    {
        Destroy(_previousNextShape);
    }

    public void DisplayNextShape(Figure shape)
    {
        if (_previousNextShape != null)
            DestroyPrevious();

        Figure gObject = Instantiate(shape, _root);
        gObject.gameObject.AddComponent<RectTransform>();
        //gObject.transform.localScale = new Vector3(100, 100, 100);

        for (int i = 0; i < gObject.Tiles.Length; i++)
        {
            gObject.Tiles[i].gameObject.layer = _root.gameObject.layer;
        }

        gObject.gameObject.GetComponent<MovementFigure>().enabled = false;
        //gObject.gameObject.GetComponent<RotationFigureMany>().enabled = false;

        if (_isSpin == true)
            gObject.gameObject.AddComponent<Spinning>().speed = 20;

        //Spinning spinning = gObject.AddComponent<Spinning>();
        //spinning.speed = nextShapeSpinningSpeed;

        _previousNextShape = gObject.gameObject;
    }
}