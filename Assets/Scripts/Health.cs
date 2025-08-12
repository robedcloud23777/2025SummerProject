using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    public int maxhp = 20;
    public Slider HpSlider;
    int hp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HpSlider.value = (float)maxhp / (float)hp;
    }
}
