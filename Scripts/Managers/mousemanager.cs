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
    RaycastHit hitinfo;//定义一条射线

    //public EventVector3 OnMouseCliked;//定义一个三维坐标称为被鼠标点击
    public static mousemanager instance;//定义一个该类静态实例

    public event Action<Vector3> OnMouseCliked;//使用范例定义事件传入的参数为三维坐标;

    public event Action<GameObject> OnEnemyCliked;

    public Texture2D point, doway, attack, target, arrow;
    private void Awake()//定义初始化变量
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;//实现单例模式 可以将物体自动拖入agent  
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()//换鼠标图片
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
        if (Input.GetMouseButtonDown(0) && hitinfo.collider != null) //点击左键且返回的碰撞体不能为空
        {
            if (hitinfo.collider.gameObject.CompareTag("Ground"))//通过射线接触的物体的碰撞体组件来得到该物体的属性  使用组件得到物体
            {
                OnMouseCliked?.Invoke(hitinfo.point);//设置事件是否被触发，被触发后调用订阅该事件的函数 hitinfo.point就是得到的三维坐标 传入到另一个
                //物体脚本里注册过OnMouseCliked的方法，再在那个物体上调用该方法，就实现了鼠标对物体的移动 在该物体上不含有任何相关的组件 方法都是在各自的物体上实现
               
            }
            if (hitinfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyCliked?.Invoke(hitinfo.collider.gameObject);
            }
        }
    }

}


