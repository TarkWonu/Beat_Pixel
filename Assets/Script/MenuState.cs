using UnityEngine;
using UnityEngine.UI;

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

public class SpriteSelectPage : State
{

    private GameObject page;
    private GridLayoutGroup grid;
    private ScrollRect scroll;
    private StateMachine expandFSM;

    private State pageState;

    public SpriteSelectPage(GameObject page,GridLayoutGroup grid,ScrollRect scroll,StateMachine expandFSM)
    {
        this.page = page;
        this.grid = grid;
        this.scroll =scroll;
        this.expandFSM = expandFSM;
        pageState = expandFSM.currentState;
    }


    public override void OnEnter()
    {
        page.SetActive(true);
        
    }

    public override void OnExit()
    {
        page.SetActive(false);
    }

    public override void OnUpdate()
    {
        if(pageState?.GetType() != expandFSM.currentState.GetType())
        {
            grid.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            pageState = expandFSM.currentState;
        }
        if(expandFSM.currentState is FullBackGround)
        {
            scroll.horizontal = false;
            scroll.vertical = true;

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 6;

            Debug.Log("확장된");
        }
        else
        {
            scroll.horizontal = true;
            scroll.vertical = false;

            grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            grid.constraintCount = 1;
            Debug.Log("축소된");
        }
    }
}



public class MusicSelectPage : State
{

    private GameObject page;
    private GameObject preview;
    private GridLayoutGroup grid;
    private ScrollRect scroll;
    private StateMachine expandFSM;

    private State pageState;

    public MusicSelectPage(GameObject page,GameObject preview,GridLayoutGroup grid,ScrollRect scroll,StateMachine expandFSM)
    {
        this.page = page;
        this.preview = preview;
        this.grid = grid;
        this.scroll =scroll;
        this.expandFSM = expandFSM;

        pageState = expandFSM.currentState;
    }


    public override void OnEnter()
    {
        page.SetActive(true);
        
    }

    public override void OnExit()
    {
        page.SetActive(false);
    }

    public override void OnUpdate()
    {

        if(pageState?.GetType() != expandFSM.currentState.GetType())
        {
            grid.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            pageState = expandFSM.currentState;
        }
        if(expandFSM.currentState is FullBackGround)
        {
            scroll.horizontal = false;
            scroll.vertical = true;

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            preview.SetActive(true);
        }
        else
        {
            scroll.horizontal = true;
            scroll.vertical = false;

            grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            grid.constraintCount = 1;
            preview.SetActive(false);
        }
    }
}

