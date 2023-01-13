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

• **Lexemes**. The different inflection a word may take (fly, flying, flied, etc.).


### 3 — Theory & Process

The generation process can be divided into two parts: *Deconstruction* and *construction*. Deconstruction breaks down a sentence by character filters (primarily *letters*, *numbers*, *delimiters*, *punctuation marks*, etc.), and merges any desired blocks (more on this later). This greatly helps the construction process, which is responsible for handling how each filter block is manipulated. For example, the *letter* filter should typically be generated into an entirely new word, whereas the language author may prefer to keep 'numbers' or 'punctuation' filters as is. Since filters and how they function are defined by the language author, there is immense flexibility.

#### 3.1 — **Deconstruction**.

The first step is to add a character filter:
```lang.AddFilter("Letters", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");```

. The `Deconstructor` class loops through the characters in your string, checking if the character matches any characters in any filter. In this case, if it's a letter, it starts counting. When it encounters a character from a different filter, it splits off the string, adds it to the list, and starts counting through the new filter block. With a delimiter and punctuation filter added, the returned list looks like this:
```
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

#### 3.2 — **Construction**. Using construct filters to process the sentence.

The ConstructFilter event is the most crucial to implement. This is where you tell the generator how you want each filter to be processed. Here is a simple example (where `lang` is the Language class reference, and `lg` is the LanguageGenerator class):
```
lang.ConstructFilter += (lg, word) => lg.CONSTRUCT_KeepAsIs(word, "DELIMITER");
```

The LanguageGenerator comes with a few common generation methods to speed up language authoring: CONSTRUCT_Hide, CONSTRUCT_KeepAsIs, CONSTRUCT_Generate. These methods start with `CONSTRUCT_` for clarity, so that auto-suggestion groups them together. If you plan to add any custom functionality (and you likely will), here's what KeepAsIs looks like:
```
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

The filter check is the most important part. If it's not included, the method is applied to every word. We'll look at CONSTRUCT_Generate in the next section.

#### 3.3 — **Generating sentences**. An overview at the code that parses your sentence, transforming it according to the language author's constraints.


### 4 — Setting Up

4.1 Create your language.