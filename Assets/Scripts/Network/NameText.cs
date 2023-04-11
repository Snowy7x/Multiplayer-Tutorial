using System;
using UnityEngine;

namespace Network
{
    public class NameText : MonoBehaviour
    {
        private Camera _camera;
        
        private void Start()
        {
            GetCamera();
        }
        
        private void Update()
        {
            if (_camera == null || !_camera.isActiveAndEnabled)
            {
                GetCamera();
            }
            else
            {
                transform.LookAt(_camera.transform);
            }
        }

        public void GetCamera()
        {
            foreach (var cam in Camera.allCameras)
            {
                if (cam != null && cam.isActiveAndEnabled)
                {
                    _camera = cam;
                    break;
                }
            }
        }
    }
}