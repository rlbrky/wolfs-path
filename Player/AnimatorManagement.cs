using UnityEngine;

public class AnimatorManagement : MonoBehaviour
{
    [SerializeField] private PlayerCombat combatSC;
    [SerializeField] private DashScript dashScript;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        if (_animator != null)
        {
            combatSC.OnAnimatorMove();
            PlayerMovement.instance.OnAnimatorMove();
            //if(dashScript != null )
                //dashScript.OnAnimatorMove();
        }    
    }
}
