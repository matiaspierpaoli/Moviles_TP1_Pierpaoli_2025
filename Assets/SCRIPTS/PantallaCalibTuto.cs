using UnityEngine;
using System.Collections;

public class PantallaCalibTuto : MonoBehaviour
{
    public Texture2D[] TutorialImagesPC;
    public Texture2D[] TutorialImagesMobile;
    public float interval = 1.2f;
    float aux = 0;
    int currentImageIndex = 0;

    public bool isPlayer1 = false;

    public Texture2D PCCalibImagePlayer1;
    public Texture2D PCCalibImagePlayer2;
    public Texture2D MobileCalibImage;

    public Texture2D ImaReady;

    public ContrCalibracion ContrCalib;

    private Texture2D[] currentImages;

    private void Awake()
    {
        if (GameManager.Instancia.IsPlatformPC())
            currentImages = TutorialImagesPC;
        else
            currentImages = TutorialImagesMobile;
    }

    void Update()
    {
        switch (ContrCalib.EstAct)
        {
            case ContrCalibracion.Estados.Calibrating:
                if (GameManager.Instancia.IsPlatformPC())
                {
                    if (isPlayer1)
                        GetComponent<Renderer>().material.mainTexture = PCCalibImagePlayer1;
                    else
                        GetComponent<Renderer>().material.mainTexture = PCCalibImagePlayer2;
                }
                else
                    GetComponent<Renderer>().material.mainTexture = MobileCalibImage;
                break;

            case ContrCalibracion.Estados.Tutorial:

                aux += T.GetDT();
                if (aux >= interval)
                {
                    aux = 0;
                    if (currentImageIndex + 1 < currentImages.Length)
                        currentImageIndex++;
                    else
                        currentImageIndex = 0;
                }
                GetComponent<Renderer>().material.mainTexture = currentImages[currentImageIndex];

                break;
            case ContrCalibracion.Estados.Finished:
                GetComponent<Renderer>().material.mainTexture = ImaReady;
                break;
        }
    }
}
