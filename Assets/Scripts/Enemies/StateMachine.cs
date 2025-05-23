using System.Collections.Generic;
using UnityEngine;

// This class manages transitions between different AI or gameplay states using a list of AStateBehaviour scripts
public class StateMachine : MonoBehaviour
{
    // List of state behaviors for this object.
    // Each element must inherit from AStateBehaviour.
    [SerializeField] private List<AStateBehaviour> stateBehaviours = new List<AStateBehaviour>();

    // This value corresponds to the index in the enum/stateBehaviours list.
    [SerializeField] private int defaultState = 0;

    // Tracks the currently active state
    private AStateBehaviour currentState = null;

    // Initializes all the states in the list by calling their InitializeState function
    // Returns false if any state fails to initialize
    bool InitializeStates()
    {
        for (int i = 0; i < stateBehaviours.Count; ++i)
        {
            AStateBehaviour stateBehaviour = stateBehaviours[i];

            // If the state exists and initializes successfully
            if (stateBehaviour && stateBehaviour.InitializeState())
            {
                // Link this state to the current state machine
                stateBehaviour.AssociatedStateMachine = this;
                continue;
            }

            // Log an error if a state fails to initialize
            Debug.Log($"StateMachine on {gameObject.name} failed to initialize state {stateBehaviours[i]?.GetType().Name}!");
            return false;
        }

        return true;
    }

    // Called once at the beginning
    void Start()
    {
        // Attempt to initialize all states
        if (!InitializeStates())
        {
            // Disable this component if initialization fails
            this.enabled = false;
            return;
        }

        // Start the default state if available
        if (stateBehaviours.Count > 0)
        {
            int firstStateIndex = defaultState < stateBehaviours.Count ? defaultState : 0;

            currentState = stateBehaviours[firstStateIndex];
            currentState.OnStateStart();
        }
        else
        {
            Debug.Log($"StateMachine on {gameObject.name} has no state behaviours assigned!");
        }
    }

    // Called once per frame
    void Update()
    {
        // Update the current state's logic
        currentState.OnStateUpdate();

        // Check if it's time to switch states
        int newState = currentState.StateTransitionCondition();
        if (IsValidNewStateIndex(newState))
        {
            // End the current state and start the new one
            currentState.OnStateEnd();
            currentState = stateBehaviours[newState];
            currentState.OnStateStart();
        }
    }

    // Ensures the state index is within range
    private bool IsValidNewStateIndex(int stateIndex)
    {
        return stateIndex >= 0 && stateIndex < stateBehaviours.Count;
    }

    // Returns the currently active state
    public AStateBehaviour GetCurrentState()
    {
        return currentState;
    }
}
