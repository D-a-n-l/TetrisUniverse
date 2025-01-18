using UnityEngine;
using UnityEngine.UI;

public class StatView : MonoBehaviour
{
    [SerializeField]
    private Stat _stat;

    [SerializeField]
    private Text _text;

    public void Refresh()
    {
        _text.text = _stat.Current.ToString();
    }
}