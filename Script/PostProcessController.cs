using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;
    public bool colorGradingOff;
    void Start()
    {
        postProcessVolume= GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out colorGrading);
    }

    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            colorGradingOff = !colorGradingOff;
        }

        colorGrading.active = colorGradingOff;
    }
}
