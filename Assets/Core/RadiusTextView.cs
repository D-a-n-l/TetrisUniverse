using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RadiusTextView : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    [Space(5)]
    [Range(0, 2)]
    [SerializeField]
    private int _side;

    private TetrisGrid _grid;

    [Inject]
    public void Construct(TetrisGrid grid)
    {
        _grid = grid;
    }

    private void Start()
    {
        Subscription();
    }

    private void Subscription()
    {
        //switch (_side)
        //{
        //    case 0:
        //        _grid.OnRadiusX += (string text) => _text.text = text;

        //        break;
        //    case 1:
        //        _grid.OnRadiusY += (string text) => _text.text = text;

        //        break;
        //    case 2:
        //        _grid.OnRadiusZ += (string text) => _text.text = text;

        //        break;
        //}
    }
}