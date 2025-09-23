using UnityEngine;
using UnityEngine.UI;

public class LoopTextura : MonoBehaviour 
{
    public float interval = 1f;
    private float aux = 0f;

    public Sprite[] ImagesPC;
    public Sprite[] ImagesMobile;
    private int Contador = 0;

    private Sprite[] currentImages;
    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (GameManager.Instancia.IsPlatformPC())
            currentImages = ImagesPC;
        else
            currentImages = ImagesMobile;

        if (currentImages.Length > 0 && imageComponent != null)
            imageComponent.sprite = currentImages[0];
    }

    void Update()
    {
        aux += Time.deltaTime;

        if (aux >= interval)
        {
            aux = 0f;
            Contador++;
            if (Contador >= currentImages.Length)
            {
                Contador = 0;
            }
            if (imageComponent != null)
                imageComponent.sprite = currentImages[Contador];
        }
    }
}
