Project: "Congratulations, You're Dead! Now What?"
Development Goal: Create a data-driven simulation experience blending physics-based interactions with a bureaucratic atmosphere.

Physical Entity and Interaction Management

Planned: The player should have direct and natural interaction with office objects (stamps, files, pens).
Implemented: A raycast (ray tracing) system managed through InteractionManager.cs.
• Technical Detail: To establish a physical connection with objects rather than just "selecting" them, the isKinematic toggle method was chosen. When an object is held, kinematic mode prevents physics engine conflicts; on release, it returns to the physics world.
• SOLID (Single Responsibility): This class is only responsible for "initiating and terminating interactions." It does not concern itself with how the character moves or what the object does. This is a preferred method to simplify debugging.

Modular Object Behaviors (Interfaces)

Planned: Unified management of different interaction types in the office, such as stampable documents, openable drawers, and pickable items.
Implemented: Objects abstracted using IInteractable and IPickable interfaces.
• Technical Detail: InteractionManager checks what "ability" the hit object has, rather than what it is. For example, if an object is IPickable, it is picked up; if only IInteractable, it is clicked.
• SOLID (Open-Closed Principle): The system's core code can be extended with unlimited new interaction types without changes. A new lamp switch...
3. Technical Review 2 – English Translation and Updates
(As docs/Technical-Review-2.md – Document 2.pdf based, plus latest updates from our conversations: book examination refactor, pickup skip for books, activeBook removal, debug logs, null-safe player cache).