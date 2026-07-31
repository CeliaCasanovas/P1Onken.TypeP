# P1Onken.TypeP

**Project**: Type P.  
**Authors**:

- P1OnKen, or パンク派第１音響合成研究班 (Punk Faction, Sound Synthesis Research
  Unit Number 1).
- P1ChouKyouInKen,
  or パンク派第１超現実主義・共産主義・インダストリアル研究班 (Punk Faction;
  Surrealist, Communist, and Industrial Research Unit Number 1).

**Lead**: Akirako Saint-Just (Cèlia Casanovas).  
**Version**: Type P.b.  
**Start date**: 2026-07-29.

P1Onken.TypeP is a digital synthesizer. Its core is a cosine oscillator that
calculates its phases through a transfer function. It uses this core for phase
distortion, phase modulation, granular synthesis and spectral synthesis. All
techniques compose with each other.

## Version history: Type P.b

Type P.a's concept was "Pikopiko Industrial Engine". It had a modular structure
with oscillator, modulator and processor slots. The user chose what component
occupied each slot. This architecture added overhead and made the code complex.
**Type P.b seeks a simpler architecture.**

For Type P.a, we wrote many different components that we thought would fit the
concept. As a result, the design became unclear. **Type P.b has a clear-cut
concept.**

Type P.b was stateful for performance reasons. **Type P.b is an investigation
into whether a functional-lite, parameter-based data flow can be equally
efficient.** It is possible that some mutable state will be unavoidable.

We spent a lot of effort on low-level optimisation for Type P.a. That made the
code difficult to reason about. **Type P.b defers optimisation to a subsequent
phase, if benchmarks show we need it.**

## Inspirations

- The Casio CZ, Roland D, Yamaha DX and Kawai K series of synthesizers.
- Iannis Xenakis' concept of granular synthesis.
  > In Xenakis' concept, stochastic and chaotic calculations merge sound design
  > and musical composition. **One of the project's goals is a simple,
  > user-friendly implementation of these techniques.**
- Kleimola, Lazzarini, Timoney and Välimäki's paper on Vector Phaseshaping
  Synthesis for DAFx11.
- Pikopiko, industrial, glitch, punk and avant-garde artists:
  - Iannis Xenakis
  - P-Model
  - Autechre
  - Orchid
  - Ampere
  - Love Lost But Not Forgotten
  - pageninetynine
  - The Stalin
  - Void
  - Yellow Magic Orchestra
  - Oval
  - Wire
  - Skinny Puppy
  - Throbbing Gristle
  - Aunt Sally
  - Phew
  - DNA
  - Black Cat #13
  - Blow Up
  - Pan Sonic
  - Suicide
  - Karlheinz Stockhausen
  - Brian Eno

## Project goals

- Phase Modulation, Phase Distortion, Granular Synthesis and Spectral Synthesis
  that compose with each other, from a combined transfer function core.
- Complex signal routing with a simple architecture.
- User-friendly operation.
- High CPU performance with functional-lite idioms.
- User-friendly stochastic and chaotic sound design and musical composition.
- A tool for punks to explore surrealism, communism and the industrial.
