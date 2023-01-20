#### **NOTE**: The readme is a work in progress. I'm updating the code faster than I can update the readme (daily). Thank you for your patience.

# (P)rocedural (L)anguage (G)eneration (L)ibrary

> No language is justly studied merely as an aid to other purposes. It will in fact better serve other purposes, philological or historical, when it is studied for love, for itself. — J. R. R. Tolkien


## 1 — Introduction

Procedural Language Generation Library (PLGL) is a code library designed for game developers who want consistent, fictional languages for their game's cultures and peoples, *without the time needed to create one*. The language author constructs the alphabet, sigma structures, letter pathing, character filtering for deconstruction, and other constraints; then, the generator processes a regular sentence, and returns a new, stylized sentence from your fictional language.

The initial thought that started this project was simple: using a word as a seed, could I procedurally generate an entirely different word? After a few obstacles and a necessary amount of feature creep, the first version of the PLGL is released.


## 2 — Contents

1. **Introduction**
2. **Contents**
3. [Examples](https://github.com/Highverve/PLGL/edit/master/README.md#3--examples)
4. [Theory & Process](https://github.com/Highverve/PLGL/edit/master/README.md#4--theory--process)
    - 4.1 [Deconstruction](https://github.com/Highverve/PLGL/edit/master/README.md#41--deconstruction)
    - 4.2 [Construction](https://github.com/Highverve/PLGL/edit/master/README.md#42--construction)
    - 4.3 [Generating Sentences](https://github.com/Highverve/PLGL/edit/master/README.md#43--generating-sentences)
5. [Setting Up](https://github.com/Highverve/PLGL/edit/master/README.md#5--setting-up)
    - 5.1 [Detailed Overview](https://github.com/Highverve/PLGL/edit/master/README.md#51-detailed-overview)
6. [Future Updates](https://github.com/Highverve/PLGL/edit/master/README.md#6--future-updates)

## 3 — Examples

Example languages will go here.


## 4 — Theory & Process

The generation process can be divided into two parts: *Deconstruction* and *construction*. Deconstruction breaks down a sentence by character filters (primarily *letters*, *numbers*, *delimiters*, *punctuation marks*, etc.), and merges any desired blocks (more on this later). This greatly helps the construction process, which is responsible for handling how each filter block is manipulated. For example, the *letters* filter should typically be generated into an entirely new word, whereas the language author may prefer to keep 'numbers' or 'punctuation' filters as is. Since filters—and how they function—are defined by the language author, there is immense flexibility.

### 4.1 — **Deconstruction**.

The first step is to add a character filter:
```c#
lang.AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
```

The `Deconstructor` class loops through the characters in your string, checking if the character matches any characters in any filter. In this case, if it's a letter, it starts counting. When it encounters a character from a different filter, it splits off the string, adds it to the list, and starts counting through the new filter block. With a delimiter and punctuation filter added, the returned list looks like this:
```c#
{LETTERS[0,1]: "My"}
{DELIMITER[2,2]: " "}
{LETTERS[3,6]: "name"}
{DELIMITER[7,7]: " "}
{LETTERS[8,9]: "is"}
{DELIMITER[10,10]: " "}
{LETTERS[11,16]: "Trevor"}
{PUNCTUATION[17,17]: "."}
```

You could also write these block separations plainly as: "My| |name| |is| |Trevor|.|". It's from this list of character blocks that the constructor operates on (specifically, after they're added to a WordInfo class). You don't have to define every character; however, any unlisted character will be included anyway under the "UNDEFINED" filter, and will appear in the returned string.

There are some circumstances where a character belongs in one filter, yet also really *should* be included in a different block based on certain conditions to help the generator process the block. Words such as "let's", or numbers with commas or decimals, or even word flagging. I've included methods that help merge character blocks based on the specified criteria. Let's add one for the apostrophe.
```c#
lang.Deconstruct += (lg, current, left, right) => lg.EVENT_MergeBlocks(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");
```
The Deconstruct event is called after all of the characters in the string have been processed. It allows the language author greater control over how blocks behave around their neighbors. In this case, EVENT_MergeBlocks compares the current iterated character block (*PUNCTUATION* filter) to its friends on the left and right; if both are *LETTERS*, it then checks if the PUNCTUATION block is a single apostrophe '. If it is, the three blocks are merged into one, taking on the filter of the last string in the parameter. Without this, the three blocks are processed separately; for a number like $9,999.99, a *NUMBERS* filter would only see 9, 999, and 99—that's not ideal at all.

### 4.2 — **Construction**.

The ConstructFilter event is the most crucial to implement. This is where you tell the generator how you want each filter to be processed. Here is a simple example (where `lang` is the Language class reference, and `lg` is the LanguageGenerator class):
```c#
lang.Construct += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
```

The LanguageGenerator comes with a few common generation methods to speed up language authoring: `CONSTRUCT_Hide`, `CONSTRUCT_KeepAsIs`, `CONSTRUCT_Generate`. These methods start with `CONSTRUCT_` for clarity, so that auto-suggestion groups them together. If you plan to add any custom functionality (and you likely will), here's what KeepAsIs looks like:
```c#
public void FILTER_KeepAsIs(WordInfo word, string filter)
{
    //Make sure the filter matches.
    if (word.Filter.Name.ToUpper() == filter)
    {
        //Set the final word to what the word started as.
        word.WordFinal = word.WordActual;
        word.IsProcessed = true;
    }
}
```

The filter check is the most important part. If it's not included, the method is applied to every word. We'll look at `CONSTRUCT_Generate` in the next section.

### 4.3 — **Generating Sentences**.

An overview at the code that parses your sentence, transforming it according to the language author's constraints.


## 5 — Setting Up

### 5.1 Detailed Overview

1. Initial setup.
    - Add a class to your project with a Language field.
    - Add a method which returns your language.
    - Fill in your language's metadata: name, description, author.
    - Set additional properties found in Language.Options.
2. Structuring.
    - Add consonants and vowels to your alphabet.
    - Add syllable structure.
    - Add all letter paths to Language.Structure. Be patient: this is the most tedious part.
3. Deconstruction.
    - Add character filters. Unlisted charactered are added to Undefined filter when a sentence is processed. Examples:
        - **Delimiter**. Usually just space. Highly recommended.
        - **Letters**. a-z, A-Z. Some letter filter is essentially required.
        - **Numbers**. 0-9. Not required, but recommended.
        - **Punctuation**. Optional, but recommended.
        - **Flags**. Also needs FlagsOpen and FlagsClose for CONSTRUCT_ContainWithin.
        - **Escape**. Allows the surrounded block to escape it's filter (e.g, "[Generate]" results in "Generate"). This can be added with flagging, so it's optional.
    - Add deconstruct events (to Language.Deconstruct; lg = LanguageGenerator). This is the second pass, and corrects blocks through a stronger contextual lens.
        - Absorb single apostrophe into letter blocks. `... => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "LETTERS", "LETTERS", "\'", "LETTERS");`
        - Absorb decimal into number blocks. `... => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ".", "NUMBERS");`
        - Absorb comma into number blocks. `... => lg.DECONSTRUCT_MergeBlocks(current, left, right, "PUNCTUATION", "NUMBERS", "NUMBERS", ",", "NUMBERS");`
        - For Escape filter. `... => lg.DECONSTRUCT_MergeBlocks(current, left, right, "LETTERS", "ESCAPE", "ESCAPE", "ESCAPE");`
        - For Flags filter. `... => lg.DECONSTRUCT_ContainWithin(current, left, right, "FLAGSOPEN", "FLAGSCLOSE", "FLAGS");`
4. Construction. Add construct events (Language.Construct).
    - Keep Undefined. `... => lg.CONSTRUCT_KeepAsIs(word, "UNDEFINED");`
    - Keep Delimiter. `... => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");`
    - Set Letters to Generate. Essential. `... => lg.CONSTRUCT_Generate(word, "LETTERS");`
    - Set Punctuation to Punctuation.Process. `... => lang.Punctuation.Process(lg, word, "PUNCTUATION");`
    - Set Flags to Flagging.Process. `... => lang.Flags.Process(lg, word, "FLAGS");`
5. Other options.
    - Add punctuation. Alternative punctuation marks, or a particle system, for stronger language style. `lang.Punctuation.Add(".", (w) => { return "·"; });`
    - Add flags (<Hide, Hide>, NoGen, ). There are a few default actions in the Language.Flags class. The result can be concatenated dynamically, if required. `lang.Flags.Add("PLAYER", (lg, word) => lang.Flags.ACTION_Replace(lg, word, () => { return PlayerName; }));`

### 5.2 


## 6 — Future Updates

- [ ] **Number processing**: custom base conversion, custom numbers and names.
- [ ] Stronger control over sigma selection.
- [ ] Better control over letters (perhaps with consonant doubling, diphthongs, or some other rules).
- [ ] Easier, or less tedious, letter pathing—if at all possible.
