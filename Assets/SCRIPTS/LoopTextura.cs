using UnityEngine;
using UnityEngine.UI;

public class LoopTextura : MonoBehaviour 
{
    public float interval = 1f;
    private float aux = 0f;

    public Sprite[] Images;
    private int Contador = 0;

    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (Images.Length > 0 && imageComponent != null)
            imageComponent.sprite = Images[0];
    }

    void Update()
    {
        aux += Time.deltaTime;

        if (aux >= interval)
        {
            aux = 0f;
            Contador++;
            if (Contador >= Images.Length)
            {
                Contador = 0;
            }
            if (imageComponent != null)
                imageComponent.sprite = Images[Contador];
        }
    }
}
