# CIT 134B Demo Project

This is an existing 2D Unity game, reused as a teaching repo for a dual-credit
C# course (CIT 134B). Students clone it and pull every morning; only the
instructor commits. Each class session adds a small demo that makes one
programming concept visible inside a real game.

## Who reads this code

High school Digital Game Development students. They are fluent Unity *users*
and read far more code than they write. **Every demo file is read by students
as an example**, so clarity matters more than cleverness.

## What a demo is for

One concept, made observable, aimed at one specific misconception.

The classroom loop is: slide explains the mechanism → students **predict**
what the demo will show → the demo runs → the result confirms or breaks the
prediction. A demo succeeds if the outcome is unmistakable on screen and the
code that causes it is easy to find and read.

## Where things live

- `Assets/Game/` — the working game. **The baseline. Leave it alone** unless
  asked; demos are added beside it, not carved into it.
- `Assets/Demos/Shared/` — reusable demo infrastructure (the on-screen HUD,
  helpers).
- `Assets/Demos/S{NN}_{Topic}/` — one folder per session, e.g.
  `S16_Static/`.
- `docs/sessions/` — the slide decks, by session number. The source of truth
  for what each concept means and how it was taught.
- `docs/briefs/` — one short brief per session: the concept, the
  misconception, the prediction prompt, what must be visible, constraints.
  Build from the brief; consult the deck when the brief isn't enough.

## Rules for demos

- **Additive.** Prefer new components over editing existing ones. To contrast
  approaches, write variants that sit on the same GameObject and are swapped in
  the Inspector (e.g. `Shoot_Instantiate` vs `Shoot_Pooled`).
- **Intentionally wrong code is labeled and left wrong.** Files demonstrating a
  mistake end in `_BROKEN` and open with a header comment saying so and why.
  Never fix, refactor, or "improve" a `_BROKEN` file unless explicitly asked.
  Each one should have a correct counterpart in the same folder.
- **Use the game's real types.** This is a 2D project: `Rigidbody2D`,
  `Vector2`, `Collider2D`. The decks often use 3D examples; the concept is
  identical, so translate rather than introducing 3D types.
- **Self-assembling.** Where practical, a demo builds its own test rig in
  `Start` (spawns targets, configures objects) so it needs minimal scene
  wiring.
- **Don't hand-edit `.unity`, `.prefab`, or `.asset` files.** When a demo needs
  editor setup, list the exact steps in your reply instead.
- **Constructors go on plain C# classes,** never on MonoBehaviours.
- **Cost claims get instrumented.** Any demo about allocation, GC, or
  performance shows its numbers through the shared HUD, not through feel.
- **Comments teach.** Point to the session (`// Session 16: ...`) and use the
  course's vocabulary: *the box holds the thing / holds an arrow to the thing*,
  *the compiler reads the box, not the thing*, *Destroy makes garbage*.

## Things that never go in the repo

Students pull everything committed here.

- No expected answers to prediction prompts, no quiz or exam content, no
  instructor notes. Put expected outcomes in your reply to me.
- No invented performance numbers. If a comment or prompt needs a measured
  value, leave a clearly marked placeholder; I fill it in after running.

## Workflow

1. I name a session or point to a brief.
2. Read the brief (and the deck if needed). Reply with a short plan: files to
   add, what the student will see, the prediction prompt, and which
   misconception it targets.
3. After I approve, build it.
4. Finish with: editor setup steps, what I should check in Play mode, and the
   expected outcome (for me, not the repo).

## Git

- `main` only holds sessions that have been taught.
- Work on a branch named for the session (`s16-static`).
- Don't merge, tag, or push unless asked.

## Code standards

Demo code follows the course's style standards; see
`.claude/rules/demo-code.md`.
