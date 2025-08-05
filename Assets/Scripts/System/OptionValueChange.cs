using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class OptionValueRecord
{
    public string language;
    public int screenMode;
    public int screenResolution;
    public int FPS;
    public float bright;
    public int postProcessing;
    public int antiAlliasing;
    public int texture;
    public int shadow;
    public bool motionBlur;
    public float FOV;
    public bool occlusionCulling;
    public float master;
    public float SE;
    public float BGM;
    public float voice;
    public float ambient;
    public float UI;
    public float sensitive;
}

public class OptionValueChange : Singleton<OptionValueChange>, IInitializeInter
{
    public PathCategory pathCategory;
    public string pathLocation;
    private string currentPath;

    // Base Object;
    public GameObject mainCameraObject;
    public AudioMixer audioMixer;
    public Image brightPanel;

    // Display
    public TMP_Dropdown screenModeOption;
    public TMP_Dropdown resolutionOption;
    public TMP_Dropdown FPSOption;
    public Slider brightOption;
    public TMP_Dropdown postProcessingOption;
    public TMP_Dropdown antiAlliasingOption;
    public TMP_Dropdown textureOption;
    public TMP_Dropdown shadowOption;
    public Toggle motionBlurOption;
    public Slider FOVOption;
    public Toggle occlusionCulllingOption;

    // Sound
    public Slider masterVolumeOption;
    public Slider SEVolumeOption;
    public Slider BGMVolumeOption;
    public Slider voiceVolumeOption;
    public Slider ambientVolumeOption;
    public Slider UIVolumeOption;

    // Control
    public Slider sensitiveOption;

    // Setting Value
    private Camera mainCamera;
    private PostProcessLayer mainPostLayer;
    private PostProcessVolume mainPostVolume;
    private FullScreenMode currentScreenMode;
    private Vector2 currentResolution;

    private OptionValueRecord optionValues;
    private bool cameraInit;
    private bool audioInit;
    private bool changePostProcessingDirect;
    [HideInInspector]
    public bool optionChanged;

    public void Initialize()
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

        if (screenModeOption) screenModeOption.onValueChanged.AddListener(ChangeScreenMode);
        else Debug.LogWarning("screenModeOption is Null");

        if (resolutionOption) resolutionOption.onValueChanged.AddListener(ChangeResolution);
        else Debug.LogWarning("resolutionOption is Null");

        if (FPSOption) FPSOption.onValueChanged.AddListener(ChangeFPSSetting);
        else Debug.LogWarning("FPSOption is Null");

        if (brightOption) brightOption.onValueChanged.AddListener((float value) => ChangeBrightValue(value));
        else Debug.LogWarning("brightOption is Null");

        if (postProcessingOption) postProcessingOption.onValueChanged.AddListener(ChangePostProcessing);
        else Debug.LogWarning("postProcessingOption is Null");

        if (antiAlliasingOption) antiAlliasingOption.onValueChanged.AddListener(ChangeAntiAlliasing);
        else Debug.LogWarning("antiAlliasingOption is Null");

        if (textureOption) textureOption.onValueChanged.AddListener(ChangeTexture);
        else Debug.LogWarning("textureOption is Null");

        if (shadowOption) shadowOption.onValueChanged.AddListener(ChangeShadow);
        else Debug.LogWarning("shadowOption is Null");

        if (motionBlurOption) motionBlurOption.onValueChanged.AddListener(SetMotionBlur);
        else Debug.LogWarning("motionBlurOption is Null");

        if (FOVOption) FOVOption.onValueChanged.AddListener(ChangeFOV);
        else Debug.LogWarning("FOVOption is Null");

        if (occlusionCulllingOption) occlusionCulllingOption.onValueChanged.AddListener(SetOcclusionCulling);
        else Debug.LogWarning("occlusionCulllingOption is Null");

