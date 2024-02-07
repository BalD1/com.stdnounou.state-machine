using System;
using System.Collections.Generic;
using UnityEngine;
using StdNounou.Core;
using StdNounou.Core.ComponentsHolder;

namespace StdNounou.FSM
{
    public abstract class FSM_Base<StateEnum> : MonoBehaviour
                             where StateEnum : Enum
    {
        [SerializeField] protected GameObject ownerObj;
        public IComponentHolder Owner { get; private set; }

        public IState CurrentState { get; private set; }
        public StateEnum CurrentStateKey { get; private set; }
        [field: SerializeField] public StateEnum BaseStateKey { get; private set; }
        public Dictionary<StateEnum, IState> States { get; private set; }

        public event Action<StateEnum> OnStateChanged;

        private void Awake()
        {
            Owner = ownerObj.GetComponent<IComponentHolder>();
            States = new Dictionary<StateEnum, IState>();
            SetupStates();
        }

        protected virtual void Start()
        {
            SetupComponents();
            SetToBaseState();
        }

        private void SetToBaseState()
        {
            if (!States.TryGetValue(BaseStateKey, out IState baseState) || baseState == null)
            {
                this.LogError("Could not find base state " + BaseStateKey);
                return;
            }

            CurrentState = baseState;
            CurrentStateKey = BaseStateKey;
            CurrentState.EnterState();
        }

        protected abstract void SetupComponents();
        protected abstract void SetupStates();

        protected virtual void Update()
        {
            if (CurrentState == null) return;
            CurrentState.Update();
            CurrentState.Conditions();
        }

        protected virtual void FixedUpdate()
        {
            if (CurrentState == null) return;
            CurrentState.FixedUpdate();
        }

        private void PerformSwitchState(StateEnum state)
        {
            CurrentState?.ExitState();
            if (!States.TryGetValue(state, out IState newState) || newState == null)
            {
                this.LogError("Could not find state " + state);
                SetToBaseState();
                return;
            }

            CurrentState = newState;
            CurrentStateKey = state;
            CurrentState?.EnterState();
            OnStateChanged?.Invoke(state);
        }

        public void AskSwitchState(StateEnum state)
        {
            PerformSwitchState(state);
        }

        private void OnDestroy()
        {
            CurrentState?.ExitState();
        }
    }
}