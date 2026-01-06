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

    [Header("Camera Settings")]
    public int cameraWidth = 640;
    public int cameraHeight = 480;

    private WebCamTexture webCamTexture;
    private BarcodeReader barcodeReader;
    private bool isScanning = false;
    private string scannedBarcode = "";

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

        UpdateStatus("Ready to scan barcode");
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

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            UpdateStatus("No camera found");
            return;
        }

        string cameraName = devices[0].name;
        webCamTexture = new WebCamTexture(cameraName, cameraWidth, cameraHeight);

        if (cameraDisplay != null)
        {
            cameraDisplay.texture = webCamTexture;
        }

        webCamTexture.Play();
        isScanning = true;
        UpdateStatus("Scanning... Point camera at barcode");

        StartCoroutine(ScanForBarcode());
    }

    public void StopScanning()
    {
        if (!isScanning) return;

        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }

        isScanning = false;
        scannedBarcode = "";
        UpdateStatus("Scan cancelled");

        if (cameraDisplay != null)
        {
            cameraDisplay.texture = null;
        }
    }

    private IEnumerator ScanForBarcode()
    {
        while (isScanning && webCamTexture != null)
        {
            yield return new WaitForSeconds(0.5f);

            if (!webCamTexture.isPlaying) continue;

            try
            {
                Color32[] pixels = webCamTexture.GetPixels32();
                if (pixels == null || pixels.Length == 0) continue;

                var result = barcodeReader.Decode(pixels, webCamTexture.width, webCamTexture.height);

                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    scannedBarcode = result.Text;
                    OnBarcodeScanned?.Invoke(scannedBarcode);
                    StopScanning();
                    UpdateStatus("Barcode scanned: " + scannedBarcode);
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Barcode scanning error: " + e.Message);
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

    public string GetLastScannedBarcode()
    {
        return scannedBarcode;
    }

    public bool IsScanning()
    {
        return isScanning;
    }
}
