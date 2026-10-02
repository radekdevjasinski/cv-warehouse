# CV Warehouse

Browser game in Unity 2D. Robots search a procedurally generated warehouse, find boxes holding fragments of a CV and load them onto trucks, while the CV assembles itself on the waybill. The full design lives in [Docs/GameDesign.md](Docs/GameDesign.md). Read it before designing any feature, and update it in the same change whenever a decision alters the design.

## Two rules that always apply

1. **Always optimize for the web build.** WebGL is the only shipping target. Every decision about code, assets, packages and settings is judged by download size, load time, memory and frame time in a browser. If something works in the Editor but is slow, large or unsupported in WebGL, it is not done.
2. **Test every feature.** No feature is finished without automated tests covering it. Logic gets EditMode tests, scene and MonoBehaviour wiring gets PlayMode tests, and anything touching browser-only behaviour gets a manual check in a real WebGL build.

## Tech stack

- Unity `6000.4.1f1`, build target WebGL (IL2CPP, WebGL 2)
- Universal Render Pipeline `17.4.0` with the 2D Renderer
- Input System `1.19.0` (the legacy `UnityEngine.Input` class is not used)
- 2D Tilemap for the warehouse grid
- Unity Test Framework `1.6.0` (NUnit)
- JSON via `JsonUtility`

## Unity Editor access (Unity MCP)

The `UnityMCP` server (MCP for Unity by CoplayDev, pinned in `Packages/manifest.json`) gives direct access to the running Editor. Always use it whenever a task needs the Editor, instead of guessing, hand-editing serialized files or asking Radek to click through the Editor.

- **Use it for:** scenes, GameObjects, components, prefabs, ScriptableObjects, materials, tilemaps, UI, packages, project and player settings, builds, and running tests.
- **Verify through it.** After changing scripts, refresh Unity, wait for compilation to finish and read the Console. A change is not done while the Console shows compile errors or new warnings caused by it.
- **Run tests through it** while the Editor is open. The command-line runner in the Testing section only works with the Editor closed.
- **Read before writing.** Check the editor state and the current scene or asset before modifying it, and do not change anything while the Editor is compiling or in Play mode unless the task is about Play mode.
- **Plain files stay plain.** Write and edit `.cs`, `.json`, `.md` and `.jslib` files with the normal file tools, then refresh Unity. Use the MCP for everything Unity serializes: `.unity`, `.prefab`, `.asset`, `.meta`.
- **Save explicitly.** Save the scene or asset after changing it, otherwise the change exists only in Editor memory and is missing from git.
- **Ask before destructive or wide-reaching actions:** deleting assets, removing packages, changing build or player settings, or running arbitrary Editor code that modifies the project.
- **Do not use the generative tools** (image, audio, model generation) unless Radek asks. They add assets to the download size.
- **If the server is not connected**, say so and ask Radek to open the Editor and start the server from Window → MCP for Unity. Do not silently fall back to editing serialized YAML by hand.

## Core design constraints

These come from the design document and shape the architecture:

- **Data-driven.** Everything about the CV comes from one JSON file: name, job title, categories, sections, entries. Adding or removing a section changes the number of trucks and the map size without code changes. Never hardcode CV content.
- **Swappable at runtime.** The JSON is loaded over HTTP at startup, not baked into the build, so it can be replaced on the server without rebuilding. A second language is a second file.
- **Robust to bad data.** Missing fields get defaults. An invalid file shows a readable message instead of a broken game.
- **Deterministic generation.** The map is generated from parameters and a seed shown on screen. The same seed and the same file must always produce the same map.
- **Generator guarantees.** Everything reachable, no one-tile dead-end aisles, enough shelves for every entry. These are invariants and must be covered by tests.
- **No fail state.** The result is the time. The "skip and show CV" button is visible from the first second and must work in every game state.

## Project structure

```
Assets/
  Scripts/
    Runtime/        CvWarehouse.Runtime.asmdef
      Core/         plain C# logic, no UnityEngine scene types
      Presentation/ MonoBehaviours, views, UI
    Editor/         CvWarehouse.Editor.asmdef
  Tests/
    EditMode/       CvWarehouse.Tests.EditMode.asmdef
    PlayMode/       CvWarehouse.Tests.PlayMode.asmdef
  Plugins/WebGL/    .jslib browser interop
  StreamingAssets/  CV JSON files
  Scenes/ Prefabs/ Sprites/ Settings/
Docs/
```

