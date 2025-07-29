using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class OptionValueChange : MonoBehaviour
{
    // Base Object;
    public GameObject mainCameraObject;
    public AudioMixer audioMixer;

    // Display
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

    // Sound
    public GameObject masterVolumeOption;
    public GameObject SEVolumeOption;
    public GameObject BGMVolumeOption;
    public GameObject voiceVolumeOption;
    public GameObject ambientVolumeOption;
    public GameObject UIVolumeOption;

    // Setting Value
    private Camera mainCamera;
    private PostProcessLayer mainPostLayer;
    private PostProcessVolume mainPostVolume;
    private Vector2 currentResolution;
    private FullScreenMode currentScreenMode;

    private bool cameraInit;
    private bool audioInit;
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

        if (!audioMixer)
        {
            audioInit = false;
            Debug.LogWarning("AudioMixer is Null");
        }
        else audioInit = true;

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

        if (masterVolumeOption) masterVolumeOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("Master", value));
        else Debug.LogWarning("masterVolumeOption is Null");

        if (SEVolumeOption) SEVolumeOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("SE", value));
        else Debug.LogWarning("SEVolumeOption is Null");

        if (BGMVolumeOption) BGMVolumeOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("BGM", value));
        else Debug.LogWarning("BGMVolumeOption is Null");

        if (voiceVolumeOption) voiceVolumeOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("Voice", value));
        else Debug.LogWarning("voiceVolumeOption is Null");

        if (ambientVolumeOption) ambientVolumeOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("Ambient", value));
        else Debug.LogWarning("ambientVolumeOption is Null");

        if (UIVolumeOption) UIVolumeOption.transform.Find("SliderArea").Find("Slider").GetComponent<Slider>().onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("UI", value));
        else Debug.LogWarning("UIVolumeOption is Null");
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
        if (!changePostProcessingDirect) ChangeValueToOptionObject(postProcessingOption, 0);

        int qualityValue = 0;

        switch (value)
        {
            case 0: qualityValue = 3; break;
            case 1: qualityValue = 2; break;
            case 2: qualityValue = 1; break;
            case 3: qualityValue = 0; break;
        }

        QualitySettings.globalTextureMipmapLimit = qualityValue;
    }

    public void ChangeShadow(Int32 value)
    {
        if (!changePostProcessingDirect) ChangeValueToOptionObject(postProcessingOption, 0);

        switch (value)
        {
            case 0: QualitySettings.shadowResolution = ShadowResolution.Low; break;
            case 1: QualitySettings.shadowResolution = ShadowResolution.Medium; break;
            case 2: QualitySettings.shadowResolution = ShadowResolution.High; break;
            case 3: QualitySettings.shadowResolution = ShadowResolution.VeryHigh; break;
        }
    }

    public void SetMotionBlur(Boolean value)
    {
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

    private void ChangeValueToOptionObject(GameObject obj, int value)
    {
        if (!obj) return;

        GameObject targetObj = obj.transform.Find("Dropdown").gameObject;
        targetObj.GetComponent<TMP_Dropdown>().value = value;
    }

    public void ChangeVolumeWithParameter(string parameter, Single value)
    {
        if (!audioInit) return;

        float DB = Mathf.Log10(Mathf.Clamp(value / 100f, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, DB);
    }
}
