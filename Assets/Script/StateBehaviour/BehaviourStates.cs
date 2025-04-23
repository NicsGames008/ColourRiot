// Enum representing the different possible states a monster can be in
public enum EEnemyState
{
    // Used as a default or error state when the monster state is not set correctly
    Invalid = -1,

    // The monster is following a predefined path or area
    Patrolling,

    // The monster has detected the player and is actively chasing them
    Chasing

}public enum EPlayerState
{
    Invalid = -1,
    Moving,
    Paused,
    InWall,
    Dead
}
