# Demo Code Standards (draft)

Students read demo code as an example of how C# should look. These rules keep
every demo consistent and readable.

## Files and folders

- One session per folder: `Assets/Demos/S{NN}_{Topic}/`, e.g. `S15_Constructors/`.
- Shared infrastructure goes in `Assets/Demos/Shared/`. Shared code may use
  features the class hasn't reached yet; session demos may not (see below).
- One concept per file. If a file needs a scroll bar to make its point, split it.
- When you contrast two approaches, write two files with parallel names that can
  be swapped on the same GameObject: `Spawn_Instantiate` / `Spawn_Pooled`.
- A file that is wrong on purpose ends in `_BROKEN`, opens with a header comment
  saying what's wrong and why it's left that way, and has a correct counterpart
  in the same folder. Don't fix a `_BROKEN` file unless asked.

## Header comment

Every demo file opens with:

```csharp
// Session 15 — What `new` Actually Calls
// Concept: a constructor is a method that runs once, when the object is made.
// Watch:   the HUD log, as each enemy is built.
```

The header says what to *watch*, never what will happen. The prediction belongs
to the students.

## Only what's been taught

- Use only language features from the current session or earlier. No LINQ, no
  lambdas, no `var` before the session that introduces them.
- Write explicit types: `int count = 0;`, not `var count = 0;`. The compiler
  reads the box, so the student should be able to as well.
- No clever one-liners. Prefer the version a student could have written.

## Formatting

- Allman braces: every `{` on its own line.
- Always use braces, even for one-line `if` bodies. One-line guard clauses like
  `if (x == null) { return; }` may stay on a single line.
- PascalCase for types, methods, and properties. camelCase for fields, locals,
  and parameters. No `m_` or `_` prefixes.
- Inspector fields are `[SerializeField] private`, unless the demo is *about*
  `public`.
- Zero compiler warnings. A demo that warns is broken.

## Unity

- 2D types only: `Rigidbody2D`, `Vector2`, `Collider2D`. When a slide uses a 3D
  example, translate it.
- Constructors go on plain C# classes, never on MonoBehaviours. Unity builds
  MonoBehaviours itself; `new` on one is a bug.
- Demos build their own setup in `Start` where practical (spawn targets,
  configure objects) so the scene needs as little hand wiring as possible.
- Results go to the screen through `DemoHUD`, not just `Debug.Log`. A
  prediction has to be confirmed or broken where the whole room can see it.
- Any claim about cost (allocation, GC, speed) is backed by numbers on the HUD.
  If a comment needs a measured value, write `/* MEASURE: ___ */` and leave it
  for the instructor to fill in after a real run. Never invent a number.

## Comments teach

- Tag the session the idea comes from: `// Session 16: no this, so no object.`
- Use the course's phrasing: *the box holds the thing / holds an arrow to the
  thing*, *the compiler reads the box, not the thing*, *Destroy makes garbage*,
  *new means "run a constructor."*
- Explain *why*, not *what*. `health = 0;` doesn't need a comment that says
  "sets health to 0."
