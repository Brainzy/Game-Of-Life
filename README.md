Unity (6000.3.13f1) C# Game of Life. Reads a Life 1.06 state from input.txt, runs ten generations, and writes output.txt.

In Unity, choose the run command under Tools > Game of Life.

Input and output files are in the project root.

Stores live cells in a HashSet<Cell> and computes each generation simultaneously.
Birth with 3 neighbours, survival with 2 or 3. Neighbours beyond signed 64-bit limits are skipped, so edges do not wrap.

Tests: Window > General > Test Runner > EditMode