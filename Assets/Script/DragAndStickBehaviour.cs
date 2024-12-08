using Export.Attribute;
using Export.AddFunc;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ק��������Ϊ�Ļ��ࡣ
/// �ṩ��ק���������⡢�Լ�����������ʵ�֡�
/// </summary>
public class DragAndStickBehaviour : MonoBehaviour
{
    /// <summary>
    /// ���п��ܵ������㣨ȫ�־�̬�б���
    /// ��Щ�㽫��Ϊ������ק�����Ǳ������Ŀ�ꡣ
    /// </summary>
    public static List<Transform> _points = new List<Transform>();

    /// <summary>
    /// ������ʾ��Ӱ��Ԥ���塣
    /// ����ק�����У���Ӱ��ʾǱ������λ�á�
    /// </summary>
    public GameObject shadowPrefab;

    /// <summary>
    /// ��ǰ�����ϵĽ��ܵ����顣
    /// ���ڼ����������ľ��롣
    /// </summary>
    [ReadOnly]
    public Transform[] receptorPoints;

    /// <summary>
    /// ����������롣
    /// �������������ܵ�֮��ľ���С�ڸ�ֵ�������������
    /// </summary>
    public float stickDistance = 0.5f;

    /// <summary>
    /// �������ƶ��ٶȡ�
    /// ��������ӵ�ǰλ�õ���������ƶ��ٶȡ�
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// ��ק����ĸ߶ȡ�
    /// ���ڵ�����������ʱ��λ�ã�ʹ������������롣
    /// </summary>
    public float height = 1f;

    /// <summary>
    /// ��ǰ�����Ƿ����ڱ���ק��
    /// </summary>
    [ReadOnly]
    public bool isDragging = false;

    /// <summary>
    /// ��ǰ�����Ƿ�����������
    /// </summary>
    [ReadOnly]
    public bool isSticking = false;

    /// <summary>
    /// ��ǰ�����Ƿ��Ѿ����������
    /// </summary>
    [ReadOnly]
    public bool isSticked = false;

    /// <summary>
    /// ������ʾ��Ӱ��ʵ����
    /// </summary>
    protected GameObject shadow;

    /// <summary>
    /// ���ڼ�¼ÿ�����ܵ�����������㡣
    /// ȷ������������ܵ�һһ��Ӧ��
    /// </summary>
    protected Dictionary<Transform, Transform> closestPointMap = new Dictionary<Transform, Transform>();

    /// <summary>
    /// ÿ֡������ק�������߼���
    /// </summary>
    protected void Upgrade()
    {
        if (isDragging)
        {
            DragObject(); // ���������ק��ִ����ק�߼�
        }
        else if (isSticking && !isSticked)
        {
            StickToObject(); // ���δ���������ִ�������߼�
        }
    }

    /// <summary>
    /// ִ����ק�߼���
    /// �������λ�ø�������λ�ã����ж��Ƿ����������
    /// </summary>
    void DragObject()
    {
        // �������������ռ��е�λ��
        float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z); // ����Z�᲻��
        //transform.position = mousePos;

        // ����Ƿ�������������
        bool canStick = CheckIfCanStick();

        if (canStick)
        {
            // ���������������ʾ��Ӱ��������Ӱλ��
            if (shadow == null)
            {
                shadow = Instantiate(shadowPrefab, transform.position,transform.rotation); // ������Ӱ
            }
            shadow.SetActive(true);
            var centerPoint = CalculateCenter(closestPointMap.ValuesToArray()); // ������Ӱλ�õ�����������
            shadow.transform.position = new Vector3(centerPoint.x, centerPoint.y + height / 2, centerPoint.z);
        }
        else if (shadow != null)
        {
            shadow.SetActive(false); // �������������������Ӱ
        }
    }

    /// <summary>
    /// ����Ƿ���������������
    /// �������н��ܵ㣬�ҵ������������������㣬����¼��
    /// </summary>
    /// <returns>�Ƿ���������������</returns>
    private bool CheckIfCanStick()
    {
        closestPointMap.Clear(); // ��յ�ǰ��¼�����������
        bool canStick = true;

        foreach (var receptor in receptorPoints)
        {
            Transform closest = null;
            float minDistance = stickDistance;

            foreach (var point in _points)
            {
                float distance = Vector3.Distance(receptor.position, point.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = point; // �������������
                }
            }

            if (closest == null)
            {
                canStick = false; // ���ĳ�����ܵ�û�����������������㣬��������
                break;
            }
            closestPointMap[receptor] = closest; // ��¼���ܵ������������Ķ�Ӧ��ϵ
        }

        return canStick;
    }

    /// <summary>
    /// ִ�������߼���
    /// �������ƶ�������������λ�ã�ֱ��������ɡ�
    /// </summary>
    void StickToObject()
    {
        Vector3 targetPosition = CalculateCenter(closestPointMap.ValuesToArray()); // ��������������
        targetPosition = new Vector3(targetPosition.x, targetPosition.y + height/2, targetPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); // ƽ���ƶ���Ŀ��λ��

        if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
        {
            isSticking = false;
            isSticked = true;

            if (shadow != null)
            {
                Destroy(shadow); // ������ɺ�������Ӱ
                shadow = null;
            }
        }
    }

    /// <summary>
    /// ����һ��Transform�����ĵ㡣
    /// </summary>
    /// <param name="points">Transform���顣</param>
    /// <returns>���ĵ�λ�á�</returns>
    private Vector3 CalculateCenter(Transform[] points)
    {
        Vector3 center = Vector3.zero;
        foreach (var point in points)
        {
            center += point.position;
        }
        return center / points.Length; // ����ƽ��λ����Ϊ���ĵ�
    }

    /// <summary>
    /// ����갴��ʱ����ʼ��ק��
    /// </summary>
    void OnMouseDown()
    {
        isDragging = true; // ������ק
        isSticked = false; // ��������״̬
    }

    /// <summary>
    /// ������ɿ�ʱ��ֹͣ��ק����ʼ������
    /// </summary>
    void OnMouseUp()
    {
        isDragging = false; // ֹͣ��ק
        if (shadow != null && shadow.activeSelf)
        {
            isSticking = true; // �����Ӱ�ɼ�����ʼ����
        }
    }
}
