# (P)rocedural (L)anguage (G)eneration (L)ibrary

> No language is justly studied merely as an aid to other purposes. It will in fact better serve other purposes, philological or historical, when it is studied for love, for itself. — J. R. R. Tolkien

### 1 — Introduction

Procedural Language Generation Library (PLGL) is a code library designed for game developers who want consistent, fictional languages for their game's cultures and peoples, *without the time needed to create one*. The language author constructs the alphabet, sigma structures, letter pathing, and other constraints; then, the generator processes a regular sentence, and returns an entirely new one.

The initial thought that started this project was simple: using a word as a seed, could I procedurally generate an entirely different word? I knew the answer, so I gave it a try. After a few obstacles and a necessary amount of feature creep, the first version of the PLGL is (almost) released.

### 2 — Glossary

• **Sigma**. The phonological symbol for syllable, containing the onset, nucleus, and coda.
1. **Onset**. The start of a sigma, containing consonants.
2. **Nucleus**. The middle of a sigma; vowels. This is always required.
3. **Coda**. The end of a sigma; consonants, once again.

• **Lexemes**. The different forms a common word may take (fly, flying, flied, etc.). I'll explain the importance in *3.4 Lexemes*.


### 3 — Theory & Process
3.1 — **Generating sentences**. An overview at the code that parses your sentence, transforming it according to your constraints.

3.2 — **Flagging**. Custom flags for extra function (e.g., (X)SkipGenerate, (x)SkipLexemes).

3.3 — **Punctuation**. Handling and processing punctuation marks.

3.4 — **Lexemes**. Handling affixes and extracting the root word.

3.4 — **Constructing**. Syllable count estimation, sigma structure generation, and letter pathways.

### 4 — Setting Up
