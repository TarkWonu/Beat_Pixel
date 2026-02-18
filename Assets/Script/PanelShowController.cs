using UnityEngine;

public class PanelShowController : MonoBehaviour
{
    [SerializeField] private RectTransform panelTransform;


    private StateMachine stateMachine;

    void Awake()
    {
        stateMachine = new StateMachine(new FullBackGround(panelTransform));
    }

    void ToggleState()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(stateMachine.currentState is FullBackGround)
            {
                stateMachine.ChangeState(new SimpleBackGround(panelTransform));
            }
            else
            {
                stateMachine.ChangeState(new FullBackGround(panelTransform));
            }
        }
    }

    void Update()
    {
        ToggleState();
        stateMachine.UpdateState();
    }
}
