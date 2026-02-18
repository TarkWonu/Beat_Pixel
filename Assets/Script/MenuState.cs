using UnityEngine;

public class FullBackGround : State
{

    private RectTransform backGround;

    public FullBackGround(RectTransform background) => this.backGround = background;

    private Vector3 fullPos;
    public override void OnEnter()
    {
        fullPos = new Vector2(backGround.anchoredPosition.x,500);
    }
    public override void OnUpdate()
    {
        
        
        
        if (backGround.anchoredPosition.y > fullPos.y-0.1f)
        {
            backGround.anchoredPosition = fullPos;
            
        }
        else
        {
            backGround.anchoredPosition = Vector3.Lerp(backGround.anchoredPosition,fullPos,Time.deltaTime*3);
        }
        
        
    }
    public override void OnExit()
    {
        
    }

    
}

public class SimpleBackGround : State
{

    private RectTransform backGround;

    public SimpleBackGround(RectTransform background) => this.backGround = background;

    private Vector3 simplePos;


    public override void OnEnter()
    {
        simplePos = new Vector2(backGround.anchoredPosition.x,-145);
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        if (backGround.anchoredPosition.y < simplePos.y+0.1f)
        {
            backGround.anchoredPosition = simplePos;
            
        }
        else
        {
            backGround.anchoredPosition = Vector3.Lerp(backGround.anchoredPosition,simplePos,Time.deltaTime*3);
        }
    }


    
}