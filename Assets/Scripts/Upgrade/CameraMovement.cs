using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FYP.Global.InputUtilities;

namespace FYP.Upgrade
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private RectTransform graphCanvasRectTransform;


        private Camera cam;

        public static MouseButtonData mouseButtonData;

        public Vector2 panLimit;
        public Vector2 zoomLimit;

        private Vector3 cameraPosition;
        private Vector2 getCameraSize { get {
                float camHeight = 2f * cam.orthographicSize;
                float camWidth = camHeight * cam.aspect;
                return new Vector2(camWidth, camHeight); 
            } }

        
        private Vector2 GraphCanvasSize;

        private void Start()
        {
            cam = Camera.main;

            GraphCanvasSize = new Vector2(graphCanvasRectTransform.rect.width, graphCanvasRectTransform.rect.height);

            Vector2 camSize = getCameraSize;
            panLimit = new Vector2((GraphCanvasSize.x - camSize.x) /2, (GraphCanvasSize.y - camSize.y) /2);
        }

        void Update()
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * 0.01f);
            }

            if (Input.GetMouseButtonDown(2))
            {
                onMouseButtonDown();
            }
            else if (Input.GetMouseButtonUp(2))
            {
                onMouseButtonUp();
            }
            else if (Input.GetMouseButton(2))
            {
                onMouseButton();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                zoom(Input.GetAxis("Mouse ScrollWheel"));
            }

        }

        private void onMouseButtonDown() {
            mouseButtonData = new MouseButtonData(Input.mousePosition);
            
        }

        private void onMouseButtonUp() { 

        }

        private void onMouseButton() {
            cameraPosition = cam.transform.position;
            Vector3 diff = mouseButtonData.getDeltaFromCurrentToOriginInWorldSpace(cam.ScreenToWorldPoint(Input.mousePosition));
            cameraPosition -= diff;
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, -panLimit.x, panLimit.x);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, -panLimit.y, panLimit.y);


            cam.transform.position = cameraPosition;
        }

        void zoom(float increment)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - 2*increment, zoomLimit.x, zoomLimit.y);
            Vector2 camSize = getCameraSize;

            panLimit = new Vector2(Mathf.Abs(GraphCanvasSize.x - camSize.x) / 2, Mathf.Abs(GraphCanvasSize.y - camSize.y) / 2);
            
            if (cameraPosition.y > -panLimit.y)
            {
                cam.transform.Translate(0, increment, 0);
            }
            else if (cameraPosition.y < panLimit.y) {
                cam.transform.Translate(0, -increment, 0);
            }
            if (cameraPosition.x < -panLimit.x)
            {
                cam.transform.Translate(-increment * cam.aspect, 0, 0);
            }
            else if (cameraPosition.x > panLimit.x)
            {
                cam.transform.Translate(increment * cam.aspect, 0, 0);
            }
        }
    }
}