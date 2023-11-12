using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3>{ }
public class mousemanager : MonoBehaviour
{
    RaycastHit hitinfo;//����һ������

    //public EventVector3 OnMouseCliked;//����һ����ά�����Ϊ�������
    public static mousemanager instance;//����һ�����ྲ̬ʵ��

    public event Action<Vector3> OnMouseCliked;//ʹ�÷��������¼�����Ĳ���Ϊ��ά����;

    public event Action<GameObject> OnEnemyCliked;

    public Texture2D point, doway, attack, target, arrow;
    private void Awake()//�����ʼ������
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;//ʵ�ֵ���ģʽ ���Խ������Զ�����agent  
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()//�����ͼƬ
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray , out hitinfo))
        {
            switch (hitinfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target,new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack,new Vector2(16,16),CursorMode.Auto); 
                    break;

            }
        }
         
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitinfo.collider != null) //�������ҷ��ص���ײ�岻��Ϊ��
        {
            if (hitinfo.collider.gameObject.CompareTag("Ground"))//ͨ�����߽Ӵ����������ײ��������õ������������  ʹ������õ�����
            {
                OnMouseCliked?.Invoke(hitinfo.point);//�����¼��Ƿ񱻴���������������ö��ĸ��¼��ĺ��� hitinfo.point���ǵõ�����ά���� ���뵽��һ��
                //����ű���ע���OnMouseCliked�ķ����������Ǹ������ϵ��ø÷�������ʵ��������������ƶ� �ڸ������ϲ������κ���ص���� ���������ڸ��Ե�������ʵ��
               
            }
            if (hitinfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyCliked?.Invoke(hitinfo.collider.gameObject);
            }
        }
    }

}


