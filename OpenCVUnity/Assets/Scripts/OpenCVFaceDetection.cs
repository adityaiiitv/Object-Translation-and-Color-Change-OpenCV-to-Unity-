using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
// Define the functions which can be called from the .dll.
internal static class OpenCVInterop
{
   
    [DllImport("OpenCVnUnity")]
    internal static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("OpenCVnUnity")]
    internal static extern int Close();

    [DllImport("OpenCVnUnity")]
    internal static extern int SetScale(int downscale);

    [DllImport("OpenCVnUnity")]
    internal unsafe static extern void Detect(CvCircle* outFaces, int maxOutFacesCount, ref int outDetectedFacesCount, ref int cR, ref int cG, ref int cB);
}

// Define the structure to be sequential and with the correct byte size (3 ints = 4 bytes * 3 = 12 bytes)
[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct CvCircle
{
    public int X, Y, Radius;
}

public class OpenCVFaceDetection : MonoBehaviour
{
    public static List<Vector2> NormalizedFacePositions { get; private set; }
    public static Vector2 CameraResolution;
    public Renderer spherecol;
    int cR, cG, cB;
    /// <summary>
    /// Downscale factor to speed up detection.
    /// </summary>
    private const int DetectionDownScale = 1;

    private bool _ready;
    private int _maxFaceDetectCount = 5;
    private CvCircle[] _faces;

    void Start()
    {
        cR = 0; cG = 0; cB = 0;
        print("In start Openc");
        int camWidth = 0, camHeight = 0;
        int result = OpenCVInterop.Init(ref camWidth, ref camHeight);
        print("dll import done");
        if (result < 0)
        {
            if (result == -1)
            {
                Debug.LogWarningFormat("[{0}] Failed to find cascades definition.", GetType());
            }
            else if (result == -2)
            {
                Debug.LogWarningFormat("[{0}] Failed to open camera stream.", GetType());
            }

            return;
        }
        spherecol = GetComponent<Renderer>();
        CameraResolution = new Vector2(camWidth, camHeight);
        _faces = new CvCircle[_maxFaceDetectCount];
        NormalizedFacePositions = new List<Vector2>();
        OpenCVInterop.SetScale(DetectionDownScale);
        _ready = true;
        print("ending start");
    }

    void OnApplicationQuit()
    {
        if (_ready)
        {
            Debug.Log("sdfsdf");
            OpenCVInterop.Close();
        }
    }

    void Update()
    {
        if (!_ready)
            return;
        int detectedFaceCount = 0;
        unsafe
        {
            fixed (CvCircle* outFaces = _faces)
            {
                // Call detect R,G,B is actually B,G,R
                OpenCVInterop.Detect(outFaces, _maxFaceDetectCount, ref detectedFaceCount, ref cR, ref cG, ref cB);
                // Change the color to blue
                //spherecol.material.color = new Color32((byte)1, (byte)cB, (byte)cG, (byte)cR);
                spherecol.material.color = new Color32((byte)cB, (byte)cG, (byte)cR, (byte)1);
            }
        }
        NormalizedFacePositions.Clear();
        NormalizedFacePositions.Add(new Vector2((_faces[0].X * DetectionDownScale) / CameraResolution.x, 1f - ((_faces[0].Y * DetectionDownScale) / CameraResolution.y)));
    }
}
