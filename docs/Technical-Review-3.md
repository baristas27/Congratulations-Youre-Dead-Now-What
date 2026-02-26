\# TECHNICAL REVIEW 3: Input Architecture \& Interaction Refactor

\*\*Date:\*\* February 26, 2026

\*\*Focus:\*\* Event-Driven Input System, Advanced Object Manipulation, and Examination Stability



\## 1. Input System Architecture Overhaul

\*\*Migration from Polling to Event-Driven:\*\*

The input handling logic within `PlayerController.cs` has been completely refactored to align with the modern Unity Input System standards.

\* \*\*Problem:\*\* Previous logic checked input states every frame in `Update()` (Polling), leading to potential performance overhead and input loss during frame drops.

\* \*\*Solution:\*\* Transitioned to an \*\*Event-Driven Architecture\*\*. Inputs are now handled via `InputAction.performed` and `canceled` callbacks.

\* \*\*Optimization:\*\* Movement vectors are cached and read directly during `Update` to ensure smooth character controller physics, while distinct actions (like Interact/Rotate) are strictly event-based.

\* \*\*Memory Management:\*\* Implemented robust `OnEnable` / `OnDisable` subscription patterns to prevent memory leaks.



\## 2. Held Object Manipulation (Rotation)

\*\*Feature Implementation:\*\*

Added the ability to rotate held objects physically while inspecting them.

\* \*\*Input Flow:\*\* Rotation input (A/D keys) is captured by `PlayerController` and delegated to `InteractionManager`.

\* \*\*Separation of Concerns (SoC):\*\* The `PlayerController` does not touch the object; it merely passes the input float value to the `InteractionManager`.

\* \*\*Physics Logic:\*\* The held object's Rigidbody is rotated using `transform.Rotate` within `FixedUpdate` to ensure collision compatibility with the environment.

\* \*\*Correction:\*\* When rotation occurs, the `initialRotationOffset` is recalculated dynamically to prevent the object from snapping back to its original orientation due to the smoothing (Lerp) logic.



\## 3. Examination System \& Camera Logic

\*\*Immersive Transition Refactor:\*\*

The transition between "Gameplay Mode" and "Examination Mode" (Reading books) has been polished for visual stability.

\* \*\*Camera Swapping:\*\* Removed the dual-camera rendering overhead. The system now strictly toggles cameras:

&nbsp;   \* \*Entry:\* Main Camera Disabled -> Examination Camera Enabled.

&nbsp;   \* \*Exit:\* Examination Camera Disabled -> Main Camera Enabled.

\* \*\*Smoothing:\*\* Implemented a Coroutine-based `Vector3.Lerp` transition to create a smooth zoom-in effect instead of a hard cut.

\* \*\*Cursor Management:\*\* Added logic to automatically unlock/show the cursor during examination and lock/hide it upon exit.



\## 4. Book Interaction State Machine

\*\*Logic Refinement:\*\*

The book reading mechanic now follows a strict logical state machine to prevent animation errors.

1\.  \*\*State 1 (Inspect):\*\* Player presses 'E'. Camera zooms in. Book remains closed.

2\.  \*\*State 2 (Open Cover):\*\* Player presses 'D' (Next) for the first time. The cover opens (`bool hasOpenedCover`).

3\.  \*\*State 3 (Read Pages):\*\* Subsequent 'D' presses trigger page flip animations.

\* \*\*Conflict Resolution:\*\* Added a specific check in `InteractionManager` to ensure `IReadable` objects (Books) are \*\*skipped\*\* by the Pickup logic. This prevents the player from accidentally throwing the book while trying to turn a page.



\## 5. Defensive Programming (Critical Fixes)

\*\*Interface Reference Safety ("Fake Null" Handling):\*\*

ADDRESSED A CRITICAL STABILITY ISSUE regarding Unity's object lifecycle vs. C# Garbage Collection.

\* \*\*The Issue:\*\* When referencing destroyed Unity objects via Interfaces (`IReadable`), standard C# null checks (`== null`) failed because Interfaces do not share Unity's custom equality operator overloading. This led to `MissingReferenceException` errors.

\* \*\*The Fix:\*\* Implemented a pattern matching check in `InteractionManager`:

&nbsp;   ```csharp

&nbsp;   if (currentReadable is MonoBehaviour mb \&\& mb == null)

&nbsp;   {

&nbsp;       currentReadable = null;

&nbsp;   }

&nbsp;   ```

&nbsp;   This forces the Interface to be cast back to a Unity Object, allowing the engine's native "IsDestroyed" check to function correctly.

