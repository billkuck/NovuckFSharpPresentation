# Plan: Alternate Script - "Window Owners"

## Overview
The "Window Owners" script takes a different conversational approach where each presenter "owns" a specific environment:
- **Dev A** owns the F# window (domain modeling, type design)
- **Dev 1** owns the C# window (consumer perspective, interop)

This creates natural dialogue as they discuss changes from their respective viewpoints.

## Priority
**SECONDARY** - Only develop after "Journey to Correctness" core script is complete and validated.

## Output
`presentation-script-windows.md` - Alternative screenplay format script

---

## Concept

### The Dynamic
- Dev A makes F# changes, explains type-driven thinking
- Dev 1 reacts from C# consumer perspective
- Natural tension: "Will this break my code?"
- Resolution: Show how F# types guide C# patterns

### Physical Staging
- Dev A: Positioned near F#/left screen
- Dev 1: Positioned near C#/right screen
- Meet in middle for discussions
- Single keyboard passed between them

---

## Script Structure

### Act 1: Setting Up Windows (5 min)
- Each dev introduces their window
- Dev A: "This is our F# domain library"
- Dev 1: "And I'm consuming it from this C# application"
- Establish the relationship

### Act 2: Dev A Makes Changes (10 min)
- Dev A introduces discriminated unions
- Dev 1 watches nervously: "What does that do to my code?"
- Show the break, then the fix
- Dev 1: "Actually, that's... clearer"

### Act 3: Collaborative Iteration (15 min)
- Dev 1: "What if a customer signs up but hasn't purchased yet?"
- Dev A: "Great question - let me show you explicit states"
- Back-and-forth as Standard case is introduced
- Each sees it from their window's perspective

### Act 4: The Payoff (10 min)
- Dev A shows final F# domain
- Dev 1 demonstrates clean C# consumption
- Both windows on screen together
- Joint conclusion: Types as shared language

---

## Key Differences from "Journey to Correctness"

| Aspect | Journey to Correctness | Window Owners |
|--------|------------------------|---------------|
| Focus | Evolution of code | Collaboration between environments |
| Tension | "How do we improve?" | "Will this break things?" |
| POV | Unified team perspective | Split perspectives |
| Staging | Flexible positioning | Fixed to windows |
| Appeal | Process-focused audiences | Teams with F#/C# split |

---

## When to Use This Script

**Choose "Window Owners" when:**
- Audience has distinct F# and C# developers
- Want to emphasize interop story
- Team considering F# library for C# consumers
- Presenters prefer fixed positions

**Choose "Journey to Correctness" when:**
- Audience is F#-curious from any background
- Want to emphasize evolution/improvement
- Team wants to adopt F# approach broadly
- Presenters prefer collaborative movement

---

## Development Steps

1. **Wait for core completion** - Journey to Correctness must be done first
2. **Analyze feedback** - Use learnings from core script development
3. **Scaffold structure** - Four acts as outlined above
4. **Write dialogue** - Focus on window-specific perspectives
5. **Map code to windows** - Clear ownership of which code shows where
6. **Test staging** - Ensure physical positioning works

---

## Compatibility Notes
- All add-on modules work with this script
- Transition points may differ slightly
- SAFE modules particularly relevant (shared types story)

## Success Criteria
- [ ] Clear window ownership established
- [ ] Natural handoffs between windows
- [ ] Interop story prominently featured
- [ ] Can be completed in same time as core script
