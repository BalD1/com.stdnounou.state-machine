namespace StdNounou.FSM
{
    public interface IState
    {
        public void EnterState();
        public void Update();
        public void FixedUpdate();
        public void ExitState();
        public void Conditions();

        public void EventsSubscriber();
        public void EventsUnSubscriber();
    }
}