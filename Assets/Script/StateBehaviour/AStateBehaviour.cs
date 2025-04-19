using UnityEngine;

// Abstract base class for defining behaviors for individual states in a state machine
public abstract class AStateBehaviour : MonoBehaviour
{
    // Reference to the state machine that this state is part of
    public StateMachine AssociatedStateMachine { get; set; }

    // Called once when the state is first initialized (e.g., for setup)
    public abstract bool InitializeState();

    // Called when this state becomes active
    public abstract void OnStateStart();

    // Called every frame while this state is active
    public abstract void OnStateUpdate();

    // Called when this state is about to exit
    public abstract void OnStateEnd();

    // Determines if and when the state should transition to another state
    // Returns an int (usually representing the next state ID)
    public abstract int StateTransitionCondition();
}

