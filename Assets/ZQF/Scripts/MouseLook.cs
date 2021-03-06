using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[Serializable]
public class MouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;


    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private Quaternion m_RightHandRot;
    private bool m_cursorIsLocked = true;

    public void Init(Transform character, Transform camera,Transform rightHand)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        m_RightHandRot = rightHand.localRotation;
    }


    public void LookRotation(Transform character, Transform camera,Transform rightHand)
    {
        if (!m_cursorIsLocked)
        {
            return;//don't rotate the camera if the cursor is not locked
        }
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

        m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
        m_RightHandRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
        {
            m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
            m_RightHandRot = ClampRotationAroundXAxis(m_RightHandRot);
        }
                
        if(smooth)
        {
            character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
            rightHand.localRotation = Quaternion.Slerp(rightHand.localRotation, m_RightHandRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
            rightHand.localRotation = m_RightHandRot;
        }

        UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if(!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public bool GetCursorLock()//获取当前指针锁定状态
    {
        return m_cursorIsLocked;
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        //if(Input.GetKeyUp(KeyCode.Escape))
        //{
        //    m_cursorIsLocked = false;
        //}
        //else if(Input.GetMouseButtonUp(0))
        //{
        //    m_cursorIsLocked = true;
        //}
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_cursorIsLocked = false;
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

        angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}