- Root namespace is `CvWarehouse`, with sub-namespaces matching folders.
- All code lives in assembly definitions. Code in the default `Assembly-CSharp` cannot be referenced by tests and slows down compilation.
- One class, struct, enum or interface per file, and the file name matches the type name.

## Architecture rules

- **Keep logic out of MonoBehaviours.** Map generation, pathfinding, the task queue, bot rules, economy, pricing and CV parsing are plain C# classes in `Core`. MonoBehaviours in `Presentation` only read state and render it, and forward input. This is what makes "test every feature" cheap: `Core` runs in fast EditMode tests without a scene.
- **The simulation is grid-based.** Bots move on the warehouse grid with our own pathfinding. Do not use Physics2D, colliders or rigidbodies for bot movement or box pickup.
- **Randomness is injected.** Use a seeded `System.Random` instance passed into the generator. Never use `UnityEngine.Random` in `Core`, because it is global state and breaks determinism and tests.
- **Time is injected.** `Core` receives delta time as a parameter instead of reading `Time.deltaTime`, so the simulation can be stepped in tests.
- **Dependencies go through constructors or serialized fields.** No `FindObjectOfType`, no `GameObject.Find`, no singletons with static access.
- **Tunable values live in ScriptableObjects**, not in constants scattered through code: bot speeds, weight limits, prices, generator parameters.
- **Upgrade effects are a fixed pool in code.** The JSON only points at an effect by id. An unknown id is a data error handled by the robustness rules, never an exception.

## WebGL rules

### Things that do not work in a browser

- **No threads.** `System.Threading.Thread`, `Task.Run`, `Parallel` and the thread pool are unavailable. Use coroutines or Unity's `Awaitable` on the main thread. Never call `.Result` or `.Wait()` on a task, because it deadlocks the page.
- **No direct file access.** `System.IO.File` cannot read `StreamingAssets`. Load the CV JSON with `UnityWebRequest`. Append a cache-busting query parameter so a replaced file is not served from the browser cache.
- **No sockets, no `System.Net`.** Only `UnityWebRequest`, and cross-origin requests need CORS headers on the server.
- **No blocking.** Any long loop freezes the whole tab. Split heavy work such as map generation across frames if it exceeds a few milliseconds.
- **Audio, fullscreen and clipboard need a user gesture.** Start audio only after the first click.
- **File download needs browser interop.** Downloading the CV goes through a `.jslib` plugin in `Assets/Plugins/WebGL`, wrapped in a C# class with an Editor fallback behind `#if UNITY_WEBGL && !UNITY_EDITOR`.

### Code stripping and IL2CPP

- Managed code stripping removes anything reached only by reflection. Avoid reflection-based libraries. `JsonUtility` is safe; a reflection-based serializer would need a `link.xml`.
- `JsonUtility` does not support dictionaries, nullable types or a top-level array. Model the CV as serializable classes with public fields or `[SerializeField]`, and represent optional values with empty strings or sentinel defaults.
- No `System.Reflection.Emit`, no `dynamic`.

### Memory and garbage

In WebGL the garbage collector runs only between frames, so garbage created inside one frame cannot be reclaimed until that frame ends, and a large burst can run the page out of memory.

- Zero allocations per frame in steady state. No LINQ, no string concatenation, no `new` collections, no lambdas capturing locals inside `Update` or the simulation tick.
- Reuse lists and buffers. Pool bots, boxes and UI rows instead of calling `Instantiate` and `Destroy` during play.
- Cache component references in `Awake`. Never call `GetComponent`, `Camera.main` or `Find` inside `Update`.
- Update text only when its value changes, and use `SetText` with numeric arguments on TextMeshPro instead of building strings.
- Cache `WaitForSeconds` instances and animator or shader property ids.

### Rendering

