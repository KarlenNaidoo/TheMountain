namespace Characters
{

    public interface IAttackListener
    {
        void OnEnableAttack();

        void OnDisableAttack();

        void ResetAttackTrigger();
    }
}
