public interface PlayerState
{
    void Update();
    void OnEnter();
    void OnExit();
    void OnEvent(string eventType); // Add this method
}