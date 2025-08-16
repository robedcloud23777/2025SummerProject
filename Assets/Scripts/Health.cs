using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxhp = 20;
    public Slider HpSlider;
    int hp;

    float displayHP;
    public float speed = 15f;

    void Start()
    {
        hp = maxhp;
        displayHP = maxhp;

        HpSlider.minValue = 0;
        HpSlider.maxValue = maxhp;
        HpSlider.value = maxhp;
    }

    void Update()
    {
       
        //if (Input.GetMouseButtonDown(0))
        //{
         //   hp = Mathf.Max(0, hp - 5);
        //}

        displayHP = Mathf.Lerp(displayHP, hp, Time.deltaTime * speed);

        HpSlider.value = displayHP;
    }
}

