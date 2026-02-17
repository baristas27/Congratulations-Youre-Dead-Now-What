# Congratulations, You're Dead! Now What?

A dark comedy bureaucracy simulator fused with FPS/Tower Defense mechanics.  
In the afterlife, you become a temporary clerk in Purgatory's cold, bureaucratic hell, judging souls for 10 days to earn merit points and ascend to Heaven — or face exile to the H-Line.

## Gameplay Overview
- **Daytime**: First-person desk job — review soul files, stamp "Up" (Heaven) or "Down" (Hell). Absurd sin/virtue records, physical interactions (stamps, files, examination books).
- **Nighttime**: FPS + Tower Defense defense — protect office corridors from escaping souls using traps and barricades.
- **Progression**: Decisions affect nighttime difficulty. Collect merit points, unlock shop items, survive 10 days for promotion.

## Technical Highlights
- Fully event-driven input system (no polling — significant CPU savings)
- SOLID principles applied (SRP: single responsibility per class, OCP: extensible via interfaces)
- Diegetic UI (no HUD — all information conveyed through in-world objects)
- Physics-based interactions (raycast + velocity/damping, clipping/tunneling prevention)
- Modular systems (IInteractable, IPickable, IBookInteractable interfaces)

## Repository Contents
- `Assets/Scripts/` — Custom game logic and core systems
- `docs/` — Game Design Document, technical reviews, and changelog

## Development Focus
This repo contains only custom code and documentation.  
Third-party assets (models, sounds, VFX packs) are excluded to keep the repository clean and focused on original work.

## License
MIT License