using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class OptionValueChange : MonoBehaviour
{
    public GameObject mainCameraObject;

    public GameObject screenModeOption;
    public GameObject resolutionOption;
    public GameObject FPSOption;
    public GameObject postProcessingOption;
    public GameObject antiAlliasingOption;
    public GameObject textureOption;
    public GameObject shadowOption;
    public GameObject motionBlurOption;
    public GameObject FOVOption;
    public GameObject occlusionCulllingOption;

    private Camera mainCamera;
    private PostProcessLayer mainPostLayer;
    private PostProcessVolume mainPostVolume;
    private Vector2 currentResolution;
    private FullScreenMode currentScreenMode;

    private bool cameraInit;
    private bool changePostProcessingDirect;

    void Awake()
    {
        if (!mainCameraObject)
        {
            cameraInit = false;
            Debug.LogWarning("Camera is Null");
        }
        else
        {
            cameraInit = true;
            changePostProcessingDirect = false;
            mainCamera = mainCameraObject.GetComponent<Camera>();
            mainPostLayer = mainCameraObject.GetComponent<PostProcessLayer>();
            mainPostVolume = mainCameraObject.GetComponent<PostProcessVolume>();
        }
        
        if (screenModeOption) screenModeOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangeScreenMode);
        else Debug.LogWarning("screenModeOption is Null");

        if (resolutionOption) resolutionOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangeResolution);
        else Debug.LogWarning("resolutionOption is Null");

        if (FPSOption) FPSOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangeFPSSetting);
        else Debug.LogWarning("FPSOption is Null");
        
        if (postProcessingOption) postProcessingOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangePostProcessing);
        else Debug.LogWarning("postProcessingOption is Null");

        if (antiAlliasingOption) antiAlliasingOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangeAntiAlliasing);
        else Debug.LogWarning("antiAlliasingOption is Null");

        if (textureOption) textureOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangeTexture);
        else Debug.LogWarning("textureOption is Null");

        if (shadowOption) shadowOption.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(ChangeShadow);
        else Debug.LogWarning("shadowOption is Null");

        if (motionBlurOption) motionBlurOption.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(SetMotionBlur);
        else Debug.LogWarning("motionBlurOption is Null");

        if (FOVOption) FOVOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener(ChangeFOV);
        else Debug.LogWarning("FOVOption is Null");

        if (occlusionCulllingOption) occlusionCulllingOption.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(SetOcclusionCulling);
        else Debug.LogWarning("occlusionCulllingOption is Null");
    }

    public void ChangeScreenMode(Int32 value)
    {
        switch (value)
        {
            case 0: currentScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: currentScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: currentScreenMode = FullScreenMode.Windowed; break;
        }

        Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, currentScreenMode);
    }

    public void ChangeResolution(Int32 value)
    {
        switch (value)
        {
            case 0: currentResolution = new Vector2(2560.0f, 1440.0f); break;
            case 1: currentResolution = new Vector2(1920.0f, 1080.0f); break;
            case 2: currentResolution = new Vector2(1280.0f, 720.0f); break;
        }

        Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, currentScreenMode);
    }

    public void ChangeFPSSetting(Int32 value)
    {
        switch (value)
        {
            case 0: Application.targetFrameRate = -1; break;
            case 1: Application.targetFrameRate = 144; break;
            case 2: Application.targetFrameRate = 120; break;
            case 3: Application.targetFrameRate = 90; break;
            case 4: Application.targetFrameRate = 60; break;
            case 5: Application.targetFrameRate = 30; break;
        }
    }

    public void ChangePostProcessing(Int32 value)
    {
        if (cameraInit && value != 0)
        {
            changePostProcessingDirect = true;
            ChangeValueToOptionObject(antiAlliasingOption, value - 1);
            ChangeValueToOptionObject(textureOption, value - 1);
            ChangeValueToOptionObject(shadowOption, value - 1);
            changePostProcessingDirect = false;
        }
    }

    public void ChangeAntiAlliasing(Int32 value)
    {
        if (!cameraInit) return;
        if (!changePostProcessingDirect) ChangeValueToOptionObject(postProcessingOption, 0);

        switch (value)
        {
            case 0: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.None; break;
            case 1: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing; break;
            case 2: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing; break;
            case 3: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing; break;
        }
    }

    public void ChangeTexture(Int32 value)
    {
        if (!cameraInit) return;
        if (!changePostProcessingDirect) ChangeValueToOptionObject(postProcessingOption, 0);
    }

    public void ChangeShadow(Int32 value)
    {
        if (!cameraInit) return;
        if (!changePostProcessingDirect) ChangeValueToOptionObject(postProcessingOption, 0);
    }

    public void SetMotionBlur(Boolean value)
    {
        if (!cameraInit) return;

        MotionBlur motionBlur;
        if (mainPostVolume.profile.TryGetSettings<MotionBlur>(out motionBlur))
        {
            motionBlur.active = value;
        }
    }

    public void ChangeFOV(Single value)
    {
        if (!cameraInit) return;

        mainCamera.fieldOfView = value;
    }

    public void SetOcclusionCulling(Boolean value)
    {
        if (!cameraInit) return;

        mainCamera.useOcclusionCulling = value;
    }

    public void ChangeValueToOptionObject(GameObject obj, int value)
    {
        if (!obj) return;

        GameObject targetObj = obj.transform.Find("Dropdown").gameObject;
        targetObj.GetComponent<TMP_Dropdown>().value = value;
    }
}
