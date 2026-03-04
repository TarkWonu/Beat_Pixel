using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    virtual protected bool UseDontDestroyOnLoad => true;
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                // 해당 컴포넌트를 가지고 있는 게임 오브젝트를 찾아서 반환한다.
                instance = (T)FindAnyObjectByType(typeof(T));

                if (instance == null) // 인스턴스를 찾지 못한 경우
                {
                    
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    
                    instance = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {

        if (UseDontDestroyOnLoad)
        {
            if(transform.parent != null && transform.root != null) 
            {
                DontDestroyOnLoad(this.transform.root.gameObject); 
            }
            else
            {
                DontDestroyOnLoad(this.gameObject); 
            }
        }
        
    }
}