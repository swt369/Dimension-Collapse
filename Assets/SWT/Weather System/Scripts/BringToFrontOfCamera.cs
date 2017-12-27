﻿using UnityEngine;

namespace DimensionCollapse
{
    public class BringToFrontOfCamera : MonoBehaviour
    {
        public float minY = 5f;
        public float offsetZ = 5f;

        private Camera localCamera;

        private void Start()
        {
            Camera[] cameras = PlayerManager.LocalPlayerInstance.GetComponentsInChildren<Camera>();
            foreach (var camera in cameras)
            {
                if (camera.tag == "MainCamera")
                {
                    localCamera = camera;
                    break;
                }
            }
        }

        private void Update()
        {
            Vector3 front = localCamera.transform.position + localCamera.transform.forward * offsetZ;
            transform.position = new Vector3(front.x, Mathf.Max(front.y, minY), front.z);
            transform.rotation = Quaternion.Euler(0, localCamera.transform.rotation.y, 0);
        }
    }
}
