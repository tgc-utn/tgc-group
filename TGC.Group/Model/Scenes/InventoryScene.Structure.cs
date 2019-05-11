using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Scenes
{
    partial class InventoryScene
    {
        public delegate void UpdateLogic(float elapsedTime);
        public UpdateLogic updateLogic = time => { };
        enum StateID
        {

            IN,
            INVENTORY,
            OUT,
            NULL
        }
        struct State
        {
            public StateID nextStateID;
            public UpdateLogic updateLogic;
            public State(UpdateLogic updateLogic, StateID nextStateID)
            {
                this.updateLogic = updateLogic;
                this.nextStateID = nextStateID;
            }
        }
        private State[] states = new State[3];
        private StateID stateID, nextStateID;
        private void SetState(StateID newStateID)
        {
            State newState = states[(int)newStateID];

            this.stateID = newStateID;
            this.updateLogic = newState.updateLogic;
            pressed[Key.I] = () => SetNextState(newState.nextStateID);
        }
        private void SetNextState(StateID newStateID)
        {
            nextStateID = newStateID;
        }
        private void BindState(StateID stateID, UpdateLogic stateUpdateLogic, StateID nextStateID)
        {
            states[(int)stateID] = new State(stateUpdateLogic, nextStateID);
        }
    }
}
