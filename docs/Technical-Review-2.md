PROJECT TECHNICAL REVIEW

Input System Refactor: Event-Driven Architecture

In the early stages of development, the polling method used (checking every frame) created unnecessary CPU load and was abandoned in favor of a modern Event-Driven architecture.
• Architecture Transformation: Unity's new Input System library was integrated, moving input operations to an Action Callback structure.
• Performance Optimization: Key checks performed every frame in Update were removed, replaced with a structure triggered only when the event occurs (performed).
• SOLID (SRP) Application: The PlayerController class was stripped of input logic and made responsible only for movement; input management was delegated to the InteractionManager class via a central InputActionAsset.
• Technical Detail: Functions are subscribed to events with the line interactAction.action.performed += OnInteractTriggered;.
C#
private void OnEnable()
{
interactAction.action.performed += OnInteractTriggered;
holdAction.action.performed += OnHoldStarted;
holdAction.action.canceled += OnHoldCanceled;
interactAction.action.Enable();
holdAction.action.Enable();
}

Physical Interaction Management and Stabilization

In the initial phases of the project, objects were held with the isKinematic setting activated to follow a transport logic outside of physics rules; however, this method was observed to cause objects to pass through tables or other solid objects (clipping/tunneling). To solve this problem and make the interaction compliant with physics rules...
Latest Updates (From Recent Development):

Book Examination Refactor: Removed activeBook state variable for cleaner toggle logic. Direct raycast check for IBookInteractable interface on each interact event, eliminating state-related bugs.
Pickup Skip for Books: Added check in HandleInteraction to skip pickup if IBookInteractable detected, ensuring books remain static (no movement on hold action).
Null-Safe Player Cache: In BookInteraction.cs, added if (player) checks for canMove toggles to prevent null reference errors.
Debug Logs: Added "Examination MODE ENTRY" and "EXIT" logs for easier testing of state transitions.

Result: Interaction stability improved, with no clipping and better performance in examination mode.