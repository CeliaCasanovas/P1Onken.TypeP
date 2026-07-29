# P1Onken.TypeP
w**Project**: Type P.  
**Author**: P1OnKen, or パンク派第１音響合成研究班 (Punk Faction, Sound Synthesis Research Unit Number 1).  
**Lead**: Akirako Saint-Just (Cèlia Casanovas).  
**Version**: Type P.b.  
**Start date**: 2026-07-29.  
  
P1Onken.TypeP is a digital synthesizer. Its core is an oscillator that reads a sine wave table with a transfer function. It uses this core for phase distortion, phase modulation, granular synthesis and spectral synthesis. All techniques compose with each other.  
  
## Version history: Type P.b  

Type P.a's concept was "Pikopiko Industrial Engine".  
It had a modular structure with oscillator, modulator and processor slots. The user chose what component each slot contained. This architecture added overhead and made the code complex. **Type P.b seeks a simpler architecture.**  

For Type P.a, We wrote many different components that we thought would fit the concept. As a result, the design became unclear. **Type P.b has a clear-cut concept.**  

## Inspirations

* The Casio CZ, Roland D, Yamaha DX and Kawai K series of synthesizers.
* Iannis Xenakis' concept of granular synthesis.
    > In Xenakis' concept, stochastic and chaotic calculations merge sound design and musical composition. **One of the project's goals is a simple, user-friendly implementation of these techniques.**

## Project goals

* Phase Modulation, Phase Distortion, Granular Synthesis and Spectral Synthesis that compose with each other, from a combined transfer function core.
* Complex signal routing with a simple architecture.
* User-friendly operation.
* High CPU performance.
* Components for user-friendly stochastic and chaotic sound design and musical composition.
