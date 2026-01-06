using UnityEngine;
using UnityEngine.UI;
using ZXing;
using System;
using System.Collections;

public class BarcodeScanner : MonoBehaviour
{
    [Header("UI References")]
    public RawImage cameraDisplay;
    public Button scanButton;
    public Button cancelButton;
    public Text statusText;
    public InputField manualBarcodeInput; // Fallback input for testing

    [Header("Camera Settings")]
    public int cameraWidth = 640;
    public int cameraHeight = 480;

    private WebCamTexture webCamTexture;
    private BarcodeReader barcodeReader;
    private bool isScanning = false;

    public delegate void BarcodeScannedCallback(string barcode);
    public event BarcodeScannedCallback OnBarcodeScanned;

    void Start()
    {
        barcodeReader = new BarcodeReader();
        SetupUI();
        RequestCameraPermission();
    }

    void SetupUI()
    {
        if (scanButton != null)
        {
            scanButton.onClick.AddListener(StartScanning);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(StopScanning);
        }

        if (manualBarcodeInput != null)
        {
            manualBarcodeInput.onEndEdit.AddListener(OnManualBarcodeEntered);
        }

        UpdateStatus("Ready to scan barcode");
    }

    void OnManualBarcodeEntered(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            OnBarcodeScanned?.Invoke(input);
            UpdateStatus("Manual barcode entered: " + input);
        }
    }

    void RequestCameraPermission()
    {
        #if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
        }
        #endif
    }

    public void StartScanning()
    {
        if (isScanning) return;

        if (StartCamera())
        {
            isScanning = true;
            UpdateStatus("Scanning... Point camera at barcode");
            StartCoroutine(ScanForBarcode());
        }
        else
        {
            UpdateStatus("Failed to start camera");
        }
    }

    private bool StartCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            UpdateStatus("No camera found");
            return false;
        }

        string cameraName = devices[0].name;
        webCamTexture = new WebCamTexture(cameraName, cameraWidth, cameraHeight);

        if (cameraDisplay != null)
        {
            cameraDisplay.texture = webCamTexture;
        }

        webCamTexture.Play();
        return true;
    }

    public void StopScanning()
    {
        if (!isScanning) return;

        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }

        isScanning = false;
        UpdateStatus("Scan cancelled");

        if (cameraDisplay != null)
        {
            cameraDisplay.texture = null;
        }
    }

    private Color32[] ScanFrame()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            return webCamTexture.GetPixels32();
        }
        return null;
    }

    private string ParseResult(Color32[] pixels)
    {
        if (pixels == null || pixels.Length == 0) return null;

        try
        {
            var result = barcodeReader.Decode(pixels, webCamTexture.width, webCamTexture.height);
            return result?.Text;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Barcode parsing error: " + e.Message);
            return null;
        }
    }

    private IEnumerator ScanForBarcode()
    {
        while (isScanning && webCamTexture != null)
        {
            yield return new WaitForSeconds(0.5f);

            if (!webCamTexture.isPlaying) continue;

            Color32[] pixels = ScanFrame();
            string result = ParseResult(pixels);

            if (!string.IsNullOrEmpty(result))
            {
                OnBarcodeScanned?.Invoke(result);
                StopScanning();
                UpdateStatus("Barcode scanned: " + result);
                break;
            }
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log("BarcodeScanner: " + message);
    }

    void OnDestroy()
    {
        StopScanning();
    }

    public bool IsScanning()
    {
        return isScanning;
    }
}
