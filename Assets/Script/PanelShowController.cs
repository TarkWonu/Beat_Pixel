using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PanelShowController : MonoBehaviour
{
    [SerializeField] private RectTransform panelTransform;

    [Header("스프라이트 선택 페이지")]
    [SerializeField] GameObject spritePage;
    [SerializeField] GridLayoutGroup spriteGrid;
    [SerializeField] ScrollRect spriteScroll;

    [Header("BGM 선택 페이지")]
    [SerializeField] GameObject musicPage;
    [SerializeField] GameObject musicPreviewPage;
    [SerializeField] GridLayoutGroup musicGrid;
    [SerializeField] ScrollRect musicScroll;


    public StateMachine expandStateMachine{get;private set;}
    private StateMachine pageStateMachine;

    void Awake()
    {
        expandStateMachine = new StateMachine(new FullBackGround(panelTransform));
        pageStateMachine = new StateMachine(new SpriteSelectPage(page:spritePage,grid:spriteGrid,scroll:spriteScroll,expandFSM:expandStateMachine));
    }

    void ToggleState()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(expandStateMachine.currentState is FullBackGround)
            {
                expandStateMachine.ChangeState(new SimpleBackGround(panelTransform));
            }
            else
            {
                expandStateMachine.ChangeState(new FullBackGround(panelTransform));
            }
        }
    }

    void ChangePage()
    {
        if((Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))&&pageStateMachine.currentState is MusicSelectPage)
        {
            pageStateMachine.ChangeState(new SpriteSelectPage(page:spritePage,grid:spriteGrid,scroll:spriteScroll,expandFSM:expandStateMachine));
        }
        if((Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))&&pageStateMachine.currentState is SpriteSelectPage)
        {
            pageStateMachine.ChangeState(new MusicSelectPage(page:musicPage,grid:musicGrid,scroll:musicScroll,expandFSM:expandStateMachine,preview:musicPreviewPage));
        }
    }

    void Update()
    {
        ToggleState();
        ChangePage();
        pageStateMachine.UpdateState();
        expandStateMachine.UpdateState();
    }
}
