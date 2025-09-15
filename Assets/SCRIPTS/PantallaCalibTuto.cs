using UnityEngine;
using System.Collections;

public class PantallaCalibTuto : MonoBehaviour
{
    public Texture2D[] TutorialImages;
    public float interval = 1.2f;
    float aux = 0;
    int currentImageIndex = 0;

    public bool isPlayer1 = false;

    public Texture2D PCCalibImage;
    public Texture2D MobileCalibImage;

    public Texture2D ImaReady;

    public ContrCalibracion ContrCalib;

    void Update()
    {
        switch (ContrCalib.EstAct)
        {
            case ContrCalibracion.Estados.Calibrating:
                if (GameManager.Instancia.IsPlatformPC())
                {
                    if (isPlayer1)
                        GetComponent<Renderer>().material.mainTexture = PCCalibImage;
                    else
                        GetComponent<Renderer>().material.mainTexture = MobileCalibImage;
                }
                else
                    GetComponent<Renderer>().material.mainTexture = MobileCalibImage;
                break;

            case ContrCalibracion.Estados.Tutorial:
                aux += T.GetDT();
                if (aux >= interval)
                {
                    aux = 0;
                    if (currentImageIndex + 1 < TutorialImages.Length)
                        currentImageIndex++;
                    else
                        currentImageIndex = 0;
                }
                GetComponent<Renderer>().material.mainTexture = TutorialImages[currentImageIndex];

                break;

            case ContrCalibracion.Estados.Finished:
                GetComponent<Renderer>().material.mainTexture = ImaReady;
                break;
        }
    }
}
