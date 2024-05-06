using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using CJM.BBox2DToolkit;
using System;
using System.Collections;

namespace OpenCVForUnityExample
{
    public class QRCodeCameraDetect : MonoBehaviour
    {
        [SerializeField, Tooltip("Camera for capturing images")]
        private Camera carCamera;
        [SerializeField, Tooltip("Visualizes detected object bounding boxes")]
        private QRcodeVisualizer qrcodeVisualizer;

        Texture2D imgTexture;
        Mat rgbaMat;
        Mat grayMat;
        Mat points;
        QRCodeDetector detector;
        List<string> decodedInfo;
        List<Mat> straightQrcode;
        BBox2DInfo[] qrcodeInfoArray;
        Color color;
        float detectionTimer = 0f;

        Coroutine qrCodeDetectionCoroutine = null;
        const float DETECTION_INTERVAL = 0.5f;

        // Use this for initialization
        void Start()
        {
            imgTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            rgbaMat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC4);
            grayMat = new Mat(imgTexture.height, imgTexture.width, CvType.CV_8UC1);
            points = new Mat();
            detector = new QRCodeDetector();
            decodedInfo = new List<string>();
            straightQrcode = new List<Mat>();
            color = new Color(255, 0, 0);
        }

        private void Update()
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= DETECTION_INTERVAL)
            {
                detectionTimer = 0f;
                if (qrCodeDetectionCoroutine != null)
                {
                    StopCoroutine(qrCodeDetectionCoroutine);
                }
                qrCodeDetectionCoroutine = StartCoroutine(QRCodeDetectionCoroutine());
            }
        }

        private IEnumerator QRCodeDetectionCoroutine()
        {
            CaptureCameraFrame();
            Utils.texture2DToMat(imgTexture, rgbaMat);
            Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);

            bool result = detector.detectAndDecodeMulti(grayMat, decodedInfo, points, straightQrcode);
            qrcodeInfoArray = new BBox2DInfo[points.rows()];

            if (result)
            {
                for (int i = 0; i < points.rows(); i++)
                {
                    float[] points_arr = new float[8];
                    points.get(i, 0, points_arr);

                    qrcodeInfoArray[i].bbox.x0 = Math.Min((int)points_arr[0], (int)points_arr[6]);
                    qrcodeInfoArray[i].bbox.y0 = Math.Min((int)points_arr[1], (int)points_arr[3]);
                    qrcodeInfoArray[i].bbox.width = Math.Max((int)points_arr[2], (int)points_arr[4]) - qrcodeInfoArray[i].bbox.x0;
                    qrcodeInfoArray[i].bbox.height = Math.Max((int)points_arr[5], (int)points_arr[7]) - qrcodeInfoArray[i].bbox.y0;

                    if (decodedInfo.Count > i && decodedInfo[i] != null)
                    {
                        qrcodeInfoArray[i].label = decodedInfo[i];
                    }
                    qrcodeInfoArray[i].color = color;
                }
            }
            qrcodeVisualizer.UpdateBoundingBoxVisualizations(qrcodeInfoArray);
            yield return null;         
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            if (detector != null)
                detector.Dispose();
        }

        private void CaptureCameraFrame()
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture targetTexture = carCamera.targetTexture;
            RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            carCamera.targetTexture = tempRT;
            carCamera.Render();

            RenderTexture.active = carCamera.targetTexture;
            imgTexture.ReadPixels(new UnityEngine.Rect(0, 0, Screen.width, Screen.height), 0, 0);
            imgTexture.Apply();

            carCamera.targetTexture = targetTexture;
            RenderTexture.ReleaseTemporary(tempRT);
            RenderTexture.active = currentRT;
        }

        public void ChangeToMainCamera()
        {
            qrcodeInfoArray = new BBox2DInfo[0];
            qrcodeVisualizer.UpdateBoundingBoxVisualizations(qrcodeInfoArray);
        }
    }
}