- Pack sprites into Sprite Atlases to keep draw calls low. Sprites sharing an atlas and material batch together.
- Use Sprite-Unlit materials by default. 2D lights and shadows cost a lot on integrated GPUs, so add them only where the design needs them.
- No post-processing unless measured and justified. No compute shaders, they are unsupported in WebGL 2.
- Draw the warehouse with Tilemaps, not one GameObject per tile.
- Split UI into separate canvases for static and frequently changing content, because one changed element rebuilds its whole canvas. Disable `Raycast Target` on everything that is not clickable.
- Leave `Application.targetFrameRate` at its default so the browser drives the frame loop.

### Build size and loading

- Every added package, font, texture and audio clip adds to the download. Do not add a dependency without a clear need.
- Textures: set a sensible max size per sprite, enable compression, disable mipmaps for 2D sprites and Read/Write.
- Audio: compressed Vorbis, mono where possible, short clips decompressed on load and music streamed.
- Fonts: TextMeshPro with a static atlas limited to the characters the supported languages need.
- Player settings for release: Brotli compression, managed stripping level High, exceptions set to "Explicitly Thrown Exceptions Only", IL2CPP code generation optimized for size, no development build flags.
- Keep the first scene small so the game becomes interactive quickly.

## Unity coding rules

- Private serialized fields with `[SerializeField]` rather than public fields on MonoBehaviours. Public fields are acceptable only on plain JSON data classes.
- Use `[RequireComponent]` when a component depends on another one on the same GameObject.
- Subscribe to events in `OnEnable` and unsubscribe in `OnDisable`. A missed unsubscribe is a leak and a source of calls into destroyed objects.
- Compare Unity objects with `== null` only where destruction matters. Do not use `?.` or `??` on `UnityEngine.Object`, because they bypass Unity's destroyed-object check.
- Use `CompareTag` instead of `tag ==`.
- Delete empty `Start` and `Update` methods. Unity calls them even when empty.
- Prefer a single simulation tick driving all bots over an `Update` on every bot.
- Input only through the Input System actions asset, and support both mouse and touch because the game runs in mobile browsers too.
- Never edit `.unity`, `.prefab` or `.asset` YAML by hand when the change can be made through the Editor or an Editor script.
- Every asset has a `.meta` file, and it is always committed together with the asset. Never delete or regenerate a `.meta` file, because that breaks every reference to the asset.
- Guard platform-specific code with `#if UNITY_WEBGL && !UNITY_EDITOR` and always provide an Editor path so the game stays playable in Play mode.

## Testing

- **EditMode tests** cover everything in `Core`. Required coverage includes the generator guarantees for many seeds and CV sizes, CV parsing with missing fields and malformed files, section splitting and empty sections, weight and category rules for each bot type, task queue ordering, pricing scaled to CV size and scoring.
- **PlayMode tests** cover scene wiring: the game boots from a CV file, bots deliver boxes, a truck departs, the final CV screen appears, and the skip button works from any state.
- **Manual WebGL check** is required for anything that behaves differently in a browser: JSON loading over HTTP, file download, audio start, fullscreen, resizing and touch input. Automated tests run in the Editor and do not prove WebGL behaviour.
- A bug fix starts with a test that reproduces the bug.
- Tests are independent, deterministic and use fixed seeds. No test depends on another test's state or on real time.
- Test names describe behaviour: `MethodOrFeature_Condition_ExpectedResult`.

With the Editor open, run tests through the Unity MCP. Running tests from the command line is the fallback for when the Editor is closed (it fails while the Editor has the project open, and the path assumes the default Unity Hub install location):

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults/editmode.xml -logFile TestResults/editmode.log
```

Use `-testPlatform PlayMode` for PlayMode tests. Do not pass `-quit` together with `-runTests`. A feature is reported as done only after the relevant tests were actually run and passed. If they could not be run, say so explicitly.

## Definition of done

1. The feature matches `Docs/GameDesign.md`, or the document was updated.
2. Logic sits in `Core` with EditMode tests, wiring has PlayMode tests where it applies.
3. All tests pass.
4. No per-frame allocations were introduced and no WebGL-unsupported API is used.
5. Browser-specific behaviour was checked in a WebGL build, or is flagged as not yet checked.