        if (masterVolumeOption) masterVolumeOption.onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("Master", value));
        else Debug.LogWarning("masterVolumeOption is Null");

        if (SEVolumeOption) SEVolumeOption.onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("SE", value));
        else Debug.LogWarning("SEVolumeOption is Null");

        if (BGMVolumeOption) BGMVolumeOption.onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("BGM", value));
        else Debug.LogWarning("BGMVolumeOption is Null");

        if (voiceVolumeOption) voiceVolumeOption.onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("Voice", value));
        else Debug.LogWarning("voiceVolumeOption is Null");

        if (ambientVolumeOption) ambientVolumeOption.onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("Ambient", value));
        else Debug.LogWarning("ambientVolumeOption is Null");

        if (UIVolumeOption) UIVolumeOption.onValueChanged.AddListener((float value) => ChangeVolumeWithParameter("UI", value));
        else Debug.LogWarning("UIVolumeOption is Null");

        if (sensitiveOption) sensitiveOption.onValueChanged.AddListener((float value) => ChangeSensitive(value));
        else Debug.LogWarning("sensitiveOption is Null");

        currentResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        currentScreenMode = Screen.fullScreenMode;
        currentPath = PathManager.instance.GetPathFromCategory(pathCategory) + pathLocation;
        optionValues = SaveManager.instance.LoadData<OptionValueRecord>(currentPath);
        if (optionValues != null)
        {
            LoadOptionData();
        }
        else
        {
            optionValues = new OptionValueRecord();
            postProcessingOption.value = 4;
            SaveOptionData();
            LoadOptionData();
        }
    }

    public void SaveOptionData()
    {
        optionValues.language = JsonTranslate.instance.languageDropBox.options[JsonTranslate.instance.languageDropBox.value].text;
        optionValues.screenMode = screenModeOption.value;
        optionValues.screenResolution = resolutionOption.value;
        optionValues.FPS = FPSOption.value;
        optionValues.bright = brightOption.value;
        optionValues.postProcessing = postProcessingOption.value;
        optionValues.antiAlliasing = antiAlliasingOption.value;
        optionValues.texture = textureOption.value;
        optionValues.shadow = shadowOption.value;
        optionValues.motionBlur = motionBlurOption.isOn;
        optionValues.FOV = FOVOption.value;
        optionValues.occlusionCulling = occlusionCulllingOption.isOn;
        optionValues.master = masterVolumeOption.value;
        optionValues.SE = SEVolumeOption.value;
        optionValues.BGM = BGMVolumeOption.value;
        optionValues.voice = voiceVolumeOption.value;
        optionValues.ambient = ambientVolumeOption.value;
        optionValues.UI = UIVolumeOption.value;
        optionValues.sensitive = sensitiveOption.value;
        SaveManager.instance.SaveData(currentPath, optionValues);
    }

    public void LoadOptionData()
    {
        optionValues = SaveManager.instance.LoadData<OptionValueRecord>(currentPath);
        optionChanged = true;
        int index = JsonTranslate.instance.languageDropBox.options.FindIndex(option => option.text == optionValues.language);
        if (index >= 0)
        {
            JsonTranslate.instance.languageDropBox.value = index;
            JsonTranslate.instance.languageDropBox.RefreshShownValue();
        }
        screenModeOption.value = optionValues.screenMode;
        resolutionOption.value = optionValues.screenResolution;
        FPSOption.value = optionValues.FPS;
        brightOption.value = optionValues.bright;
        postProcessingOption.value = optionValues.postProcessing;
        antiAlliasingOption.value = optionValues.antiAlliasing;
        textureOption.value = optionValues.texture;
        shadowOption.value = optionValues.shadow;
        motionBlurOption.isOn = optionValues.motionBlur;
        FOVOption.value = optionValues.FOV;
        occlusionCulllingOption.isOn = optionValues.occlusionCulling;
        masterVolumeOption.value = optionValues.master;
        SEVolumeOption.value = optionValues.SE;
        BGMVolumeOption.value = optionValues.BGM;
        voiceVolumeOption.value = optionValues.voice;
        ambientVolumeOption.value = optionValues.ambient;
        UIVolumeOption.value = optionValues.UI;
        sensitiveOption.value = optionValues.sensitive;
        optionChanged = false;
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
        if (optionChanged == false) SaveOptionData();
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
        if (optionChanged == false) SaveOptionData();
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
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeBrightValue(Single value)
    {
        brightPanel.color = new Color32(0, 0, 0, (byte)(200 * ((100 - value) * 0.01f)));
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangePostProcessing(Int32 value)
    {
        if (cameraInit && value != 0)
        {
            changePostProcessingDirect = true;
            antiAlliasingOption.value = value - 1;
            textureOption.value = value - 1;
            shadowOption.value = value - 1;
            changePostProcessingDirect = false;
        }
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeAntiAlliasing(Int32 value)
    {
        if (!cameraInit) return;
        if (!changePostProcessingDirect) postProcessingOption.value = 0;

        switch (value)
        {
            case 0: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.None; break;
            case 1: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing; break;
            case 2: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing; break;
            case 3: mainPostLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing; break;
        }
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeTexture(Int32 value)
    {
        if (!changePostProcessingDirect) postProcessingOption.value = 0;

        int qualityValue = 0;

        switch (value)
        {
            case 0: qualityValue = 3; break;
            case 1: qualityValue = 2; break;
            case 2: qualityValue = 1; break;
            case 3: qualityValue = 0; break;
        }

        QualitySettings.globalTextureMipmapLimit = qualityValue;
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeShadow(Int32 value)
    {
        if (!changePostProcessingDirect) postProcessingOption.value = 0;

        switch (value)
        {
            case 0: QualitySettings.shadowResolution = ShadowResolution.Low; break;
            case 1: QualitySettings.shadowResolution = ShadowResolution.Medium; break;
            case 2: QualitySettings.shadowResolution = ShadowResolution.High; break;
            case 3: QualitySettings.shadowResolution = ShadowResolution.VeryHigh; break;
        }
        if (optionChanged == false) SaveOptionData();
    }

    public void SetMotionBlur(Boolean value)
    {
        MotionBlur motionBlur;
        if (mainPostVolume.profile.TryGetSettings<MotionBlur>(out motionBlur))
        {
            motionBlur.active = value;
        }
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeFOV(Single value)
    {
        if (!cameraInit) return;

        mainCamera.fieldOfView = value;
        if (optionChanged == false) SaveOptionData();
    }

    public void SetOcclusionCulling(Boolean value)
    {
        if (!cameraInit) return;

        mainCamera.useOcclusionCulling = value;
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeVolumeWithParameter(string parameter, Single value)
    {
        if (!audioInit) return;

        float DB = Mathf.Log10(Mathf.Clamp(value / 100f, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, DB);
        if (optionChanged == false) SaveOptionData();
    }

    public void ChangeSensitive(Single value)
    {
        if (optionChanged == false) SaveOptionData();
    }
}
