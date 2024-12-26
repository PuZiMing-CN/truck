using Export.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����Ӷ���
/// </summary>
public class ClickAdd : MonoBehaviour
{
    /// <summary>
    /// ��ӵĶ���
    /// </summary>
    [SerializeField]
    public GameObject AddPrefab;

    /// <summary>
    /// ͼƬ��ʾ����
    /// </summary>
    [SerializeField]
    [ReadOnly]
    public Image image;

    /// <summary>
    /// ��Ⱦ�õ�RenderTexture
    /// </summary>
    [SerializeField]
    [ReadOnly]
    public RenderTexture renderTexture;

    /// <summary>
    /// ������Ⱦ��Camera
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Camera renderCamera;

    private GameObject model;

    private void Awake()
    {
        if (AddPrefab == null || renderTexture == null || image == null)
        {
            return;
        }

        // ���ҹ������������ÿ�ζ������µ����
        renderCamera = Camera.main; // ʹ������������ٿ���

        if (renderCamera == null)
        {
            // �������������ڣ��򴴽�һ����ʱ�������Ⱦ
            renderCamera = new GameObject("RenderCamera").AddComponent<Camera>();
        }

        renderCamera.targetTexture = renderTexture;  // ������ȾĿ��ΪRenderTexture
        renderCamera.clearFlags = CameraClearFlags.SolidColor; // �������ɫ
        renderCamera.backgroundColor = Color.clear; // ������Ϊ͸��

        if (model == null)
        {
            // ʵ�������3D����Prefab��ֻʵ����һ�Σ�
            model = Instantiate(AddPrefab);
            model.SetActive(false); // ��ʼ״̬����Ⱦ���ȴ���ʾʱ������

            // ��ģ�͵�λ�÷��������ǰ�棬ȷ��������ܹ�������
            model.transform.position = new Vector3(0, 0, 5);

            // �����������ͼ����Ӧģ��
            renderCamera.transform.position = new Vector3(0, 0, -5);  // ���������ģ��
            renderCamera.transform.LookAt(model.transform);  // �����������ģ��
        }

        // ��UI����ʾRenderTexture
        if (image != null)
        {
            // ��RenderTextureת��ΪSprite
            image.material.mainTexture = renderTexture;
        }
    }

    // ������Ҫ��ʾģ��ʱ���ô˷���
    public void ShowModel()
    {
        if (model != null)
        {
            model.SetActive(true); // ����ģ�ͣ�������Ⱦ
            renderCamera.enabled = true; // �������������Ⱦ
        }
    }

    // ���㲻����Ҫ��ʾģ��ʱ���ô˷���
    public void HideModel()
    {
        if (model != null)
        {
            model.SetActive(false); // ����ģ�ͣ�ֹͣ��Ⱦ
            renderCamera.enabled = false; // �������
        }
    }
}
