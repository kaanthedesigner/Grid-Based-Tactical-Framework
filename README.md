
# 3D Grid-Based Tactical Framework (Pure Logic & Decoupled Architecture)

A robust, performance-optimized grid system developed for turn-based tactical games in Unity. This project focuses on professional software architecture, strict decoupling of gameplay logic from internal engine visuals, and real-time visual feedback for player movement.

## Key Engineering Features

- **Decoupled Architecture (MVC/Data-Driven Pattern):** The grid map data resides entirely in memory using a pure `Node` class without inheriting from Unity's `MonoBehaviour`. Visual components (`Cube` primitives) are connected as independent view layers.
- **Optimized Data Structure:** Utilizes a `Dictionary<Vector2Int, Node>` rather than traditional 2D arrays, allowing high flexibility for asymmetrical and dynamic map generations with $O(1)$ lookup performance.
- **Graph-Theory Pathfinding:** Implements the **Breadth-First Search (BFS)** algorithm to compute the shortest path dynamically while safely maneuvering around obstacles (unwalkable nodes).
- **Proactive UX (Real-time Range Visualization):** Instead of calculating simple Manhattan distance on hover, the framework pre-runs the pathfinder engine every frame. It previews the exact traversal path (including the origin tile) in **Green** or prompts an immediate **Red** restriction if obstructed or out of bounds.
- **Coroutines-Driven Grid Movement:** Dispatches clean asynchronously managed execution sequences (`IEnumerator`) to dynamically slide units from tile to tile without triggering hard frame stutters.

### Architecture Breakdown

- `Node.cs`: Pure C# data model representing coordinate states, cell structures, object interactions, and terrain walkability.
- `GridManager.cs`: The core engine map database. Handles coordinate-to-world projections, neighbor extractions, and memory cleanup sequences.
- `Pathfinding.cs`: Pure algorithmic processor responsible for path indexing, backtracking loops, and tile distance validation rules.
- `Unit.cs`: Active controller regulating movement limits (`movementRange`), world pacing, and positional grid occupation states.
- `InputManager.cs`: Player raycast controller bridging viewport operations with logical tile indexes.
