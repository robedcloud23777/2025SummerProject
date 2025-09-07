using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;

    private void Start()
    {
        HealthManager.Instance.SetSliders(slider1, slider2);
    }
}
