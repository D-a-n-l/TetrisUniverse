using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Spinning))]
public class NextFigureView : MonoBehaviour
{
    [SerializeField]
    private Transform _root;

    [SerializeField]
    private bool _isSpin = true;

    private GameObject _previousNextShape;

    private Spinning _spinning;

    private Spawner _spawner;

    [Inject]
    public void Construct(Spawner spawner) 
    {  
        _spawner = spawner; 
    }

    private void Start()
    {
        _spinning = GetComponent<Spinning>();

        Subscription();//in Bootstrap
    }

    public void Subscription()
    {
        _spawner.OnSpawned += DisplayNext;
    }

    public void Unsubscription()
    {
        _spawner.OnSpawned -= DisplayNext;
    }

    private void OnDisable()
    {
        Unsubscription();
    }

    public void DestroyPrevious()
    {
        Destroy(_previousNextShape);
    }

    public void DisplayNext(Figure newFigure)
    {
        if (_previousNextShape != null)
            DestroyPrevious();

        Figure figure = Instantiate(newFigure, _root);

        for (int i = 0; i < figure.Tiles.Length; i++)
        {
            figure.Tiles[i].gameObject.layer = _root.gameObject.layer;
        }

        figure.gameObject.GetComponent<MovementFigure>().enabled = false;

        if (_isSpin == false)
        {
            _root.rotation = Quaternion.identity;

            _spinning.IsSpin = false;
        }

        _previousNextShape = figure.gameObject;
    }
}