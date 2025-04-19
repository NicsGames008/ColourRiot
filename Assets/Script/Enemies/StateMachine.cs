using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // List of state behaviours, this is an abstract class pointer, children use this tempalte to implement the correcet functions
    // Important that the order of these is the same as the enumeration
    [SerializeField] private List<AStateBehaviour> stateBehaviours = new List<AStateBehaviour>();

    // In Case you dont want the first one to be the default state, you can use this, number equivalent to the enumeration
    [SerializeField] private int defaultState = 0;

    // Tracks the StateBehaviour
    private AStateBehaviour currentState = null;

    bool InitializeStates()
    {
        // Initializes all the states, if it fails then this turns off till the person configuring this fixes it
        for (int i = 0; i < stateBehaviours.Count; ++i)
        {
            AStateBehaviour stateBehaviour = stateBehaviours[i];
            if (stateBehaviour && stateBehaviour.InitializeState())
            {
                stateBehaviour.AssociatedStateMachine = this;
                continue;
            }

            Debug.Log($"StateMachine On {gameObject.name} has failed to initalize the state {stateBehaviours[i]?.GetType().Name}!");
            return false;
        }

        return true;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (!InitializeStates())
        {
            // Stop This class from executing
            this.enabled = false;
            return;
        }

        if (stateBehaviours.Count > 0)
        {
            int firstStateIndex = defaultState < stateBehaviours.Count ? defaultState : 0;

            currentState = stateBehaviours[firstStateIndex];
            currentState.OnStateStart();
        }
        else
        {
            Debug.Log($"StateMachine On {gameObject.name} is has no state behaviours associated with it!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnStateUpdate();


        int newState = currentState.StateTransitionCondition();
        if (IsValidNewStateIndex(newState))
        {
            currentState.OnStateEnd();
            currentState = stateBehaviours[newState];
            currentState.OnStateStart();
        }
    }

    // Helper Function To See If States Are The Same, Unused atm
    public bool IsCurrentState(AStateBehaviour stateBehaviour)
    {
        return currentState == stateBehaviour;
    }

    // Helper Function to Force A New State
    public void SetState(int index)
    {
        if (IsValidNewStateIndex(index))
        {
            currentState.OnStateEnd();
            currentState = stateBehaviours[index];
            currentState.OnStateStart();
        }
    }

    // Ensure Index is Valid
    private bool IsValidNewStateIndex(int stateIndex)
    {
        return stateIndex < stateBehaviours.Count && stateIndex >= 0;
    }

    // Gets The Current Running State
    public AStateBehaviour GetCurrentState()
    {
        return currentState;
    }
}
