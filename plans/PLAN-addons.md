# Plan: Add-on Modules

## Overview
Create modular script segments that can extend the core presentation. Each add-on is independent and optional. Only pursue after core script is complete and solid.

## Prerequisites
- Core "Journey to Correctness" script completed and validated
- Core script includes bridge transition dialogue option

---

## Add-on 1: VIP Refactoring Demo

### Output
`presentation-addon-vip.md` - 5 minute segment

### Purpose
Demonstrate compiler-guided refactoring across F# and C#. Shows adding a new case to discriminated union and watching C# compiler warnings appear.

### Content
- Dev adds `| VIP of RegisteredCustomer` to F# Customer type
- Show C# compiler warnings in switch expressions
- Fix C# code with VIP discount (15% vs Standard's 10%)
- Celebrate cross-language refactoring safety

### Insertion Point
After core Act 3 (minute ~30), before C# Interop deep-dive

### Dialogue Style
More dramatic/demo-focused than conversational. Build tension as warnings appear.

### Risk
Can feel gimmicky if audience isn't engaged. Include graceful exit dialogue.

---

## Add-on 2: Advanced C# Patterns

### Output
`presentation-addon-csharp-patterns.md` - 5-7 minute segment

### Purpose
Deep-dive on C# pattern matching and wrapper patterns for teams adopting F# types in C# codebases.

### Content
- Composing business rules with multiple switch expressions
- C#-friendly wrapper/facade patterns
- Extension methods for F# types
- When to wrap vs use directly

### Insertion Point
After basic C# interop (minute ~35), extends Act 4

### Audience
C#-heavy teams evaluating F# adoption. Skip for F#-curious audiences.

---

## Add-on 3: SAFE Stack Introduction

### Output
`presentation-addon-safe-intro.md` - 3 minute segment

### Purpose
Quick orientation to SAFE Stack for audiences interested in full-stack F#.

### Content
- SAFE acronym explanation (Saturn, Azure, Fable, Elmish)
- Project structure overview
- Shared types concept (no DTO mapping)

### Insertion Point
After core wrap-up, as gateway to deeper SAFE content

### Dialogue Style
Faster paced, more presentational than conversational

---

## Add-on 4: SAFE Stack Server

### Output
`presentation-addon-safe-server.md` - 5-7 minute segment

### Purpose
Show Saturn server endpoint creation with shared types.

### Content
- Copy domain model to Shared project
- Create simple API endpoint
- Wire up router
- Quick browser/Postman test

### Prerequisites
SAFE Intro add-on (or audience already knows SAFE basics)

---

## Add-on 5: SAFE Stack Elmish/MVU

### Output
`presentation-addon-safe-elmish.md` - 10-12 minute segment

### Purpose
Full MVU pattern walkthrough with Customer type editor.

### Content
- Model type definition
- Message type (events)
- Update function (pure state transitions)
- View function (UI from model)
- Walk through complete cycle

### Prerequisites
SAFE Intro add-on

### Dialogue Style
More presentation/narration than conversation due to complexity

---

## Add-on 6: Copilot UI Generation

### Output
`presentation-addon-copilot.md` - 4 minute segment

### Purpose
Show AI-assisted UI generation leveraging strong types.

### Content
- Prompt Copilot to generate form fields
- Show generated code
- Quick test that it works
- "Power of strong typing meets modern tooling"

### Prerequisites
SAFE Elmish add-on (needs existing Elmish context)

### Risk
AI is unpredictable. Include recovery dialogue for surprises.

### Dialogue Style
Improvisational, lighter tone, recovery-ready

---

## Implementation Order
1. VIP Refactoring (most likely to be used, dramatic impact)
2. SAFE Intro (gateway to other SAFE add-ons)
3. SAFE Elmish (most substantial SAFE content)
4. Advanced C# Patterns (audience-dependent)
5. SAFE Server (often combined with Elmish)
6. Copilot (highest risk, lowest priority)

## Success Criteria
- [ ] Each add-on works independently
- [ ] Each has clear insertion point
- [ ] Each has graceful exit if needed
- [ ] Transitions from core feel natural
- [ ] Can combine multiple add-ons coherently
