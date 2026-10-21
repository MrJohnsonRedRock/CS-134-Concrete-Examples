# CS-134 Concrete Examples: How to Follow Along

## Once, on the first day

1. Open **PowerShell** (Start menu, type `PowerShell`).
2. Paste this line and press Enter. It downloads ("clones") the class project into
   a folder in your profile:

   ```powershell
   git clone https://github.com/MrJohnsonRedRock/CS-134-Concrete-Examples.git "$HOME\Unity Projects\CS-134 Concrete Examples"
   ```

   Read what it prints. `Cloning into ...` followed by `done` means it worked.
3. Open **Unity Hub**, click **Add > Add project from disk**, and pick
   `Unity Projects\CS-134 Concrete Examples` in your user folder.
4. Open it. The first time takes a few minutes while Unity imports everything.

If git says the folder `already exists and is not an empty directory`, tell your
instructor before deleting anything.

## Every class

1. Open the project from Unity Hub, and click **CIT134 > Get Latest Lesson**.
2. Open **Assets/Scenes/Level1**.
3. In the Hierarchy, click **Lessons**.
4. In the Inspector, find today's session (for example **Session15**) and set **Step**
   to the number on the slide.
5. Press **Play** and read the HUD. **Predict first**, then look.

Change the Step and press Play again for the next slide. Press **H** to hide the HUD.

## When a step says "uncomment"

Some lessons show you a mistake. The step tells you which file to open and which
lines to uncomment. Do it, save, and read the Console. You don't need to undo it:
Get Latest Lesson sets your changes aside next class.

## Your own code

Put your experiments in **Assets/Playground**. That folder is yours, and updates
never touch it. Read the README in there before you start.

## If Get Latest Lesson shows an error

Tell your instructor. Your changes are safe: it sets them aside before it downloads anything.
